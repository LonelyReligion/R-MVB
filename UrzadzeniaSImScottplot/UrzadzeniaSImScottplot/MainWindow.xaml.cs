using ScottPlot;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                SiatkaUrzadzen.ItemsSource = ctx.Urzadzenia.ToList();

                plot.Plot.Axes.SetLimits(14.11, 24.15, 49, 54.83);
                plot.UserInputProcessor.Disable();

            }
        }

        public void AktualizujSiatkeUrzadzen() {
            using (var ctx = new Kontekst())
                SiatkaUrzadzen.ItemsSource = ctx.Urzadzenia.ToList();
        }

        public MainWindow()
        {
            _repozytorium = _rmvb.zwrocRepo();
            _generator = new Generatory(_repozytorium);
            _repozytorium.InicjujBazeDanych();

            InitializeComponent();
            InicjujKontrolki();
        }

        private void GenerujLosowy_Click(object sender, RoutedEventArgs e)
        {
            (decimal dlugosc, decimal szerokosc) = _generator.generujWspolrzedne();
            Urzadzenie u = new Urzadzenie((dlugosc, szerokosc));

            double dlugosc_w_systemie_dziesietnym = (int)dlugosc + (double)(dlugosc - (int)dlugosc) * 100 / 60;
            double szerokosc_w_systemie_dziesietnym = (int)szerokosc + (double)(szerokosc - (int)szerokosc) * 100 / 60;

            double[] x = { dlugosc_w_systemie_dziesietnym };
            double[] y = { szerokosc_w_systemie_dziesietnym };

            var sp = plot.Plot.Add.Scatter(x, y);
            sp.Color = ScottPlot.Color.FromHex("#6F9942");
            plot.Refresh();

            _rmvb.dodajUrzadzenie(u); //dodaje tez do bazy

            AktualizujSiatkeUrzadzen();
        }



        private void przycisk_wyszukaj_urzadzenia_Click(object sender, RoutedEventArgs e)
        {
            wyszukaj_urzadzenia okno = new wyszukaj_urzadzenia(_generator, _rmvb);
            okno.ShowDialog();

            TabelaWynikow.Items.Clear();
            if (okno.sukces) {
                //wyswietlic czasy i liczby odnalezionych urzadzen, jezeli sa zgodne to odnalezione urzadzenia w tabeli
                int liczba = okno.odnalezione_urzadzenia.Count();

                string tekst = $"Zapytanie zwróciło {liczba} " +
                               (liczba == 1 ? "urządzenie" :
                                liczba >= 2 && liczba <= 4 ? "urządzenia" :
                                "urządzeń");

                wyniki_pomiarow.Text = tekst + ". Drzewo RMVB zrealizowalo zapytanie dziesięciokrotnie w czasie " + okno.czas_drzewo10.ToString() + " ms." + ", a baza w czasie " + okno.czas_baza10.ToString() + " ms.";
                //TabelaWynikow.Items.Add(okno.odnalezione_urzadzenia);
                TabelaWynikow.ItemsSource = okno.odnalezione_urzadzenia;
            }
        }
    }
}