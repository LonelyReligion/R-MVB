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
        public DateTime DataOstatniejModyfikacji { get; set; }
        public DateTime DataWygasniecia { get; set; }

        //wlasnosc nawigacyjna
        public virtual ICollection<Pomiar> Pomiary { get; set; }
        public virtual Urzadzenie_Model UrzadzenieRodzic { get; set; }

        private Repo _repo;

        //potrzebne do firstordefualt
        public Wersja()
        {
            Pomiary = new HashSet<Pomiar>();
            DataOstatniejModyfikacji = DateTime.Now;
            DataWygasniecia = DateTime.MaxValue;
            Aktywne = true;
        }
        public Wersja(Repo r) : this()
        {
            _repo = r;
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
                DataOstatniejModyfikacji = data_wprowadzenia_zmiany;
                w.DataWygasniecia = data_wprowadzenia_zmiany;
                DataWygasniecia = DateTime.MaxValue;

                _ustalWersje(this.UrzadzenieID, r);
            }
        }

        //konstruktor kopiujący
        public Wersja(Wersja w, Repo r) : this(r)
        {
            this.UrzadzenieID = w.UrzadzenieID;

            _ustalWersje(this.UrzadzenieID, _repo);

            foreach (var element in w.Pomiary)
                this.Pomiary.Add(element);

            DateTime data_wprowadzenia_zmiany = DateTime.Now;
            DataOstatniejModyfikacji = data_wprowadzenia_zmiany;
            w.DataWygasniecia = data_wprowadzenia_zmiany;
            DataWygasniecia = DateTime.MaxValue;

        }

        //przetestowac, ograniczyc
        //nie używać bezpośrednio!! tylko poprzez mvb
        internal void _dezaktywuj()
        {
            this.Aktywne = false;
            DataWygasniecia = DateTime.Now;
        }

        private void _ustalWersje(int UrzadzenieID, Repo repo)
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
                ostatni_element._dezaktywuj();
            }
        }

        public void DodajPomiar(Pomiar testowy)
        {
            testowy.WersjeUrzadzenia.Add(this);
            this.Pomiary.Add(testowy);
            DataOstatniejModyfikacji = DateTime.Now;
        }

        public void UsunPomiar(Pomiar testowy)
        {
            this.Pomiary.Remove(testowy);
            DataOstatniejModyfikacji = DateTime.Now;
        }
    }
}
