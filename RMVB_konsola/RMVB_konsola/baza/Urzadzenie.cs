using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMVB_konsola
{
    public class Urzadzenie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UrzadzenieID { get; set; }

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
