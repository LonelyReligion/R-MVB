using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMVB_konsola
{
    public class Urzadzenie
    {
        //klucz złożony
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UrzadzenieID { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Wersja { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Szerokosc { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Dlugosc { get; set; }

        public bool Aktywne { get; set; }
        public virtual ICollection<Pomiar> Pomiary { get; set; }
        protected Urzadzenie() { Pomiary = new HashSet<Pomiar>(); }
        public Urzadzenie(int UrzadzenieID, decimal szerokosc, decimal dlugosc) : this()
        {
            Szerokosc = szerokosc;
            Dlugosc = dlugosc;
            Aktywne = true;
            this.UrzadzenieID = UrzadzenieID;
            ustalWersje(UrzadzenieID);
        }

        public Urzadzenie(int UrzadzenieID) : this() {
            Aktywne = true;
            this.UrzadzenieID = UrzadzenieID;
            ustalWersje(UrzadzenieID);
        }

        //konstruktor kopiujący
        public Urzadzenie(Urzadzenie urzadzenie) : this() {
            this.Aktywne = true;

            this.Dlugosc = urzadzenie.Dlugosc;
            this.Szerokosc = urzadzenie.Szerokosc;
            this.UrzadzenieID = urzadzenie.UrzadzenieID;

            foreach (var element in urzadzenie.Pomiary)
                this.Pomiary.Add(element);

            ustalWersje(urzadzenie.UrzadzenieID);

        }

        //czy umozliwic ponowne aktywowanie?
        private void dezaktywuj() {
            this.Aktywne = false;
        }

        private void ustalWersje(int UrzadzenieID) {
            //zrobic osobna wersje dla drzewa R-MVB? moze będzie szybciej
            using (var ctx = new Kontekst())
            {
                var wersje = from u in ctx.Urzadzenia
                             where u.UrzadzenieID == UrzadzenieID
                             select u;
                var zmaterializowane_wersje = wersje.ToList();
                if (!zmaterializowane_wersje.Any())
                {
                    this.Wersja = 0;
                }
                else
                {
                    var ostatni_element = zmaterializowane_wersje.LastOrDefault();
                    this.Wersja = ostatni_element.Wersja + 1;

                    //zakładając (na razie pewnie błędnie) zatwierdzanie po każdej tranzacji 
                    ostatni_element.Aktywne = false;
                    ctx.SaveChanges();
                }
            }
        }
    }
}
