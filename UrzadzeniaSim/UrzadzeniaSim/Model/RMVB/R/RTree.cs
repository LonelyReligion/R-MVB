using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model;

namespace UrzadzeniaSim.Model.RMVB.R
{
    public class RTree : RootHandler
    {
        public static Rectangle ROOTMBR = new Rectangle(49m, 14.07m, 54.5m, 24.09m);

        private RNode root = new RLeaf(ROOTMBR);

        private Repo repository;
        Kontekst ctx;

        public RTree(Repo r, Kontekst context)
        {
            repository = r;
            ctx = context;
        }

        public void Insert(Urzadzenie_Model dev)
        {
            root.Insert(dev, new TreeWalker(this));
        }

/*        public void AddMeasure(int id, Pomiar p)
        {
            Urzadzenie_Model dev = repository.pobierzUrzadzenia()[id];
            if (dev != null)
            {
                dev.AddMeasure(p, repository);
            }
        }*/

/*        public void AddMeasure(int ix, DateTime t, Decimal v)
        {
            Urzadzenie_Model dev = repository.pobierzUrzadzenia()[ix];
            if (dev != null)
            {
                dev.AddMeasure(t, v, repository);
            }
        }*/

        public RNode ProvideRoot()
        {
            return root;
        }

        public void UpdateRoot(RNode r)
        {
            root = r;
        }

        public List<Urzadzenie_Model> SearchBy(Rectangle rect)
        {
            return root.SearchBy(rect);
        }

        public Urzadzenie_Model SearchBy(Decimal x, Decimal y)
        {
            return root.SearchBy(x, y);
        }

        public (Decimal, Decimal) FindSpaceAggregate(Rectangle rect)
        {
            List<Urzadzenie_Model> devicesInRect = SearchBy(rect);
            Decimal sum = 0;
            Decimal liczba_pomiarow = 0;

            foreach (Urzadzenie_Model device in devicesInRect)
            {
                sum += device.get_liczba_suma().Item2;
                liczba_pomiarow += device.get_liczba_suma().Item1;
            }

            if (liczba_pomiarow != 0)
                return (liczba_pomiarow, sum / liczba_pomiarow);
            else
                return (0m, 0m);
        }

/*        public void SpaceAggregate()
        {
            using (var ctx = new Kontekst())
            {
                if (ctx.Pomiary.Count() != 0)
                {
                    //Stopwatch stopwatch = Stopwatch.StartNew();

                    root.SpaceAggregate(repository);

                    //long czas = stopwatch.ElapsedMilliseconds;
                    //Console.WriteLine("Czas potrzebny na agregowanie powierzchniowe: " + czas);
                }
            }
        }*/

        public Decimal GetTimeAggregate(Decimal x, Decimal y)
        {
            Urzadzenie_Model dev = SearchBy(x, y);
            Decimal result = dev.GetTimeAggregate();
            return result;
        }

        public int GtDeviceCout()
        {
            return repository.pobierzUrzadzenia().Count;
        }
    }


}
