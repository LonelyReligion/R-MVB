using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Model.DB;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy PanelBoczny.xaml
    /// </summary>
    public partial class PanelBoczny : UserControl, INotifyPropertyChanged
    {
        public event Action DodajUrzadzenie;
        public event PropertyChangedEventHandler? PropertyChanged;

        private double oryginalna_wysokosc;
        private double oryginalna_szerokosc;

        private const int bazowa_wielkosc_czcionki = 15;
        
        public static Kontekst ctx;

        private string wielkosc_czcionki = "15";

        public string Wielkosc_czcionki 
        {
            get 
            {
                return wielkosc_czcionki;
            }
            set 
            {
                wielkosc_czcionki = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Wielkosc_czcionki"));
            } 
        }

        private double oryginalna_wysokosc_przycisku = 30;
        private double oryginalna_szerokosc_przycisku = 120;

        private bool mamy_wymiary = false;
        public PanelBoczny()
        {
            DataContext = this;
            InitializeComponent();
            
            panel.SizeChanged += (s,e) => 
            {

                double wysokosc_panelu = panel.ActualHeight;
                double szerokosc_panelu = panel.ActualWidth;

                if (!mamy_wymiary) {
                    mamy_wymiary = true;
                    oryginalna_wysokosc = wysokosc_panelu;
                    oryginalna_szerokosc = szerokosc_panelu;
                }

                double skala_x = szerokosc_panelu / oryginalna_szerokosc;
                double skala_y = wysokosc_panelu / oryginalna_wysokosc;
                double skala = Math.Min(skala_x, skala_y);

                Wielkosc_czcionki = (skala * (double)bazowa_wielkosc_czcionki).ToString();
                
                przycisk_generuj.Height = oryginalna_wysokosc_przycisku * skala;
                przycisk_generuj.Width = oryginalna_szerokosc_przycisku * skala;
                przycisk_generuj.Margin = new Thickness(0, 0, 0, 10 * skala );

            };
        }

        public void ZlecGenerowanie(object sender, RoutedEventArgs e) {
            DodajUrzadzenie?.Invoke();
        }

        public void uzupelnijInformacjeOurzadzeniu(int id_urzadzenia) {
            if (id_urzadzenia == -1) {
                //odznaczamy
                Label_ID.Content = "UrzadzenieID:";
                Label_Dlugosc.Content = "Długość:";
                Label_Szerokosc.Content = "Szerokość:";
            }
            else
            {
                Urzadzenie_Model szukane = ctx.Urzadzenia.Where(p => p.UrzadzenieID == id_urzadzenia).First();
                Label_ID.Content = "UrzadzenieID: " + szukane.UrzadzenieID;
                Label_Dlugosc.Content = "Długość: " + (int)szukane.Dlugosc + "°" + (int)((szukane.Dlugosc % 1) * 100) + "'" + "E"; //poprawic wyswietlanie ze znaczkami minuty i stopni
                Label_Szerokosc.Content = "Szerokość: " + (int)szukane.Szerokosc + "°" + (int)((szukane.Szerokosc % 1) * 100) + "'" + "N";
            }
        }
    }
}
