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
        private Repo _repozytorium = new Repo();
        private Generatory _generator; 
        public void InicjujKontrolki() {
            using (var ctx = new Kontekst())
            {
                SiatkaUrzadzen.ItemsSource = ctx.Urzadzenia.ToList();
                plot.Plot.Axes.SetLimits(14.11, 24.15, 49, 54.83);
            }
        }

        public void AktualizujSiatkeUrzadzen() {
            using (var ctx = new Kontekst())
                SiatkaUrzadzen.ItemsSource = ctx.Urzadzenia.ToList();
        }

        public MainWindow()
        {
            _generator = new Generatory(_repozytorium);
            _repozytorium.InicjujBazeDanych();

            InitializeComponent();
            InicjujKontrolki();
        }

        private void GenerujLosowy_Click(object sender, RoutedEventArgs e)
        {
            (decimal dlugosc, decimal szerokosc) = _generator.generujWspolrzedne();
            Urzadzenie u = new Urzadzenie((dlugosc, szerokosc));

            double[] x = { (double)u.Dlugosc };
            double[] y = { (double)u.Szerokosc};

            plot.Plot.Add.Scatter(x, y);
            plot.Refresh();

            using (var ctx = new Kontekst())
            {
                ctx.Urzadzenia.Add(u);
                ctx.SaveChanges();
            }

            AktualizujSiatkeUrzadzen();
        }
    }
}