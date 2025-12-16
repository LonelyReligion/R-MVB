using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RMVB_konsola.R
{
    public class TreeWalker   {

        private RootHandler rootHandler;

        public TreeWalker (RootHandler rh)
        {
            rootHandler = rh;
        }

        public void Split(RNode node)   {
            RNode root = rootHandler.ProvideRoot();

            (RNode, RNode) newNodes = QuadraticReproduce(node);

            if (node == root)  {
                RBranch newRoot = new RBranch(RTree.ROOTMBR);
                newRoot.Add(newNodes.Item1);
                newRoot.Add(newNodes.Item2);
                rootHandler.UpdateRoot(newRoot);
            }
            else {
                RBranch parent = root.FindParent(node);
                parent.Add(newNodes.Item1);
                parent.Add(newNodes.Item2);
                parent.RemoveEntry(node);

                if (parent.EntriesCount() > RNode.MAX_ITEM_COUNT)  {
                    Split(parent);
                }
            }
        }

        protected (RNode, RNode) QuadraticReproduce (RNode node)
        {
            Decimal maxD = Decimal.MinValue;
            int p0 = 0;
            int k0 = 0;
            int count = node.EntriesCount();
            for (int p = 0; p < count - 1; p++)   {
                for (int k = p + 1; k < count; k++) {
                    Decimal d = node.Distance(p, k);
                    if (d > maxD)  {
                        maxD = d;
                        p0 = p;
                        k0 = k;
                    }
                }
            }

            RNode pNode = node.Clone();
            RNode kNode = node.Clone();
            node.MoveEntry(pNode, p0).MoveEntry(kNode, k0);
            pNode.UpdateMBR();
            kNode.UpdateMBR();
            if (p0 < k0)  {
                node.RemoveEntry(k0).RemoveEntry(p0);
            }
            else  {
                node.RemoveEntry(p0).RemoveEntry(k0);
            }

            while (node.EntriesCount() > 0)  {

                node.SplitEntries(pNode, kNode);

            }
            pNode.UpdateMBR();
            kNode.UpdateMBR();

            return (pNode, kNode);

        }
    }
}
