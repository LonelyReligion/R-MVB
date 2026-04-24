using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UrzadzeniaSim.Model;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy Siatka.xaml
    /// </summary>
    public partial class Siatka : UserControl
    {
        public List<Urządzenie> urządzenia = new List<Urządzenie>();
        private int numer_porzadkowy_zaznaczonego = -1;//przechowywac w urzadzeniu, niech sie przestawi po zaznaczeniu, wtedy poprzednie bedzie odznaczone, a nowy numer tu wpisany, to inny numer niz ten w bazie

        double wysokosc_plotna;
        double szerokosc_plotna;

        bool minuty_dlugosc = false;
        bool minuty_szerokosc = false;

        const double grubosc_stopnie = 0.8;
        const double grubosc_minuty = 0.2;

        const int bazowa_wielkosc_czcionki = 12;
        int wielkosc_czcionki = 12;

        double marginesX;
        double marginesY;
        double krokX;
        double krokY;


        const double oryginalna_wysokosc = 361; //nwm czy beda takie same na kazdym pc, pewnie to wina suwaków
        const double oryginalna_szerokosc = 784;

        public event Action<int> znaczono_urzadzenie; //hehe, bo (za)znaczono i (od)znaczono
        public Siatka()
        {
            InitializeComponent();

            plotno.SizeChanged += (s, e) =>
            {
                wysokosc_plotna = plotno.ActualHeight;
                szerokosc_plotna = plotno.ActualWidth;

                // 
                pionowy_scroll.Value = pionowy_scroll.Minimum + ( pionowy_scroll.Maximum - pionowy_scroll.Minimum ) / 5;
                double min = pionowy_scroll.Minimum;
                double max = pionowy_scroll.Maximum;
                double wartosc = pionowy_scroll.Value;

                skalowanie.CenterY = wysokosc_plotna * (wartosc - min) / (max - min);

                poziomy_scroll.Value = poziomy_scroll.Minimum + (poziomy_scroll.Maximum - poziomy_scroll.Minimum) / 5;
                double min_h = poziomy_scroll.Minimum;
                double max_h = poziomy_scroll.Maximum;
                double wartosc_h = poziomy_scroll.Value;

                skalowanie.CenterX = szerokosc_plotna * (wartosc_h - min_h) / (max_h - min_h);
                //
                double skala = obliczSkale();

                wielkosc_czcionki = (int)(skala * (double)bazowa_wielkosc_czcionki);

                //
               foreach (Urządzenie u in urządzenia) {
                    u.Szerokosc_wysokosc_zaznaczenia = Math.Max(Urządzenie.oryg_szerokosc_wysokosc * skala, Urządzenie.oryg_szerokosc_wysokosc);
                    u.Szerokosc_wysokosc = Math.Max(Urządzenie.oryg_szerokosc_wysokosc * skala, Urządzenie.oryg_szerokosc_wysokosc);

                    obliczPozycjePunktu(u);
                }
                //
                rysuj(); 
            };

        }

        public double obliczSkale() {
            double skala_x = szerokosc_plotna / oryginalna_szerokosc;
            double skala_y = wysokosc_plotna / oryginalna_wysokosc;

            double skala = Math.Min(skala_x, skala_y);

            return skala;
        }

        private void obliczPozycjePunktu(Urządzenie u) {
            
            if(u.ActualWidth != 0)
                u.Szerokosc_wysokosc = u.ActualWidth;

            int dlugosc_minuty = (int)((u.dlugosc % 1) * 100);
            int dlugosc_stopnie = (int)(u.dlugosc / 1);
            double dlugosc_przesuniecie = 0;

            if (dlugosc_stopnie == 14)
            {
                dlugosc_przesuniecie += ((double)(dlugosc_minuty - 7)) * krokX / 60.0;
            }
            else 
            { 
                dlugosc_przesuniecie += 53 * krokX / 60.0;
                dlugosc_przesuniecie += (dlugosc_stopnie-15) * krokX + ((double)(dlugosc_minuty)) * krokX / 60.0;
            }

            int szerokosc_minuty = (int)((u.szerokosc % 1) * 100);
            int szerokosc_stopnie = (int)(u.szerokosc / 1);
            double szerokosc_przesuniecie = (szerokosc_stopnie - 49) * krokY + ((double)(szerokosc_minuty)) * krokY / 60.0;

            u.Margin = new System.Windows.Thickness(marginesX + dlugosc_przesuniecie - u.Szerokosc_wysokosc/2, szerokosc_przesuniecie + wysokosc_plotna - marginesY - u.Szerokosc_wysokosc/2, 0, 0); 
        }
        private void rysujUrządzenia() {
            foreach (Urządzenie u in urządzenia) {
                obliczPozycjePunktu(u);
                plotno.Children.Add(u);
            }
        }

        //Właściwie to siatka walcowa?
        private void rysujSiatkeGeograficzna()
        {
            marginesX = szerokosc_plotna / 16;
            marginesY = wysokosc_plotna / 4.5;

            int liczba_poludnikow_grubych = 10;
            int liczba_równoleżników_grubych = 6;

            //int liczba_minut_poludnik_pierwszy;
            //int liczba_minut_polunik_ostatni;
            double liczba_minut_rownoleznik = 50;

            double minutyX = (liczba_poludnikow_grubych - 1) * 60.0 + 2.0;
            krokX = 60 * (szerokosc_plotna - 4 * marginesX) / minutyX;

            double minutyY = (liczba_równoleżników_grubych - 1) * 60.0 + liczba_minut_rownoleznik;
            krokY = -60 * (wysokosc_plotna - 2 * marginesY) / minutyY;

            double pierwszy_poludnik_x = marginesX;
            double ostatni_poludnik_x = marginesX + (liczba_poludnikow_grubych) * krokX + 2.0 * (krokX / 60.0); // bo 53 min. + 9 min. =  1 st. 2 min.
            double pierwszy_rownoleznik_y = wysokosc_plotna - marginesY + (liczba_równoleżników_grubych - 1) * krokY + liczba_minut_rownoleznik / 60.0 * krokY;
            double ostatni_rownoleznik_y = wysokosc_plotna - marginesY;

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
                    X1 = marginesX + (53 * krokX / 60.0) + i * krokX,
                    X2 = marginesX + (53 * krokX / 60.0) + i * krokX,
                    Y1 = marginesY - 10,
                    Y2 = ostatni_rownoleznik_y
                };

                plotno.Children.Add(poludnik);

                TextBlock opis = new TextBlock();
                opis.Text = (15 + i).ToString() + "\u00B0" + " E";
                opis.FontSize = wielkosc_czcionki;
                opis.Margin = new Thickness(poludnik.X1 - 8 - (wielkosc_czcionki/2), poludnik.Y1 - 15 - wielkosc_czcionki, 0, 0);
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
                        X1 = marginesX + (53 * krokX / 60.0) + (liczba_poludnikow_grubych - 1) * krokX + ((j + 1) * krokX / 60.0),
                        X2 = marginesX + (53 * krokX / 60.0) + (liczba_poludnikow_grubych - 1) * krokX + ((j + 1) * krokX / 60.0),
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
                X1 = ostatni_poludnik_x,
                X2 = ostatni_poludnik_x,
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
                    X1 = pierwszy_poludnik.X1,
                    X2 = ostatni_poludnik.X1,
                    Y1 = wysokosc_plotna - marginesY + i * krokY,
                    Y2 = wysokosc_plotna - marginesY + i * krokY
                };
                plotno.Children.Add(rownoleznik);

                TextBlock opis = new TextBlock();
                opis.Text = (49 + i).ToString() + "\u00B0" + " N";
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
                for (int j = 0; j < liczba_minut_rownoleznik; j++)
                {
                    Line minutka = new Line
                    {
                        Visibility = Visibility.Visible,
                        Stroke = Brushes.Black,
                        StrokeThickness = grubosc_minuty,
                        X1 = pierwszy_poludnik.X1,
                        X2 = ostatni_poludnik.X1,
                        Y1 = pierwszy_rownoleznik_y - ((j + 1) * krokY / 60.0),
                        Y2 = pierwszy_rownoleznik_y - ((j + 1) * krokY / 60.0)
                    };
                    plotno.Children.Add(minutka);
                }
            }

            Line pierwszy_rownoleznik = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.Black,
                StrokeThickness = grubosc_minuty,
                X1 = pierwszy_poludnik_x,
                X2 = ostatni_poludnik_x,
                Y1 = pierwszy_rownoleznik_y,
                Y2 = pierwszy_rownoleznik_y
            };//ten 54 st. 5 min.
            plotno.Children.Add(pierwszy_rownoleznik);
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

                rysuj();
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
            rysuj();

        }

        public void zmienDokladnoscRownolezniki(bool taknie) { 
            minuty_szerokosc = taknie;
            rysuj();

        }

        public void dodajUrzadzenie(Urzadzenie_Model u) {
            urządzenia.Add(u.punkt);
            double skala = obliczSkale();

            if (skala != 1) {
                u.punkt.Szerokosc_wysokosc_zaznaczenia = Math.Max(Urządzenie.oryg_szerokosc_wysokosc * skala, Urządzenie.oryg_szerokosc_wysokosc);
                u.punkt.Szerokosc_wysokosc = Math.Max(Urządzenie.oryg_szerokosc_wysokosc * skala, Urządzenie.oryg_szerokosc_wysokosc);
            }

            
            u.punkt.ustaw_id_siatka(urządzenia.Count - 1);
            u.punkt.zaznaczono += zmianaZaznaczenia;

            obliczPozycjePunktu(u.punkt); 
            plotno.Children.Add(u.punkt);
        }

        public void dodajUrzadzenia(List<Urzadzenie_Model> urzadzenia) {
            foreach (Urzadzenie_Model u in urzadzenia) {
                dodajUrzadzenie(u);
            }
        }

        private void rysuj() {
            plotno.Children.Clear();
            rysujSiatkeGeograficzna();
            rysujUrządzenia(); //tylko widoczne urządzenia?
        }

        public void zmianaZaznaczenia(int id)
        {
            if (numer_porzadkowy_zaznaczonego == id)//odznaczamy
            {
                //tzn. nie potrzeba nic zmieniac w wyswietlaniu
                numer_porzadkowy_zaznaczonego = -1;
            }
            else
            {
                if (numer_porzadkowy_zaznaczonego != -1)
                {
                    urządzenia[numer_porzadkowy_zaznaczonego].Odznacz();
                }
                numer_porzadkowy_zaznaczonego = id;

                //musimy poinformowac panel boczny o id urządzenia, dlugosci i szerokosci
                //id bierzemy z listy, kolejne elementy z bazy? repo? drzewa?

            }
            znaczono_urzadzenie?.Invoke(numer_porzadkowy_zaznaczonego);
        }
    }
}
