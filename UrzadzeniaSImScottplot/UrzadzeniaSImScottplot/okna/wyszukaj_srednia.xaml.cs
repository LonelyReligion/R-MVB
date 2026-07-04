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

        private int? _maxId = null;

        public event PropertyChangedEventHandler? PropertyChanged;

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
        public wyszukaj_srednia()
        {
            DataContext = this;
            InitializeComponent();
            _inicjujKontrolki();
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;
        }

        private void Przeslij1_Click(object sender, RoutedEventArgs e) //wariant z prostokatem
        {
            bool blad = false;
            Rectangle szukany = new Rectangle(prostkat.Ymin, prostkat.Xmin, prostkat.Ymax, prostkat.Xmax);

            List<Decimal> resultDB = new List<Decimal>();
            List<Decimal> resultRTree = new List<Decimal>();
            List<string> Out = new List<string>();

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            List<int> ile = new List<int>();
            int cnt_1 = 0;

            /*using (var ctx = new Kontekst())
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
                long wynik = sw.ElapsedMilliseconds;
            }

            sw = Stopwatch.StartNew();
            int cnt_r = 0;
            List<Decimal> ile_r = new List<Decimal>();
            for (int i = 0; i < 10; i++)
            {
                (Decimal liczba_elementow, Decimal srednia) = rmvb.szukajAgregatu(szukany);
                resultRTree.Add(srednia);
                ile_r.Add(liczba_elementow);

            }
            long wynik3 = sw.ElapsedMilliseconds;

            Console.WriteLine("**********************************");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Szukanie agregatu powierzchniowego dla obszaru: xMin(" + szukany.XMin + "), " + "yMin(" + szukany.YMin + "), " +
                    "xMax(" + szukany.XMax + "), " + "yMax(" + szukany.YMax + "), ");
                Console.WriteLine("WARTOŚCI: Recznie: " + resultDB[i] + " vs " + "RMVB: " + resultRTree[i] + "\n");
                Console.WriteLine(Out[i] + ")/" + ile[i]);


                if (ile[i] != ile_r[i])
                {
                    if (blad == false) //powinno wykonać się tylko raz :)
                    {
                        bledy.Add("Działanie testów zakończyło się na wyszukiwaniu agregatu powierzchniowego. Poprzednie testy przebiegły pomyślnie, kolejne nie zostały zrealizowane.");
                        bledy.Add("Komunikat(y) błędu(ów): \n");
                    }
                    Console.WriteLine("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[i] + " (baza) " +
                        ile_r[i] + " (r)");
                    blad = true;
                    rmvb.szukajAgregatu(szukany);

                    bledy.Add("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[i] + " (baza) " +
                        ile_r[i] + " (r)");
                    bledy.Add("Współrzędne prostokąta: " + "xMin(" + szukany.XMin + "), " + "yMin(" + szukany.YMin + "), " +
                    "xMax(" + szukany.XMax + "), " + "yMax(" + szukany.YMax + ")");

                    if (resultDB[i] == resultRTree[i])
                    {
                        Console.WriteLine("Obliczone wartości SĄ ZBIEŻNE");
                        bledy.Add("Obliczone wartości SĄ ZBIEŻNE");
                    }
                    else
                    {
                        bledy.Add("Obliczone wartości: " + "Recznie: " + resultDB[i] + " vs " + "RMVB: " + resultRTree[i] + "\n");
                    }
                }
                else if (resultDB[i] != resultRTree[i])
                {
                    if (blad == false) //powinno wykonać się tylko raz :)
                    {
                        bledy.Add("Działanie testów zakończyło się na wyszukiwaniu agregatu powierzchniowego. Poprzednie testy przebiegły pomyślnie, kolejne nie zostały zrealizowane.");
                        bledy.Add("Komunikat(y) błędu(ów): \n");
                    }

                    Console.WriteLine("Mamy rozbieznosc miedzy wartościami agregatu: " + resultDB[i] + " (baza) " +
                        resultRTree[i] + " (r)");
                    //rmvb.szukajAgregatu(szukany);
                    blad = true;
                    rmvb.szukajAgregatu(szukany);

                    bledy.Add("Mamy rozbieznosc miedzy wartościami agregatu: " + "Recznie: " + resultDB[i] + " vs " + "RMVB: " + resultRTree[i] + "\n");
                    bledy.Add("Współrzędne prostokąta: " + "xMin(" + szukany.XMin + "), " + "yMin(" + szukany.YMin + "), " +
                    "xMax(" + szukany.YMin + "), " + "yMax(" + szukany.YMax + ")");
                    bledy.Add("Liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[i]);

                }

                Console.WriteLine("**********************************");
            }

            if (!blad)
            {
                Console.WriteLine("CZASY:    Recznie: " + wynik + " vs " + "RMVB: " + wynik3);
                wyniki.Add("R | wyszukuje agregaty powierzchniowe losowego prostokata | " + wynik + " | " + wynik3);
            }
*/
            sukces = !blad;
            Close();
        }

        private void Przeslij2_Click(object sender, RoutedEventArgs e)//wariant z id
        {
            sukces = true;
            Close();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }
    }
}
