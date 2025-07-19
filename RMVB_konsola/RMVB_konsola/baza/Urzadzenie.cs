using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMVB_konsola
{
    public class Urzadzenie
    {
        public static Kontekst ctx;

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
        public DateTime dataOstatniejModyfikacji { get; set; }
        public DateTime dataWygasniecia { get; set; }

        //wlasnosc nawigacyjna
        public virtual ICollection<Pomiar> Pomiary { get; set; }

        //metody
        protected Urzadzenie() { 
            Pomiary = new HashSet<Pomiar>(); 
            //Console.WriteLine("wywolano konstruktor bezarg. urzadzenia");
        }
        public Urzadzenie(int UrzadzenieID, decimal szerokosc, decimal dlugosc, Repo repo) : this()
        {
            Szerokosc = szerokosc;
            Dlugosc = dlugosc;
            Aktywne = true;
            this.UrzadzenieID = UrzadzenieID;
            ustalWersje(UrzadzenieID, repo);

            dataOstatniejModyfikacji = DateTime.Now;
            dataWygasniecia = DateTime.MaxValue;
        }

        public Urzadzenie(int UrzadzenieID, Repo repo) : this() {
            Aktywne = true;
            this.UrzadzenieID = UrzadzenieID;
            ustalWersje(UrzadzenieID, repo);
            dataOstatniejModyfikacji = DateTime.Now;
            dataWygasniecia = DateTime.MaxValue;
        }

        //konstruktor kopiujący
        public Urzadzenie(Urzadzenie urzadzenie, Repo repo) : this() {
            this.Aktywne = true;

            this.Dlugosc = urzadzenie.Dlugosc;
            this.Szerokosc = urzadzenie.Szerokosc;
            this.UrzadzenieID = urzadzenie.UrzadzenieID;

            foreach (var element in urzadzenie.Pomiary)
                this.Pomiary.Add(element);

            ustalWersje(urzadzenie.UrzadzenieID, repo);
            dataOstatniejModyfikacji = urzadzenie.dataWygasniecia;
            dataWygasniecia = DateTime.MaxValue; 
        }

        public void dezaktywuj() {
            this.Aktywne = false;
            dataWygasniecia = DateTime.Now;
        }

        private void ustalWersje(int UrzadzenieID, Repo repo) {
            var wersje = repo.urzadzenia.Where(u => u.UrzadzenieID == UrzadzenieID).ToList();
            if (!wersje.Any())
            {
                this.Wersja = 0;
            }
            else
            {
                var ostatni_element = wersje.LastOrDefault(); 
                this.Wersja = ostatni_element.Wersja + 1;

                ctx.Urzadzenia.Attach(ostatni_element);
                ostatni_element.dezaktywuj();
            }
        }

        public void dodajPomiar(Pomiar testowy) {
            testowy.UrzadzeniaPomiarowe.Add(this);
            dataOstatniejModyfikacji = DateTime.Now;
        }

        public void usunPomiar(Pomiar testowy) {
            this.Pomiary.Remove(testowy);
            dataOstatniejModyfikacji = DateTime.Now;
        }
    }
}
