using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Narzedzia;
using UrzadzeniaSim.Widok.Kontrolki;

namespace UrzadzeniaSim.Widok.Okna
{
    /// <summary>
    /// Logika interakcji dla klasy Generowanie.xaml
    /// </summary>
    public partial class Generowanie : Window, INotifyPropertyChanged
    {
        private Generatory _generator;
        
        //zrob jakas akcje co bedzie informowala ze sie zmienilo pole informujace o tym czy aktualnie generujwmy podepnij do metody w panelu
        //int to id urzadzenia
        public event Action<int> ZmieniloSieCzyGenerujemy;
        private PanelBoczny _rodzic;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static HashSet<int> OtwarteOkna = new HashSet<int>();

        private Urzadzenie_Model _urzadzenie;
        private Urządzenie _urzadzenieGui;
        private int _id;

        private bool _pracaWtoku = false;
        public bool PracaWtoku {
            get { return _pracaWtoku; }
            set { 
                _pracaWtoku = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PracaWtoku"));
            }
        }

        public int id
        {
            get { return _id; }
            set { _id = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("id")); }
        }
        public Generowanie(PanelBoczny kontolkaRodzic, Urzadzenie_Model urzadzenie, Generatory generator)
        {
            _generator = generator;
            _rodzic = kontolkaRodzic;

            DataContext = this;
            id = urzadzenie.UrzadzenieID;
            _urzadzenie = urzadzenie;
            _urzadzenieGui = urzadzenie.punkt;

            _urzadzenieGui.token = _urzadzenieGui.cancellationTokenSource.Token;
            InitializeComponent();

            OtwarteOkna.Add(urzadzenie.UrzadzenieID);

            Loaded += Generowanie_Loaded;
        }

        //gdyby bylo w konstruktorze, nie zadzialaloby
        private async void Generowanie_Loaded(object sender, RoutedEventArgs e)
        {
            if (_urzadzenieGui.status_urzadzenia == STATUS.AKTYWNY_NADAJE)
            {
                PasekPostepu.IsIndeterminate = true;
                _pracaWtoku = true;

                await Task.Yield();

                Stop.IsEnabled = true;
                Start.IsEnabled = false;

                if (_urzadzenie.punkt.IleCykli != null)
                {
                    liczbaCykli.Value = _urzadzenie.punkt.IleCykli;
                    generowanieZparemetryzowane.IsChecked = true;
                }
                else
                {
                    generowanieCykliczne.IsChecked = true;
                }

                sekundy.Value = _urzadzenie.punkt.Interwal;

            }
        }
        private void _zatrzymajGenerowanie()
        {
            _odblokujPrzyjmowanieDanych();
            _urzadzenie.CzyGenerujemy = false;
            ZmieniloSieCzyGenerujemy?.Invoke(_id);

            _urzadzenieGui.cancellationTokenSource.Cancel(); //to nie jest zatrzymanie tylko uprzejma prośba
            _urzadzenieGui.status_urzadzenia = STATUS.AKTYWNY;

            PasekPostepu.IsIndeterminate = false;

            _pracaWtoku = false;

            if (sekundy.Value != null && liczbaCykli.Value != null)
                Start.IsEnabled = true;
            else
                Start.IsEnabled = false;
            Stop.IsEnabled = false;
        }

        private async void generowaniePomiarowUrzadzenia() {
            
            _pracaWtoku = true;
            _urzadzenie.punkt.status_urzadzenia = STATUS.AKTYWNY_NADAJE;
            int? _liczbaCykliDoKonca = _urzadzenie.punkt.IleCykli;

            while (!_urzadzenieGui.token.IsCancellationRequested && (_liczbaCykliDoKonca == null || _liczbaCykliDoKonca > 0))
            {
                await Task.Delay(_urzadzenie.punkt.Interwal * 1000); //tyle ile w updown
                if (_liczbaCykliDoKonca != null) _liczbaCykliDoKonca -= 1;
            }
            Trace.WriteLine("Zadanie zostało anulowane przez użytkownika lub zakończyło się pomyślnie.");
            //musimy jakos dac znac ze stop

            Application.Current.Dispatcher.Invoke( //glowny watek
            () =>
            {
                _zatrzymajGenerowanie();
            });
            
        }
        private void _zablokujPrzyjmowanieDanych()
        {
            sekundy.IsEnabled = false;
            generowanieZparemetryzowane.IsEnabled = false;
            liczbaCykli.IsEnabled = false;
            generowanieCykliczne.IsEnabled = false;
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            _zablokujPrzyjmowanieDanych();
            _urzadzenieGui.IleCykli = generowanieZparemetryzowane.IsChecked == true ? liczbaCykli.Value : null;
            _urzadzenieGui.Interwal = (int)sekundy.Value;
            
            _urzadzenie.CzyGenerujemy = true;
            ZmieniloSieCzyGenerujemy?.Invoke(_id);

            PasekPostepu.IsIndeterminate = true;

            await Task.Yield(); //potrzebne żeby UI się zaktualizowało
            
            _urzadzenieGui.cancellationTokenSource = new CancellationTokenSource();
            _urzadzenieGui.token = _urzadzenieGui.cancellationTokenSource.Token;

            
            Task.Run(() => generowaniePomiarowUrzadzenia());

            Start.IsEnabled = false;
            Stop.IsEnabled = true;


        }

        private void _odblokujPrzyjmowanieDanych() {
            sekundy.IsEnabled = true;
            generowanieZparemetryzowane.IsEnabled = true;
            liczbaCykli.IsEnabled = true;
            generowanieCykliczne.IsEnabled = true;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _zatrzymajGenerowanie();
        }

        private void sekundy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_pracaWtoku == false && (sekundy.Value == null || (liczbaCykli.Value == null && generowanieZparemetryzowane.IsChecked == true))) {
                Start.IsEnabled = false;
                Stop.IsEnabled = false;
            }

            if(_pracaWtoku == false && sekundy.Value != null && (liczbaCykli.Value != null || generowanieCykliczne.IsChecked == true))
                Start.IsEnabled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            OtwarteOkna.Remove(_id);
            _rodzic.OtwarteOkna.Remove(_id);
        }

        private void generowanieZparemetryzowane_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            if (liczbaCykli.Value == null) {
                Start.IsEnabled = false;
                Stop.IsEnabled = false;
            }
        }
    }
}
