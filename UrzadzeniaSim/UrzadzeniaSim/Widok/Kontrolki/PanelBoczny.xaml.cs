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

        private static List<string> statusy = new List<string> { "AKTYWNY", "NIEAKTYWNY", "NADAJE" };
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
            Statusy.ItemsSource = new List<string>(); 
            
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
                Statusy.ItemsSource = new List<string>();
                Label_ID.Content = "";
                Label_Dlugosc.Content = "";
                Label_Szerokosc.Content = "";

                //odznaczamy
            }
            else
            {
                Statusy.ItemsSource = statusy;

                wyswietlane = ctx.Urzadzenia.Where(p => p.UrzadzenieID == id_urzadzenia).First();
                Label_ID.Content = wyswietlane.UrzadzenieID;
                Label_Dlugosc.Content = (int)wyswietlane.Dlugosc + "°" + (int)((wyswietlane.Dlugosc % 1) * 100) + "'" + "E";
                Label_Szerokosc.Content = (int)wyswietlane.Szerokosc + "°" + (int)((wyswietlane.Szerokosc % 1) * 100) + "'" + "N";

                if (repo.czyJestAktywne(wyswietlane.UrzadzenieID))
                    Statusy.SelectedIndex = 0; //na razie nie mamy jak sprawdzic czy nadaje tutaj
                else
                    Statusy.SelectedIndex = 1;
            }
        }

        private void Statusy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Statusy.SelectedItem != null)
            {
                if (Statusy.SelectedIndex == 0) //Aktywny
                {
                    wyswietlane.Aktywuj();
                }
                else if (Statusy.SelectedIndex == 1)//Nieaktywny 
                {
                    wyswietlane.Dezaktywuj();
                }
                else //Nadaje
                {
                    wyswietlane.AddMeasure(new Pomiar(), repo); // tu bedzie sie otwierac okno z opcjami generowania/dodaniem pojedynczego pomiaru
                }
            }
        }
    }
}
