using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrzadzeniaSim.Model.DB
{
    public interface TreeRepository
    {
        void saveDevice(Urzadzenie_Model device);
        void saveVersion(Wersja version);

/*        void saveMeasurement(Pomiar measure);

        void saveTimeAggregate(TimeAggregate timeAggregate);

        void saveSpaceAggregate(SpaceAggregate spaceAggregate);

        void saveSrednia(Srednia srednia);*/
    }
}