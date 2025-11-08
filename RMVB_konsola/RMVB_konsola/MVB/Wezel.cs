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
        public static int pojemnoscWezla = 6;

        //moze samo urzadzenie atp
        internal List<(int, Urzadzenie)> wpisy; //zmienic
        internal Wezel() { 
            wpisy = new List<(int, Urzadzenie)> ();
            id = aktualne_id++;
        }
        
        //zwraca true jezeli sie zmiescilo, false jezeli block ov
        internal bool dodaj(Urzadzenie u) {
            if (wpisy.Count() < pojemnoscWezla) {
                wpisy.Add((u.UrzadzenieID, u));
                wpisy = wpisy.OrderBy(w => w.Item1).ToList();
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
                    wynikowy.Add("<" + (wpisy[i].Item1.ToString() + "v" + wpisy[i].Item2.Wersja.ToString()) + "," + wpisy[i].Item2.dataOstatniejModyfikacji.ToString() + "," + wpisy[i].Item2.dataWygasniecia.ToString() + ">");
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

        internal bool strongVersionOverflow(double Psvo)
        {
            return wpisy.Count > pojemnoscWezla * Psvo;
        }

        internal bool strongVersionUnderflow(double Psvu)
        {
            return wpisy.Count < pojemnoscWezla * Psvu;
        }

        public List<Urzadzenie> zwrocUrzadzenia() { 
            List<Urzadzenie> output = new List<Urzadzenie>();
            foreach (var wpis in wpisy)
                output.Add(wpis.Item2);
            return output;
        }
    }
}
