using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy Urządzenie.xaml
    /// </summary>
    public enum STATUS
    {
        NIEAKTYWNY,//wyszarzony?
        AKTYWNY,
        AKTYWNY_NADAJE
    };

    public partial class Urządzenie : UserControl, INotifyPropertyChanged
    {
        private static readonly Color _kolorAktywny = (Color)ColorConverter.ConvertFromString("#5C7D60");
        private static readonly Color _kolorNieaktywny = (Color)ColorConverter.ConvertFromString("#9AB7D6");
        private static readonly Color _kolorNadajnik = (Color)ColorConverter.ConvertFromString("#BE756F");

        private static readonly Color _kolorZaznaczenia = Colors.Blue;

        public event Action<int> Zaznaczono;
        private bool _zaznaczone = false;

        public decimal Dlugosc;
        public decimal Szerokosc;

        private int _idSiatka;

        private const int _orygSzerokoscWysokoscZaznaczenia = 7;
        private const int _orygSzerokoscWysokosc = 5;
        private const double _orygWysokoscSzerokoscOkregu = 6;
        private const double _orygMaxWysokoscSzerokoscOkregu = 10;
        private const double _orygGruboscOkregu = 1;

        public event PropertyChangedEventHandler? PropertyChanged;
        private double _szerokoscWysokoscZaznaczenia = _orygSzerokoscWysokoscZaznaczenia;

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

        private double _wysokoscSzerokoscOkregu = _orygWysokoscSzerokoscOkregu;
        public double WysokoscSzerokoscOkregu
        {
            get { return _wysokoscSzerokoscOkregu; }
            set
            {
                _wysokoscSzerokoscOkregu = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WysokoscSzerokoscOkregu"));
            }
        }

        private double _wysokoscMaxSzerokoscOkregu = _orygMaxWysokoscSzerokoscOkregu;
        public double MaxWysokoscSzerokoscOkregu
        {
            get { return _wysokoscMaxSzerokoscOkregu; }
            set
            {
                _wysokoscMaxSzerokoscOkregu = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxWysokoscSzerokoscOkregu"));

                if (animujemy)
                    _resetujAnimacje();
            }
        }

        private Brush _kolorUrzadzenia;
        public Brush KolorUrzadzenia
        {
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

        private double _gruboscOkregu = _orygGruboscOkregu;
        public double GruboscOkregu
        {
            get { return _gruboscOkregu; }
            set
            {
                _gruboscOkregu = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GruboscOkregu"));
            }
        }
        public static readonly DependencyProperty IsAnimatedProperty =
    DependencyProperty.Register(
        nameof(IsAnimated),
        typeof(bool),
        typeof(Urządzenie),
        new PropertyMetadata(false));
        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        // te zmienne dotyczą okna generowania
        public STATUS StatusUrzadzenia = STATUS.AKTYWNY;
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        public CancellationToken Token;
        public int Interwal = 0;
        public int? IleCykli = null;
        //

        Grid siatka;
        Storyboard sb;

        public Urządzenie(decimal dlugosc, decimal szerokosc)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Dlugosc = dlugosc;
            this.Szerokosc = szerokosc;
            _kolorUrzadzenia = new SolidColorBrush(_kolorAktywny);

            this.Loaded += (sender, e) =>
            {
                siatka = (Grid)przycisk.Template.FindName("siatka", przycisk);
                sb = (Storyboard)siatka.FindResource("AnimacjaGenerowania");
            };

        }

        bool animujemy = false;

        //#00FFFFFF
        private static readonly Brush _domyslnyKolorOkregu = (SolidColorBrush)new BrushConverter().ConvertFrom("Transparent");
        private Brush _kolorOkregu = _domyslnyKolorOkregu;
        public Brush KolorOkregu
        {
            get { return _kolorOkregu; }
            set
            {
                _kolorOkregu = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KolorOKregu"));
            }
        }

        public void ZatrzymajAnimacje()
        {
            if (!this.IsLoaded)
                return;

            sb.Stop();

            animujemy = false;
            Ellipse krag_generowania = (Ellipse)przycisk.Template.FindName("krag_generowania", przycisk);
            KolorOkregu = (SolidColorBrush)new BrushConverter().ConvertFrom("Transparent");

            IsAnimated = false;
        }
        public void UruchomAnimacje()
        {
            if (!this.IsLoaded)
                return;

            sb.Begin();

            animujemy = true;
            KolorOkregu = (SolidColorBrush)new BrushConverter().ConvertFrom("Red");
            IsAnimated = true;
        }
        private void _resetujAnimacje()
        {
            ZatrzymajAnimacje();
            UruchomAnimacje();
        }

        public void UstawIdSiatka(int id)
        {
            _idSiatka = id;
        }

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

        public void Odznacz()
        {
            WypelnienieZaznaczenia = Brushes.Transparent;
            _zaznaczone = false;
        }

        public void Aktywuj()
        {
            KolorUrzadzenia = new SolidColorBrush(_kolorAktywny);
            StatusUrzadzenia = STATUS.AKTYWNY;
        }

        public void Dezktywuj()
        {
            KolorUrzadzenia = new SolidColorBrush(_kolorNieaktywny);
            StatusUrzadzenia = STATUS.NIEAKTYWNY;
        }


        public void Emituj()
        {
            KolorUrzadzenia = new SolidColorBrush(_kolorNadajnik);
            StatusUrzadzenia = STATUS.AKTYWNY_NADAJE;
        }

        public void Skaluj(double skala)
        {
            Width = Math.Max(14 * skala, 14);
            Height = Math.Max(14 * skala, 14);

            SzerokoscWysokoscZaznaczenia = Math.Max(Urządzenie._orygSzerokoscWysokoscZaznaczenia * skala, Urządzenie._orygSzerokoscWysokoscZaznaczenia);
            SzerokoscWysokosc = Math.Max(Urządzenie._orygSzerokoscWysokosc * skala, Urządzenie._orygSzerokoscWysokosc);
            WysokoscSzerokoscOkregu = Math.Max(Urządzenie._orygWysokoscSzerokoscOkregu * skala, Urządzenie._orygWysokoscSzerokoscOkregu);
            MaxWysokoscSzerokoscOkregu = Math.Max(Urządzenie._orygMaxWysokoscSzerokoscOkregu * skala, Urządzenie._orygMaxWysokoscSzerokoscOkregu);
            GruboscOkregu = Math.Max(Urządzenie._orygGruboscOkregu * skala, Urządzenie._orygGruboscOkregu);
        }

    }
}
