using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulacja_strumieni.rmvb.r
{
    public interface RootHandler
    {
        RNode ProvideRoot();

        void UpdateRoot(RNode root);
    }
}
