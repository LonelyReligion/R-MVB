using System.Diagnostics;
using System.Windows;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model.RMVB.R;
using UrzadzeniaSim.Widok.Kontrolki;
using UrzadzeniaSim.Widok.Okna;

namespace UrzadzeniaSim.Narzedzia
{
    //singleton?
    public class Generatory
    {
        private const Decimal _poczatkowaSzerokosc = 49.00m; //czy to nalezy do drzewa?
        private const Decimal _poczatkowaDlugosc = 14.07m; //czy to nalezy do drzewa?

        private static Random s_rnd = new Random();
        private bool _pierwszy = true;
        private Repo _repo;

        public Generatory(Repo repozytorium)
        {
            this._repo = repozytorium;
        }

        private int _stopnieNaMinuty(Decimal wejsciowa)
        {
            int stopnie = ((int)wejsciowa / 1);
            int minuty = (int)(wejsciowa % 1 / 0.01m);

            return stopnie * 60 + minuty;
        }

        private Decimal _minutyNaStopnie(int wejsciowa)
        {
            int stopnie = (int)(wejsciowa / 60);
            int minuty = (int)(wejsciowa - stopnie * 60);

            return stopnie + minuty * 0.01m;
        }

        private List<(decimal, decimal)> _wspolrzedne = new List<(decimal, decimal)>();

        //rozklad jednostajny (dyskretny?)
        //wiesz co, ztablicujemy to 
        public void GenerujWspolrzedneDeterministycznie(int liczba_urzadzen)
        {
            //rozciaglosc poludnikowa to 5 st. 50 min.
            //w minutach: 5 * 60 + 50 = 360
            int szerokosc_w_minutach = _stopnieNaMinuty(_poczatkowaSzerokosc);

            //rozciaglosc rownoleznikowa to 10 st. 2 min.
            //w minutach: 10 * 60 + 2 = 602
            int dlugosc_w_minutach = _stopnieNaMinuty(_poczatkowaDlugosc);

            double max_double = Math.Sqrt(liczba_urzadzen);
            int max_i, max_j;

            if (max_double % 1 == 0)
            {
                max_i = (int)max_double;
                max_j = (int)max_double;
            }
            else
            {
                double wartosc_poczatkowa = Math.Floor(max_double);
                while (liczba_urzadzen % wartosc_poczatkowa != 0)
                {
                    wartosc_poczatkowa--;
                }
                max_i = (int)wartosc_poczatkowa;
                max_j = (int)(liczba_urzadzen / wartosc_poczatkowa);
            }


            for (int i = 0; i < max_i; i++)
            {
                for (int j = 0; j < max_j; j++)
                {
                    _wspolrzedne.Add((_minutyNaStopnie(dlugosc_w_minutach + j * 602 / max_j), _minutyNaStopnie(szerokosc_w_minutach + i * 360 / max_i)));
                }
            }
        }

        private int _index = 0;
        public (decimal, decimal) ZwrocNoweWspolrzedneDeterministyczne()
        {
            return _wspolrzedne[_index++];
        }


        public (Decimal, Decimal) GenerujWspolrzedne()
        {
            Decimal szerokosc = (Decimal)(s_rnd.Next(49, 55) * 100);
            if (szerokosc < 5400)
            {
                szerokosc += s_rnd.Next(00, 59);
            }
            else
            {
                szerokosc += s_rnd.Next(00, 50);
            }
            szerokosc = szerokosc / 100.0m;

            Decimal dlugosc = (Decimal)(s_rnd.Next(14, 24) * 100);
            if (dlugosc < 2400)
            {
                dlugosc += s_rnd.Next(07, 59);
            }
            else
            {
                dlugosc += s_rnd.Next(00, 09);
            }
            dlugosc = dlugosc / 100.0m;

            return (dlugosc, szerokosc);
        }

        public Rectangle GenerujProstokatDeterministycznie()
        {
            return new Rectangle(50, 15, 52, 19);
        }

