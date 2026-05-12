using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Widok.Kontrolki;

namespace UrzadzeniaSim.Widok.Okna
{
    /// <summary>
    /// Logika interakcji dla klasy Generowanie.xaml
    /// </summary>
    public partial class Generowanie : Window, INotifyPropertyChanged
    {
        public static HashSet<int> OtwarteOkna = new HashSet<int>();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token;

        private Urzadzenie_Model _urzadzenie;
        private int _id;
        private bool pracaWtoku = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int id
        {
            get { return _id; }
            set { _id = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("id")); }
        }
        public Generowanie(Urzadzenie_Model urzadzenie)
        {
            DataContext = this;
            id = urzadzenie.UrzadzenieID;
            token = cancellationTokenSource.Token;
            _urzadzenie = urzadzenie;

            InitializeComponent();

            OtwarteOkna.Add(urzadzenie.UrzadzenieID);

            if (_urzadzenie.punkt.status_urzadzenia == STATUS.AKTYWNY_NADAJE) {
                PasekPostepu.IsIndeterminate = true;
                await Task.Yield();

                Stop.IsEnabled = true;
                Start.IsEnabled = false;

                if (_urzadzenie.punkt.IleCykli != null)
                {
                    liczbaCykli.Value = _urzadzenie.punkt.IleCykli;
                    generowanieZparemetryzowane.IsChecked = true;
                }
                else {
                    generowanieCykliczne.IsChecked = true;
                }

                sekundy.Value = _urzadzenie.punkt.Interwal;

                pracaWtoku = true;
            }
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            _urzadzenie.punkt.IleCykli = liczbaCykli.Value;
            _urzadzenie.punkt.Interwal = (int)sekundy.Value; 

            PasekPostepu.IsIndeterminate = true;
            await Task.Yield(); //potrzebne żeby UI się zaktualizowało
            
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;

            pracaWtoku = true;
            Task.Run(async () => 
                {
                    _urzadzenie.punkt.status_urzadzenia = STATUS.AKTYWNY_NADAJE;
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(1000); //tyle ile w updown
                    }
                    Trace.WriteLine("Zadanie zostało anulowane przez użytkownika lub zakończyło się pomyślnie.");
                }
            );
            Start.IsEnabled = false;
            Stop.IsEnabled = true;


        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel(); //to nie jest zatrzymanie tylko uprzejma prośba
            _urzadzenie.punkt.status_urzadzenia = STATUS.AKTYWNY;

            PasekPostepu.IsIndeterminate = false;

            pracaWtoku = false;

            if (sekundy.Value != null && liczbaCykli.Value != null)
                Start.IsEnabled = true;
            else 
                Start.IsEnabled = false;
            Stop.IsEnabled = false;

        }

        private void sekundy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (pracaWtoku == false && (sekundy.Value == null || liczbaCykli.Value == null)) {
                Start.IsEnabled = false;
                Stop.IsEnabled = false;
            }

            if(pracaWtoku == false && sekundy.Value != null && liczbaCykli.Value != null)
                Start.IsEnabled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            OtwarteOkna.Remove(_id);
        }
    }
}
