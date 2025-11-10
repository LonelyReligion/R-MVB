using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.R
{
    public interface RootHandler
    {
        RNode ProvideRoot ();

        void UpdateRoot (RNode root);
    }
}
