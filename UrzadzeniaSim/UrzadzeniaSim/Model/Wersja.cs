using System.ComponentModel.DataAnnotations.Schema;
using UrzadzeniaSim.Model.DB;

namespace UrzadzeniaSim.Model
{
    public class Wersja
    {
        public static Kontekst ctx;
        [ForeignKey("UrzadzenieRodzic")]
        public int UrzadzenieID { get; set; }

        public int WersjaID { get; set; }
        
        public bool Aktywne { get; set; }
        public DateTime dataOstatniejModyfikacji { get; set; }
        public DateTime dataWygasniecia { get; set; }

        //wlasnosc nawigacyjna
        public virtual ICollection<Pomiar> Pomiary { get; set; }
        public virtual Urzadzenie_Model UrzadzenieRodzic { get; set; }

        private Repo repo;

        //potrzebne do firstordefualt
        public Wersja() 
        {
            Pomiary = new HashSet<Pomiar>();
            dataOstatniejModyfikacji = DateTime.Now;
            dataWygasniecia = DateTime.MaxValue;
            Aktywne = true;
        }
        public Wersja(Repo r) : this()
        {
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

                DateTime data_wprowadzenia_zmiany = DateTime.Now;
                dataOstatniejModyfikacji = data_wprowadzenia_zmiany;
                w.dataWygasniecia = data_wprowadzenia_zmiany;
                dataWygasniecia = DateTime.MaxValue;

                ustalWersje(this.UrzadzenieID, r);
            }
        }

        //konstruktor kopiujący
        public Wersja(Wersja w, Repo r) : this(r)
        {
            this.UrzadzenieID = w.UrzadzenieID;

            ustalWersje(this.UrzadzenieID, repo);

            foreach (var element in w.Pomiary)
                this.Pomiary.Add(element);

            DateTime data_wprowadzenia_zmiany = DateTime.Now;
            dataOstatniejModyfikacji = data_wprowadzenia_zmiany;
            w.dataWygasniecia = data_wprowadzenia_zmiany;
            dataWygasniecia = DateTime.MaxValue;

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
                var ostatni_element = wersje.Last();
                this.WersjaID = ostatni_element.WersjaID + 1;

                ctx.Wersje.Attach(ostatni_element);
                ostatni_element.dezaktywuj();
            }
        }

        public void dodajPomiar(Pomiar testowy)
        {
            testowy.WersjeUrzadzenia.Add(this);
            this.Pomiary.Add(testowy);
            dataOstatniejModyfikacji = DateTime.Now;
        }

        public void usunPomiar(Pomiar testowy)
        {
            this.Pomiary.Remove(testowy);
            dataOstatniejModyfikacji = DateTime.Now;
        }
    }
}
