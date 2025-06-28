using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMVB_konsola
{
    public class Urzadzenie
    {
        //klucz złożony
        [Key, Column(Order = 0)]
        public int UrzadzenieID { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Wersja { get; set; }


        [Column(TypeName = "decimal")]
        public decimal Szerokosc { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Dlugosc { get; set; }

        public bool Aktywne { get; set; }
        public ICollection<Pomiar> Pomiary { get; set; }

        public Urzadzenie(decimal szerokosc, decimal dlugosc)
        {
            Szerokosc = szerokosc;
            Dlugosc = dlugosc;
        }

        public Urzadzenie() { }
    }
}
