using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    
    /// <summary>
    /// Logika interakcji dla klasy PasekNarzedzi.xaml
    /// </summary>
    public partial class PasekNarzedzi : UserControl
    {
        public double powiekszenie;
        public event Action<double> zmienionoPowiekszenie;
        public event Action<bool> zmienionoDokladnoscPoludniki;
        public event Action<bool> zmienionoDokladnoscRownolezniki;

        public PasekNarzedzi()
        {
            InitializeComponent();
            Powiekszenie.ItemsSource = new List<string> { "100%", "150%", "250%", "500%", "1000%" };
            Powiekszenie.SelectedIndex = 0;
        }

        private void zmianaPowiekszenia(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            string procenty = Powiekszenie.SelectedItem?.ToString();
            int procenty_i = int.Parse(procenty.Substring(0, procenty.Length-1));
            Trace.WriteLine("Zmieniono powiekszenie na " + procenty_i);
            powiekszenie = (double) procenty_i / 100;
            
            if (procenty_i >= 500)
            {
                poludniki_minuta.IsEnabled = true;
                rownolezniki_minuta.IsEnabled = true;
            }
            else {
                poludniki_minuta.IsEnabled = false;
                rownolezniki_minuta.IsEnabled = false;
            }

            zmienionoPowiekszenie?.Invoke(powiekszenie);
        }

        private void przycisnieto_poludniki_minuta(object sender, RoutedEventArgs e)
        {
            zmienionoDokladnoscPoludniki?.Invoke(poludniki_minuta.IsChecked);
        }

        private void przycisnieto_rownolezniki_minuta(object sender, RoutedEventArgs e)
        {
            zmienionoDokladnoscRownolezniki?.Invoke(rownolezniki_minuta.IsChecked);
        }
    }
}
