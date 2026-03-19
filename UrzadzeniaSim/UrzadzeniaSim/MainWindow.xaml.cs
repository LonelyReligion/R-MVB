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

        //Właściwie to siatka walcowa?
        private void rysujSiatkeGeograficzna() {

            int margines = 100;

            int liczba_poludnikow_grubych = 10;
            int liczba_równoleżników_grubych = 5;

            double krokX = (szerokosc_plotna - 2 * margines) / (double)(liczba_poludnikow_grubych - 1);
            double krokY = (wysokosc_plotna - 2 * margines) / (double)(liczba_równoleżników_grubych - 1);

            double pierwszy_poludnik_x = margines - 53 * krokX / 60.0;
            double ostatni_poludnik_x = margines + (liczba_poludnikow_grubych - 1) * krokX + 9 * krokX / 60.0;
            double ostatni_rownoleznik_y = margines + (liczba_równoleżników_grubych - 1) * krokY + 5 / 60.0 * krokY;

            Line pierwszy_poludnik = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5,
                X1 = pierwszy_poludnik_x,
                X2 = pierwszy_poludnik_x,
                Y1 = margines,
                Y2 = ostatni_rownoleznik_y
            }; //ten 14 st. 7 min.
            plotno.Children.Add(pierwszy_poludnik);

            for (int i = 0; i < liczba_poludnikow_grubych; i++)
            {
                Line poludnik = new Line
                {
                    Visibility = Visibility.Visible,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    X1 = margines + i * krokX,
                    X2 = margines + i * krokX,
                    Y1 = margines,
                    Y2 = ostatni_rownoleznik_y
                };
                plotno.Children.Add(poludnik);

                //umiescic generowanie poludnikow przy przyblizeniu tych mniejszych
            }

            Line ostatni_poludnik = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5,
                X1 = margines + (liczba_poludnikow_grubych - 1) * krokX + 9 * krokX / 60.0,
                X2 = margines + (liczba_poludnikow_grubych - 1) * krokX + 9 * krokX / 60.0,
                Y1 = margines,
                Y2 = ostatni_rownoleznik_y
            };
            plotno.Children.Add(ostatni_poludnik);
            //ten 24 st. 09 min.

            for (int i = 0; i < liczba_równoleżników_grubych; i++)
            {
                Line rownoleznik = new Line
                {
                    Visibility = Visibility.Visible,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    X1 = pierwszy_poludnik_x,
                    X2 = ostatni_poludnik_x,
                    Y1 = margines + i * krokY,
                    Y2 = margines + i * krokY
                };
                plotno.Children.Add(rownoleznik);

                //umiescic generowanie rownoleznikow przy przyblizeniu tych mniejszych
            }

            Line ostatni_rownoleznik = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = pierwszy_poludnik_x,
                X2 = ostatni_poludnik_x,
                Y1 = ostatni_rownoleznik_y,
                Y2 = ostatni_rownoleznik_y
            };//ten 54 st. 5 min.
            plotno.Children.Add(ostatni_rownoleznik);
        }
    }
    
}