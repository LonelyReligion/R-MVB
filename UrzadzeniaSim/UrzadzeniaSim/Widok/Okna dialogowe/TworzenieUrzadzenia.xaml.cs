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

namespace UrzadzeniaSim.Widok.Okna_dialogowe
{
    /// <summary>
    /// Logika interakcji dla klasy TworzenieUrzadzenia.xaml
    /// </summary>
    public partial class TworzenieUrzadzenia : Window
    {
        public bool sukces { get; set; } //czy otrzymalismy poprawne dane

        public Decimal dlugosc = new Decimal(14.07);

        public Decimal szerokosc = new Decimal(49);

        public TworzenieUrzadzenia()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Przeslij_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        bool dlugosc_set = false;
        private void dlugosc_spinner_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            dlugosc_set = true;
            if(szerokosc_set)
                Przeslij.IsEnabled = true;

            if (dlugosc_spinner.Value == 14.06m)
                dlugosc_spinner.Value = 24.09m;
            else if (dlugosc_spinner.Value == 24.10m)
                dlugosc_spinner.Value = 14.07m;
            else if ((dlugosc_spinner.Value % 1) == 0.99m)
                dlugosc_spinner.Value = ((int)(dlugosc_spinner.Value / 1)) + 0.59m;
            else if ((dlugosc_spinner.Value % 1) == 0.60m)
                dlugosc_spinner.Value = ((int)(dlugosc_spinner.Value / 1)) + ((int)((dlugosc_spinner.Value % 1) * 100) / 60) + ((dlugosc_spinner.Value % 1) - 0.6m);
            else
                ;
        }

        bool szerokosc_set = false;
        private void szerokosc_spinner_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            szerokosc_set = true;
            if(dlugosc_set)
                Przeslij.IsEnabled = true;

            if (szerokosc_spinner.Value == 48.99m)
                szerokosc_spinner.Value = 54.5m;
            else if (szerokosc_spinner.Value == 54.51m)
                szerokosc_spinner.Value = 49m;
            if ((szerokosc_spinner.Value % 1) == 0.99m)
                szerokosc_spinner.Value = ((int)(szerokosc_spinner.Value / 1)) + 0.59m;
            else if ((szerokosc_spinner.Value % 1) == 0.60m)
                szerokosc_spinner.Value = ((int)(szerokosc_spinner.Value / 1)) + ((int)((szerokosc_spinner.Value % 1) * 100) / 60) + ((szerokosc_spinner.Value % 1) - 0.6m);
            else
                ;
        }
    }
}
