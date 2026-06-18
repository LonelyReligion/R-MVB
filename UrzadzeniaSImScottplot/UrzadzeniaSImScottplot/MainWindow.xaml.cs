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

        private void przycisk_wyszukaj_urzadzenia_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}