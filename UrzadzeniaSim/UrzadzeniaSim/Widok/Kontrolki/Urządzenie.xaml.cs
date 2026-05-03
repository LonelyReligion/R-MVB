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
        private double szerokosc_wysokosc_zaznaczenia = oryg_szerokosc_wysokosc_zaznaczenia;

        public double Szerokosc_wysokosc_zaznaczenia
        {
            get
            {
                return szerokosc_wysokosc_zaznaczenia;
            }
            set
            {
                szerokosc_wysokosc_zaznaczenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Szerokosc_wysokosc_zaznaczenia"));
            }
        }

        private double szerokosc_wysokosc = 5;

        public double Szerokosc_wysokosc
        {
            get
            {
                return szerokosc_wysokosc;
            }
            set
            {
                szerokosc_wysokosc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Szerokosc_wysokosc"));
            }

        }

        private Brush kolor_urzadzenia = new SolidColorBrush(kolor_aktywny);
        public Brush Kolor_urzadzenia {
            get
            {
                return kolor_urzadzenia;
            }
            set
            {
                kolor_urzadzenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Kolor_urzadzenia"));
            }
        }
        private Brush wypelnienie_zaznaczenia = Brushes.Transparent;
        public Brush Wypelnienie_zaznaczenia
        {
            get
            {
                return wypelnienie_zaznaczenia;
            }
            set
            {
                wypelnienie_zaznaczenia = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Wypelnienie_zaznaczenia"));
            }
        }

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
            zaznaczone = !zaznaczone;

            if (zaznaczone)
            {
                Wypelnienie_zaznaczenia = new SolidColorBrush(kolor_zaznaczenia);
            }
            else 
            {
                Wypelnienie_zaznaczenia = Brushes.Transparent;
            }

            zaznaczono?.Invoke (this.id_siatka);
        }

        public void Odznacz() {
            Wypelnienie_zaznaczenia = Brushes.Transparent;
            zaznaczone = false;
        }

        public void Aktywuj() {
            Kolor_urzadzenia = new SolidColorBrush(kolor_aktywny);
        }

        public void Dezktywuj() {
            Kolor_urzadzenia = new SolidColorBrush(kolor_nieaktywny);
        }
        public void Emituj() {
            Kolor_urzadzenia = new SolidColorBrush(kolor_nadajnik);
        }

    }
}
