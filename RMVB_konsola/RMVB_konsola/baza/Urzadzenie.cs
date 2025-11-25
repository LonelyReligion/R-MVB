using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using RMVB_konsola.MVB;
using RMVB_konsola;
using RMVB_konsola.R;

namespace RMVB_konsola
{
    public class Urzadzenie
    {
        public static Kontekst ctx;
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UrzadzenieID { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Szerokosc { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Dlugosc { get; set; }
        
        //wlasnosc nawigacyjna
        public virtual ICollection<Wersja> Wersje { get; set; }

        //metody
        protected Urzadzenie() { 
            Wersje = new HashSet<Wersja>();
        }
        public Urzadzenie(int UrzadzenieID, (decimal, decimal) dlugosc_szerokosc, Repo repo) : this()
        {
 
            Dlugosc = dlugosc_szerokosc.Item1;
            Szerokosc = dlugosc_szerokosc.Item2;

            this.UrzadzenieID = UrzadzenieID;
        }

        public Urzadzenie(int UrzadzenieID, Repo repo) : this() {
            this.UrzadzenieID = UrzadzenieID;
        }

        //rtree
        public decimal suma = 0;

        private int liczba_uwzglednionych = 0;
        private decimal rTimeAggregate { get; set; }

        public void AddMeasure(Pomiar p, TreeRepository repository)
        {
            //if (p.dtpomiaru > granica)
            {
                suma += p.Wartosc;
                liczba_uwzglednionych++;

                rTimeAggregate = suma / liczba_uwzglednionych;
                TimeAggregate timeAggregate = new TimeAggregate(rTimeAggregate, DateTime.Now, UrzadzenieID);
                repository.saveTimeAggregate(timeAggregate);
            }
        }

        public void AddMeasure(DateTime t, decimal v, TreeRepository repository)
        {
            //if (t > granica)
            {
                suma += v;
                liczba_uwzglednionych++;

                rTimeAggregate = suma / liczba_uwzglednionych;
                TimeAggregate timeAggregate = new TimeAggregate(rTimeAggregate, DateTime.Now, UrzadzenieID);
                repository.saveTimeAggregate(timeAggregate);
            }
        }

        public decimal GetTimeAggregate()
        {
            return rTimeAggregate;
        }

        public (int, decimal) get_liczba_suma()
        {
            return (liczba_uwzglednionych, suma);
        }

        public Pomiar LastMeasurement()
        {
            return Wersje.Last().Pomiary.Count > 0 ? Wersje.Last().Pomiary.Last() : null;
        }
        public bool IsMeasurementValid()
        {
            return Wersje.Last().Pomiary.Count > 0;
        }

        public bool IsTimeAggregateValid()
        {
            return rTimeAggregate != null;
        }
    }
}
