using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
        int wysokosc_okna = 450;
        int szerokosc_okna = 800;

        int wysokosc_plotna = 400;
        int szerokosc_plotna = 800;

        bool klik = false;
        public MainWindow()
        {
            InitializeComponent();

            okno.Height = wysokosc_okna;
            okno.Width = szerokosc_okna;
            
            rysujSiatke();
        }

        private void rysujSiatke() {
            int margines = 50;

            int liczba_poludnikow = 10;
            int liczba_równoleżników = 5;

            double krokX = (szerokosc_plotna - 2 * margines) / (double)(liczba_poludnikow - 1);
            double krokY = (wysokosc_plotna - 2 * margines) / (double)(liczba_równoleżników - 1);

            for (int i = 0; i < liczba_poludnikow; i++)
            {
                Line poludnik = new Line
                {
                    Visibility = Visibility.Visible,
                    Stroke = Brushes.Black,
                    X1 = margines + i * krokX,
                    X2 = margines + i * krokX,
                    Y1 = margines,
                    Y2 = wysokosc_plotna - margines
                };
                siatka.Children.Add(poludnik);
            }

            for (int i = 0; i < liczba_równoleżników; i++)
            {
                Line rownoleznik = new Line
                {
                    Visibility = Visibility.Visible,
                    Stroke = Brushes.Black,
                    X1 = margines,
                    X2 = szerokosc_plotna - margines,
                    Y1 = margines + i * krokY,
                    Y2 = margines + i * krokY
                };
                siatka.Children.Add(rownoleznik);
            }
        }
    }
    
}