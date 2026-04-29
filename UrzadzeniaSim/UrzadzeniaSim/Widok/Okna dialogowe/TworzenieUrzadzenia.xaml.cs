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
        public Decimal dlugosc { get; set; }
        public Decimal szerokosc { get; set; }

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

        private void dlugosc_spinner_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((dlugosc_spinner.Value % 1) == 0.99m)
                dlugosc_spinner.Value = ((int)(dlugosc_spinner.Value / 1)) + 0.59m;
            else if ((dlugosc_spinner.Value % 1) == 0.60m)
                dlugosc_spinner.Value = ((int)(dlugosc_spinner.Value / 1)) + ((int)((dlugosc_spinner.Value % 1) * 100) / 60) + ((dlugosc_spinner.Value % 1) - 0.6m);
            else
                ;
        }

        private void szerokosc_spinner_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((szerokosc_spinner.Value % 1) == 0.99m)
                szerokosc_spinner.Value = ((int)(szerokosc_spinner.Value / 1)) + 0.59m;
            else if ((szerokosc_spinner.Value % 1) == 0.60m)
                szerokosc_spinner.Value = ((int)(szerokosc_spinner.Value / 1)) + ((int)((szerokosc_spinner.Value % 1) * 100) / 60) + ((szerokosc_spinner.Value % 1) - 0.6m);
            else
                ;
        }
    }
}
