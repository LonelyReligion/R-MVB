using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Narzędzia;

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
    }
}
