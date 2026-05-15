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
        //zrob jakas akcje co bedzie informowala ze sie zmienilo pole informujace o tym czy aktualnie generujwmy podepnij do metody w panelu
        //int to id urzadzenia
        public event Action<int> ZmieniloSieCzyGenerujemy;
        private PanelBoczny _rodzic;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static HashSet<int> OtwarteOkna = new HashSet<int>();

        private Urzadzenie_Model _urzadzenie;
        private Urządzenie _urzadzenie_gui;
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
        public Generowanie(PanelBoczny kontolkaRodzic, Urzadzenie_Model urzadzenie)
        {
            _rodzic = kontolkaRodzic;

            DataContext = this;
            id = urzadzenie.UrzadzenieID;
            _urzadzenie = urzadzenie;
            _urzadzenie_gui = urzadzenie.punkt;

            _urzadzenie_gui.token = _urzadzenie_gui.cancellationTokenSource.Token;
            InitializeComponent();

            OtwarteOkna.Add(urzadzenie.UrzadzenieID);

            Loaded += Generowanie_Loaded;
        }

        //gdyby bylo w konstruktorze, nie zadzialaloby
        private async void Generowanie_Loaded(object sender, RoutedEventArgs e)
        {
            if (_urzadzenie.punkt.status_urzadzenia == STATUS.AKTYWNY_NADAJE)
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

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            _urzadzenie.punkt.IleCykli = liczbaCykli.Value;
            _urzadzenie.punkt.Interwal = (int)sekundy.Value;
            
            _urzadzenie.CzyGenerujemy = true;
            ZmieniloSieCzyGenerujemy?.Invoke(_id);

            PasekPostepu.IsIndeterminate = true;
            await Task.Yield(); //potrzebne żeby UI się zaktualizowało
            
            _urzadzenie_gui.cancellationTokenSource = new CancellationTokenSource();
            _urzadzenie_gui.token = _urzadzenie_gui.cancellationTokenSource.Token;

            _pracaWtoku = true;
            Task.Run(async () => 
                {
                    _urzadzenie.punkt.status_urzadzenia = STATUS.AKTYWNY_NADAJE;
                    while (!_urzadzenie_gui.token.IsCancellationRequested)
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
            _urzadzenie.CzyGenerujemy = false;
            ZmieniloSieCzyGenerujemy?.Invoke(_id);

            _urzadzenie_gui.cancellationTokenSource.Cancel(); //to nie jest zatrzymanie tylko uprzejma prośba
            _urzadzenie.punkt.status_urzadzenia = STATUS.AKTYWNY;

            PasekPostepu.IsIndeterminate = false;

            _pracaWtoku = false;

            if (sekundy.Value != null && liczbaCykli.Value != null)
                Start.IsEnabled = true;
            else 
                Start.IsEnabled = false;
            Stop.IsEnabled = false;

        }

        private void sekundy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_pracaWtoku == false && (sekundy.Value == null || liczbaCykli.Value == null)) {
                Start.IsEnabled = false;
                Stop.IsEnabled = false;
            }

            if(_pracaWtoku == false && sekundy.Value != null && liczbaCykli.Value != null)
                Start.IsEnabled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            OtwarteOkna.Remove(_id);
        }
    }
}
