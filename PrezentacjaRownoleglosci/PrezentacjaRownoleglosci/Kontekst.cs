using RMVB_konsola.Indeks.R;
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
        public Kontekst() : base() {
            Database.SetInitializer<Kontekst>(new DropCreateDatabaseAlways<Kontekst>());
        }
        public DbSet<Pomiar> Pomiary { get; set; }
        public DbSet<Urzadzenie> Urzadzenia { get; set; }
        public DbSet<Wersja> Wersje { get; set; }
        public DbSet<SpaceAggregate> SpaceAggregates { get; set; }
        public DbSet<TimeAggregate> TimeAggregates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //wieksza precyzja
            modelBuilder.Entity<Wersja>()
            .Property(u => u.dataOstatniejModyfikacji)
            .HasColumnType("datetime2");

            modelBuilder.Entity<Wersja>()
            .Property(u => u.dataWygasniecia)
            .HasColumnType("datetime2");

            modelBuilder.Entity<Pomiar>()
            .Property(u => u.dtpomiaru)
            .HasColumnType("datetime2");

            modelBuilder.Entity<Urzadzenie>()
                .Property(e => e.Szerokosc)
                .HasColumnType("Decimal")
                .HasPrecision(4, 2);

            modelBuilder.Entity<Urzadzenie>()
                .Property(e => e.Dlugosc)
                .HasColumnType("Decimal")
                .HasPrecision(4, 2);

            //klucz zlozony
            modelBuilder.Entity<Wersja>()
            .HasRequired(w => w.UrzadzenieRodzic)
            .WithMany(u => u.Wersje)
            .HasForeignKey(w => w.UrzadzenieID)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Wersja>()
            .HasKey(w => new { w.UrzadzenieID, w.WersjaID });

            base.OnModelCreating(modelBuilder);
        }
    }
}
