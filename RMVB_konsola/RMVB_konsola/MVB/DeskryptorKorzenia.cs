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
        List<(int, DateTime, DateTime, Wezel)> wpisy;
        internal DeskryptorKorzenia() { 
            wpisy = new List<(int, DateTime, DateTime, Wezel)> ();
        }

        internal void dodaj(Urzadzenie u)
        {
            if (wpisy.Count() == 0)
            {
                Wezel nowy = new Wezel();
                nowy.dodaj(u);
                //przekazac przez ref jakos?
                wpisy.Add((u.UrzadzenieID, u.dataOstaniejModyfikacji, u.dataWygasniecia, nowy));
            }
            else {
                bool dodano = false;
                foreach (var wpis in wpisy) {
                    if (wpis.Item4.dodaj(u))
                    {
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
                    wynikowy.Add("<" + wpisy[i].Item1.ToString() + "," + wpisy[i].Item2.ToString() + "," + wpisy[i].Item3.ToString() + "," + wpisy[i].Item4.id + ">");
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
                wpis.Item4.wypisz();
            }
        }
    }
}
