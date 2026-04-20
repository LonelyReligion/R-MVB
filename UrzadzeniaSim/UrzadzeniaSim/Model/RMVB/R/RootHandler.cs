using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrzadzeniaSim.Model.RMVB.R
{
    public interface RootHandler
    {
        RNode ProvideRoot ();

        void UpdateRoot (RNode root);
    }
}
