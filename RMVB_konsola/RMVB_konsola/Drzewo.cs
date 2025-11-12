using RMVB_konsola.MVB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    internal interface Drzewo
    {
        internal abstract void wypiszDrzewo();

        internal abstract void dodajUrzadzenie(Urzadzenie u);

        internal abstract void usunUrzadzenie(Urzadzenie testowe2);
    }
}
