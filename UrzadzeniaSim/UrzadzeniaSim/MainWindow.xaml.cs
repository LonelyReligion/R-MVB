using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
        double wysokosc_okna = 450;
        double szerokosc_okna = 800;

        double wysokosc_plotna;
        double szerokosc_plotna;

        bool klik = false;
        public MainWindow()
        {
            InitializeComponent();

            plotno.SizeChanged += (s, e) =>
            {
                wysokosc_plotna = plotno.ActualHeight;
                szerokosc_plotna = plotno.ActualWidth;

                plotno.Children.Clear();
                rysujSiatkeGeograficzna();
            };

        }

        private void rysujSiatkeGeograficzna() {
            
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
                plotno.Children.Add(poludnik);
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
                plotno.Children.Add(rownoleznik);
            }
        }
    }
    
}