        public Rectangle GenerujProstokat()
        {
            List<Decimal> szerokosci = new List<Decimal>();
            for (int i = 0; i < 2; i++)
            {
                Decimal szerokosc = (Decimal)(s_rnd.Next(49, 54) * 10000);
                if (szerokosc < 540000)
                {
                    szerokosc += s_rnd.Next(00, 59) * 100;
                    szerokosc += s_rnd.Next(00, 59);
                }
                else
                {
                    szerokosc += s_rnd.Next(00, 50) * 100;
                    if (szerokosc != 545000)
                        szerokosc += s_rnd.Next(00, 59);
                }
                szerokosc = szerokosc / 10000.0m;
                szerokosci.Add(szerokosc);
            }

            List<Decimal> dlugosci = new List<Decimal>();
            for (int i = 0; i < 2; i++)
            {
                Decimal dlugosc = (Decimal)(s_rnd.Next(14, 24) * 10000);
                if (dlugosc < 240000)
                {
                    dlugosc += s_rnd.Next(07, 59) * 100;
                    dlugosc += s_rnd.Next(00, 59);
                }
                else
                {
                    dlugosc += s_rnd.Next(00, 09) * 100;
                    if (dlugosc != 240900)
                        dlugosc += s_rnd.Next(00, 59);
                }
                dlugosc = dlugosc / 10000.0m;
                dlugosci.Add(dlugosc);
            }
            return new Rectangle(szerokosci.Min(), dlugosci.Min(), szerokosci.Max(), dlugosci.Max());
        }

        public Pomiar GenerujLosowyPomiar()
        {
            Decimal temp = (Decimal)(s_rnd.NextDouble() * (41.0 - (-41.0)) - 41.0);
            Pomiar testowy = new Pomiar(temp, DateTime.Now);
            return testowy;
        }

        //losowanie ze zwracaniem
        public List<Wersja> WylosujWersje(int ile)
        {
            List<Wersja> szukane_wersje = new List<Wersja>();
            for (int i = 0; i < ile; i++)
            {
                Wersja losowa_wersja = _repo.pobierzWersje().ElementAt(s_rnd.Next(_repo.pobierzWersje().Count - 1));
                szukane_wersje.Add(losowa_wersja);
            }
            return szukane_wersje;
        }

        //zwraca wspolrzedne losowego urzadzenia
        public (Decimal, Decimal) WylosujWspolrzedne()
        {
            Urzadzenie_Model losowe = _repo.pobierzUrzadzenia().ElementAt(s_rnd.Next(_repo.pobierzUrzadzenia().Count - 1)).Value;
            return (losowe.Dlugosc, losowe.Szerokosc);
        }

        public async void GenerowaniePomiarowUrzadzenia(Generowanie nadawca)
        {

            nadawca.UstawPracaWToku(true);
            nadawca.ZwrocUrzadzenieGui().StatusUrzadzenia = STATUS.AKTYWNY_NADAJE; //CZY ROBIC TO PRZEZ _URZADZENIE JEDNAK?
            int? _liczbaCykliDoKonca = nadawca.ZwrocUrzadzenieGui().IleCykli;

            while (!nadawca.ZwrocUrzadzenieGui().Token.IsCancellationRequested && (_liczbaCykliDoKonca == null || _liczbaCykliDoKonca > 0))
            {
                await Task.Delay(nadawca.ZwrocUrzadzenieGui().Interwal * 1000); //tyle ile w updown
                if (_liczbaCykliDoKonca != null) _liczbaCykliDoKonca -= 1;
            }
            Trace.WriteLine("Zadanie zostało anulowane przez użytkownika lub zakończyło się pomyślnie.");
            //musimy jakos dac znac ze stop

            Application.Current.Dispatcher.Invoke( //glowny watek
            () =>
            {
                nadawca.ZatrzymajGenerowanie();
            });

        }
    }
}
