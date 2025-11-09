using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class Korzen
    {
        public static Kontekst ctx;
        Repo repo;
        List<(int, Wpis)> wpisy;

        //parametry drzewa, sa zdefiniowane w klasie drzewa
        static double Pversion;
        static double Psvu;
        static double Psvo;

        internal Korzen(Repo repo, double pversion, double psvu, double psvo)
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
                wpisy.Add((wpisy.Count, new Wpis(u.UrzadzenieID, u.UrzadzenieID, u.dataOstatniejModyfikacji, u.dataWygasniecia, nowy)));
            }
            else
            {
                bool dodano = false;
                int numer_wezla = wpisy.Count - 1; //do tego powinnismy wstawic jezeli nie nalezy
                //do zadnego przedzialu

                for (int i = wpisy.Count - 1; i >= 0 ; i--) //szukamy od najnowszych do najstarszych
                {
                    //czy nalezy do odp przedzialu kluczy
                    if (wpisy[i].Item2.maxKlucz >= u.UrzadzenieID && wpisy[i].Item2.minKlucz <= u.UrzadzenieID) //uwzglednic tez daty?
                    {
                        numer_wezla = i;
                        if (wpisy[i].Item2.wezel.dodaj(u)) //czy jest miejsce
                        {
                            //czy to sie wgl wykona kiedykolwiek?
                            if (wpisy[i].Item2.minKlucz > u.UrzadzenieID)
                                wpisy[i].Item2.minKlucz = u.UrzadzenieID;
                            else if (wpisy[i].Item2.maxKlucz < u.UrzadzenieID)
                                wpisy[i].Item2.maxKlucz = u.UrzadzenieID;

                            if (wpisy[i].Item2.minData > u.dataOstatniejModyfikacji)
                                wpisy[i].Item2.minData = u.dataOstatniejModyfikacji;
                            else if (wpisy[i].Item2.maxData < u.dataWygasniecia)
                                wpisy[i].Item2.maxData = u.dataWygasniecia;

                            dodano = true;
                            break;
                        }
                    }
                }

                //jezeli nie dodano i id nie jest wieksze niz maxId ostatniego wezla lub on sam nie ma miejsca do wstawienia
                if (!dodano && !(numer_wezla == wpisy.Count - 1 && wpisy[numer_wezla].Item2.wezel.dodaj(u)))
                {
                    //wezel jest pelny
                    versionSplit(numer_wezla, u);
                }
                else {
                    if (wpisy[numer_wezla].Item2.minKlucz > u.UrzadzenieID)
                        wpisy[numer_wezla].Item2.minKlucz = u.UrzadzenieID;
                    else if (wpisy[numer_wezla].Item2.maxKlucz < u.UrzadzenieID)
                        wpisy[numer_wezla].Item2.maxKlucz = u.UrzadzenieID;

                    if (wpisy[numer_wezla].Item2.minData > u.dataOstatniejModyfikacji)
                        wpisy[numer_wezla].Item2.minData = u.dataOstatniejModyfikacji;
                    else if (wpisy[numer_wezla].Item2.maxData < u.dataWygasniecia)
                        wpisy[numer_wezla].Item2.maxData = u.dataWygasniecia;
                }
            }
        }

        //dla POTENCJALNEGO węzła
        internal bool strongVersionOverflow(int rozm_listy) {
            return rozm_listy > Wezel.pojemnoscWezla * Psvo;
        }

        internal void versionSplit(int numer_wezla, Urzadzenie u)
        {
            //version split
            List<Urzadzenie> kopie = new List<Urzadzenie>();
            if (u != null) kopie.Add(u);
            foreach (var urzadzenie in wpisy[numer_wezla].Item2.wezel.urzadzenia)
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
            //posortuj liste po id 
            var posortowanaLista = kopie.OrderBy(q => q.UrzadzenieID);
            
 

            wpisy[numer_wezla].Item2.maxData = DateTime.Now;

            //czy tylko w last cos takiego moze zajsc? przy wstawianiu tez
            if (strongVersionOverflow(posortowanaLista.ToList().Count))
            {
                keySplit(posortowanaLista);
            }
/*            //nietestowane, czy to jest potrzebne? czy tylko przy usuwaniu logicznym
            else if (wpisy.Last().Item2.wezel.strongVersionUnderflow(Psvu))
            {
                // u juz jest w wezle
                versionSplit(wpisy.Last().Item1, null); //czy tu jest potencjalnie pętla?
            }*/
            else
            {
                dodajZlisty(posortowanaLista);
            };
        }

        //nietestowane
        internal void keySplit(IEnumerable<Urzadzenie> kopie)
        {
            var drugi_wezel = kopie.OrderBy(q => q.UrzadzenieID).Skip(kopie.ToList().Count/2);
            var pierwszy_wezel = kopie.Except(drugi_wezel);

            dodajZlisty(pierwszy_wezel);
            dodajZlisty(drugi_wezel);
        }

        //tworzy nowy wezel z datami (data utworzenia, max_data], dodaje do niego urzadzenia z listy, dodaje wezel do drzewa
        internal void dodajZlisty(IEnumerable<Urzadzenie> lista) {
            Wezel nowy = new Wezel();
            foreach (Urzadzenie urzadzenie in lista)
            {
                nowy.dodaj(urzadzenie);
            }

            //dodaj wpis
            wpisy.Add((wpisy.Count, new Wpis(lista.First().UrzadzenieID, lista.Last().UrzadzenieID, lista.OrderBy(q => q.dataOstatniejModyfikacji).First().dataOstatniejModyfikacji, lista.OrderBy(q => q.dataWygasniecia).Last().dataWygasniecia, nowy)));
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
            HashSet<int> odwiedzone = new HashSet<int>(); //powinno naprawic nieskonczona petle

            do_przejrzenia.Push(wpisy[poczatkowy_indeks]);
            int najwyzsza_wersja = -1;
            while (do_przejrzenia.Count != 0){
                (int indeks, Wpis w) = do_przejrzenia.Pop();
                if (w.minKlucz <= id && w.maxKlucz >= id) {
                    var wpisy_wezla = w.wezel.urzadzenia;
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
                if (najwyzsza_wersja == -1) 
                {
                    if (indeks + 1 < wpisy.Count && !odwiedzone.Contains(indeks + 1))
                    {
                        do_przejrzenia.Push(wpisy[indeks + 1]);
                        odwiedzone.Add(indeks + 1);
                    }
                    if (indeks - 1 >= 0 && !odwiedzone.Contains(indeks - 1))
                    {
                        do_przejrzenia.Push(wpisy[indeks - 1]);
                        odwiedzone.Add(indeks - 1);
                    }
                }
                else {
                    if (najwyzsza_wersja < v && indeks + 1 < wpisy.Count)
                    {
                        do_przejrzenia.Push(wpisy[indeks + 1]);
                        odwiedzone.Add(indeks + 1);
                    }
                    else if (indeks - 1 >= 0 && !odwiedzone.Contains(indeks - 1))
                    {
                        do_przejrzenia.Push(wpisy[indeks - 1]);
                        odwiedzone.Add(indeks - 1);
                    }
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
                    for (int j = 0; j < wpis.wezel.urzadzenia.Count(); j++) {
                        (int index, Urzadzenie urzadzenie) = wpis.wezel.urzadzenia[j];
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
                    for (int j = w.wezel.urzadzenia.Count - 1; j >= 0; j--) {
                        (int index_urzadzenia, Urzadzenie u) = w.wezel.urzadzenia[j];
                        if (index_urzadzenia == id)
                            return u;
                    }
                }
            }

            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null;
        }

        internal List<Urzadzenie> szukaj(DateTime poczatek, DateTime koniec)
        {
            if(poczatek == DateTime.MinValue && koniec==DateTime.MaxValue)
                return repo.urzadzenia.ToList();

            List<Urzadzenie> wynikowa = new List<Urzadzenie>();
            for (int i = 0; i < wpisy.Count; i++) {
                Wpis wpis = wpisy[i].Item2;
                if ((wpis.minData < poczatek && wpis.maxData < poczatek)||(wpis.minData >= koniec && wpis.maxData >= koniec))
                    ;
                else if (wpis.minData == poczatek && wpis.maxData < koniec)
                    wynikowa.AddRange(wpis.wezel.zwrocUrzadzenia());
                else {
                    var urzadzenia = wpis.wezel.urzadzenia;
                    for (int j = 0; j < urzadzenia.Count(); j++) {
                        if (urzadzenia[j].Item2.dataOstatniejModyfikacji >= poczatek && urzadzenia[j].Item2.dataWygasniecia < koniec)
                            wynikowa.Add(urzadzenia[j].Item2);
                    }
                }
            }
            return wynikowa; 
        }
    }
    }
