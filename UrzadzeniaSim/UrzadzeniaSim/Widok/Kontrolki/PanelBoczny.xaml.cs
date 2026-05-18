using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Model.DB;
using Xceed.Wpf.Toolkit.Primitives;
using UrzadzeniaSim.Widok.Okna;
using System.Windows.Input;
using UrzadzeniaSim.Narzedzia;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy PanelBoczny.xaml
    /// </summary>
    public partial class PanelBoczny : UserControl, INotifyPropertyChanged
    {
        public Dictionary<int, Generowanie> OtwarteOkna = new Dictionary<int, Generowanie>();
        public static Repo Repozytorium;
        public event PropertyChangedEventHandler? PropertyChanged;

        private double _oryginalnaWysokosc;
        private double _oryginalnaSzerokosc;

        private const int _bazowaWielkoscCzcionki = 15;
        
        public static Kontekst Ctx;

        private string _wielkoscCzcionki = "12";

        private static List<string> s_statusy = new List<string> { "AKTYWNY", "NIEAKTYWNY", "NADAJE" };
        public string WielkoscCzcionki 
        {
            get 
            {
                return _wielkoscCzcionki;
            }
            set 
            {
                _wielkoscCzcionki = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WielkoscCzcionki"));
            } 
        }

        private double _oryginalnaWysokoscPrzycisku = 30;
        private double _oryginalnaSzerokoscPrzycisku = 120;

        private bool _mamyWymiary = false; //przechowuje informację nt. tego czy zczytaliśmy oryginalne wymiary okna
        public static Generatory Generator;
        public bool MozemyZmienicStatus
        {
            get => _wyswietlane != null && !_wyswietlane.CzyGenerujemy;
        }

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

                if (!_mamyWymiary) {
                    _mamyWymiary = true;
                    _oryginalnaWysokosc = wysokosc_panelu;
                    _oryginalnaSzerokosc = szerokosc_panelu;
                }

                double skala_x = szerokosc_panelu / _oryginalnaSzerokosc;
                double skala_y = wysokosc_panelu / _oryginalnaWysokosc;
                double skala = Math.Min(skala_x, skala_y);

                WielkoscCzcionki = (skala * (double)_bazowaWielkoscCzcionki).ToString();
              

            };

        }
        private Urzadzenie_Model _wyswietlane;
        public void uzupelnijInformacjeOurzadzeniu(int id_urzadzenia) {
            if (id_urzadzenia == -1) {
                _wyswietlane = null;
                Statusy.ItemsSource = new List<string>();
                Label_ID.Content = "";
                Label_Dlugosc.Content = "";
                Label_Szerokosc.Content = "";

                //odznaczamy
            }
            else
            {
                Statusy.ItemsSource = s_statusy;

                _wyswietlane = Ctx.Urzadzenia.Where(p => p.UrzadzenieID == id_urzadzenia).First();
                Label_ID.Content = _wyswietlane.UrzadzenieID;
                Label_Dlugosc.Content = (int)_wyswietlane.Dlugosc + "°" + (int)((_wyswietlane.Dlugosc % 1) * 100) + "'" + "E";
                Label_Szerokosc.Content = (int)_wyswietlane.Szerokosc + "°" + (int)((_wyswietlane.Szerokosc % 1) * 100) + "'" + "N";

                if (_wyswietlane.CzyGenerujemy)
                    Statusy.SelectedIndex = 2;
                else if (Repozytorium.czyJestAktywne(_wyswietlane.UrzadzenieID))
                    Statusy.SelectedIndex = 0; //na razie nie mamy jak sprawdzic czy nadaje tutaj
                else
                    Statusy.SelectedIndex = 1;

                //MozemyZmienicStatus = !_wyswietlane.CzyGenerujemy;
            }

            _zaktualizujMozemyZmienicStatus();
        }
        public void ZmienStatusDla(int IdUrzadzenia) { 
            if(_wyswietlane.UrzadzenieID == IdUrzadzenia)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MozemyZmienicStatus)));
        }
        private void _zaktualizujMozemyZmienicStatus() {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MozemyZmienicStatus)));
        }
        private void Statusy_SelectionChanged(object sender, EventArgs e)
        {
            if (Statusy.SelectedItem != null)
            {
                if (Statusy.SelectedIndex == 0) //Aktywny
                {
                    _wyswietlane.Aktywuj();

                    try
                    {
                        OtwarteOkna.ElementAt(_wyswietlane.UrzadzenieID).Value.Close();
                        OtwarteOkna.Remove(_wyswietlane.UrzadzenieID);
                    }
                    catch { 
                        //to nic nie musimy robic
                    }
                }
                else if (Statusy.SelectedIndex == 1)//Nieaktywny 
                {
                    _wyswietlane.Dezaktywuj();
                    try
                    {
                        OtwarteOkna.ElementAt(_wyswietlane.UrzadzenieID).Value.Close();
                        OtwarteOkna.Remove(_wyswietlane.UrzadzenieID);
                    }
                    catch
                    {
                        //to nic nie musimy robic
                    }
                }
                else //Nadaje
                {
                    if (!Generowanie.OtwarteOkna.Contains(_wyswietlane.UrzadzenieID))
                    {
                        Generowanie okno_generowania = new Generowanie(this, _wyswietlane, Generator);
                        okno_generowania.ZmieniloSieCzyGenerujemy += ZmienStatusDla;
                        okno_generowania.Show();

                        OtwarteOkna.Add(_wyswietlane.UrzadzenieID, okno_generowania);
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

        private void RamkaWokolComboboxa_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!MozemyZmienicStatus)
            {
                try
                {
                    OtwarteOkna.ElementAt(_wyswietlane.UrzadzenieID);
                    //nic, juz mamy okno
                }
                catch
                {
                    //otworz okno
                    Generowanie okno_generowania = new Generowanie(this, _wyswietlane, Generator);
                    okno_generowania.ZmieniloSieCzyGenerujemy += ZmienStatusDla;
                    okno_generowania.Show();

                    OtwarteOkna.Add(_wyswietlane.UrzadzenieID, okno_generowania);
                }
            }
        }
    }
}
