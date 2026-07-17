using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UrzadzeniaSImScottplot
{
    /// <summary>
    /// Logika interakcji dla klasy wyszukaj_srednia.xaml
    /// </summary>
    public partial class wyszukaj_srednia : Window, INotifyPropertyChanged
    {
        public bool sukces = false;
        public bool blad = false;

        private int? _maxId = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        private RMVB _rmvb;

        public List<string> bledy = new List<string>();
        public String komunikat_bledu;

        public decimal czas_bd;
        public decimal czas_rmvb;
        public decimal srednia;
        public Rectangle szukany;
        public int id = -1;

        public bool wariant = false; //false to prostokat, a true to urzadzenie 
        public int? maxId
        {
            get { return _maxId; }
            set
            {
                _maxId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("maxId"));
            }

        }
        
        private void _inicjujKontrolki() {
            using (var ctx = new Kontekst()) {
                maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);
            }
        }
        private void _sprawdzCzyMamyUrzadzenia(object sender, EventArgs e)
        {
            using (var ctx = new Kontekst())
            {
                bool istnieje = ctx.Urzadzenia.Any();

                if (!istnieje)
                {
                    Window dialog = (Window)new brak_urzadzen_w_bazie(this);
                    dialog.ShowDialog();
                    Close();
                }
            }
        }
        public wyszukaj_srednia(RMVB rmvb)
        {
            DataContext = this;
            InitializeComponent();
            _inicjujKontrolki();
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;
            _rmvb = rmvb;

            this.Loaded += Wyszukaj_srednia_Loaded;
        }

        private void Wyszukaj_srednia_Loaded(object sender, RoutedEventArgs e)
        {
            using (var ctx = new Kontekst())
            {
                Urzadzenie zerowe = ctx.Urzadzenia.Where(u => u.UrzadzenieID == 0).First();
                dlugosc.Content = zerowe.Dlugosc.ToString();
                szerokosc.Content = zerowe.Szerokosc.ToString();
            }
        }

        private void Przeslij1_Click(object sender, RoutedEventArgs e) //wariant z prostokatem
        {
            szukany = new Rectangle(prostkat.Ymin, prostkat.Xmin, prostkat.Ymax, prostkat.Xmax);

            List<Decimal> resultDB = new List<Decimal>();
            List<Decimal> resultRTree = new List<Decimal>();
            List<string> Out = new List<string>();

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            List<int> ile = new List<int>();
            int cnt_1 = 0;

            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    Decimal x1 = szukany.XMin;
                    Decimal y1 = szukany.YMin;
                    Decimal x2 = szukany.XMax;
                    Decimal y2 = szukany.YMax;

                    ile.Add(0);

                    int cnt = 0;
                    resultDB.Add(0);

                    List<int> ids = new List<int>();

                    ids.AddRange(ctx.Urzadzenia
                    .AsNoTracking()
                    //contains
                    .Where(u => x1 <= u.Dlugosc)
                    .Where(u => y1 <= u.Szerokosc)
                    .Where(u => x2 >= u.Dlugosc)
                    .Where(u => y2 >= u.Szerokosc)
                    .Select(o => o.UrzadzenieID)
                    .ToList());


                    Out.Add("(");

                    List<Pomiar> aktualne_pomiary = ctx.Pomiary.AsNoTracking().Where(p => ids.Contains(p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID))
                        .Where(p => p.dtpomiaru > new DateTime(2024, 7, 18, 0, 0, 0)).ToList();

                    foreach (Pomiar p in aktualne_pomiary)
                    {
                        ile[i]++;
                        resultDB[i] += p.Wartosc;
                        Out[i] += p.Wartosc + "+";
                    }

                    if (ile[i] != 0)
                        resultDB[i] /= ile[i];
                    else
                        resultDB[i] = 0;
                    cnt_1 = cnt;
                }
                czas_bd = sw.ElapsedMilliseconds;
            }

            sw = Stopwatch.StartNew();
            int cnt_r = 0;
            List<Decimal> ile_r = new List<Decimal>();
            for (int i = 0; i < 10; i++)
            {
                (Decimal liczba_elementow, Decimal srednia) = _rmvb.szukajAgregatu(szukany);
                resultRTree.Add(srednia);
                ile_r.Add(liczba_elementow);

            }
            czas_rmvb = sw.ElapsedMilliseconds;

            if (ile[0] != ile_r[0])
            {
                if (blad == false) //powinno wykonać się tylko raz :)
                {
                    komunikat_bledu = "Wyszukiwanie agregatu powierzchniowego dla obszaru: xMin(" + szukany.XMin + "), " + "yMin(" + szukany.YMin + "), " +
                    "xMax(" + szukany.XMax + "), " + "yMax(" + szukany.YMax + ") nie powiodło się. \nKomunikat(y) błędu(ów): ";
                }
                Console.WriteLine("Rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[0] + " (baza) " +
                    ile_r[0] + " (r)");
                blad = true;
                   
                bledy.Add("Rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[0] + " (baza) " +
                    ile_r[0] + " (r)");

                if (resultDB[0] == resultRTree[0])
                {
                    Console.WriteLine("Obliczone wartości SĄ ZBIEŻNE");
                    bledy.Add("Obliczone wartości SĄ ZBIEŻNE");
                }
                else
                {
                    bledy.Add("Obliczone wartości: " + "Recznie: " + resultDB[0] + " vs " + "RMVB: " + resultRTree[0] + "\n");
                }
            }

            else if (resultDB[0] != resultRTree[0])
            {
                if (blad == false) //powinno wykonać się tylko raz :)
                {
                    komunikat_bledu = "Wyszukiwanie agregatu powierzchniowego dla obszaru: xMin(" + +szukany.XMin + "), " + "yMin(" + szukany.YMin + "), " +
                    "xMax(" + szukany.XMax + "), " + "yMax(" + szukany.YMax + ") nie powiodło się. \nKomunikat(y) błędu(ów): ";
                }

                Console.WriteLine("Rozbieznosc miedzy wartościami agregatu: " + resultDB[0] + " (baza) " +
                    resultRTree[0] + " (r)");
                //rmvb.szukajAgregatu(szukany);
                blad = true;
                    
                bledy.Add("Rozbieznosc miedzy wartościami agregatu: " + "Recznie: " + resultDB[0] + " vs " + "RMVB: " + resultRTree[0]);
                bledy.Add("Liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[0]);

            }
            

            srednia = resultRTree[0]; // jezeli !blad to sa rowne

            sukces = true;
            Close();
        }

        private void Przeslij2_Click(object sender, RoutedEventArgs e)//wariant z id
        {
            wariant = true;
            sukces = true;
            (Decimal, Decimal) wspolrzedne = (Convert.ToDecimal(dlugosc.Content), Convert.ToDecimal(szerokosc.Content));
            
            List<Decimal> wynikBD = new List<Decimal>();
            Decimal wynikR = 0;

            Stopwatch sw;
            int cnt_1 = 0;

            sw = Stopwatch.StartNew();
            List<int> liczby = new List<int>();

            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    (Decimal x, Decimal y) = wspolrzedne;
                    wynikBD.Add(0);
                    liczby.Add(0);

                    id = ctx.Urzadzenia
                        .AsNoTracking()
                        .Where(u => u.Szerokosc == y)
                        .Where(u => u.Dlugosc == x)
                        .First()
                        .UrzadzenieID;

                    if (id != -1)
                    {
                        List<Pomiar> pomiary = ctx.Pomiary
                                                .AsNoTracking()
                                                .Where(p => p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID == id)
                                                .ToList();
                        liczby[i] += pomiary.Count;
                        foreach (Pomiar p in pomiary) wynikBD[i] += p.Wartosc;

                        if (liczby[i] != 0)
                            wynikBD[i] /= liczby[i];
                        else
                            wynikBD[i] = 0;
                    }
                    else
                    {
                        Console.WriteLine("Urzadzenie o wsp. " + x + " " + y + " nie istnieje w bazie");
                        blad = true;
                    }
                }
            }
            long czasBD = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();

            List<Urzadzenie> resDevices = new List<Urzadzenie>();
            int liczba = 0;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                (Decimal x, Decimal y) = wspolrzedne;
                (liczba, wynikR) =_rmvb.szukajAgregatuCzasowego(x, y);
            }
            long czas = sw.ElapsedMilliseconds;
            for (int i = 0; i < 10; i++)
            {
                (Decimal x, Decimal y) = wspolrzedne;
                Console.WriteLine("Szukanie agregatu czasowego dla urządzenia o (x, y) = (" + x + ", " + y + ") i id = " + id.ToString());
                Console.WriteLine("WARTOŚCI: Baza: " + wynikBD[i] + " vs " + "Rtree: " + wynikR);


                if (wynikBD[i] != wynikR || liczba != liczby[i])
                {
                    if (!blad)
                    {
                        bledy.Add("Działanie testów zakończyło się na wyszukiwaniu agregatu czasowego. Poprzednie testy przebiegły pomyślnie, kolejne nie zostały zrealizowane.");
                        bledy.Add("Komunikat(y) błędu(ów): \n");
                    }
                    blad = true;

                    if (wynikBD[i] != wynikR)
                    {
                        bledy.Add("Mamy rozbieznosc miedzy obliczonymi wartościami: " + wynikR + "(R) " + wynikBD[i] + "(ręcznie)");
                        Console.WriteLine("Mamy rozbieznosc miedzy obliczonymi wartościami.");
                    }

                    if (liczba != liczby[i])
                    {
                        bledy.Add("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + liczby[i] + " (baza) " +
                            liczba + " (r)");

                        Console.WriteLine("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu czasowego urządzenia o współrzędnych: (" +
                            wspolrzedne.Item1 + "," + wspolrzedne.Item2 + ") i id: " + id);
                        Console.WriteLine("Na podstawie " + liczba + " (R) " + liczby[i] + " (ręcznie)" + " pomiarów");
                    }
                    bledy.Add("");

                }
                Console.WriteLine("**********************************");
            }

            srednia = wynikR;

            sukces = true;
            Close();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        private void id_urzadzenia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            using (var ctx = new Kontekst()) {
                Urzadzenie wybrane = ctx.Urzadzenia.Where(u => u.UrzadzenieID == (int)id_urzadzenia.Value).First();
                dlugosc.Content = wybrane.Dlugosc.ToString(); 
                szerokosc.Content = wybrane.Szerokosc.ToString();
            }
        }
    }
}
