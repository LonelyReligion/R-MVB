using System;
using System.Collections.Generic;
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
        bool sukces = false;

        
        public wyszukaj_urzadzenia()
        {
            InitializeComponent();
            DataContext = this;
        }

        double krok_x = 200.0/602.0; //tyle pikseli to minuta(?)
        double krok_y = 200.0/305.0;

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
            sukces = true;
            Close();
        }

        private int stopnieNaMinuty(decimal stopnie) {
            return (int)(((int)stopnie) * 60 + (stopnie % 1)*100);
        }

        private void _aktualizujObszar() {
            int xMin = (int)(-100 + krok_x * ( -stopnieNaMinuty(14.08m) + stopnieNaMinuty((decimal)SpinnerXmin.Value)));
            int yMin = (int)(-100 + krok_y * ( -stopnieNaMinuty(49) + stopnieNaMinuty((decimal)SpinnerYmin.Value)));

            int szerokosc = 100; //(int)(krok_x * ((stopnieNaMinuty((decimal)SpinnerXmin.Value) - stopnieNaMinuty((decimal)SpinnerXmax.Value))));
            int wysokosc = 100;//(int)(krok_y * ((stopnieNaMinuty((decimal)SpinnerYmin.Value) - stopnieNaMinuty((decimal)SpinnerYmax.Value))));

            RectangleGeometry rect = (RectangleGeometry)Obszar.Data;
            rect.Rect = new Rect(xMin, yMin, szerokosc, wysokosc);
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

            System.Diagnostics.Debug.WriteLine($"Old={e.OldValue}, New={e.NewValue}, Value={SpinnerYmin.Value}");

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
