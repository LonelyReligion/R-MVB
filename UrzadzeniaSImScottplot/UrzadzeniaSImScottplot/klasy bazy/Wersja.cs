using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace UrzadzeniaSImScottplot
{
    public class Wersja
    {
        [ForeignKey("UrzadzenieRodzic")]
        public int UrzadzenieID { get; set; }

        public int WersjaID { get; set; }
        
        public bool Aktywne { get; set; }
        public DateTime dataOstatniejModyfikacji { get; set; }
        public DateTime dataWygasniecia { get; set; }

        //wlasnosc nawigacyjna
        public virtual ICollection<Pomiar> Pomiary { get; set; }
        public virtual Urzadzenie UrzadzenieRodzic { get; set; }

        private Repo repo;
        private RMVB _rmvb;

        //potrzebne do firstordefualt
        public Wersja() 
        {
            Pomiary = new HashSet<Pomiar>();
            dataOstatniejModyfikacji = DateTime.Now;
            dataWygasniecia = DateTime.MaxValue;
            Aktywne = true;
        }
        public Wersja(Repo r, RMVB rmvb) : this()
        {
            repo = r;
            _rmvb = rmvb;
        }

        public Wersja(int UrzadzenieID, Repo r, RMVB mvb) : this(r, mvb)
        {
            this.UrzadzenieID = UrzadzenieID;
            if (r.czyUrzadzenieIstnieje(UrzadzenieID) && r.zwroc_urzadzenie_wersje()[UrzadzenieID].Count() != 0)
            {
                Wersja w;
                using (var ctx = new Kontekst())
                {
                    int id_wersji = r.zwroc_urzadzenie_wersje()[UrzadzenieID].Last();
                    w = ctx.Wersje.Include(x => x.Pomiary).First(x => x.UrzadzenieID == UrzadzenieID && x.WersjaID == id_wersji);

                    r.zwroc_urzadzenie_wersje()[UrzadzenieID].Last();
                    foreach (var element in w.Pomiary)
                        this.Pomiary.Add(element);

                    DateTime data_wprowadzenia_zmiany = DateTime.Now;

                    dataOstatniejModyfikacji = data_wprowadzenia_zmiany;




                    dataWygasniecia = DateTime.MaxValue;

                    ustalWersje(this.UrzadzenieID, r);
                }
            }
        }

        //konstruktor kopiujący
        public Wersja(Wersja w, Repo r, RMVB rmvb) : this(r, rmvb) {
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
        internal void dezaktywuj(DateTime moment)
        {
            this.Aktywne = false;
            dataWygasniecia = moment;
        }

        private void ustalWersje(int UrzadzenieID, Repo repo)
        {
            DateTime moment = DateTime.Now;
            var wersje = repo.zwroc_urzadzenie_wersje()[UrzadzenieID];
            if (!wersje.Any())
            {
                this.WersjaID = 0;
            }
            else
            {
                var ostatni_element = wersje.Last();
                this.WersjaID = ostatni_element + 1;

                using (var ctx = new Kontekst())
                {
                    Wersja wersja = ctx.Wersje.Where(w => (w.UrzadzenieID == UrzadzenieID && w.WersjaID == ostatni_element)).First();
                    wersja.dezaktywuj(moment);
                    _rmvb.szukaj(UrzadzenieID, ostatni_element).dezaktywuj(moment);
                    ctx.SaveChanges();
                }
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
