using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class DeskryptorKorzenia
    {
        Repo repo;
        List<(int, Wpis)> wpisy;

        //parametry drzewa, sa zdefiniowane w klasie drzewa
        static double Pversion;
        static double Psvu;
        static double Psvo;

        internal DeskryptorKorzenia(Repo repo, double pversion, double psvu, double psvo)
        {
            wpisy = new List<(int, Wpis)>();
            this.repo = repo;

            Pversion = pversion;
            Psvu = psvu;
            Psvo = psvo;
        }

        internal void dodaj(Urzadzenie u)
        {
            if (wpisy.Count() == 0)
            {
                Wezel nowy = new Wezel();
                nowy.dodaj(u);
                //przekazac przez ref jakos?
                wpisy.Add((wpisy.Count, new Wpis(u.UrzadzenieID, u.UrzadzenieID, u.dataOstatniejModyfikacji, u.dataWygasniecia, nowy)));
            }
            else
            {
                bool dodano = false;
                int numer_wezla = wpisy.Count - 1;//do tego powinnismy wstawic jezeli nie nalezy
                //do zadnego przedzialu

                for (int i = 0; i < wpisy.Count; i++)
                {
                    //czy nalezy do odp przedzialu kluczy
                    if (wpisy[i].Item2.maxKlucz > u.UrzadzenieID)
                    {
                        numer_wezla = i;
                        if (wpisy[i].Item2.wezel.dodaj(u)) //czy jest miejsce
                        {
                            if (wpisy[i].Item2.minKlucz > u.UrzadzenieID)
                                wpisy[i].Item2.minKlucz = u.UrzadzenieID;
                            else if (wpisy[i].Item2.maxKlucz < u.UrzadzenieID)
                                wpisy[i].Item2.maxKlucz = u.UrzadzenieID;

                            dodano = true;
                            break;
                        }
                    }
                }

                //jezeli nie dodano i id nie jest wieksze niz maxId ostatniego wezla lub on sam nie ma miejsca do wstawienia
                if (!dodano && !(numer_wezla == wpisy.Count - 1 && wpisy[numer_wezla].Item2.wezel.dodaj(u)))
                {
                    versionSplit(numer_wezla, u);
                }
                else {
                    if (wpisy[numer_wezla].Item2.minKlucz > u.UrzadzenieID)
                        wpisy[numer_wezla].Item2.minKlucz = u.UrzadzenieID;
                    else if (wpisy[numer_wezla].Item2.maxKlucz < u.UrzadzenieID)
                        wpisy[numer_wezla].Item2.maxKlucz = u.UrzadzenieID;
                }
            }
        }
        internal void versionSplit(int numer_wezla, Urzadzenie u)
        {
            //version split
            List<Urzadzenie> kopie = new List<Urzadzenie>();
            if (u != null) kopie.Add(u);
            using (var ctx = new Kontekst())
            { //zrobic jeden kontekst dla klasy? 
                foreach (var urzadzenie in wpisy[numer_wezla].Item2.wezel.wpisy)
                {
                    if (urzadzenie.Item2.dataWygasniecia == DateTime.MaxValue)
                    { //kopiujemy zywe
                        Urzadzenie kopia = new Urzadzenie(urzadzenie.Item1, repo);
                        urzadzenie.Item2.dataWygasniecia = DateTime.Now;
                        kopia.dataOstatniejModyfikacji = DateTime.Now;
                        kopie.Add(kopia);

                        repo.dodajUrzadzenie(kopia);

                        ctx.Urzadzenia.Add(kopia);
                        ctx.SaveChanges();
                        repo.dodajUrzadzenie(kopia);
                    
                    }
                }
            }
            //posortuj liste po id 
            var posortowanaLista = kopie.OrderBy(q => q.UrzadzenieID).ToList();
            //dodaj do wezla (w odp kolejnosci?)
            Wezel nowy = new Wezel();
            foreach (Urzadzenie urzadzenie in posortowanaLista)
            {
                nowy.dodaj(urzadzenie);
            }

            //dodaj wpis
            wpisy.Add((wpisy.Count, new Wpis(posortowanaLista[0].UrzadzenieID, posortowanaLista.Last().UrzadzenieID, DateTime.Now, DateTime.MaxValue, nowy)));
            wpisy[numer_wezla].Item2.maxData = DateTime.Now;

            //czy tylko w last cos takiego moze zajsc? przy wstawianiu tez
            if (wpisy.Last().Item2.wezel.strongVersionOverflow(Psvo))
            {
                keySplit(numer_wezla);
            };

            //nietestowane
            if (wpisy.Last().Item2.wezel.strongVersionUnderflow(Psvu))
            {
                // u juz jest w wezle
                versionSplit(numer_wezla, null);
            };
        }

        //nietestowane
        internal void keySplit(int numer_wezla)
        {
            //keysplit
            List<Urzadzenie> kopie = new List<Urzadzenie>();
            //wybieramy zywe
            foreach (var urzadzenie in wpisy[numer_wezla].Item2.wezel.wpisy)
            {
                if (urzadzenie.Item2.dataWygasniecia == DateTime.MaxValue)
                { //kopiujemy zywe
                    Urzadzenie kopia = new Urzadzenie(urzadzenie.Item1, repo);
                    urzadzenie.Item2.dataWygasniecia = DateTime.Now;
                    kopia.dataOstatniejModyfikacji = DateTime.Now;
                    kopie.Add(kopia);
                }
            }

            var pierwszy_wezel = kopie.Skip(kopie.Count/2);
            var drugi_wezel = kopie.Except(pierwszy_wezel);

            //moze zamiast tego zrobic metode dodajWezel, dla czytelnosci 
            //posortuj liste po id 
            var posortowanaLista = pierwszy_wezel.OrderBy(q => q.UrzadzenieID).ToList();
            //dodaj do wezla (w odp kolejnosci?)
            Wezel nowy = new Wezel();
            foreach (Urzadzenie urzadzenie in posortowanaLista)
            {
                nowy.dodaj(urzadzenie);
            }

            //dodaj wpis
            wpisy.Add((wpisy.Count, new Wpis(posortowanaLista[0].UrzadzenieID, posortowanaLista.Last().UrzadzenieID, DateTime.Now, DateTime.MaxValue, nowy)));
            wpisy[numer_wezla].Item2.maxData = DateTime.Now;

            posortowanaLista = drugi_wezel.OrderBy(q => q.UrzadzenieID).ToList();
            //dodaj do wezla (w odp kolejnosci?)
            Wezel nowy2 = new Wezel();
            foreach (Urzadzenie urzadzenie in posortowanaLista)
            {
                nowy2.dodaj(urzadzenie);
            }

            //dodaj wpis
            wpisy.Add((wpisy.Count, new Wpis(posortowanaLista[0].UrzadzenieID, posortowanaLista.Last().UrzadzenieID, DateTime.Now, DateTime.MaxValue, nowy2)));
            wpisy[numer_wezla].Item2.maxData = DateTime.Now;
        }

        internal void wypisz()
        {
            Console.WriteLine("Korzen");
            if (wpisy.Count == 0)
            {
                Console.WriteLine("******");
                Console.WriteLine("*    *");
                Console.WriteLine("******");
            }
            else
            {
                List<String> wynikowy = new List<String>();
                for (int i = 0; i < wpisy.Count; i++)
                {
                    wynikowy.Add("<" + wpisy[i].Item2.minKlucz.ToString() + "," + wpisy[i].Item2.maxKlucz.ToString() + "," + wpisy[i].Item2.minData.ToString() + "," + wpisy[i].Item2.maxData.ToString() + "," + wpisy[i].Item2.wezel.id + ">");
                }
                int max = wynikowy.Max(x => x.Length);
                String pozioma = "";
                for (int i = -2; i < max; i++)
                {
                    pozioma += "*";
                }
                Console.WriteLine(pozioma);
                for (int i = 0; i < wynikowy.Count; i++)
                {
                    Console.WriteLine("*" + wynikowy[i] + "*");
                }
                Console.WriteLine(pozioma);
            }

            foreach (var wpis in wpisy)
            {
                wpis.Item2.wezel.wypisz();
            }
        }

        //dezaktywuj urzadzenie, usuwanie nie jest dostepne i potrzebne
        internal void usun(Urzadzenie u)
        {
            throw new NotImplementedException();
        }

        //szukaj id i wersji
        internal Urzadzenie szukaj(int id, int v)
        {
            //binarysearch
            int dlugosc_listy = wpisy.Count;
            int poczatkowy_indeks = dlugosc_listy / 2;
            Stack<(int, Wpis)> do_przejrzenia = new Stack<(int, Wpis)>();
            do_przejrzenia.Push(wpisy[poczatkowy_indeks]);

            while(do_przejrzenia.Count != 0){
                (int indeks, Wpis w) = do_przejrzenia.Pop();
                int najwyzsza_wersja = -1;
                if (w.minKlucz <= id && w.maxKlucz >= id) {
                    var wpisy_wezla = w.wezel.wpisy;
                    for (int i = 0; i < wpisy_wezla.Count; i++) {
                        if (wpisy_wezla[i].Item1 == id)
                        {
                            int wersja = wpisy_wezla[i].Item2.Wersja;
                            if (wersja == v) {
                                return wpisy_wezla[i].Item2;
                            }
                            najwyzsza_wersja = wersja;
                        }
                        else if (wpisy_wezla[i].Item1 > id)
                            break;
                    }
                }
                if (najwyzsza_wersja == -1) //ryzyko nieskonczonej petli
                {
                    if (indeks + 1 < wpisy.Count)
                        do_przejrzenia.Push(wpisy[indeks + 1]);
                    if (indeks - 1 > 0)
                        do_przejrzenia.Push(wpisy[indeks - 1]);
                }
                else {
                    if (najwyzsza_wersja < v && indeks + 1 < wpisy.Count)
                        do_przejrzenia.Push(wpisy[indeks + 1]);
                    else if (indeks - 1 >= 0)
                        do_przejrzenia.Push(wpisy[indeks - 1]);
                }
            }

            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null; //nie znaleziono
        }

        //szukaj wersji aktualnej w danym momencie
        // [)[)
        internal Urzadzenie szukaj(int id, DateTime dt)
        {
            //zastapic jakas wersja z binary search i stosem?
            for (int i = wpisy.Count - 1; i >= 0; i--) {
                var wpis = wpisy[i].Item2;
                if ((wpis.minData <= dt && wpis.maxData > dt) && (wpis.minKlucz <= id && wpis.maxKlucz >= id)) {
                    for (int j = 0; j < wpis.wezel.wpisy.Count(); j++) {
                        (int index, Urzadzenie urzadzenie) = wpis.wezel.wpisy[j];
                        if(index == id && urzadzenie.dataOstatniejModyfikacji <= dt && urzadzenie.dataWygasniecia > dt)
                            return urzadzenie;
                    }
                }
            }

            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null;
        }

        //szukaj ostatniej wersji
        internal Urzadzenie szukaj(int id)
        {
            for (int i = wpisy.Count-1; i >= 0; i--) { //od tylu 
                (int index, Wpis w) = wpisy[i];
                if (w.minKlucz <= id && w.maxKlucz >= id) {
                    for (int j = w.wezel.wpisy.Count - 1; j >= 0; j--) {
                        (int index_urzadzenia, Urzadzenie u) = w.wezel.wpisy[j];
                        if (index_urzadzenia == id)
                            return u;
                    }
                }
            }

            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null;
        }
    }
    }
