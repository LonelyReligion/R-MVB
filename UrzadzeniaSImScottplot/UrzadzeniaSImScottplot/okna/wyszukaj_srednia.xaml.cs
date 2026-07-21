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
        
        private int? _maxId = null;
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


                    var pomiary = ctx.Pomiary
                        .AsNoTracking()
                        .SelectMany(
                            p => p.WersjeUrzadzenia,
                            (p, w) => new
                            {
                                Pomiar = p,
                                w.UrzadzenieID
                            })
                        .Where(x => ids.Contains(x.UrzadzenieID))
                        .GroupBy(x => x.UrzadzenieID)
                        .Select(g => g
                            .OrderByDescending(x => x.Pomiar.dtpomiaru)
                            .Select(x => x.Pomiar)
                            .FirstOrDefault())
                        .ToList();

                    foreach (Pomiar p in pomiary)
                    {
                        ile[i]++;
                        resultDB[i] += p.Wartosc;
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
                (Decimal liczba_elementow, Decimal suma) = _rmvb.szukajAgregatu(szukany);
                if (liczba_elementow != 0)
                    resultRTree.Add(suma / liczba_elementow);
                else
                    resultRTree.Add(0);
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
            
            Decimal wynikBD = 0;
            Decimal wynikRMVB = 0;

            Stopwatch sw;
            int cnt_1 = 0;

            sw = Stopwatch.StartNew();
            int liczby = 0;

            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    wynikBD = 0;
                    liczby = 0;

                    (Decimal xx, Decimal yy) = wspolrzedne;
                    
                    id = ctx.Urzadzenia
                        .AsNoTracking()
                        .Where(u => u.Szerokosc == yy)
                        .Where(u => u.Dlugosc == xx)
                        .First()
                        .UrzadzenieID;

                    if (id != -1)
                    {
                        List<Pomiar> pomiary = ctx.Pomiary
                                                .AsNoTracking()
                                                .Where(p => p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID == id)
                                                .ToList();
                        liczby += pomiary.Count;
                        foreach (Pomiar p in pomiary) wynikBD += p.Wartosc;

                        if (liczby != 0)
                            wynikBD /= liczby;
                        else
                            wynikBD = 0;
                    }
                    else
                    {
                        Console.WriteLine("Urzadzenie o wsp. " + xx + " " + yy + " nie istnieje w bazie");
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
                (Decimal xx, Decimal yy) = wspolrzedne;
                (liczba, wynikRMVB) =_rmvb.szukajAgregatuCzasowego(xx, yy);
            }
            long czas = sw.ElapsedMilliseconds;

            (Decimal x, Decimal y) = wspolrzedne;

            if (wynikBD != wynikRMVB || liczba != liczby)
            {
                if (!blad)
                {
                    komunikat_bledu = "Wyszukiwanie agregatu czasowego nie powiodło się. \nKomunikat(y) błędu(ów): ";
                }
                blad = true;

                if (wynikBD != wynikRMVB)
                {
                    bledy.Add("Mamy rozbieznosc miedzy obliczonymi wartościami: " + wynikRMVB + "(R) " + wynikBD + "(ręcznie)");
                }

                if (liczba != liczby)
                {
                    bledy.Add("Mamy rozbieznosc miedzy liczbą pomiarow wykorzystanych do policzenia agregatu czasowego urządzenia o współrzędnych: (" + wspolrzedne.Item1 + "," + wspolrzedne.Item2 + ") i id: " + id);
                    bledy.Add(liczby + " (baza) " + liczba + " (r)");

                }

            }
        
            srednia = wynikRMVB;

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
