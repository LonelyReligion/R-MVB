using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Pomiar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PomiarID { get; set; }
        public decimal Wartosc { get; set; }
        public DateTime dtpomiaru { get; set; }

        //klucz obcy
        [ForeignKey("UrzadzeniePomiarowe")]
        public int? UrzadzenieID { get; set; }
        public Urzadzenie UrzadzeniePomiarowe { get; set; }
    }
}
