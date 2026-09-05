using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using RMVB_konsola.Indeks.MVB;
using System.Diagnostics.CodeAnalysis;
using RMVB_konsola.Indeks.R;
using RMVB_konsola.baza;

namespace RMVB_konsola
{
    public class Urzadzenie
    {
        public static int nastepne_wolne_id = 0;
        public static Repo repo;
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UrzadzenieID { get; set; }

        [Column(TypeName = "Decimal")]
        public Decimal Szerokosc { get; set; }

        [Column(TypeName = "Decimal")]
        public Decimal Dlugosc { get; set; }
        
        //wlasnosc nawigacyjna
        public virtual ICollection<Wersja> Wersje { get; set; }

        //metody
        protected Urzadzenie() { 
            Wersje = new HashSet<Wersja>();
        }
        public Urzadzenie((Decimal, Decimal) dlugosc_szerokosc) : this()
        {
 
            Dlugosc = dlugosc_szerokosc.Item1;
            Szerokosc = dlugosc_szerokosc.Item2;

            this.UrzadzenieID = nastepne_wolne_id++;
        }

        public Urzadzenie(int UrzadzenieID) : this() {
            this.UrzadzenieID = UrzadzenieID;
        }

        //rtree
        public Decimal suma = 0;

        private int liczba_uwzglednionych = 0;
        public Decimal rTimeAggregate { get; set; }
        public void AddMeasure(Pomiar p, Repo repository)
        {
            suma += p.Wartosc;
            liczba_uwzglednionych++;

            rTimeAggregate = suma / liczba_uwzglednionych;
            TimeAggregate timeAggregate = new TimeAggregate(rTimeAggregate, DateTime.Now, UrzadzenieID);
            repository.saveTimeAggregate(timeAggregate);
        }

        public void AddMeasure(DateTime t, Decimal v, Repo repository)
        {
            suma += v;
            liczba_uwzglednionych++;

            rTimeAggregate = suma / liczba_uwzglednionych;
            TimeAggregate timeAggregate = new TimeAggregate(rTimeAggregate, DateTime.Now, UrzadzenieID);
            repository.saveTimeAggregate(timeAggregate);
        }

        public Decimal GetTimeAggregate()
        {
            return rTimeAggregate;
        }

        public (int, Decimal) get_liczba_suma()
        {
            return (liczba_uwzglednionych, suma);
        }

        public Pomiar LastMeasurement()
        {
            // test
            bool isValid = IsMeasurementValid();
            using (var ctx = new Kontekst())
            {
                Urzadzenie thisDevice = ctx.Urzadzenia.Where(u => u.UrzadzenieID == this.UrzadzenieID).First();
                Wersja ostatnia = thisDevice.Wersje.Last();
                return isValid ? ostatnia.Pomiary.Last() : null;
            }

            //
            //return isValid ? Wersje.Last().Pomiary.Last() : null;
            //return isValid ? repo.zwroc_urzadzenie_wersje()[UrzadzenieID].Last().Pomiary.Last() : null;
        }
        public bool IsMeasurementValid()
        {
            // test
            using (var ctx = new Kontekst())
                return ctx.Urzadzenia.Where(u => u.UrzadzenieID == this.UrzadzenieID).First().Wersje.Last().Pomiary.Count > 0;
            
            //
            //return Wersje.Last().Pomiary.Count > 0;
            //return repo.zwroc_urzadzenie_wersje()[UrzadzenieID].Last().Pomiary.Count > 0;
        }

        public bool IsTimeAggregateValid()
        {
            return rTimeAggregate != null;
        }
    }
}
