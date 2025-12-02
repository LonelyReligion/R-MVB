using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMVB_konsola.R;

namespace RMVB_konsola
{
    //singleton?
    internal class Generatory
    {
        private static Decimal szerokosc = 49.0001m;
        private static Decimal dlugosc = 14.0701m;
        public static int liczba_urzadzen;
        private static Random rnd = new Random();
        private bool pierwszy = true;
        private Repo repo;
        
        public Generatory (Repo repozytorium)
        {
            this.repo = repozytorium;
        }

        private int stopnieNaSekundy(Decimal wejsciowa) {
            int stopnie = ((int)wejsciowa / 1);
            int minuty = (int)(wejsciowa % 1 / 0.01m);
            int sekundy = (int)((wejsciowa % 0.01m) * 10000);
            
            return stopnie * 3600 + minuty * 60 + sekundy;
        }

        private Decimal sekundyNaStopnie(int wejsciowa) {
            int stopnie = (int)(wejsciowa / 3600);
            int minuty = (int)((wejsciowa - stopnie * 3600) / 60);
            int sekundy = (int)(wejsciowa - minuty * 60 - stopnie * 3600);

           return stopnie + minuty * 0.01m + sekundy * 0.0001m;

        }

        //rozklad jednostajny (dyskretny?)
        public (Decimal, Decimal) generujWspolrzedneDeterministycznie()
        {
            if (!pierwszy)
            {
                //rozciaglosc poludnikowa to 5 st. 50 min.
                //w minutach: 5 * 60 + 50 = 360
                //w sekundach: 350 * 60 = 21000
                int szerokosc_w_sekundach = stopnieNaSekundy(szerokosc);
                szerokosc_w_sekundach += 21000 / liczba_urzadzen;
                szerokosc = sekundyNaStopnie(szerokosc_w_sekundach);

                //rozciaglosc rownoleznikowa to 10 st. 2 min.
                //w minutach: 10 * 60 + 2 = 602
                //w sekundach: 602 * 60 = 36120
                int dlugosc_w_sekundach = stopnieNaSekundy(dlugosc);
                dlugosc_w_sekundach += 36120 / liczba_urzadzen;
                dlugosc = sekundyNaStopnie(dlugosc_w_sekundach);
            }
            pierwszy = false;
            return (dlugosc, szerokosc);
        }

        public (Decimal, Decimal) generujWspolrzedne() {
            Decimal szerokosc = (Decimal)(rnd.Next(49, 54) * 10000);
            if (szerokosc < 540000)
            {
                szerokosc += rnd.Next(00, 59) * 100;
                szerokosc += rnd.Next(00, 59);
            }
            else
            {
                szerokosc += rnd.Next(00, 50) * 100;
                if (szerokosc != 545000)
                    szerokosc += rnd.Next(00, 59);
            }
            szerokosc = szerokosc / 10000.0m;

            Decimal dlugosc = (Decimal)(rnd.Next(14, 24) * 10000);
            if (dlugosc < 240000)
            {
                dlugosc += rnd.Next(07, 59) * 100;
                dlugosc += rnd.Next(00, 59);
            }
            else
            {
                dlugosc += rnd.Next(00, 09) * 100;
                if (dlugosc != 240900)
                    dlugosc += rnd.Next(00, 59);
            }
            dlugosc = dlugosc / 10000.0m;

            return (dlugosc, szerokosc);
        }

        public Rectangle generujProstokatDeterministycznie() {
            return new Rectangle(50, 15, 52, 19);
        }

        public Rectangle generujProstokat() {
            List<decimal> szerokosci = new List<decimal>();
            for (int i = 0; i < 2; i++) {
                Decimal szerokosc = (Decimal)(rnd.Next(49, 54) * 10000);
                if (szerokosc < 540000)
                {
                    szerokosc += rnd.Next(00, 59) * 100;
                    szerokosc += rnd.Next(00, 59);
                }
                else
                {
                    szerokosc += rnd.Next(00, 50) * 100;
                    if (szerokosc != 545000)
                        szerokosc += rnd.Next(00, 59);
                }
                szerokosc = szerokosc / 10000.0m;
                szerokosci.Add(szerokosc);
            }

            List<decimal> dlugosci = new List<decimal>();
            for (int i = 0; i < 2; i++) {
                Decimal dlugosc = (Decimal)(rnd.Next(14, 24) * 10000);
                if (dlugosc < 240000)
                {
                    dlugosc += rnd.Next(07, 59) * 100;
                    dlugosc += rnd.Next(00, 59);
                }
                else
                {
                    dlugosc += rnd.Next(00, 09) * 100;
                    if (dlugosc != 240900)
                        dlugosc += rnd.Next(00, 59);
                }
                dlugosc = dlugosc / 10000.0m;
                dlugosci.Add(dlugosc);
            }
            return new Rectangle(szerokosci.Min(), dlugosci.Min(), szerokosci.Max(), dlugosci.Max());
        }

        public Pomiar generujLosowyPomiar() {
            Decimal temp = (Decimal)(rnd.NextDouble() * (41.0 - (-41.0)) - 41.0);
            Pomiar testowy = new Pomiar(temp, DateTime.Now);
            return testowy;
        }

        //losowanie ze zwracaniem
        public List<Wersja> wylosujWersje(int ile)
        {
            List<Wersja> szukane_wersje = new List<Wersja>();
            for (int i = 0; i < ile; i++)
            {
                Wersja losowa_wersja = repo.pobierzWersje().ElementAt(rnd.Next(repo.pobierzWersje().Count - 1));
                szukane_wersje.Add(losowa_wersja);
            }
            return szukane_wersje;
        }

        //zwraca wspolrzedne losowego urzadzenia
        public (Decimal, Decimal) wylosujWspolrzedne()
        {
            Urzadzenie losowe = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Value;
            return (losowe.Dlugosc, losowe.Szerokosc);
        }
    }
}
