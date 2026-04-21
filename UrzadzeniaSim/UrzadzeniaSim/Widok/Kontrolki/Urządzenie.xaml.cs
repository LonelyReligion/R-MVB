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

public partial class Urządzenie : UserControl
    {
        public event Action<int> zaznaczono;
        bool zaznaczone = false;

        public decimal dlugosc;
        public decimal szerokosc;

        private int id_siatka;

        public const int oryg_szerokosc_wysokosc = 6;

        public Urządzenie(decimal dlugosc, decimal szerokosc)
        {
            InitializeComponent();
            this.dlugosc = dlugosc;
            this.szerokosc = szerokosc;
        }

        public void ustaw_id_siatka(int id) { 
            id_siatka = id;
        }

        //czy musi byc public?
        public void Zaznacz(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var zaznaczenie = btn.Template.FindName("zaznaczenie", btn) as Ellipse;

            zaznaczone = !zaznaczone;

            if (zaznaczone)
            {
                zaznaczenie.Fill = new SolidColorBrush(Colors.Red);
            }
            else 
            {
                zaznaczenie.Fill = new SolidColorBrush(Colors.Transparent);
            }

            zaznaczono?.Invoke (this.id_siatka);
        }

        public void Odznacz() {
            var zaznaczenie = (Ellipse)przycisk.Template.FindName("zaznaczenie", przycisk);
            zaznaczenie.Fill = new SolidColorBrush(Colors.Transparent);
            zaznaczone = false;
        }
    }
}
