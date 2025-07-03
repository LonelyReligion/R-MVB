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
        List<Wpis> wpisy;
        internal DeskryptorKorzenia(Repo repo) { 
            wpisy = new List<Wpis> ();
            this.repo = repo;
        }

        internal void dodaj(Urzadzenie u)
        {
            if (wpisy.Count() == 0)
            {
                Wezel nowy = new Wezel();
                nowy.dodaj(u);
                //przekazac przez ref jakos?
                wpisy.Add(new Wpis(u.UrzadzenieID, u.UrzadzenieID, u.dataOstaniejModyfikacji, u.dataWygasniecia, nowy));
            }
            else {
                bool dodano = false;
                int numer_wezla = wpisy.Count-1;//do tego powinnismy wstawic jezeli nie nalezy
                //do zadnego przedzialu

                for (int i = 0; i < wpisy.Count; i++) {
                    //czy nalezy do odp przedzialu kluczy
                    if (wpisy[i].maxKlucz > u.UrzadzenieID)
                    {
                        numer_wezla = i;
                        if (wpisy[i].wezel.dodaj(u)) //czy jest miejsce
                        {
                            if (wpisy[i].minKlucz > u.UrzadzenieID)
                                wpisy[i].minKlucz = u.UrzadzenieID;
                            else if (wpisy[i].maxKlucz < u.UrzadzenieID)
                                wpisy[i].maxKlucz = u.UrzadzenieID;

                            dodano = true;
                            break;
                        }
                    }
                }

                //jezeli nie dodano i id nie jest wieksze niz maxId ostatniego wezla lub on sam nie ma miejsca do wstawienia
                if (!dodano && !(numer_wezla == wpisy.Count - 1 && wpisy[numer_wezla].wezel.dodaj(u))) {
                    //version split
                    List<Urzadzenie> kopie = new List<Urzadzenie>();
                    kopie.Add(u);
                    foreach(var urzadzenie in wpisy[numer_wezla].wezel.wpisy) {
                        if (urzadzenie.Item2.dataWygasniecia == DateTime.MaxValue) { //kopiujemy zywe
                            Urzadzenie kopia = new Urzadzenie(urzadzenie.Item1, repo);
                            urzadzenie.Item2.dataWygasniecia = DateTime.Now;
                            kopia.dataOstaniejModyfikacji = DateTime.Now;
                            kopie.Add(kopia);
                        }
                    }
                    //posortuj liste po id 
                    var posortowanaLista = kopie.OrderBy(q => q.UrzadzenieID).ToList();
                    //dodaj do wezla (w odp kolejnosci?)
                    Wezel nowy = new Wezel();
                    foreach (Urzadzenie urzadzenie in posortowanaLista) {
                        nowy.dodaj(urzadzenie);
                    }

                    //dodaj wpis
                    wpisy.Add(new Wpis(posortowanaLista[0].UrzadzenieID, posortowanaLista.Last().UrzadzenieID, DateTime.Now, DateTime.MaxValue, nowy));
                    wpisy[numer_wezla].maxData = DateTime.Now;
                }
            }
        }

        internal void wypisz()
        {
            Console.WriteLine("Korzen");
            if (wpisy.Count == 0) {
                Console.WriteLine("******");
                Console.WriteLine("*    *");
                Console.WriteLine("******");
            }
            else
            {
                List<String> wynikowy = new List<String>();
                for (int i = 0; i < wpisy.Count; i++)
                {
                    wynikowy.Add("<" + wpisy[i].minKlucz.ToString() + "," + wpisy[i].maxKlucz.ToString() + "," + wpisy[i].minData.ToString() + "," + wpisy[i].maxData.ToString() + "," + wpisy[i].wezel.id + ">");
                }
                int max = wynikowy.Max(x => x.Length);
                String pozioma = "";
                for (int i = -2; i < max; i++) {
                    pozioma += "*";
                }
                Console.WriteLine(pozioma);
                for (int i = 0; i < wynikowy.Count; i++)
                {
                    Console.WriteLine("*" + wynikowy[i] + "*");
                }
                Console.WriteLine(pozioma);
            }

            foreach (var wpis in wpisy) {
                wpis.wezel.wypisz();
            }
        }
    }
}
