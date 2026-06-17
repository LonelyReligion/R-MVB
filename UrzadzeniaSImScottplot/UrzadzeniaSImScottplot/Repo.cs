using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrzadzeniaSImScottplot
{
    class Repo
    {

        public Repo() { }
        public void InicjujBazeDanych() {
            using (var ctx = new Kontekst())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM Urzadzenies");
                ctx.Urzadzenia.FirstOrDefault(); //ma przyspieszyc pierwsze zapytanie
            }
        }
    }
}
