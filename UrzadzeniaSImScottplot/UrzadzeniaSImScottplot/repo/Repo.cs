using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrzadzeniaSImScottplot
{
    public class Repo
    {

        public Repo() { }
        //zmienic na przechowywanie samych id urzadzen i wersji
        //list, bo często sięgamy do ostatniego (największego) elementu
        private Dictionary<int, List<Wersja>> urzadzenia_wersje = new Dictionary<int, List<Wersja>>();

        //do zwrocenia wszystkich
        private List<Wersja> wersje = new List<Wersja>();


        internal Dictionary<int, List<Wersja>> zwroc_urzadzenie_wersje() { return urzadzenia_wersje; }
        //override jest konieczne inaczej realizowana jest wersja z klasy bazowej
        //"Modyfikator override jest wymagany do rozszerzenia lub zmodyfikowania abstrakcyjnej lub wirtualnej implementacji dziedziczonej metody, właściwości, indeksatora lub zdarzenia."
        public void saveDevice(Urzadzenie device)
        {
            using (var ctx = new Kontekst())
            {
                ctx.Urzadzenia.Add(device);
                urzadzenia_wersje.Add(device.UrzadzenieID, new List<Wersja>());
                ctx.SaveChanges();
            }
        }

        public void saveVersion(Wersja v)
        {
   

            using (var ctx = new Kontekst())
            {
                ctx.Wersje.Add(v);//or update?
                ctx.SaveChanges();
            }
            urzadzenia_wersje[v.UrzadzenieID].Add(v);

            wersje.Add(v);
           
        }

        public bool czyUrzadzenieIstnieje(int UrzadzenieID)
        {
            using (var ctx = new Kontekst())
            {
                return ctx.Urzadzenia.Where(u => u.UrzadzenieID == UrzadzenieID) != null;
            }
        }

        public bool czyWersjaIstnieje(int UrzadzenieID, int WersjaID)
        {
            if (!czyUrzadzenieIstnieje(UrzadzenieID))
                return false;
            else
                foreach (Wersja w in urzadzenia_wersje[UrzadzenieID])
                    if (w.WersjaID == WersjaID)
                        return true;
            return false;
        }



        public List<Wersja> pobierzWersje()
        {
            return wersje;
        }

        public void Reset()
        {
            urzadzenia_wersje = new Dictionary<int, List<Wersja>>();
            wersje = new List<Wersja>();
        }

        public void InicjujBazeDanych() {
            using (var ctx = new Kontekst())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM Urzadzenies");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Wersjas");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Pomiars");

                ctx.Urzadzenia.FirstOrDefault(); //ma przyspieszyc pierwsze zapytanie
            }
        }
    }
}
