using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Repo
    {
        public List<Urzadzenie> urzadzenia = new List<Urzadzenie>();

        public void dodajUrzadzenie(Urzadzenie u) {
            urzadzenia.Add(u);
        }
    }
}
