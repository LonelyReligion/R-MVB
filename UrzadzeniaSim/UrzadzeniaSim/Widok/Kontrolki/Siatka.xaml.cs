using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy Siatka.xaml
    /// </summary>
    public partial class Siatka : UserControl
    {
        double wysokosc_plotna;
        double szerokosc_plotna;
        public Siatka()
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
        private void rysujSiatkeGeograficzna()
        {


            double marginesX = szerokosc_plotna / 8;
            double marginesY = wysokosc_plotna / 4.5;

            int liczba_poludnikow_grubych = 10;
            int liczba_równoleżników_grubych = 5;

            double krokX = Math.Abs((szerokosc_plotna - 2 * marginesX)) / (double)(liczba_poludnikow_grubych - 1);
            double krokY = Math.Abs((wysokosc_plotna - 2 * marginesY)) / (double)(liczba_równoleżników_grubych - 1);

            double pierwszy_poludnik_x = marginesX - 53 * krokX / 60.0;
            double ostatni_poludnik_x = marginesX + (liczba_poludnikow_grubych - 1) * krokX + 9 * krokX / 60.0;
            double ostatni_rownoleznik_y = marginesY + (liczba_równoleżników_grubych - 1) * krokY + 5 / 60.0 * krokY;

            /* czy da sie jakos zrobic zeby bylo rowno?
            double margines_lewo = 100 - pierwszy_poludnik_x;
            double margines_prawo = 100 - ostatni_poludnik_x;
            double margines_dol = 100 - ostatni_rownoleznik_y;
            */

            Line pierwszy_poludnik = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5,
                X1 = pierwszy_poludnik_x,
                X2 = pierwszy_poludnik_x,
                Y1 = marginesY,
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
                    X1 = marginesX + i * krokX,
                    X2 = marginesX + i * krokX,
                    Y1 = marginesY,
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
                X1 = marginesX + (liczba_poludnikow_grubych - 1) * krokX + 9 * krokX / 60.0,
                X2 = marginesX + (liczba_poludnikow_grubych - 1) * krokX + 9 * krokX / 60.0,
                Y1 = marginesY,
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
                    Y1 = marginesY + i * krokY,
                    Y2 = marginesY + i * krokY
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

        public void powiekszSiatke(double powiekszenie) {
            Trace.WriteLine("Tu siatka. Juz ogarniam.");

            if (powiekszenie != 1.0)
            {
                pionowy_scroll.Visibility = Visibility.Visible;
            }
            else {
                pionowy_scroll.Visibility = Visibility.Collapsed;
            }

            skalowanie.ScaleX = powiekszenie;
            skalowanie.ScaleY = powiekszenie;
        }

        public void przewijanie_pionowe(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {
            double min = pionowy_scroll.Minimum;
            double max = pionowy_scroll.Maximum;
            double wartosc = pionowy_scroll.Value;
            skalowanie.CenterY = wysokosc_plotna * (wartosc - min) / (max - min);
        }
    }
}
