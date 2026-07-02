using ScottPlot;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
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
using UrzadzeniaSImScottplot;

namespace UrzadzeniaSImScottplot
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

        public wyszukaj_urzadzenia(Generatory gen, RMVB drzewo)
        {
            InitializeComponent();

            DataContext = this;
            _drzewo = drzewo;

            this.ContentRendered += _inicjalizujKontrolki;

        }

        private void _inicjalizujKontrolki(object sender, EventArgs e)
        {
            using (var ctx = new Kontekst())
            {
                int? maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);

                if (maxId == null)
                {
                    Window dialog = (Window)new brak_urzadzen_w_bazie(this);
                    dialog.ShowDialog();
                    Close();
                }
            }
        }

        private void anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        private void przeslij_Click(object sender, RoutedEventArgs e)
        {
            //tu wykonamy test i przekazemy wynik do MainWindow

            Rectangle rect = new Rectangle(prostkat.Ymin, prostkat.Xmin, prostkat.Ymax, prostkat.Xmax);
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

            for (int i = 0; i < 10; i++)
            {
                System.Diagnostics.Debug.WriteLine("Znaleziono " + cnt_r[i].Count.ToString() + "(rt) " + cnt_1[i].Count.ToString() + "(zapytanie w bazie)");
                if (cnt_r[i].Count != cnt_1[i].Count) //a co jezeli znalazla inne, ale liczba się zgadza?
                {

                    List<Urzadzenie> nadmiarowe = new List<Urzadzenie>();
                    if (cnt_r[i].Count > cnt_1[i].Count)
                    {
                        System.Diagnostics.Debug.WriteLine("R-drzewo dodatkowo znalazło następujące urządzenia: ");
                        nadmiarowe = (cnt_r[i].Where(u => !cnt_1[i].Any(u1 => (u1.UrzadzenieID == u.UrzadzenieID))).ToList());
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Baza dodatkowo znalazła następujące urządzenia: ");
                        nadmiarowe = (cnt_1[i].Where(u => !cnt_r[i].Any(u1 => (u1.UrzadzenieID == u.UrzadzenieID))).ToList());
                    }

                    foreach (Urzadzenie u in nadmiarowe)
                    {
                        System.Diagnostics.Debug.WriteLine("UrzadzenieID: " + u.UrzadzenieID + " x: " + u.Dlugosc + " y: " + u.Szerokosc);
                    }
                    throw new Exception("blad w wyszukaj_urzadzenia");
                }
                System.Diagnostics.Debug.WriteLine("");
            }

            if (blad)
            {

            }
            else {
                odnalezione_urzadzenia = cnt_r.Last();
            }

            sukces = !blad;
            Close();
        }

    }
}
