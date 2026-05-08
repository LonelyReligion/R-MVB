using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Widok.Okna_dialogowe;
using UrzadzeniaSim.Widok.Okna;
using System.Xml.Linq;
using System.ComponentModel;

namespace UrzadzeniaSim.Widok.Kontrolki
{

    /// <summary>
    /// Logika interakcji dla klasy PasekNarzedzi.xaml
    /// </summary>
    public partial class PasekNarzedzi : UserControl, INotifyPropertyChanged
    {
        public double powiekszenie;

        public event Action<double> zmienionoPowiekszenie;
        public event Action<bool> zmienionoDokladnoscPoludniki;
        public event Action<bool> zmienionoDokladnoscRownolezniki;
        
        public event Action wyczysc_wszystko;
        public event Action dodaj_losowe;
        public event Action<(decimal, decimal)> dodaj_urzadzenie;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Window rodzic;
        public Repo repo;
        
        private List<string> _domyslne_powiekszenia = new List<string> { "100%", "150%", "250%", "500%", "1000%" };
        public List<string> domyslne_powiekszenia {
            get {
                return _domyslne_powiekszenia;
            }
            set {
                _domyslne_powiekszenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("domyslne_powiekszenia"));
            }
        }
        public PasekNarzedzi()
        {
            DataContext = this;
            InitializeComponent();
            
            Powiekszenie.SelectedIndex = 0;
        }

        private void zmianaPowiekszenia(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            
            string procenty = Powiekszenie.SelectedItem?.ToString();
            int procenty_i;
            if (procenty == null)
                return;

            procenty_i = int.Parse(procenty.Substring(0, procenty.Length - 1));
        
            Trace.WriteLine("Zmieniono powiekszenie na " + procenty_i);
            powiekszenie = (double)procenty_i / 100;

            if (procenty_i >= 250)
            {
                poludniki_minuta.IsEnabled = true;
                rownolezniki_minuta.IsEnabled = true;
            }
            else
            {
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
            TworzenieUrzadzenia dialog = new TworzenieUrzadzenia(rodzic, repo);
            dialog.ShowDialog(); //zatrzymujemy glowne okno

            if (dialog.sukces == true) {
                dodaj_urzadzenie?.Invoke((dialog.dlugosc, dialog.szerokosc));
            }
        }

        private void Ustawienia_Click(object sender, RoutedEventArgs e)
        {
            Ustawienia okno = new Ustawienia();
            okno.Show();
        }

        private void Powiekszenie_LostFocus(object sender, RoutedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
                return;
            var newItem = comboBox.Text;
            domyslne_powiekszenia.Add(newItem);
            comboBox.SelectedItem = newItem;
        }
    }
}
