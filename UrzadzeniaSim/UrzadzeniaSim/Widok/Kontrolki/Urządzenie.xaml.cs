using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy Urządzenie.xaml
    /// </summary>
    public enum STATUS { 
                NIEAKTYWNY,//wyszarzony?
                AKTYWNY,
                AKTYWNY_NADAJE
    };

    public partial class Urządzenie : UserControl, INotifyPropertyChanged
    {
        private static Color _kolorAktywny = (Color)ColorConverter.ConvertFromString("#5C7D60");
        private static Color _kolorNieaktywny = (Color)ColorConverter.ConvertFromString("#9AB7D6");
        private static Color _kolorNadajnik = (Color)ColorConverter.ConvertFromString("#BE756F");

        private static Color _kolorZaznaczenia = Colors.Blue;

        public event Action<int> Zaznaczono;
        private bool _zaznaczone = false;

        public decimal Dlugosc;
        public decimal Szerokosc;

        private int _idSiatka;

        public const int OrygSzerokoscWysokoscZaznaczenia = 7;
        public const int OrygSzerokoscWysokosc = 5;

        public event PropertyChangedEventHandler? PropertyChanged;
        private double _szerokoscWysokoscZaznaczenia = OrygSzerokoscWysokoscZaznaczenia;

        public double SzerokoscWysokoscZaznaczenia
        {
            get
            {
                return _szerokoscWysokoscZaznaczenia;
            }
            set
            {
                _szerokoscWysokoscZaznaczenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SzerokoscWysokoscZaznaczenia"));
            }
        }

        private double _szerokoscWysokosc = 5;

        public double SzerokoscWysokosc
        {
            get
            {
                return _szerokoscWysokosc;
            }
            set
            {
                _szerokoscWysokosc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SzerokoscWysokosc"));
            }

        }

        private double _wysokoscSzerokoscOkregu = 6;
        public double WysokoscSzerokoscOkregu
        {
            get { return _wysokoscSzerokoscOkregu; }
            set {
                _wysokoscSzerokoscOkregu = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WysokoscSzerokoscOkregu"));
            }
        }

        private Brush _kolorUrzadzenia = new SolidColorBrush(_kolorAktywny);
        public Brush KolorUrzadzenia {
            get
            {
                return _kolorUrzadzenia;
            }
            set
            {
                _kolorUrzadzenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KolorUrzadzenia"));
            }
        }
        private Brush _wypelnienieZaznaczenia = Brushes.Transparent;
        public Brush WypelnienieZaznaczenia
        {
            get
            {
                return _wypelnienieZaznaczenia;
            }
            set
            {
                _wypelnienieZaznaczenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WypelnienieZaznaczenia"));
            }
        }

        // te zmienne dotyczą okna generowania
        public STATUS StatusUrzadzenia = STATUS.AKTYWNY;
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        public CancellationToken Token;
        public int Interwal = 0;
        public int? IleCykli = null;
        //

        public Urządzenie(decimal dlugosc, decimal szerokosc)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Dlugosc = dlugosc;
            this.Szerokosc = szerokosc;
        }

        public void UstawIdSiatka(int id) { 
            _idSiatka = id;
        }

        //czy musi byc public?
        public void Zaznacz(object sender, RoutedEventArgs e)
        {
            Zaznacz();
        }

        public void Zaznacz()
        {
            _zaznaczone = !_zaznaczone;

            if (_zaznaczone)
            {
                WypelnienieZaznaczenia = new SolidColorBrush(_kolorZaznaczenia);
            }
            else
            {
                WypelnienieZaznaczenia = Brushes.Transparent;
            }

            Zaznaczono?.Invoke(this._idSiatka);
        }

        public void Odznacz() {
            WypelnienieZaznaczenia = Brushes.Transparent;
            _zaznaczone = false;
        }

        public void Aktywuj() {
            KolorUrzadzenia = new SolidColorBrush(_kolorAktywny);
            StatusUrzadzenia = STATUS.AKTYWNY;
        }

        public void Dezktywuj() {
            KolorUrzadzenia = new SolidColorBrush(_kolorNieaktywny);
            StatusUrzadzenia = STATUS.NIEAKTYWNY;
        }


        public void Emituj() {
            KolorUrzadzenia = new SolidColorBrush(_kolorNadajnik);
            StatusUrzadzenia = STATUS.AKTYWNY_NADAJE;
        }

    }
}
