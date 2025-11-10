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

        //relacja wiele do wielu wynika z dodania wersji
        public virtual ICollection<Urzadzenie> UrzadzeniaPomiarowe { get; set; }

        public Pomiar() { 
            UrzadzeniaPomiarowe = new HashSet<Urzadzenie>();
        }
    }
}
