using System.Collections.Generic;
using Symulacja_strumieni.model;

namespace Symulacja_strumieni.rmvb.r
{

    abstract public class RNode
    {

        public static int MAX_ITEM_COUNT = 4;

        public Rectangle mbr { get; }

        protected SpaceAggregate spaceAggregate;

        //zwraca liczbe dzieci albo 0 jezeli to lisc
        public abstract int zwrocLiczbeDzieci();
        public RNode(decimal xmin, decimal ymin, decimal xmax, decimal ymax)
        {
            mbr = new Rectangle(xmin, ymin, xmax, ymax);
        }

        public RNode(Rectangle r)
        {
            mbr = r;
        }

        public abstract void Insert(Urzadzenie dev, TreeWalker adaptor);

        public virtual RBranch FindParent(RNode node)
        {
            return null;
        }

        public abstract void UpdateMBR();

        public abstract int EntriesCount();

        public abstract decimal Distance(int i, int j);

        public abstract RNode Clone();

        public abstract RNode MoveEntry(RNode destination, int index);

        public abstract RNode RemoveEntry(int index);

        public abstract (double, int) SpaceAggregate(Repo repository);

        public abstract void SplitEntries(RNode pNode, RNode kNode);

        abstract public List<Urzadzenie> SearchBy(Rectangle rect);

        abstract public Urzadzenie SearchBy(decimal x, decimal y);

        abstract public (List<int>, decimal, decimal) FindSpaceAggregate(Rectangle rect);
    }
}
