using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.R
{
    public interface TreeRepository
    {
        void saveDevice(Urzadzenie device);
        void saveVersion(Wersja version);

        void saveMeasurement(Pomiar measure);

        void saveTimeAggregate(TimeAggregate timeAggregate);

        void saveSpaceAggregate(SpaceAggregate spaceAggregate);

        void saveSrednia(Srednia srednia);
    }
}