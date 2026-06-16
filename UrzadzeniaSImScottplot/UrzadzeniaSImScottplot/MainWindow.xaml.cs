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
        public MainWindow()
        {
            InitializeComponent();
        }

        static int i = 1;
        private void GenerujLosowy_Click(object sender, RoutedEventArgs e)
        {
            Urzadzenie u = new Urzadzenie((i, i));
            double[] x = { (double)u.Dlugosc };
            double[] y = { (double)u.Szerokosc};

            plot.Plot.Add.Scatter(x, y);
            plot.Refresh();
            i++;

            using (var ctx = new Kontekst())
            {
                //dodac to przyspieszenie przed 1 query
                ctx.Urzadzenia.Add(u);
                ctx.SaveChanges();
            }
        }
    }
}