using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UrzadzeniaSim.Widok.Kontrolki;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model.RMVB.R;

namespace UrzadzeniaSim.Model
{
    public class Urzadzenie_Model
    {
        public Urządzenie punkt; //reprezentacja na ekranie

        public static Repo repo;
        public static Kontekst ctx;

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
        protected Urzadzenie_Model() {
            Wersje = new HashSet<Wersja>();
            punkt = new Urządzenie(0, 0);
        }
        public Urzadzenie_Model(int UrzadzenieID, (Decimal, Decimal) dlugosc_szerokosc) : this()
        {
 
            Dlugosc = dlugosc_szerokosc.Item1;
            Szerokosc = dlugosc_szerokosc.Item2;

            punkt = new Urządzenie(Dlugosc, Szerokosc);

            this.UrzadzenieID = UrzadzenieID;
        }

        public Urzadzenie_Model(int UrzadzenieID) : this() {
            this.UrzadzenieID = UrzadzenieID;
            punkt = new Urządzenie(0, 0);
        }

        //rtree
        public Decimal suma = 0;

        private int liczba_uwzglednionych = 0;
        private Decimal rTimeAggregate { get; set; }
        DateTime granica = new DateTime(2024, 7, 18, 0, 0, 0);
        public void AddMeasure(Pomiar p, TreeRepository repository)
        {
            if (p.dtpomiaru > granica)
            {
                suma += p.Wartosc;
                liczba_uwzglednionych++;

                rTimeAggregate = suma / liczba_uwzglednionych;
                TimeAggregate timeAggregate = new TimeAggregate(rTimeAggregate, DateTime.Now, UrzadzenieID);
                repository.saveTimeAggregate(timeAggregate);
            }
        }

        public void AddMeasure(DateTime t, Decimal v, TreeRepository repository)
        {
            if (t > granica)
            {
                suma += v;
                liczba_uwzglednionych++;

                rTimeAggregate = suma / liczba_uwzglednionych;
                TimeAggregate timeAggregate = new TimeAggregate(rTimeAggregate, DateTime.Now, UrzadzenieID);
                repository.saveTimeAggregate(timeAggregate);
            }
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
            //Urzadzenie thisDevice = ctx.Urzadzenia.Where(u => u.UrzadzenieID == this.UrzadzenieID).First();
            //return isValid ? thisDevice.Wersje.Last().Pomiary.Last() : null;
            //
            //return isValid ? Wersje.Last().Pomiary.Last() : null;
            return isValid ? repo.pobierzUrzadzeniaWersje()[UrzadzenieID].Last().Pomiary.Last() : null;
        }
        public bool IsMeasurementValid()
        {
            // test
            //return ctx.Urzadzenia.Where(u => u.UrzadzenieID == this.UrzadzenieID).First().Wersje.Last().Pomiary.Count > 0;
            //
            //return Wersje.Last().Pomiary.Count > 0;
            
            return false;

            //return repo.pobierzUrzadzeniaWersje()[UrzadzenieID].Last().Pomiary.Count > 0;
        }

        public bool IsTimeAggregateValid()
        {
            return rTimeAggregate != null;
        }
    }
}
