using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace RMVB_konsola.MVB
{

    abstract public class RNode
    {

        public static int MAX_ITEM_COUNT = 4;

        public Rectangle mbr { get; }

        protected SpaceAggregate spaceAggregate;

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

        public abstract (double, int) SpaceAggregate(TreeRepository repository);

        public abstract void SplitEntries(RNode pNode, RNode kNode);

        abstract public List<Urzadzenie> SearchBy(Rectangle rect);

        abstract public Urzadzenie SearchBy(decimal x, decimal y);
    }
}
