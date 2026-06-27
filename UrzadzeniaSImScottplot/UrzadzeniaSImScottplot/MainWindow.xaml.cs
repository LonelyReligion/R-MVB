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

            plot.Plot.Add.Scatter(x, y);
            plot.Refresh();

            _rmvb.dodajUrzadzenie(u); //dodaje tez do bazy

            AktualizujSiatkeUrzadzen();
        }

        public class Wynik {
            public string liczba_powtorzen { get; set; }
            public string wynik_rmvb { get; set; }
            public string wynik_baza { get; set; }
        }

        private void przycisk_wyszukaj_urzadzenia_Click(object sender, RoutedEventArgs e)
        {
            wyszukaj_urzadzenia okno = new wyszukaj_urzadzenia(_generator, _rmvb);
            okno.ShowDialog();

            if (okno.sukces) {
                // Liczba powtorzen | RMVB | Baza
                //
                //dodac jeszcze liczbe urzadzen *facepalm*
                //nie dodawac kolumn wielokrotnie
                //co z roznymi tabelami dla roznych rodzajow testu? dodac kolumne z rodzajem testu czy resetowac tabele 
                //co z bledami (na razie mamy nieobsluzony wyjatek, zakladamy ze to sie nie wydarzy?)

                DataGridColumn kolumna1 = new DataGridTextColumn {
                    Header = "Liczba powtórzeń",
                    Binding = new Binding("liczba_powtorzen")
                };
                TabelaWynikow.Columns.Add(kolumna1);

                DataGridColumn kolumna2 = new DataGridTextColumn {
                    Header = "RMVB (sek.)",
                    Binding =  new Binding("wynik_rmvb")
                };
                TabelaWynikow.Columns.Add(kolumna2);

                DataGridColumn kolumna3 = new DataGridTextColumn {
                    Header = "Baza (sek. )",
                    Binding = new Binding("wynik_baza")
                };
                TabelaWynikow.Columns.Add(kolumna3);

                TabelaWynikow.Items.Add(new Wynik{ liczba_powtorzen = "10", wynik_rmvb = okno.drzewo10.ToString(), wynik_baza = okno.baza10.ToString() });
            }
        }
    }
}