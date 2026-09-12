using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RMVB_konsola.baza;

namespace RMVB_konsola.Indeks.MVB
{
    internal class Wezel
    {
        //A to 65
        public static char aktualne_id = 'A';
        public char id; //do wyswietlania, ale i wyszukiwania sasiada

        public static int B;
        public static double Psvu;
        public static double Psvo;
        public static double Pversion;

        //moze samo urzadzenie atp
        internal List<(int, Wersja)> urzadzenia; //zmienic
        internal Wezel()
        {
            urzadzenia = new List<(int, Wersja)>();
            id = aktualne_id++;
        }

        //zwraca true jezeli sie zmiescilo, false jezeli block ov
        internal bool dodaj(Wersja u)
        {
            if (urzadzenia.Count() <= B * Psvo)//czy wystapi svo
            {
                urzadzenia.Add((u.UrzadzenieID, u));
                urzadzenia = urzadzenia.OrderBy(w => w.Item1).ToList();
                return true;
            }
            return false;
        }
        internal List<string> drukuj()
        {
            List<string> wyjsciowa = new List<string>();
            wyjsciowa.Add(id.ToString());
            if (urzadzenia.Count == 0)
            {
                wyjsciowa.Add("******");
                wyjsciowa.Add("*    *");
                wyjsciowa.Add("******");
            }
            else
            {
                List<string> wynikowy = new List<string>();
                for (int i = 0; i < urzadzenia.Count; i++)
                {
                    wynikowy.Add("<" + urzadzenia[i].Item1.ToString() + "v" + urzadzenia[i].Item2.WersjaID.ToString() + "," + urzadzenia[i].Item2.dataOstatniejModyfikacji.ToString() + "," + urzadzenia[i].Item2.dataWygasniecia.ToString() + ">");
                }
                int max = wynikowy.Max(x => x.Length);
                string pozioma = "";
                for (int i = -2; i < max; i++)
                {
                    pozioma += "*";
                }
                wyjsciowa.Add(pozioma);
                for (int i = 0; i < wynikowy.Count; i++)
                {
                    wyjsciowa.Add("*" + wynikowy[i] + "*");
                }
                wyjsciowa.Add(pozioma);
            }
            return wyjsciowa;
        }

        internal bool strongVersionOverflow()
        {
            return liczbaZywych() > B * Psvo;
        }

        //"A strong version underflow occurs when the number of entries becomes lower than B x Psvu." 
        internal bool strongVersionUnderflow()
        {
            return liczbaZywych() < B * Psvu;
        }

        /* either none or at least P*Pversion entries are alive at time t... */
        internal bool weakVersionUnderFlow()
        {
            return liczbaZywych() != 0 && liczbaZywych() < B * Pversion;
        }

        internal int liczbaZywych()
        {
            int output = 0;
            foreach (var u in urzadzenia)
            {
                if (u.Item2.Aktywne)
                    output++;
            }
            return output;
        }

        public List<Wersja> zwrocUrzadzenia()
        {
            List<Wersja> output = new List<Wersja>();
            foreach (var wpis in urzadzenia)
                output.Add(wpis.Item2);
            return output;
        }

        internal List<Wersja> pobierzZyweUrzadzenia()
        {
            List<Wersja> output = new List<Wersja>();
            foreach (var u in urzadzenia)
            {
                if (u.Item2.Aktywne)
                    output.Add(u.Item2);
            }
            return output;
        }
    }
}
