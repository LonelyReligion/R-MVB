using Microsoft.Win32;
using RMVB_konsola.R;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class DrzewoMVB
    {
        private double Pversion = 1.0/3;
        private double Psvu = 1.0/3;
        private double Psvo = 5.0/6;

        private TreeRepository Repo;
        private Kontekst ctx;

        private List<DeskryptorKorzenia> desk = new List<DeskryptorKorzenia>(); // "List of tree descriptors. Descriptors for all roots in the tree are connected in a list(or other structures) according to growing, separable life spans."

        internal DrzewoMVB(TreeRepository repo, Kontekst ctx)
        {
            Korzen k = new Korzen(repo, Pversion);

            Wezel.Psvu = Psvu;
            Wezel.Psvo = Psvo;

            desk.Add(new DeskryptorKorzenia(DateTime.Now, DateTime.MaxValue, k));
            this.Repo = repo;
            this.ctx = ctx;
        }

        internal void wypiszDrzewo() 
        {
            foreach (DeskryptorKorzenia dk in desk)
            {
                Console.WriteLine("Okres obowiazywania od " + dk.zwrocPoczatek().ToString() + " do " + dk.zwrocKoniec().ToString());
                dk.zwrocKorzen().wypisz();
                Console.WriteLine("\n");
            }

            Console.WriteLine("Liczba korzeni MVB: " + desk.Count);
        }

        internal int zwrocLiczbeWpisowKorzenia(int nr) {
            return desk[nr].zwrocKorzen().zwrocLiczbeWpisow();
        }

        internal void dodajUrzadzenie(Wersja u) 
        {
            Korzen ostatni_korzen = desk.Last().zwrocKorzen();
            if (!ostatni_korzen.dodaj(u))
            {
                DateTime czas_zmiany = DateTime.Now;
                desk.Last().ustawKoniec(czas_zmiany);
                //zczytac zywe
                List<Wersja> do_dodania = ostatni_korzen.zwrocZywe();
                //zabic te w starym korzeniu z data wyzej
                //zywe maja miec to jako date ostatniej modyfikacji w nowym korzeniu
                Korzen nowy = new Korzen(Repo, Pversion);
                Wezel.aktualne_id = 'A';
                desk.Add(new DeskryptorKorzenia(czas_zmiany, DateTime.MaxValue, nowy));
                foreach(Wersja w in do_dodania)
                    dodajUrzadzenie(w); //rekurencja ups.
            }; 
            
        }

        internal void usunUrzadzenie(Wersja testowe2)
        {
            var dk = desk[0].zwrocKorzen();
            dk.usun(testowe2);
        }

        //szukaj id i wersji
        internal Wersja szukaj(int id, int v)
        {
            //prawdopodobnie jest gdzieś na początku
            if (v == 1)
            {
                for (int i = 0; i < desk.Count(); i++)
                {
                    Wersja wartosc = desk[i].zwrocKorzen().szukaj(id, v).Item3;
                    if(wartosc != null)
                        return wartosc;
                }
            }
            
            else { 
                int poczatkowy_indeks = desk.Count() / 2;
                Stack<DeskryptorKorzenia> do_przejrzenia = new Stack<DeskryptorKorzenia>();
                int aktualny_indeks = poczatkowy_indeks;
                do_przejrzenia.Push(desk[poczatkowy_indeks]);
                HashSet<int> odwiedzone = new HashSet<int>();
                bool kierunek = true; //domyslnie idziemy w gore, kierunek poszukiwan

                while (do_przejrzenia.Count != 0) {
                    DeskryptorKorzenia analizowany = do_przejrzenia.Pop();
                    (byte, Wezel, Wersja) wartosc = analizowany.zwrocKorzen().szukaj(id, v);
                    odwiedzone.Add(aktualny_indeks);
                    if (wartosc.Item1 == 0) { //znalezliśmy 
                        return wartosc.Item3;
                    }
                    else if (wartosc.Item1 == 1) //znaleziona wersja jest mniejsza
                    {
                        //szukamy wyszej
                        kierunek = true;
                        if (aktualny_indeks != desk.Count() - 1)
                        {
                            aktualny_indeks = aktualny_indeks + 1;
                            do_przejrzenia.Push(desk[aktualny_indeks]);
                        }
                        else
                        {
                            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
                            return null;
                        }
                    }
                    else if (wartosc.Item1 == 2) //znaleziona wersja jest większa
                    {
                        //szukamy nizej
                        kierunek = false;
                        if (aktualny_indeks != 0) {
                            aktualny_indeks = aktualny_indeks - 1;
                            do_przejrzenia.Push(desk[aktualny_indeks]);
                        }
                        else {
                            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
                            return null;
                        }
                    }
                    else if(wartosc.Item1 == 3)//nie znaleziono
                    {                        
                        //szukamy zgodnie z kierunkiem
                        if (!(aktualny_indeks == desk.Count() - 1 && kierunek) && !(!kierunek && aktualny_indeks == 0))
                        {
                            int jeden = kierunek ? 1 : -1;
                            if (!odwiedzone.Contains(aktualny_indeks + jeden))
                            {
                                aktualny_indeks += jeden;
                                do_przejrzenia.Push(desk[aktualny_indeks]);
                            }
                        }
                        //uwzglednic co jak nam sie skoncza w jednym kierunku a nie znaleziono zadnego
                        else 
                        {
                            if (odwiedzone.Count == desk.Count)
                            {
                                Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
                                return null;
                            }
                            else if (aktualny_indeks == desk.Count() - 1)
                            {
                                kierunek = true;
                                aktualny_indeks = 0;
                                do_przejrzenia.Push(desk[aktualny_indeks]);
                            }
                            else
                            {
                                kierunek = false;
                                aktualny_indeks = desk.Count() - 1;
                                do_przejrzenia.Push(desk[aktualny_indeks]);
                            }
                        }
                    };
                }

            }
            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null;
        }

        //szukaj wersji aktualnej w danym momencie
        internal Wersja szukaj(int id, DateTime dt) 
        {
            for (int i = 0; i < desk.Count(); i++)
            {
                if(desk[i].zwrocPoczatek() <= dt && dt < desk[i].zwrocKoniec())
                    return desk[i].zwrocKorzen().szukaj(id, dt);

            }
            Console.WriteLine("Uwaga: Nie znaleziono odpowiedniego korzenia");
            return null;
        }

        //szukaj ostatniej wersji
        internal Wersja szukaj(int id) 
        {
            for (int i = desk.Count() - 1; i >= 0;  i--)
            {
                Wersja? wartosc = desk[i].zwrocKorzen().szukaj(id);
                if (wartosc != null)
                    return wartosc;
            }
            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null;
        }

        //zwraca wersje z danego skonczonego przedzialu czasowego
        internal List<Wersja> szukaj(DateTime poczatek, DateTime koniec) 
        {
            List<Wersja> wyjsciowa =  new List<Wersja>();
            for (int i = 0; i < desk.Count - 1; i++) {
                if (!(desk[i].zwrocKoniec() < koniec)) {
                    return wyjsciowa; //przejrzelismy wszystko co pasowalo do przedzialu
                }
                else if (desk[i].zwrocPoczatek() >= poczatek) {
                    wyjsciowa.AddRange(desk[i].zwrocKorzen().szukaj(poczatek, koniec));
                }
            }
            return wyjsciowa; 
        }
    }
}
