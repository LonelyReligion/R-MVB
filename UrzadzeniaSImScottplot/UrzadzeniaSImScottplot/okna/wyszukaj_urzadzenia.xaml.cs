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
        }

        double krok_x = 200.0/602.0; //tyle pikseli to minuta(?)
        double krok_y = 200.0/350.0;

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

            Rectangle rect = new Rectangle((decimal)SpinnerYmin.Value, (decimal)SpinnerXmin.Value, (decimal)SpinnerYmax.Value, (decimal)SpinnerXmax.Value);
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

        private int stopnieNaMinuty(decimal stopnie) {
            return (int)(((int)stopnie) * 60 + (stopnie % 1)*100);
        }

        private void _aktualizujObszar() {
            int xMin = (int)(-100 + krok_x * ( -stopnieNaMinuty(14.08m) + stopnieNaMinuty((decimal)SpinnerXmin.Value)));
            int yMin = (int)(100 - krok_y * ( -stopnieNaMinuty(49) + stopnieNaMinuty((decimal)SpinnerYmin.Value)));
            
            int xMax = (int)(-100 + krok_x * (-stopnieNaMinuty(14.08m) + stopnieNaMinuty((decimal)SpinnerXmax.Value)));
            int yMax = (int)(100 - krok_y * (-stopnieNaMinuty(49) + stopnieNaMinuty((decimal)SpinnerYmax.Value)));

            int szerokosc = xMax - xMin;
            int wysokosc = yMin - yMax;//(int)(krok_y * ((stopnieNaMinuty((decimal)SpinnerYmin.Value) - stopnieNaMinuty((decimal)SpinnerYmax.Value))));

            RectangleGeometry rect = (RectangleGeometry)Obszar.Data;
            rect.Rect = new Rect(xMin, yMax, szerokosc, wysokosc);
        }
        private void SpinnerXmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            if (SpinnerXmin.Value == 14.06m)
                SpinnerXmin.Value = 24.09m;
            else if (SpinnerXmin.Value == 24.10m)
                SpinnerXmin.Value = 14.07m;
            else if ((SpinnerXmin.Value % 1) == 0.99m)
                SpinnerXmin.Value = ((int)(SpinnerXmin.Value / 1)) + 0.59m;
            else if ((SpinnerXmin.Value % 1) == 0.60m)
                SpinnerXmin.Value = ((int)(SpinnerXmin.Value / 1)) + ((int)((SpinnerXmin.Value % 1) * 100) / 60) + ((SpinnerXmin.Value % 1) - 0.6m);
            else
                ;

            if (SpinnerXmax.Value != null && SpinnerXmin.Value >= SpinnerXmax.Value && SpinnerXmin.Value != 24.09m && SpinnerXmax.Value != 14.07m) {
                SpinnerXmax.Value = SpinnerXmin.Value + 0.01m;
            }
            else if (SpinnerXmin.Value == 24.09m)
            {
                SpinnerXmax.Value = 24.09m;
                SpinnerXmin.Value = 24.08m;
            }
            else if (SpinnerXmax.Value == 14.07m)
            {
                SpinnerXmin.Value = 14.07m;
                SpinnerXmax.Value = 14.08m;
            }

            _aktualizujObszar();
        }

        private void SpinnerXmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            if (SpinnerXmax.Value == 14.06m)
                SpinnerXmax.Value = 24.09m;
            else if (SpinnerXmax.Value == 24.10m)
                SpinnerXmax.Value = 14.07m;
            else if ((SpinnerXmax.Value % 1) == 0.99m)
                SpinnerXmax.Value = ((int)(SpinnerXmax.Value / 1)) + 0.59m;
            else if ((SpinnerXmax.Value % 1) == 0.60m)
                SpinnerXmax.Value = ((int)(SpinnerXmax.Value / 1)) + ((int)((SpinnerXmax.Value % 1) * 100) / 60) + ((SpinnerXmax.Value % 1) - 0.6m);
            else
                ;

            if (SpinnerXmin.Value != null && SpinnerXmin.Value >= SpinnerXmax.Value && SpinnerXmin.Value != 24.09m && SpinnerXmax.Value != 14.07m)
            {
                SpinnerXmin.Value = SpinnerXmax.Value - 0.01m;
            }
            else if (SpinnerXmin.Value == 24.09m) {
                SpinnerXmax.Value = 24.09m;
                SpinnerXmin.Value = 24.08m;
            }
            else if (SpinnerXmax.Value == 14.07m)
            {
                SpinnerXmin.Value = 14.07m;
                SpinnerXmax.Value = 14.08m;
            }

            _aktualizujObszar();
        }

        private void SpinnerYmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            if (SpinnerYmin.Value == 48.99m)
                SpinnerYmin.Value = 54.5m;
            else if (SpinnerYmin.Value == 54.51m)
                SpinnerYmin.Value = 49m;
            if ((SpinnerYmin.Value % 1) == 0.99m)
                SpinnerYmin.Value = ((int)(SpinnerYmin.Value / 1)) + 0.59m;
            else if ((SpinnerYmin.Value % 1) == 0.60m)
                SpinnerYmin.Value = ((int)(SpinnerYmin.Value / 1)) + ((int)((SpinnerYmin.Value % 1) * 100) / 60) + ((SpinnerYmin.Value % 1) - 0.6m);
            else
                ;

            if (SpinnerYmax.Value != null && SpinnerYmin.Value >= SpinnerYmax.Value)
            {
                if (SpinnerYmin.Value != 54.5m && SpinnerYmax.Value != 49m)
                {
                    SpinnerYmax.Value = SpinnerYmin.Value + 0.01m;
                }
                else if (SpinnerYmin.Value == 54.5m)
                {
                    SpinnerYmax.Value = 54.5m;
                    SpinnerYmin.Value = 54.49m;
                }
                else if (SpinnerYmax.Value == 49m)
                {
                    SpinnerYmin.Value = 49m;
                    SpinnerYmax.Value = 49.01m;
                }
            }

            _aktualizujObszar();
        }

        private void SpinnerYmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            if (SpinnerYmax.Value == 48.99m)
                SpinnerYmax.Value = 54.5m;
            else if (SpinnerYmax.Value == 54.51m)
                SpinnerYmax.Value = 49m;
            if ((SpinnerYmax.Value % 1) == 0.99m)
                SpinnerYmax.Value = ((int)(SpinnerYmax.Value / 1)) + 0.59m;
            else if ((SpinnerYmax.Value % 1) == 0.60m)
                SpinnerYmax.Value = ((int)(SpinnerYmax.Value / 1)) + ((int)((SpinnerYmax.Value % 1) * 100) / 60) + ((SpinnerYmax.Value % 1) - 0.6m);
            else
                ;

            if (SpinnerYmin.Value != null && SpinnerYmin.Value >= SpinnerYmax.Value && SpinnerYmin.Value != 54.5m && SpinnerYmax.Value != 49m)
            {
                SpinnerYmin.Value = SpinnerYmax.Value - 0.01m;
            }
            else if (SpinnerYmin.Value == 54.5m)
            {
                SpinnerYmax.Value = 54.5m;
                SpinnerYmin.Value = 54.49m;
            }
            else if (SpinnerYmax.Value == 49m)
            {
                SpinnerYmin.Value = 49m;
                SpinnerYmax.Value = 49.01m;
            }

            _aktualizujObszar();
        }

    }
}
