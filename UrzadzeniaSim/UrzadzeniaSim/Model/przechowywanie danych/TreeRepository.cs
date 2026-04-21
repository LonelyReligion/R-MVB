using UrzadzeniaSim.Model.RMVB.R;

namespace UrzadzeniaSim.Model.DB
{
    public interface TreeRepository
    {
        void saveDevice(Urzadzenie_Model device);
        void saveVersion(Wersja version);

        void saveMeasurement(Pomiar measure);

        void saveTimeAggregate(TimeAggregate timeAggregate);

        void saveSpaceAggregate(SpaceAggregate spaceAggregate);

        void saveSrednia(Srednia srednia);
    }
}