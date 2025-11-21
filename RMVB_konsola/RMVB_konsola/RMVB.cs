using RMVB_konsola.MVB;
using RMVB_konsola.R;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    internal class RMVB
    {
        private Kontekst ctx;
        private DrzewoMVB MVB;
        private RTreeAdapter R;
        private Repo repo;
        public RMVB(Kontekst ctx) {
            this.ctx = ctx;
            repo = new Repo();
            MVB = new DrzewoMVB(repo, ctx);
            R = new RTreeAdapter(new RTree(repo, ctx));
        }

        //dodaj
        public void dodajUrzadzenie(Urzadzenie u) {
            R.dodajUrzadzenie(u);
            ctx.SaveChanges();
        }

        public void dodajWersje(Wersja w) {
            MVB.dodajUrzadzenie(w);
            repo.saveVersion(w);
        }

        public void dodajPomiar(int UrzadzenieID, Pomiar p) {
            ctx.Pomiary.Add(p);
            R.dodajPomiar(UrzadzenieID, p);
            ctx.SaveChanges();//?
        }

        //usun
        public void usunWersje(Wersja w) {
            MVB.dodajUrzadzenie(w);
            MVB.usunUrzadzenie(w);
            repo.saveVersion(w);
        }

        //szukaj


    }
}
