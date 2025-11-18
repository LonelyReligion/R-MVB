using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Wersja
    {
        public static Kontekst ctx;
        public int WersjaID { get; set; }
        [ForeignKey("UrzadzenieRodzic")]
        public int UrzadzenieID { get; set; }

        public bool Aktywne { get; set; }
        public DateTime dataOstatniejModyfikacji { get; set; }
        public DateTime dataWygasniecia { get; set; }

        //wlasnosc nawigacyjna
        public virtual ICollection<Pomiar> Pomiary { get; set; }
        public virtual Urzadzenie UrzadzenieRodzic { get; set; }

        private Repo repo;

        public Wersja(Repo r) {
            Pomiary = new HashSet<Pomiar>();
            dataOstatniejModyfikacji = DateTime.Now;
            dataWygasniecia = DateTime.MaxValue;
            Aktywne = true;
            repo = r;
        }

        public Wersja(int UrzadzenieID, Repo r) : this(r)
        {
            this.UrzadzenieID = UrzadzenieID;
            if (r.czyUrzadzenieIstnieje(UrzadzenieID) && r.pobierzUrzadzeniaWersje()[UrzadzenieID].Count() != 0)
            {
                Wersja w = r.pobierzUrzadzeniaWersje()[UrzadzenieID].Last();
                foreach (var element in w.Pomiary)
                    this.Pomiary.Add(element);
                dataOstatniejModyfikacji = w.dataWygasniecia;
                dataWygasniecia = DateTime.MaxValue;
                ustalWersje(this.UrzadzenieID, r);
            }
        }

        //czy istnieje taki przypadek
        protected Wersja(DateTime start, DateTime koniec, Repo r) : this(r)
        {
            dataOstatniejModyfikacji = start;
            dataWygasniecia = koniec;
            ustalWersje(this.UrzadzenieID, repo);
        }

        //konstruktor kopiujący
        public Wersja(Wersja w, Repo r) : this(r) {
            this.UrzadzenieID = w.UrzadzenieID;
            foreach (var element in w.Pomiary)
                this.Pomiary.Add(element);
            dataOstatniejModyfikacji = w.dataWygasniecia;
            dataWygasniecia = DateTime.MaxValue;
            ustalWersje(this.UrzadzenieID, repo);
        }

        //przetestowac, ograniczyc
        //nie używać bezpośrednio!! tylko poprzez mvb
        internal void dezaktywuj()
        {
            this.Aktywne = false;
            dataWygasniecia = DateTime.Now;
        }

        private void ustalWersje(int UrzadzenieID, Repo repo)
        {
            var wersje = repo.pobierzUrzadzeniaWersje()[UrzadzenieID];
            if (!wersje.Any())
            {
                this.WersjaID = 0;
            }
            else
            {
                var ostatni_element = wersje.LastOrDefault();
                this.WersjaID = ostatni_element.WersjaID + 1;

                ctx.Wersje.Attach(ostatni_element);
                ostatni_element.dezaktywuj();
            }
        }

        public void dodajPomiar(Pomiar testowy)
        {
            testowy.WersjeUrzadzenia.Add(this);
            dataOstatniejModyfikacji = DateTime.Now;
        }

        public void usunPomiar(Pomiar testowy)
        {
            this.Pomiary.Remove(testowy);
            dataOstatniejModyfikacji = DateTime.Now;
        }
    }
}
