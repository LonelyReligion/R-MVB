using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RMVB_konsola.R
{
    public class RBranch : RNode
    {
        private List<RNode> children = new List<RNode>();

        public override int zwrocLiczbeDzieci() { 
            return children.Count;
        }
        public RBranch(decimal xmin, decimal ymin, decimal xmax, decimal ymax) : base(xmin, ymin, xmax, ymax)
        {

        }

        public RBranch(Rectangle r) : base(r)
        {

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
            return new RBranch(0, 0, 0, 0);
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

        public override (double, int) SpaceAggregate(TreeRepository repository)
        {
            double sum = 0;
            int counter = 0;
            Parallel.ForEach(children, ch =>
            {
                (double, int) res = ch.SpaceAggregate(repository);
                if (res.Item2 > 0)
                {
                    sum += res.Item1;
                    counter += res.Item2;
                }
            });

            if (counter > 0)
            {
                Decimal valueSpaceAggregate = (Decimal)sum / counter;
                SpaceAggregate spaceAggregate = new SpaceAggregate(mbr, DateTime.Now, valueSpaceAggregate);
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
                    if ( wynikowe != null)
                        return wynikowe;
                }
            }
            return null;
        }
    }
}
