using System.Data.Entity.Migrations;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace UrzadzeniaSim.Model.DB
{
    public class Repo : InDBStorage
    {

        //zmienic na przechowywanie samych id urzadzen i wersji
        //list, bo często sięgamy do ostatniego (największego) elementu
        private Dictionary<int, List<Wersja>> urzadzenia_wersje = new Dictionary<int, List<Wersja>>();

        private Dictionary<int, Urzadzenie_Model> urzadzenia = new Dictionary<int, Urzadzenie_Model>();

        //do zwrocenia wszystkich
        private List<Wersja> wersje = new List<Wersja>();

        public static Kontekst s_Ctx;

        internal Dictionary<int, List<Wersja>> zwroc_urzadzenie_wersje() { return urzadzenia_wersje; }
        //override jest konieczne inaczej realizowana jest wersja z klasy bazowej
        //"Modyfikator override jest wymagany do rozszerzenia lub zmodyfikowania abstrakcyjnej lub wirtualnej implementacji dziedziczonej metody, właściwości, indeksatora lub zdarzenia."
        public void saveDevice(Urzadzenie_Model device)
        {
            using (Kontekst ctx = new Kontekst())
            {
                ctx.Urzadzenia.Add(device);
                urzadzenia_wersje.Add(device.UrzadzenieID, new List<Wersja>());

                urzadzenia.Add(device.UrzadzenieID, device);

                ctx.SaveChanges();
            }
        }

        internal void saveDevices(List<Urzadzenie_Model> devices)
        {
            using (Kontekst ctx = new Kontekst()) { 
                foreach (Urzadzenie_Model u in devices) {
                    ctx.Urzadzenia.Add(u);
                    urzadzenia_wersje.Add(u.UrzadzenieID, new List<Wersja>());

                    urzadzenia.Add(u.UrzadzenieID, u);
                }
                ctx.SaveChanges();
            }
        }

        public void saveVersion(Wersja v)
        {
            this.pobierzUrzadzenia()[v.UrzadzenieID].Wersje.Add(v);

            using (Kontekst ctx = new Kontekst())
            {    
                ctx.Wersje.AddOrUpdate(v);
                urzadzenia_wersje[v.UrzadzenieID].Add(v);

                wersje.Add(v);
                ctx.SaveChanges();
            }
        }

        internal void saveVersions(List<Wersja> w)
        {
            foreach (Wersja v in w) {
                urzadzenia_wersje[v.UrzadzenieID].Add(v);
                wersje.Add(v);
            }

            using (Kontekst ctx = new Kontekst()) { 
                foreach (Wersja v in w)
                {
                    this.pobierzUrzadzenia()[v.UrzadzenieID].Wersje.Add(v);
                    ctx.Wersje.Add(v);
                }
                ctx.SaveChanges() ;
            }
        }

        public bool czyUrzadzenieIstnieje(int UrzadzenieID)
        {
            return urzadzenia.ContainsKey(UrzadzenieID);
        }

        public bool czyWersjaIstnieje(int UrzadzenieID, int WersjaID)
        {
            if (!urzadzenia.ContainsKey(UrzadzenieID))
                return false;
            else
                foreach (Wersja w in urzadzenia_wersje[UrzadzenieID])
                    if (w.WersjaID == WersjaID)
                        return true;
            return false;
        }

        public bool czyUrzadzenieIstnieje(decimal dlugosc, decimal szerokosc)
        {
            foreach (Urzadzenie_Model urzadzenie in urzadzenia.Values)
            {
                if (urzadzenie.Dlugosc == dlugosc && urzadzenie.Szerokosc == szerokosc)
                {
                    return true;
                };
            };
            return false;
        }

        public Dictionary<int, List<Wersja>> pobierzUrzadzeniaWersje()
        {
            return urzadzenia_wersje;
        }

        public Dictionary<int, Urzadzenie_Model> pobierzUrzadzenia()
        {
            return urzadzenia;
        }

        public List<Wersja> pobierzWersje()
        {
            return wersje;
        }
        public void Reset()
        {
            base.Reset();
            urzadzenia_wersje = new Dictionary<int, List<Wersja>>();
            urzadzenia = new Dictionary<int, Urzadzenie_Model>();
            wersje = new List<Wersja>();
        }

        public bool czyJestAktywne(int UrzadzenieID)
        {
            return (urzadzenia_wersje[UrzadzenieID].Last().Aktywne);
        }

    }
}
