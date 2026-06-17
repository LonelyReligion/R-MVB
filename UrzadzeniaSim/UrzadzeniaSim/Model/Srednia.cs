using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrzadzeniaSim.Model
{
    public class Srednia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SredniaID { get; set; }
        public Decimal Wartosc { get; set; }
        public Decimal X1 { get; set; }
        public Decimal X2 { get; set; }
        public Decimal Y1 { get; set; }
        public Decimal Y2 { get; set; }
        public DateTime Utworzona { get; set; }
    }
}
