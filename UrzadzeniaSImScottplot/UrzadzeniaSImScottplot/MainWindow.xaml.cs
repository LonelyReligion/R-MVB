using ScottPlot;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UrzadzeniaSImScottplot.okna;
using UrzadzeniaSImScottplot.narzedzia;
using UrzadzeniaSImScottplot.repo;

namespace UrzadzeniaSImScottplot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Repo _repozytorium;
        private Generatory _generator; 
        private RMVB _rmvb = new RMVB();
        public void InicjujKontrolki() {
            using (var ctx = new Kontekst())
            {
                generuj_pomiary.aktualizujTabele += AktualizujSiatkeUrzadzen;

                SiatkaUrzadzen.ItemsSource = ctx.Urzadzenia.ToList();
                SiatkaUrzadzen.AutoGeneratingColumn += generowanieKolumn;
                TabelaWynikow.AutoGeneratingColumn += generowanieKolumn;
                
                DataGridTextColumn liczba_pomiarow =  new DataGridTextColumn();
                liczba_pomiarow.Header = "Liczba pomiarów";
                liczba_pomiarow.Binding = new Binding("LiczbaPomiarow");

                DataGridTextColumn aktywne = new DataGridTextColumn();
                liczba_pomiarow.Header = "Aktywne";
                liczba_pomiarow.Binding = new Binding("Aktywne");

                plot.Plot.Axes.SetLimits(14.11, 24.15, 49, 54.83);
                plot.UserInputProcessor.Disable();

            }
        }

        public void generowanieKolumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            if (e.PropertyName == "Wersje" ||e.PropertyName == "Pomiary" || 
                e.PropertyName == "UrzadzenieRodzic" || e.PropertyName == "Aktywne" ||
                e.PropertyName == "Generujemy")
            {
                e.Column.Visibility = Visibility.Collapsed;
            }

            else if (e.PropertyName == "rTimeAggregate")
            {
                e.Column.Header = "Średnia";
            }

            else if (e.PropertyName == "Szerokosc") 
            {
                e.Column.Header = "Szerokość";
            }

            else if (e.PropertyName == "Dlugosc")
            {
                e.Column.Header = "Długość";
            }

            else if (e.PropertyName == "LiczbaPomiarow")
            {
                e.Column.Header = "Liczba pomiarów";
            }


            else if (e.PropertyName == "Aktywne")
            {
                e.Column.IsReadOnly = true;
            }

        }
        public void AktualizujSiatkeUrzadzen() {
            using (var ctx = new Kontekst())
            {
                SiatkaUrzadzen.ItemsSource = ctx.Urzadzenia.ToList();
            }
        }

        public MainWindow()
        {
            _repozytorium = _rmvb.zwrocRepo();
            _generator = new Generatory(_repozytorium);
            _repozytorium.InicjujBazeDanych();

            InitializeComponent();
            InicjujKontrolki();
        }

        private double do_dziesietnego(decimal wartosc) {
            return (int)wartosc + (double)(wartosc - (int)wartosc) * 100 / 60;
        }
        private void GenerujLosowy_Click(object sender, RoutedEventArgs e)
        {
            DodajUrzadzenia okno_generowania = new DodajUrzadzenia(_repozytorium);
            okno_generowania.ShowDialog();

            if (okno_generowania.sukces)
            {
                foreach (Urzadzenie u in okno_generowania.wygenerowane) {
                    var dlugosc = u.Dlugosc;
                    var szerokosc = u.Szerokosc;

                    _rmvb.dodajUrzadzenie(u); //dodaje tez do bazy
                    Wersja pierwsza = new Wersja(u.UrzadzenieID, _repozytorium, _rmvb);
                    _rmvb.dodajWersje(pierwsza);

                    double dlugosc_w_systemie_dziesietnym = do_dziesietnego(dlugosc);
                    double szerokosc_w_systemie_dziesietnym = do_dziesietnego(szerokosc);

                    double[] x = { dlugosc_w_systemie_dziesietnym };
                    double[] y = { szerokosc_w_systemie_dziesietnym };

                    var sp = plot.Plot.Add.Scatter(x, y);
                    sp.Color = ScottPlot.Color.FromHex("#6F9942");
                }
            }
            else 
            { 
            
            }

            plot.Refresh();
            AktualizujSiatkeUrzadzen();
        }



        private void przycisk_wyszukaj_urzadzenia_Click(object sender, RoutedEventArgs e)
        {
            wyszukaj_urzadzenia okno = new wyszukaj_urzadzenia(_generator, _rmvb);
            okno.ShowDialog();

            //TabelaWynikow.Items.Clear();
            if (okno.sukces) {
                if (!okno.blad)
                {
                    //wyswietlic czasy i liczby odnalezionych urzadzen, jezeli sa zgodne to odnalezione urzadzenia w tabeli
                    int liczba = okno.odnalezione_urzadzenia.Count();

                    string tekst = $"Zapytanie zwróciło {liczba} " +
                                   (liczba == 1 ? "urządzenie znajdujące się na obszarze: " :
                                    liczba >= 2 && liczba <= 4 ? "urządzenia znajdujące się na obszarze: " :
                                    "urządzeń znajdujących się na obszarze: ");

                    wyniki_pomiarow.Text = tekst + "xMin(" + okno.rect.XMin + ")," + " " + "yMin(" + okno.rect.YMin + "), " + "xMax(" + okno.rect.XMax + "), " + "yMax(" + okno.rect.YMax + ")" +
                        ". Drzewo RMVB zrealizowalo zapytanie dziesięciokrotnie w czasie " + okno.czas_drzewo10.ToString() + " ms." + ", a baza w czasie " + okno.czas_baza10.ToString() + " ms.";

                    //TabelaWynikow.Items.Add(okno.odnalezione_urzadzenia);
                    TabelaWynikow.AutoGenerateColumns = true;
                    TabelaWynikow.ItemsSource = okno.odnalezione_urzadzenia;
                }
                else {
                    wyniki_pomiarow.Text = okno.rodzaj_bledu;
                    TabelaWynikow.ItemsSource = okno.nadmiarowe;
                }
            }
        }

        private void przycisk_wyszukaj_srednia_Click(object sender, RoutedEventArgs e)
        {
            wyszukaj_srednia okno = new wyszukaj_srednia(_rmvb);
            okno.ShowDialog();

            if (okno.sukces)
            {
                if (okno.blad)
                {
                    TabelaWynikow.AutoGenerateColumns = false;

                    TabelaWynikow.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Błąd",
                        Binding = new Binding(".")  
                    });

                    TabelaWynikow.ItemsSource = okno.bledy;
                    wyniki_pomiarow.Text = okno.komunikat_bledu;
                }
                else
                {
                    TabelaWynikow.AutoGenerateColumns = false;
                    if (okno.wariant == 0)
                    {
                        wyniki_pomiarow.Text = "Średnia pomiarów urządzeń znajdujących się na obszarze: xMin(" + okno.szukany.XMin + ")," + " " + "yMin(" + okno.szukany.YMin + "), " + "xMax(" + okno.szukany.XMax + "), " + "yMax(" + okno.szukany.YMax + ") to " + okno.srednia.ToString() + ".";
                    }
                    else if (okno.wariant == 1)
                    {
                        wyniki_pomiarow.Text = "Średnia z pomiarów urządzenia o id: " + okno.id.ToString() + " to " + okno.srednia + ".";
                    }
                    else if (okno.wariant == 2) 
                    {
                        wyniki_pomiarow.Text = "Średnia z pomiarów urządzeń znajdujących się na obszarze: xMin(" + okno.szukany.XMin + ")," + " " + "yMin(" + okno.szukany.YMin + "), " + "xMax(" + okno.szukany.XMax + "), " + "yMax(" + okno.szukany.YMax + ")" +
                             "w okresie czasu od " + okno.poczatek + " do " + okno.koniec + " to " + okno.srednia + ".";
                    }
                    wyniki_pomiarow.Text += " Baza zrealizowała zapytanie dziesięciokrotnie w czasie " + okno.czasBD + " ms., a drzewo RMVB w czasie " + okno.czasRMVB + " ms.";

                }
            }
            else {
                //Anulowano
            }
        }

        private void GenerujPomiary_Click(object sender, RoutedEventArgs e)
        {
            generuj_pomiary okno = new generuj_pomiary(_generator);
            okno.ShowDialog();

            if (okno.sukces) {
                
                foreach (var (id,pomiar) in okno.wygenerowane) {
                    Wersja nowa = new Wersja(id, _repozytorium, _rmvb);
                    _rmvb.dodajWersje(nowa);
                    _rmvb.dodajPomiar(id, pomiar, nowa);
                }
                AktualizujSiatkeUrzadzen();
            }

        }

        private void przycisk_wyszukaj_wersje_Click(object sender, RoutedEventArgs e)
        {
            wyszukiwanie_wersji okno = new wyszukiwanie_wersji(_rmvb);
            okno.ShowDialog();

            if (okno.sukces)
            {
                if (okno.blad) 
                {
                    TabelaWynikow.AutoGenerateColumns = false;

                    TabelaWynikow.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Błąd",
                        Binding = new Binding(".")
                    });

                    if (okno.wariant_tesktu != 2)
                        TabelaWynikow.ItemsSource = okno.bledy;
                    else 
                    {
                        TabelaWynikow.AutoGenerateColumns = true;
                        TabelaWynikow.ItemsSource = okno.nadmiarowe_nieodnalezione;
                    }

                    wyniki_pomiarow.Text = okno.komunikat_bledu;
                }
                else
                {
                    if(okno.wariant_tesktu == 0)
                        wyniki_pomiarow.Text = "Odnaleziono ostatnią wersję urządzenia o id: " + ((int)okno.szukane_id).ToString() + ".";
                    if (okno.wariant_tesktu == 1)
                        wyniki_pomiarow.Text = "Odnaleziono wersję urządzenia o UrządzenieID: " + ((int)okno.szukane_id).ToString() + " i WersjaID: " + okno.szukane_v + ".";
                    if (okno.wariant_tesktu == 2)
                        wyniki_pomiarow.Text = "Odnaleziono następujące wersje urządzeń aktualne od " + ((DateTime)okno.poczatek).ToString() + " do " + ((DateTime)okno.koniec).ToString() +".";

                    wyniki_pomiarow.Text += " Baza zrealizowała zapytanie dziesięciokrotnie w czasie " + okno.czasBD + " ms., a drzewo RMVB w czasie " + okno.czasRMVB + " ms.";

                    //tu dodac wypisywanie czasow wyzej
                    TabelaWynikow.AutoGenerateColumns = true;
                    TabelaWynikow.ItemsSource = okno.odnalezione_wersje;
                }
            }
        }

        private void Dezaktywuj_Click(object sender, RoutedEventArgs e)
        {
            dezaktywuj_urzadzenie okno = new dezaktywuj_urzadzenie(_rmvb);
            okno.ShowDialog();
            AktualizujSiatkeUrzadzen();
        }
    }
}