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
        private static Color kolor_aktywny = (Color)ColorConverter.ConvertFromString("#5C7D60");
        private static Color kolor_nieaktywny = (Color)ColorConverter.ConvertFromString("#9AB7D6");
        private static Color kolor_nadajnik = (Color)ColorConverter.ConvertFromString("#BE756F");
        private static List<Color> kolory = new List<Color>
        {
            kolor_nieaktywny,
            kolor_aktywny,
            kolor_nadajnik
        };
        private static Color kolor_zaznaczenia = Colors.Blue;

        public event Action<int> zaznaczono;
        bool zaznaczone = false;

        public decimal dlugosc;
        public decimal szerokosc;

        private int id_siatka;

        public const int oryg_szerokosc_wysokosc_zaznaczenia = 7;
        public const int oryg_szerokosc_wysokosc = 5;

        public event PropertyChangedEventHandler? PropertyChanged;
        private double _szerokosc_wysokosc_zaznaczenia = oryg_szerokosc_wysokosc_zaznaczenia;

        public double szerokosc_wysokosc_zaznaczenia
        {
            get
            {
                return _szerokosc_wysokosc_zaznaczenia;
            }
            set
            {
                _szerokosc_wysokosc_zaznaczenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("szerokosc_wysokosc_zaznaczenia"));
            }
        }

        private double _szerokosc_wysokosc = 5;

        public double szerokosc_wysokosc
        {
            get
            {
                return _szerokosc_wysokosc;
            }
            set
            {
                _szerokosc_wysokosc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("szerokosc_wysokosc"));
            }

        }

        private Brush _kolor_urzadzenia = new SolidColorBrush(kolor_aktywny);
        public Brush kolor_urzadzenia {
            get
            {
                return _kolor_urzadzenia;
            }
            set
            {
                _kolor_urzadzenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("kolor_urzadzenia"));
            }
        }
        private Brush _wypelnienie_zaznaczenia = Brushes.Transparent;
        public Brush wypelnienie_zaznaczenia
        {
            get
            {
                return _wypelnienie_zaznaczenia;
            }
            set
            {
                _wypelnienie_zaznaczenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("wypelnienie_zaznaczenia"));
            }
        }

        public STATUS status_urzadzenia = STATUS.AKTYWNY;
        public Urządzenie(decimal dlugosc, decimal szerokosc)
        {
            InitializeComponent();
            this.DataContext = this;
            this.dlugosc = dlugosc;
            this.szerokosc = szerokosc;
        }

        public void ustaw_id_siatka(int id) { 
            id_siatka = id;
        }

        //czy musi byc public?
        public void Zaznacz(object sender, RoutedEventArgs e)
        {
            Zaznacz();
        }

        public void Zaznacz()
        {
            zaznaczone = !zaznaczone;

            if (zaznaczone)
            {
                wypelnienie_zaznaczenia = new SolidColorBrush(kolor_zaznaczenia);
            }
            else
            {
                wypelnienie_zaznaczenia = Brushes.Transparent;
            }

            zaznaczono?.Invoke(this.id_siatka);
        }

        public void Odznacz() {
            wypelnienie_zaznaczenia = Brushes.Transparent;
            zaznaczone = false;
        }

        public void Aktywuj() {
            kolor_urzadzenia = new SolidColorBrush(kolor_aktywny);
            status_urzadzenia = STATUS.AKTYWNY;
        }

        public void Dezktywuj() {
            kolor_urzadzenia = new SolidColorBrush(kolor_nieaktywny);
            status_urzadzenia = STATUS.NIEAKTYWNY;
        }

        public int Interwal = 0;
        public int? IleCykli = null;
        public void Emituj() {
            kolor_urzadzenia = new SolidColorBrush(kolor_nadajnik);
            status_urzadzenia = STATUS.AKTYWNY_NADAJE;
        }

    }
}
