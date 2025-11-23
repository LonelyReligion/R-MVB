using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Repo : InDBStorage
    {

        //zmienic na przechowywanie samych id urzadzen i wersji
        //list, bo często sięgamy do ostatniego (największego) elementu
        private Dictionary<int, List<Wersja>> urzadzenia_wersje = new Dictionary<int, List<Wersja>>();
        private Dictionary<int, Urzadzenie> urzadzenia = new Dictionary<int, Urzadzenie>();
        //do zwrocenia wszystkich
        private List<Wersja> wersje = new List<Wersja>();

        public static Kontekst ctx;

        internal Dictionary<int, List<Wersja>>  zwroc_urzadzenie_wersje() { return urzadzenia_wersje; }
        //override jest konieczne inaczej realizowana jest wersja z klasy bazowej
        //"Modyfikator override jest wymagany do rozszerzenia lub zmodyfikowania abstrakcyjnej lub wirtualnej implementacji dziedziczonej metody, właściwości, indeksatora lub zdarzenia."
        public override void saveDevice(Urzadzenie device) {
            ctx.Urzadzenia.Add(device);
            urzadzenia_wersje.Add(device.UrzadzenieID, new List<Wersja>()); //System.ArgumentException: „An item with the same key has already been added. Key: 0”
            urzadzenia.Add(device.UrzadzenieID, device);
            base.saveDevice(device);
        }

        public override void saveVersion(Wersja v) {
            urzadzenia_wersje[v.UrzadzenieID].Add(v);
            wersje.Add(v);
            ctx.Wersje.Add(v);
            base.saveVersion(v);
        }

        public bool czyUrzadzenieIstnieje(int UrzadzenieID) {
            return urzadzenia.ContainsKey(UrzadzenieID);
        }

        public bool czyWersjaIstnieje(int UrzadzenieID, int WersjaID) {
            if (!urzadzenia.ContainsKey(UrzadzenieID))
                return false;
            else 
                foreach (Wersja w in urzadzenia_wersje[UrzadzenieID]) 
                    if(w.WersjaID == WersjaID)
                        return  true;
            return false;
        }

        public Dictionary<int, List<Wersja>> pobierzUrzadzeniaWersje() {
            return urzadzenia_wersje;
        }

        public Dictionary<int, Urzadzenie> pobierzUrzadzenia() {
            return urzadzenia;
        }

        public List<Wersja> pobierzWersje() {
            return wersje;
        }
    }
}
