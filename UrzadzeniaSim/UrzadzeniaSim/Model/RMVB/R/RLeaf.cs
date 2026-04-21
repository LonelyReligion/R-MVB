using UrzadzeniaSim.Model.DB;

namespace UrzadzeniaSim.Model.RMVB.R
{
    internal class RLeaf : RNode
    {

        private List<Urzadzenie_Model> devices = new List<Urzadzenie_Model>();

        public override int zwrocLiczbeDzieci() {
            return 0;
        }
        public RLeaf(Decimal xmin, Decimal ymin, Decimal xmax, Decimal ymax) : base(xmin, ymin, xmax, ymax)
        {
        }

        public RLeaf(Rectangle r) : base(r)
        {

        }

        public override int EntriesCount()
        {
            return devices.Count;
        }

        public override RNode Clone()
        {
            return new RLeaf(0, 0, 0, 0);
        }

        public void Add(Urzadzenie_Model dev)
        {
            devices.Add(dev);
        }


        public override RNode MoveEntry(RNode destination, int index)
        {
            RLeaf dest = (RLeaf)destination;
            dest.devices.Add(devices[index]);
            return this;
        }

        public override RNode RemoveEntry(int index)
        {
            devices.RemoveAt(index);
            return this;
        }

        public override Decimal Distance(int i, int j)
        {
            Decimal xDistance = Math.Max(devices[i].Dlugosc, devices[j].Dlugosc);
            Decimal yDistance = Math.Max(devices[i].Szerokosc, devices[j].Szerokosc); 
            return xDistance + yDistance;
        }

        public override void Insert(Urzadzenie_Model dev, TreeWalker adaptor)
        {
            devices.Add(dev);
            if (devices.Count > MAX_ITEM_COUNT)
            {
                adaptor.Split(this);

            }
        }

        public override void UpdateMBR()
        {
            mbr.XMin = Decimal.MaxValue;
            mbr.YMin = Decimal.MaxValue;
            mbr.XMax = Decimal.MinValue;
            mbr.YMax = Decimal.MinValue;

            foreach (Urzadzenie_Model dev in devices)
            {
                mbr.XMin = Math.Min(mbr.XMin, dev.Dlugosc);
                mbr.YMin = Math.Min(mbr.YMin, dev.Szerokosc);
                mbr.XMax = Math.Max(mbr.XMax, dev.Dlugosc);
                mbr.YMax = Math.Max(mbr.YMax, dev.Szerokosc);
            }

        }

        public override void SplitEntries(RNode pNode, RNode kNode)
        {
            Decimal maxDiff = Decimal.MinValue;
            Urzadzenie_Model devToAdd = null;
            RLeaf destination = (RLeaf)pNode;

            foreach (Urzadzenie_Model dev in devices)
            {
                Decimal distP = pNode.mbr.EnlargedBy(dev.Szerokosc, dev.Dlugosc).Area() - pNode.mbr.Area();
                Decimal distK = kNode.mbr.EnlargedBy(dev.Szerokosc, dev.Dlugosc).Area() - kNode.mbr.Area();
                Decimal diff = Math.Abs(distP - distK);

                if (diff > maxDiff)
                {
                    maxDiff = diff;
                    devToAdd = dev;
                    if (distP > distK)
                    {
                        destination = (RLeaf)kNode;
                    }
                    else
                    {
                        destination = (RLeaf)pNode;
                    }
                }
            }
            destination.Add(devToAdd);
            devices.Remove(devToAdd);
        }

        public override (double, int) SpaceAggregate(TreeRepository repository)
        {
            double sum = 0;
            int counter = 0;
            foreach (Urzadzenie_Model dev in devices)
            {
                if (dev.IsMeasurementValid())
                {
                    sum += (double)dev.LastMeasurement().Wartosc;
                    counter++;
                }
            }
            if (counter > 0)
            {
                Decimal valueSpaceAggregate = (Decimal)sum / counter;
                spaceAggregate = new SpaceAggregate(mbr, DateTime.Now, valueSpaceAggregate);
                repository.saveSpaceAggregate(spaceAggregate);
            }
            return (sum, counter);
        }

        public override List<Urzadzenie_Model> SearchBy(Rectangle rect)
        {
            List<Urzadzenie_Model> result = new List<Urzadzenie_Model>();

            foreach (Urzadzenie_Model dev in devices)
            {
                if (rect.Contains(dev.Dlugosc, dev.Szerokosc))
                {
                    result.Add(dev);
                }
            }
            return result;
        }

        public override Urzadzenie_Model SearchBy(Decimal x, Decimal y)
        {
            Urzadzenie_Model result = null;
            foreach (Urzadzenie_Model dev in devices)
            {
                if (Equals(x, dev.Dlugosc) && Equals(y, dev.Szerokosc))
                {
                    result = dev;
                }
            }
            return result;
        }
    }
}
