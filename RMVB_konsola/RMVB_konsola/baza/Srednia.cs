using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Srednia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SredniaID { get; set; }
        public Decimal wartosc { get; set; }
        public Decimal x1 { get; set; }
        public Decimal x2 { get; set; }
        public Decimal y1 { get; set; }
        public Decimal y2 { get; set; }
        public DateTime utworzona { get; set; }
    }
}
