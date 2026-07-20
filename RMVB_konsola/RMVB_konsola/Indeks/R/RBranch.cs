using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RMVB_konsola.Indeks.R
{
    public class RBranch : RNode
    {
        private List<RNode> children = new List<RNode>();

        decimal? ostatni_agregat_czasowy;
        decimal? liczba_elementów;
        
        Repo _repo;

        public override int zwrocLiczbeDzieci()
        {
            return children.Count;
        }
        public RBranch(decimal xmin, decimal ymin, decimal xmax, decimal ymax, Repo repo) : base(xmin, ymin, xmax, ymax)
        {
            _repo = repo;
        }

        public RBranch(Rectangle r, Repo repo) : base(r)
        {
            _repo = repo;
        }

        public void AddChild(RNode child)
        {
            children.Add(child);
        }

        public override int EntriesCount()
        {
            return children.Count;
        }

        public override RNode Clone()
        {
            return new RBranch(0, 0, 0, 0, _repo);
        }

        public override RNode MoveEntry(RNode destination, int index)
        {
            RBranch dest = (RBranch)destination;
            dest.children.Add(children[index]);
            return this;
        }

        public override RNode RemoveEntry(int index)
        {
            children.RemoveAt(index);
            return this;
        }

        public RNode RemoveEntry(RNode child)
        {
            children.Remove(child);
            return this;
        }

        public override void Insert(Urzadzenie dev, TreeWalker adaptor)
        {
            foreach (var ch in children)
            {
                if (ch.mbr.Contains(dev.Szerokosc, dev.Dlugosc))
                {
                    ch.Insert(dev, adaptor);
                    return;
                }
            }
            decimal minEnlargement = decimal.MaxValue;
            RNode minimal = null;

            foreach (var ch in children)
            {
                decimal enlargement = Math.Abs(ch.mbr.EnlargedBy(dev.Szerokosc, dev.Dlugosc).Area() - ch.mbr.Area());

                if (enlargement < minEnlargement)
                {
                    minEnlargement = enlargement;
                    minimal = ch;
                }
            }
            minimal.mbr.Enlarge(dev.Dlugosc, dev.Szerokosc);
            minimal.Insert(dev, adaptor);
        }

        public override decimal Distance(int i, int j)
        {
            return children[i].mbr.Distance(children[j].mbr);
        }

        public void Add(RNode node)
        {
            children.Add(node);
        }

        public override RBranch FindParent(RNode node)
        {
            foreach (RNode ch in children)
            {
                if (ch == node)
                {
                    return this;
                }

                RBranch parent = ch.FindParent(node);
                if (parent != null)
                {
                    return parent;
                }
            }
            return null;
        }

        public override void SplitEntries(RNode pNode, RNode kNode)
        {
            decimal maxDiff = decimal.MinValue;
            RNode chToAdd = null;
            RBranch destination = (RBranch)pNode;

            foreach (RNode ch in children)
            {

                decimal distP = pNode.mbr.EnlargedBy(ch.mbr).Area() - pNode.mbr.Area();
                decimal distK = kNode.mbr.EnlargedBy(ch.mbr).Area() - kNode.mbr.Area();
                decimal diff = Math.Abs(distP - distK);

                if (diff > maxDiff)
                {
                    maxDiff = diff;
                    chToAdd = ch;
                    if (distP > distK)
                    {
                        destination = (RBranch)kNode;
                    }
                    else
                    {
                        destination = (RBranch)pNode;
                    }
                }
            }
            destination.Add(chToAdd);
            children.Remove(chToAdd);
        }

        public override void UpdateMBR()
        {
            mbr.XMin = decimal.MaxValue;
            mbr.YMin = decimal.MaxValue;
            mbr.XMax = decimal.MinValue;
            mbr.YMax = decimal.MinValue;

            foreach (RNode ch in children)
            {
                mbr.XMin = Math.Min(mbr.XMin, ch.mbr.XMin);
                mbr.YMin = Math.Min(mbr.YMin, ch.mbr.YMin);
                mbr.XMax = Math.Max(mbr.XMax, ch.mbr.XMax);
                mbr.YMax = Math.Max(mbr.YMax, ch.mbr.YMax);
            }
        }

        public override (double, int) SpaceAggregate(Repo repository)
        {
            double sum = 0;
            int counter = 0;
            foreach (RNode ch in children)
            {
                // test
                //Parallel.ForEach(children, ch =>
                //{
                //
                (double, int) res = ch.SpaceAggregate(repository);
                if (res.Item2 > 0)
                {
                    sum += res.Item1;
                    counter += res.Item2;
                }
            }
            //});

            if (counter > 0)
            {
                decimal valueSpaceAggregate = (decimal)sum / counter;
                SpaceAggregate spaceAggregate = new SpaceAggregate(mbr, DateTime.Now, valueSpaceAggregate);
                
                ostatni_agregat_czasowy = valueSpaceAggregate;
                liczba_elementów = counter;

                repository.saveSpaceAggregate(spaceAggregate);
            }

            return (sum, counter);
        }

        public override List<Urzadzenie> SearchBy(Rectangle rect)
        {
            List<Urzadzenie> result = new List<Urzadzenie>();

            foreach (RNode ch in children)
            {
                if (rect.Intersects(mbr) || mbr.Contains(rect))
                {
                    result.AddRange(ch.SearchBy(rect));
                }
            }
            return result;
        }

        public override Urzadzenie SearchBy(decimal x, decimal y)
        {
            if (mbr.Contains(x, y))
            {
                foreach (RNode ch in children)
                {
                    Urzadzenie wynikowe = ch.SearchBy(x, y);
                    if (wynikowe != null)
                        return wynikowe;
                }
            }
            return null;
        }
        //(liczba pomiarow, suma)
        public override (decimal, decimal) FindSpaceAggregate(Rectangle rect)
        {
            if (rect.Intersects(mbr) || mbr.Contains(rect))
            {
                if (rect == mbr || rect.Contains(mbr))
                {
                    if (ostatni_agregat_czasowy != null)
                        return ((decimal)liczba_elementów, (decimal)(liczba_elementów * ostatni_agregat_czasowy));
                    else 
                    {
                        double suma = SpaceAggregate(_repo).Item1;
                        int ctr = SpaceAggregate(_repo).Item2;
                        return (Convert.ToDecimal(ctr), Convert.ToDecimal(suma));
                    }
                }
                else
                {
                    decimal liczba = 0;
                    decimal suma = 0;

                    foreach (RNode ch in children)
                    {
                        liczba += ch.FindSpaceAggregate(rect).Item1;
                        suma += ch.FindSpaceAggregate(rect).Item2;
                    }

                    if (liczba != 0)
                        return (liczba, suma);
                    return (0m, 0m);
                }
            }
            else {
                return (0m, 0m);   
            }
        }

    }
}
