using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Model.DB;
using Xceed.Wpf.Toolkit.Primitives;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy PanelBoczny.xaml
    /// </summary>
    public partial class PanelBoczny : UserControl, INotifyPropertyChanged
    {
        public static Repo repo;
        public event PropertyChangedEventHandler? PropertyChanged;

        private double oryginalna_wysokosc;
        private double oryginalna_szerokosc;

        private const int bazowa_wielkosc_czcionki = 15;
        
        public static Kontekst ctx;

        private string wielkosc_czcionki = "12";

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
            Statusy.ItemsSource = new List<string>{ "AKTYWNY", "NIEAKTYWNY", "NADAJE" };
            
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
              

            };
        }
        private Urzadzenie_Model wyswietlane;
        public void uzupelnijInformacjeOurzadzeniu(int id_urzadzenia) {
            if (id_urzadzenia == -1) {
                wyswietlane = null;
                //odznaczamy
                Label_ID.Content = "UrzadzenieID:";
                Label_Dlugosc.Content = "Długość:";
                Label_Szerokosc.Content = "Szerokość:";
            }
            else
            {
                wyswietlane = ctx.Urzadzenia.Where(p => p.UrzadzenieID == id_urzadzenia).First();
                Label_ID.Content = "UrzadzenieID: " + wyswietlane.UrzadzenieID;
                Label_Dlugosc.Content = "Długość: " + (int)wyswietlane.Dlugosc + "°" + (int)((wyswietlane.Dlugosc % 1) * 100) + "'" + "E"; //poprawic wyswietlanie ze znaczkami minuty i stopni
                Label_Szerokosc.Content = "Szerokość: " + (int)wyswietlane.Szerokosc + "°" + (int)((wyswietlane.Szerokosc % 1) * 100) + "'" + "N";

                if (repo.czyJestAktywne(wyswietlane.UrzadzenieID))
                    Statusy.SelectedIndex = 0; //na razie nie mamy jak sprawdzic czy nadaje tutaj
                else
                    Statusy.SelectedIndex = 1;
            }
        }

        private void Statusy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Statusy.SelectedIndex == 0) //Aktywny
            {

            }
            else if (Statusy.SelectedIndex == 1)//Nieaktywny 
            {

            }
            else //Nadaje
            { 
            
            }

        }
    }
}
