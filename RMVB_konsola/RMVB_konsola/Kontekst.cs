using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Kontekst : DbContext
    {
        public Kontekst() : base() {}
        public DbSet<Pomiar> Pomiary { get; set; }
        public DbSet<Srednia> Srednie { get; set; }
        public DbSet<Urzadzenie> Urzadzenia { get; set; }
    }
}
