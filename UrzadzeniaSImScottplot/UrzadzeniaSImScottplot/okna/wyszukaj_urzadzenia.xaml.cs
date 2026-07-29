using System.Diagnostics;
using System.Windows;
using UrzadzeniaSImScottplot.narzedzia;
using UrzadzeniaSImScottplot.okna;

namespace UrzadzeniaSImScottplot.okna
{
    /// <summary>
    /// Logika interakcji dla klasy wyszukaj_urzadzenia.xaml
    /// </summary>
    public partial class wyszukaj_urzadzenia : Window
    {
        public bool sukces = false;
        public bool blad = false;
        RMVB _drzewo;

        public decimal czas_drzewo10;

        public decimal czas_baza10;

        public List<Urzadzenie> odnalezione_urzadzenia;
        public Rectangle rect;

        public String rodzaj_bledu;
        public List<Urzadzenie> nadmiarowe =  new List<Urzadzenie>();

        public wyszukaj_urzadzenia(Generatory gen, RMVB drzewo)
        {
            InitializeComponent();

            DataContext = this;
            _drzewo = drzewo;
            _inicjalizujKontrolki();
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;

        }
        private void _sprawdzCzyMamyUrzadzenia(object sender, EventArgs e) {
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


        private void _inicjalizujKontrolki()
        {
            ;
        }

        private void anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        private void przeslij_Click(object sender, RoutedEventArgs e)
        {
            //tu wykonamy test i przekazemy wynik do MainWindow

            rect = new Rectangle(prostkat.Ymin, prostkat.Xmin, prostkat.Ymax, prostkat.Xmax);
            Stopwatch sw;
            sw = Stopwatch.StartNew();
            List<List<Urzadzenie>> cnt_1 = new List<List<Urzadzenie>>();

            using (Kontekst ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    cnt_1.Add(ctx.Urzadzenia
                    .AsNoTracking()
                    .Where(u => rect.XMin <= u.Dlugosc)
                    .Where(u => rect.YMin <= u.Szerokosc)
                    .Where(u => rect.XMax >= u.Dlugosc)
                    .Where(u => rect.YMax >= u.Szerokosc)
                    .ToList());
                }
                czas_baza10 = sw.ElapsedMilliseconds;
            }

            sw = Stopwatch.StartNew();

            List<List<Urzadzenie>> cnt_r = new List<List<Urzadzenie>>();
            for (int i = 0; i < 10; i++)
            {
                cnt_r.Add(_drzewo.szukaj(rect));
            }
            czas_drzewo10 = sw.ElapsedMilliseconds;



            System.Diagnostics.Debug.WriteLine("Prostokat: " + rect.XMin + " " + rect.XMax + "(x) " + rect.YMin + " " + rect.YMax + "(y)");
            odnalezione_urzadzenia = null;



            System.Diagnostics.Debug.WriteLine("Znaleziono " + cnt_r[0].Count.ToString() + "(rt) " + cnt_1[0].Count.ToString() + "(zapytanie w bazie)");
            if (cnt_r[0].Count != cnt_1[0].Count) //a co jezeli znalazla inne, ale liczba się zgadza?
            {
                blad = true;
                if (cnt_r[0].Count > cnt_1[0].Count)
                {
                    nadmiarowe = (cnt_r[0].Where(u => !cnt_1[0].Any(u1 => (u1.UrzadzenieID == u.UrzadzenieID))).ToList());

                    if (nadmiarowe.Count == 0)
                    {
                        HashSet<Urzadzenie> bez_powtorek = new HashSet<Urzadzenie>(cnt_r[0]);
                        foreach(Urzadzenie u in bez_powtorek.ToList())
                            cnt_r[0].Remove(u);
                        nadmiarowe = cnt_r[0];

                        rodzaj_bledu = "Drzewo RMVB znalazło duplikat(y): ";
                    }
                    else
                    {
                        rodzaj_bledu = "Drzewo RMVB dodatkowo znalazło następujące urządzenia: ";
                    }
                         
                }
                else
                {
                    nadmiarowe = (cnt_1[0].Where(u => !cnt_r[0].Any(u1 => (u1.UrzadzenieID == u.UrzadzenieID))).ToList());

                    if (nadmiarowe.Count == 0)
                    {
                        HashSet<Urzadzenie> bez_powtorek = new HashSet<Urzadzenie>(cnt_1[0]);
                        foreach (Urzadzenie u in bez_powtorek.ToList())
                            cnt_r[0].Remove(u);
                        nadmiarowe = cnt_r[0];

                        rodzaj_bledu = "Baza znalazła duplikat(y): ";
                    }
                    else {
                        rodzaj_bledu = "Baza dodatkowo znalazła następujące urządzenia: ";
                    }
                }

                foreach (Urzadzenie u in nadmiarowe)
                {
                    System.Diagnostics.Debug.WriteLine("UrzadzenieID: " + u.UrzadzenieID + " x: " + u.Dlugosc + " y: " + u.Szerokosc);
                }
            }
            System.Diagnostics.Debug.WriteLine("");
            

            if (blad)
            {

            }
            else {
                odnalezione_urzadzenia = cnt_r.Last();
            }

            sukces = true;
            Close();
        }

    }
}
