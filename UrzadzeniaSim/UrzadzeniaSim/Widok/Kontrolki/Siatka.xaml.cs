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

        bool minuty_dlugosc = false;
        bool minuty_szerokosc = false;

        const double grubosc_stopnie = 0.8;
        const double grubosc_minuty = 0.2;

        const int bazowa_wielkosc_czcionki = 12;
        int wielkosc_czcionki;
        public Siatka()
        {
            InitializeComponent();
         
            const double oryginalna_wysokosc = 361; //nwm czy beda takie same na kazdym pc, pewnie to wina suwaków
            const double oryginalna_szerokosc = 784;

            plotno.SizeChanged += (s, e) =>
            {
                // 
                pionowy_scroll.Value = pionowy_scroll.Minimum + ( pionowy_scroll.Maximum - pionowy_scroll.Minimum ) / 5;
                double min = pionowy_scroll.Minimum;
                double max = pionowy_scroll.Maximum;
                double wartosc = pionowy_scroll.Value;

                skalowanie.CenterY = wysokosc_plotna * (wartosc - min) / (max - min);
                //
                wysokosc_plotna = plotno.ActualHeight;
                szerokosc_plotna = plotno.ActualWidth;


                double skala_x = szerokosc_plotna / oryginalna_szerokosc;
                double skala_y = wysokosc_plotna / oryginalna_wysokosc;
                double skala = Math.Min(skala_x, skala_y);
                
                wielkosc_czcionki = (int)skala * bazowa_wielkosc_czcionki;

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
                StrokeThickness = grubosc_minuty,
                X1 = pierwszy_poludnik_x,
                X2 = pierwszy_poludnik_x,
                Y1 = marginesY ,
                Y2 = ostatni_rownoleznik_y
            }; //ten 14 st. 7 min.
            plotno.Children.Add(pierwszy_poludnik);

            if (minuty_dlugosc) {
                for (int i = 0; i < 52; i++) //51 minut? 
                {
                    Line minutka = new Line
                    {
                        Visibility = Visibility.Visible,
                        Stroke = Brushes.Black,
                        StrokeThickness = grubosc_minuty,
                        X1 = pierwszy_poludnik_x + (52 - i) * krokX / 60.0,
                        X2 = pierwszy_poludnik_x + (52 - i) * krokX / 60.0,
                        Y1 = marginesY,
                        Y2 = ostatni_rownoleznik_y
                    };
                    plotno.Children.Add(minutka);
                }
            }

            for (int i = 0; i < liczba_poludnikow_grubych; i++)
            {

                Line poludnik = new Line
                {
                    Visibility = Visibility.Visible,
                    Stroke = Brushes.Black,
                    StrokeThickness = grubosc_stopnie,
                    X1 = marginesX + i * krokX,
                    X2 = marginesX + i * krokX,
                    Y1 = marginesY - 10,
                    Y2 = ostatni_rownoleznik_y + 10
                };

                plotno.Children.Add(poludnik);

                TextBlock opis = new TextBlock();
                opis.Text = (15 + i).ToString() + "\u00B0";
                opis.FontSize = wielkosc_czcionki;
                opis.Margin = new Thickness(poludnik.X1 + 1 - (wielkosc_czcionki/2), poludnik.Y1 - 15 - wielkosc_czcionki, 0, 0);
                plotno.Children.Add(opis);

                if (minuty_dlugosc && i != liczba_poludnikow_grubych - 1)
                {
                    for (int j = 0; j < 59; j++)
                    {
                        Line minutka = new Line
                        {
                            Visibility = Visibility.Visible,
                            Stroke = Brushes.Black,
                            StrokeThickness = grubosc_minuty,
                            X1 = poludnik.X1 + ((j+1) * krokX / 60.0),
                            X2 = poludnik.X2 + ((j+1) * krokX / 60.0),
                            Y1 = marginesY,
                            Y2 = ostatni_rownoleznik_y
                        };
                        plotno.Children.Add(minutka);
                    }
                }

                //umiescic generowanie poludnikow przy przyblizeniu tych mniejszych
            }
            
            if (minuty_dlugosc)
            {
                for (int j = 0; j < 9; j++)
                {
                    Line minutka = new Line
                    {
                        Visibility = Visibility.Visible,
                        Stroke = Brushes.Black,
                        StrokeThickness = grubosc_minuty,
                        X1 = marginesX + (liczba_poludnikow_grubych-1) * krokX + ((j + 1) * krokX / 60.0),
                        X2 = marginesX + (liczba_poludnikow_grubych - 1) * krokX + ((j + 1) * krokX / 60.0),
                        Y1 = marginesY,
                        Y2 = ostatni_rownoleznik_y
                    };
                    plotno.Children.Add(minutka);
                }
            }

            Line ostatni_poludnik = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.Black,
                StrokeThickness = grubosc_minuty,
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
                    StrokeThickness = grubosc_stopnie,
                    X1 = pierwszy_poludnik_x - 10,
                    X2 = ostatni_poludnik_x + 10,
                    Y1 = marginesY + i * krokY,
                    Y2 = marginesY + i * krokY
                };
                plotno.Children.Add(rownoleznik);

                TextBlock opis = new TextBlock();
                opis.Text = (49 + i).ToString() + "\u00B0";
                opis.FontSize = wielkosc_czcionki;
                opis.Margin = new Thickness(ostatni_poludnik_x + 20, rownoleznik.Y1 + 2 - wielkosc_czcionki, 0, 0);
                plotno.Children.Add(opis);

                if (minuty_szerokosc && i != liczba_równoleżników_grubych - 1) {
                    for (int j = 0; j < 59; j++)
                    {
                        Line minutka = new Line
                        {
                            Visibility = Visibility.Visible,
                            Stroke = Brushes.Black,
                            StrokeThickness = grubosc_minuty,
                            X1 = pierwszy_poludnik_x,
                            X2 = ostatni_poludnik_x,
                            Y1 = rownoleznik.Y1 + ((j+1) * krokY/60.0),
                            Y2 = rownoleznik.Y1 + ((j+1) * krokY/60.0)
                        };
                        plotno.Children.Add(minutka);
                    }
                }
            }
            
            if (minuty_szerokosc) {
                for (int j = 0; j < 5; j++)
                {
                    Line minutka = new Line
                    {
                        Visibility = Visibility.Visible,
                        Stroke = Brushes.Black,
                        StrokeThickness = grubosc_minuty,
                        X1 = pierwszy_poludnik_x,
                        X2 = ostatni_poludnik_x,
                        Y1 = ostatni_rownoleznik_y - ((j + 1) * krokY / 60.0),
                        Y2 = ostatni_rownoleznik_y - ((j + 1) * krokY / 60.0)
                    };
                    plotno.Children.Add(minutka);
                }
            }

            Line ostatni_rownoleznik = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.Black,
                StrokeThickness = grubosc_minuty,
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
                poziomy_scroll.Visibility = Visibility.Visible;
            }
            else {
                pionowy_scroll.Visibility = Visibility.Collapsed; 
                poziomy_scroll.Visibility = Visibility.Collapsed;
            }

            skalowanie.ScaleX = powiekszenie;
            skalowanie.ScaleY = powiekszenie;

            if (powiekszenie < 2.5) {
                minuty_dlugosc = false;
                minuty_szerokosc = false;

                plotno.Children.Clear();
                rysujSiatkeGeograficzna();
            }
        }

        private void przewijanie_pionowe(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {
            double min = pionowy_scroll.Minimum;
            double max = pionowy_scroll.Maximum;
            double wartosc = pionowy_scroll.Value;

            skalowanie.CenterY = wysokosc_plotna * (wartosc - min) / (max - min);
        }

        private void przewijanie_poziome(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {
            double min = poziomy_scroll.Minimum;
            double max = poziomy_scroll.Maximum;
            double wartosc = poziomy_scroll.Value;

            skalowanie.CenterX = szerokosc_plotna * (wartosc - min) / (max - min);
        }

        public void zmienDokladnoscPoludniki(bool taknie) { 
            minuty_dlugosc = taknie;
            plotno.Children.Clear();
            rysujSiatkeGeograficzna();

        }

        public void zmienDokladnoscRownolezniki(bool taknie) { 
            minuty_szerokosc = taknie;
            plotno.Children.Clear();
            rysujSiatkeGeograficzna();
        }
    }
}
