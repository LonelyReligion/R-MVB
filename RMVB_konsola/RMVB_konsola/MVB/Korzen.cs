using Microsoft.SqlServer.Server;
using RMVB_konsola.R;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
        TreeRepository repo;

        List<(int, Wpis)> wpisy; //po to zeby mozna bylo znalezc ostatni wezel szybko np.

        //parametry drzewa, sa zdefiniowane w klasie drzewa
        static double Pversion;

        internal Korzen(TreeRepository repo, double pversion)
        {
            wpisy = new List<(int, Wpis)>();
            this.repo = repo;

            Pversion = pversion;
        }

        internal void dodaj(Wersja u)
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
            return rozm_listy > Wezel.pojemnoscWezla * Wezel.Psvo;
        }
        private bool strongVersionUnderflow(int count)
        {
            //zalozenie -- w liscie sa same zywe
            return count < Wezel.pojemnoscWezla * Wezel.Psvu;
        }

        internal void versionSplit(int numer_wezla, Wersja u)
        {
            //version split
            List<Wersja> kopie = new List<Wersja>();
            if (u != null) kopie.Add(u);
            foreach (var urzadzenie in wpisy[numer_wezla].Item2.wezel.urzadzenia)
            {
                if (urzadzenie.Item2.dataWygasniecia == DateTime.MaxValue)
                { //kopiujemy zywe
                    Wersja kopia = new Wersja(urzadzenie.Item2, (Repo)repo);
                    urzadzenie.Item2.dataWygasniecia = DateTime.Now;
                    kopia.dataOstatniejModyfikacji = DateTime.Now;
                    kopie.Add(kopia);

                    repo.saveVersion(kopia);

                    ctx.Wersje.Add(kopia);
                    repo.saveVersion(kopia);
                    
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
            else
            {
                //Strong version underflows are similar to weak version
                //underflows, the only difference being that the former
                //happen after a version split, while the latter occur when
                //the weak version condition is violated.
                if (strongVersionUnderflow(posortowanaLista.Count())) //po version split w wezle 1 są same żywe czyli jest miejsce
                {
                    //a merge is attempted with the copy of a sibling node using only its live entries

                    //dodalismy na koniec, wiec sąsiad to przedostatni węzeł
                    //dodajemy same zywe wpisy z przedostatniego i zmieniamy daty obowiazywania wersji aż nie przedobrzymy, ale tym sie zajmuje .dodaj() ;)
                    List<Wersja> dzieci_sasiada = wpisy[wpisy.Count - 2].Item2.wezel.pobierzZyweUrzadzenia();

                    List<Wersja> zywe = new List<Wersja>(); //zawiera zywe
                    foreach (var urzadzenie in dzieci_sasiada.Concat(kopie))
                    {
                        if (urzadzenie.dataWygasniecia == DateTime.MaxValue)
                        { //kopiujemy zywe
                            Wersja kopia = new Wersja(urzadzenie, (Repo)repo);
                            urzadzenie.dataWygasniecia = DateTime.Now;
                            kopia.dataOstatniejModyfikacji = DateTime.Now;
                            kopie.Add(kopia);

                            repo.saveVersion(kopia);

                            ctx.Wersje.Add(kopia);
                            repo.saveVersion(kopia);

                        }
                    }

                    //posortuj liste po id 
                    var posortowaneZywe = zywe.OrderBy(q => q.UrzadzenieID);

                    this.wpisy[wpisy.Count - 2].Item2.maxData = DateTime.Now;

                    //czy tylko w last cos takiego moze zajsc? przy wstawianiu tez
                    if (strongVersionOverflow(posortowaneZywe.ToList().Count))
                    {
                        keySplit(posortowaneZywe);
                    }
                    else
                    {
                        //a jezeli dalej underflow no to chyba juz trudno? innego sasiada nie ma
                        dodajZlisty(posortowaneZywe);
                    };
                }
            };
        }
        
        //nietestowane
        internal void keySplit(IEnumerable<Wersja> kopie)
        {
            var drugi_wezel = kopie.OrderBy(q => q.UrzadzenieID).Skip(kopie.ToList().Count/2);
            var pierwszy_wezel = kopie.Except(drugi_wezel);

            dodajZlisty(pierwszy_wezel);
            dodajZlisty(drugi_wezel);
        }

        //do używania tylko wewnątrz metody .dodaj() i .versionSplit()!
        //tworzy nowy wezel z datami (data utworzenia, max_data], dodaje do niego urzadzenia z listy, dodaje wezel do drzewa
        private Wezel dodajZlisty(IEnumerable<Wersja> lista) {
            Wezel nowy = new Wezel();
            foreach (Wersja urzadzenie in lista)
            {
                nowy.dodaj(urzadzenie);
            }

            //dodaj wpis
            wpisy.Add((wpisy.Count, new Wpis(lista.First().UrzadzenieID, lista.Last().UrzadzenieID, lista.OrderBy(q => q.dataOstatniejModyfikacji).First().dataOstatniejModyfikacji, lista.OrderBy(q => q.dataWygasniecia).Last().dataWygasniecia, nowy)));
            return nowy;
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

        //potrzebne sprawdzenie weakVersionUnderflow
        internal void usun(Wersja u)
        {
            //dezaktywuj
            u.dezaktywuj();

            //znajdz wersje
            Wezel wezel_zawierający = szukaj(u.UrzadzenieID, u.WersjaID).Item1;
            //sprawdz warunek
            if (wezel_zawierający.weakVersionUnderFlow() && this.wpisy.Count != 1 /*musi miec sasiada*/)
            {
                //In both cases, a
                //merge is attempted with the copy of a sibling node using
                //only its live entries. 
                Wezel sasiad;
                if (this.wpisy.Count > wezel_zawierający.id + 1)
                    sasiad = this.wpisy[wezel_zawierający.id - 65 + 1].Item2.wezel;
                else
                    sasiad = this.wpisy[wezel_zawierający.id - 65 - 1].Item2.wezel;

                List<Wersja> kopie = new List<Wersja>(); //zawiera zywe
                foreach (var urzadzenie in wezel_zawierający.urzadzenia.Concat(sasiad.urzadzenia))
                {
                    if (urzadzenie.Item2.dataWygasniecia == DateTime.MaxValue)
                    { //kopiujemy zywe
                        Wersja kopia = new Wersja(urzadzenie.Item2, (Repo)repo);
                        urzadzenie.Item2.dataWygasniecia = DateTime.Now;
                        kopia.dataOstatniejModyfikacji = DateTime.Now;
                        kopie.Add(kopia);

                        repo.saveVersion(kopia);

                        ctx.Wersje.Add(kopia);
                        repo.saveVersion(kopia);

                    }
                }

                //posortuj liste po id 
                var posortowanaLista = kopie.OrderBy(q => q.UrzadzenieID);

                this.wpisy[wezel_zawierający.id - 65].Item2.maxData = DateTime.Now;
                this.wpisy[sasiad.id - 65].Item2.maxData = DateTime.Now;

                //czy tylko w last cos takiego moze zajsc? przy wstawianiu tez
                if (strongVersionOverflow(posortowanaLista.ToList().Count))
                {
                    keySplit(posortowanaLista);
                }
                else
                {
                    dodajZlisty(posortowanaLista);
                };
            }; 
        }

        //szukaj id i wersji
        internal (Wezel, Wersja) szukaj(int id, int v)
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
                            int wersja = wpisy_wezla[i].Item2.WersjaID;
                            if (wersja == v) {
                                return (w.wezel, wpisy_wezla[i].Item2);
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
            return (null, null); //nie znaleziono
        }

        //szukaj wersji aktualnej w danym momencie
        // [)[)
        internal Wersja szukaj(int id, DateTime dt)
        {
            //zastapic jakas wersja z binary search i stosem?
            for (int i = wpisy.Count - 1; i >= 0; i--) {
                var wpis = wpisy[i].Item2;
                if ((wpis.minData <= dt && wpis.maxData > dt) && (wpis.minKlucz <= id && wpis.maxKlucz >= id)) {
                    for (int j = 0; j < wpis.wezel.urzadzenia.Count(); j++) {
                        (int index, Wersja urzadzenie) = wpis.wezel.urzadzenia[j];
                        if(index == id && urzadzenie.dataOstatniejModyfikacji <= dt && urzadzenie.dataWygasniecia > dt)
                            return urzadzenie;
                    }
                }
            }

            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null;
        }

        //szukaj ostatniej wersji
        internal Wersja szukaj(int id)
        {
            for (int i = wpisy.Count-1; i >= 0; i--) { //od tylu 
                (int index, Wpis w) = wpisy[i];
                if (w.minKlucz <= id && w.maxKlucz >= id) {
                    for (int j = w.wezel.urzadzenia.Count - 1; j >= 0; j--) {
                        (int index_urzadzenia, Wersja u) = w.wezel.urzadzenia[j];
                        if (index_urzadzenia == id)
                            return u;
                    }
                }
            }

            Console.WriteLine("Uwaga: Nie znaleziono urzadzenia");
            return null;
        }

        internal List<Wersja> szukaj(DateTime poczatek, DateTime koniec)
        {
            if(poczatek == DateTime.MinValue && koniec==DateTime.MaxValue)
                return ((Repo)repo).wersje.ToList();

            List<Wersja> wynikowa = new List<Wersja>();
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
