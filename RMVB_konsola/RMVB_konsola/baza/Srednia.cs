using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO
//zrobic konstruktor kopiujący dla urządzenia

namespace RMVB_konsola
{
    public class Srednia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SredniaID { get; set; }
        public decimal wartosc { get; set; }
        public decimal x1 { get; set; }
        public decimal x2 { get; set; }
        public decimal y1 { get; set; }
        public decimal y2 { get; set; }
        public DateTime utworzona { get; set; }
    }
}
