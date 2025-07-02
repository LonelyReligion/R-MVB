using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class Wezel
    {
        static char aktualne_id = 'A';
        public char id;
        static int pojemnoscWezla = 6;
        //moze samo urzadzenie atp
        List<(int, Urzadzenie)> wpisy;
        internal Wezel() { 
            wpisy = new List<(int, Urzadzenie)> ();
            id = aktualne_id++;
        }
        //zwraca true jezeli sie zmiescilo, false jezeli block ov
        internal bool dodaj(Urzadzenie u) {
            if (wpisy.Count() <= pojemnoscWezla) {
                //przekazac przez ref jakos?
                wpisy.Add((u.UrzadzenieID, u));
                return true;
            }
            return false;
        }
        internal void wypisz()
        {
            Console.WriteLine(id);
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
                    wynikowy.Add("<" + (wpisy[i].Item1.ToString() + "v" + wpisy[i].Item2.Wersja.ToString()) + "," + wpisy[i].Item2.dataOstaniejModyfikacji.ToString() + "," + wpisy[i].Item2.dataWygasniecia.ToString() + ">");
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
        }
    }
}
