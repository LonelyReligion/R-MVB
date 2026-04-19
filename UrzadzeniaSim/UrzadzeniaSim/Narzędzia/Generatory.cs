using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model;

namespace UrzadzeniaSim.Narzedzia
{
    //singleton?
    internal class Generatory
    {
        private const Decimal poczatkowa_szerokosc = 49.00m; //czy to nalezy do drzewa?
        private const Decimal poczatkowa_dlugosc = 14.07m; //czy to nalezy do drzewa?

        public static int liczba_urzadzen;
        private static Random rnd = new Random();
        private bool pierwszy = true;
        private Repo repo;
        
        public Generatory (Repo repozytorium)
        {
            this.repo = repozytorium;
            generujWspolrzedneDeterministycznie();
        }

        private int stopnieNaMinuty(Decimal wejsciowa) {
            int stopnie = ((int)wejsciowa / 1);
            int minuty = (int)(wejsciowa % 1 / 0.01m);
            
            return stopnie * 60 + minuty;
        }

        private Decimal minutyNaStopnie(int wejsciowa) {
            int stopnie = (int)(wejsciowa / 60);
            int minuty = (int)(wejsciowa - stopnie * 60);
            
            return stopnie + minuty * 0.01m;
        }

        private List<(decimal, decimal)> wspolrzedne = new List<(decimal, decimal)>();

        //rozklad jednostajny (dyskretny?)
        //wiesz co, ztablicujemy to 
        public void generujWspolrzedneDeterministycznie()
        {
            //rozciaglosc poludnikowa to 5 st. 50 min.
            //w minutach: 5 * 60 + 50 = 360
            int szerokosc_w_minutach = stopnieNaMinuty(poczatkowa_szerokosc);

            //rozciaglosc rownoleznikowa to 10 st. 2 min.
            //w minutach: 10 * 60 + 2 = 602
            int dlugosc_w_minutach = stopnieNaMinuty(poczatkowa_dlugosc);
                
            double max_double = Math.Sqrt(liczba_urzadzen);
            int max_i, max_j;

            if (max_double % 1 == 0)
            {
                max_i = (int) max_double;
                max_j = (int) max_double;
            }
            else
            {
                double wartosc_poczatkowa = Math.Floor(max_double);
                while (liczba_urzadzen % wartosc_poczatkowa != 0) {
                    wartosc_poczatkowa--;
                }
                max_i = (int) wartosc_poczatkowa;
                max_j = (int) (liczba_urzadzen / wartosc_poczatkowa);
            }

                
            for (int i = 0; i < max_i; i++) {
                for (int j = 0; j < max_j; j++) {
                    wspolrzedne.Add((minutyNaStopnie(dlugosc_w_minutach + j * 602 / max_j), minutyNaStopnie(szerokosc_w_minutach + i * 360 / max_i)));
                }
            }
        }

        private int index = 0;
        public (decimal, decimal) zwrocNoweWspolrzedneDeterministyczne() {
            return wspolrzedne[index++];
        }


        public (Decimal, Decimal) generujWspolrzedne() {
            Decimal szerokosc = (Decimal)(rnd.Next(49, 55) * 100);
            if (szerokosc < 5400)
            {
                szerokosc += rnd.Next(00, 59);
            }
            else
            {
                szerokosc += rnd.Next(00, 50);
            }
            szerokosc = szerokosc / 100.0m;

            Decimal dlugosc = (Decimal)(rnd.Next(14, 24) * 100);
            if (dlugosc < 2400)
            {
                dlugosc += rnd.Next(07, 59);
            }
            else
            {
                dlugosc += rnd.Next(00, 09);
            }
            dlugosc = dlugosc / 100.0m;

            return (dlugosc, szerokosc);
        }
/*
        public Rectangle generujProstokatDeterministycznie() {
            return new Rectangle(50, 15, 52, 19);
        }

        public Rectangle generujProstokat() {
            List<Decimal> szerokosci = new List<Decimal>();
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

            List<Decimal> dlugosci = new List<Decimal>();
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
        }*/

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
            Urzadzenie_Model losowe = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Value;
            return (losowe.Dlugosc, losowe.Szerokosc);
        }
    }
}
