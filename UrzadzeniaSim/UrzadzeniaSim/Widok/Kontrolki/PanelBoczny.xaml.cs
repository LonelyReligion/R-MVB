using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Model.DB;
using Xceed.Wpf.Toolkit.Primitives;
using UrzadzeniaSim.Widok.Okna;

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

        private string _wielkosc_czcionki = "12";

        private static List<string> statusy = new List<string> { "AKTYWNY", "NIEAKTYWNY", "NADAJE" };
        public string wielkosc_czcionki 
        {
            get 
            {
                return _wielkosc_czcionki;
            }
            set 
            {
                _wielkosc_czcionki = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("wielkosc_czcionki"));
            } 
        }

        private double oryginalna_wysokosc_przycisku = 30;
        private double oryginalna_szerokosc_przycisku = 120;

        private bool mamy_wymiary = false;


        public PanelBoczny()
        {
            InitializeComponent();

            listaUrzadzen.ItemsSource = MainWindow.UrzadzeniaUI;
            Statusy.ItemsSource = new List<string>();


            DataContext = this;

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

                wielkosc_czcionki = (skala * (double)bazowa_wielkosc_czcionki).ToString();
              

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

        private void Statusy_SelectionChanged(object sender, EventArgs e)
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
                    if (!Generowanie.OtwarteOkna.Contains(wyswietlane.UrzadzenieID))
                    {
                        Window okno_generowania = new Generowanie(wyswietlane);
                        okno_generowania.Show();
                    }
                    else { 
                        //tu powinnismy dac znac uzytkowanikowi co robi zle
                    }
                }
            }
        }

        private void listaUrzadzen_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName == "Wersje")
                e.Column.Visibility = Visibility.Collapsed;

            if (e.PropertyName == "UrzadzenieID")
            {
                e.Column.Header = "ID";
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
            else {
                e.Column.Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            }
        }

        private void listaUrzadzen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Urzadzenie_Model zaznaczone = listaUrzadzen.SelectedItem as Urzadzenie_Model;
            zaznaczone.punkt.Zaznacz();
        }
    }
}
