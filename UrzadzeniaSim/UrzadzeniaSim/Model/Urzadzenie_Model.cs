using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model.RMVB;
using UrzadzeniaSim.Model.RMVB.R;
using UrzadzeniaSim.Widok.Kontrolki;

namespace UrzadzeniaSim.Model
{
    public class Urzadzenie_Model : INotifyPropertyChanged
    {
        public Urządzenie punkt; //reprezentacja na ekranie

        public event PropertyChangedEventHandler? PropertyChanged;

        private const bool _domyslnaWartoscCzyGenerujemy = false;
        private bool _czyGenerujemy = _domyslnaWartoscCzyGenerujemy;
        public bool CzyGenerujemy
        {
            get { return _czyGenerujemy; }
            set
            {
                _czyGenerujemy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CzyGenerujemy"));
                if (value)
                {
                    punkt.UruchomAnimacje();
                }
                else
                {
                    punkt.ZatrzymajAnimacje();
                }
            }
        }

        public static DrzewoRMVB rMVB;
        public static int nastepne_wolne_id = 0;

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
        protected Urzadzenie_Model()
        {
            Wersje = new HashSet<Wersja>();
            punkt = new Urządzenie(0, 0);
        }
        public Urzadzenie_Model((Decimal, Decimal) dlugosc_szerokosc) : this()
        {

            Dlugosc = dlugosc_szerokosc.Item1;
            Szerokosc = dlugosc_szerokosc.Item2;

            punkt = new Urządzenie(Dlugosc, Szerokosc);

            this.UrzadzenieID = nastepne_wolne_id++;
        }

        public Urzadzenie_Model(int UrzadzenieID) : this()
        {
            this.UrzadzenieID = UrzadzenieID;
            punkt = new Urządzenie(0, 0);
        }

        //rtree
        public Decimal suma = 0;

        private int _liczbaUwzglednionych = 0;
        private Decimal _rTimeAggregate { get; set; }

        //to jest do usunięcia?
        DateTime granica = new DateTime(2024, 7, 18, 0, 0, 0);


        public void AddMeasure(Pomiar p)
        {
            if (p.DataPomiaru > granica)
            {
                suma += p.Wartosc;
                _liczbaUwzglednionych++;

                _rTimeAggregate = suma / _liczbaUwzglednionych;
                TimeAggregate timeAggregate = new TimeAggregate(_rTimeAggregate, DateTime.Now, UrzadzenieID);
                repo.saveTimeAggregate(timeAggregate);
            }
            punkt.Emituj();
        }

        public void AddMeasure(DateTime t, Decimal v)
        {
            if (t > granica)
            {
                suma += v;
                _liczbaUwzglednionych++;

                _rTimeAggregate = suma / _liczbaUwzglednionych;
                TimeAggregate timeAggregate = new TimeAggregate(_rTimeAggregate, DateTime.Now, UrzadzenieID);
                repo.saveTimeAggregate(timeAggregate);
            }
            punkt.Emituj();
        }

        public Decimal GetTimeAggregate()
        {
            return _rTimeAggregate;
        }

        public (int, Decimal) get_liczba_suma()
        {
            return (_liczbaUwzglednionych, suma);
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


            return repo.pobierzUrzadzeniaWersje()[UrzadzenieID].Last().Pomiary.Count > 0;
        }

        public bool IsTimeAggregateValid()
        {
            return _rTimeAggregate != null;
        }

        public void Aktywuj()
        {
            rMVB.dodajWersje(new Wersja(UrzadzenieID, repo));
            punkt.Aktywuj();
        }

        public void Dezaktywuj()
        {
            //TODO: to potestowac w og repo
            //odnalezc ostatnia wersje i dezaktywowac, pamietac ze w mvb ma sie tez zmienic
            punkt.Dezktywuj();
        }

        internal void skaluj()
        {
            throw new NotImplementedException();
        }
    }
}
