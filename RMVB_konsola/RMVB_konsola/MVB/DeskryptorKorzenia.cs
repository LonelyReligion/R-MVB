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
        List<Wpis> wpisy;
        internal DeskryptorKorzenia() { 
            wpisy = new List<Wpis> ();
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
                for (int i = 0; i < wpisy.Count; i++) {
                    if (wpisy[i].wezel.dodaj(u))
                    {
                        if(wpisy[i].minKlucz > u.UrzadzenieID)
                            wpisy[i].minKlucz = u.UrzadzenieID;
                        else if (wpisy[i].maxKlucz < u.UrzadzenieID)
                            wpisy[i].maxKlucz = u.UrzadzenieID;

                        dodano = true;
                        break;
                    }
                }

                if (!dodano) {
                    //version split
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
