using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RMVB_konsola.R;

namespace RMVB_konsola.Indeks.R
{
    public class RTree : RootHandler
    {
        public static Rectangle ROOTMBR = new Rectangle(49m, 14.07m, 54.5m, 24.09m);

        private RNode root = new RLeaf(ROOTMBR);

        private Repo repository;

        public RTree(Repo r)
        {
            repository = r;
        }

        public void Insert(Urzadzenie dev)
        {
            root.Insert(dev, new TreeWalker(this));
        }

        public void AddMeasure(int id, Pomiar p)
        {
            Urzadzenie dev = repository.pobierzUrzadzenia()[id];
            if (dev != null)
            {
                dev.AddMeasure(p, repository);
            }
        }

        public void AddMeasure(int ix, DateTime t, decimal v)
        {
            Urzadzenie dev = repository.pobierzUrzadzenia()[ix];
            if (dev != null)
            {
                dev.AddMeasure(t, v, repository);
            }
        }

        public RNode ProvideRoot()
        {
            return root;
        }

        public void UpdateRoot(RNode r)
        {
            root = r;
        }

        public List<Urzadzenie> SearchBy(Rectangle rect)
        {
            return root.SearchBy(rect);
        }

        public Urzadzenie SearchBy(decimal x, decimal y)
        {
            return root.SearchBy(x, y);
        }

        //liczona w inny sposob, ba agregatow, zwraca wiecej informacji
        public (List<int>, decimal, decimal) FindSpaceAggregateDEBUG(Rectangle rect)
        {
            List<Urzadzenie> devicesInRect = SearchBy(rect);

            decimal sum = 0;
            decimal liczba_pomiarow = 0;

            foreach (Urzadzenie device in devicesInRect)
            {
                sum += device.get_liczba_suma().Item2;
                liczba_pomiarow += device.get_liczba_suma().Item1;
            }

            List<int> ids = devicesInRect.Select((o) => o.UrzadzenieID).ToList();
            if (liczba_pomiarow != 0)
                return (ids, liczba_pomiarow, sum / liczba_pomiarow);
            else
                return (ids, 0m, 0m);
        }

        // (liczba pomiarow, wynik)
        public (decimal, decimal) FindSpaceAggregate(Rectangle rect)
        {
            return root.FindSpaceAggregate(rect);
        }
        //

        public void SpaceAggregate()
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
        }

        public decimal GetTimeAggregate(decimal x, decimal y)
        {
            Urzadzenie dev = SearchBy(x, y);
            decimal result = dev.GetTimeAggregate();
            return result;
        }

        public int GtDeviceCout()
        {
            return repository.pobierzUrzadzenia().Count;
        }
    }


}
