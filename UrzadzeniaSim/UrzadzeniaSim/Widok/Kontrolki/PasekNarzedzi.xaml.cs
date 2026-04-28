using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UrzadzeniaSim.Widok.Okna_dialogowe;

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
        
        public event Action wyczysc_wszystko;
        public event Action dodaj_losowe;
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
            
            if (procenty_i >= 250)
            {
                poludniki_minuta.IsEnabled = true;
                rownolezniki_minuta.IsEnabled = true;
            }
            else {
                poludniki_minuta.IsEnabled = false;
                rownolezniki_minuta.IsEnabled = false;

                poludniki_minuta.IsChecked = false;
                rownolezniki_minuta.IsChecked = false;

                poludniki_minuta.ToolTip = "Aby zwiększyć dokładność ustaw powiększenie większe lub równe 250%";
                rownolezniki_minuta.ToolTip = "Aby zwiększyć dokładność ustaw powiększenie większe lub równe 250%";
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

        private void KliknietoWyczysc(object sender, RoutedEventArgs e)
        {
            wyczysc_wszystko?.Invoke();
        }

        private void DodajLosoweUrzadzenie(object sender, RoutedEventArgs e)
        {
            dodaj_losowe?.Invoke();
        }

        private void DodajUrzadzenie(object sender, RoutedEventArgs e)
        {
            TworzenieUrzadzenia dialog = new TworzenieUrzadzenia();
            dialog.ShowDialog(); //zatrzymujemy glowne okno
        }
    }
}
