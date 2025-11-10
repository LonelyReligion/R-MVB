using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.R
{
    internal class RLeaf : RNode
    {

        private List<Urzadzenie> devices = new List<Urzadzenie>();


        public RLeaf(decimal xmin, decimal ymin, decimal xmax, decimal ymax) : base(xmin, ymin, xmax, ymax)
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

        public void Add(Urzadzenie dev)
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

        public override decimal Distance(int i, int j)
        {
            decimal xDistance = Math.Max(devices[i].Szerokosc, devices[j].Szerokosc);
            decimal yDistance = Math.Max(devices[i].Dlugosc, devices[j].Dlugosc);
            return xDistance + yDistance;
        }

        public override void Insert(Urzadzenie dev, TreeWalker adaptor)
        {
            devices.Add(dev);
            if (devices.Count > MAX_ITEM_COUNT)
            {
                adaptor.Split(this);

            }
        }

        public override void UpdateMBR()
        {
            mbr.XMin = decimal.MaxValue;
            mbr.YMin = decimal.MaxValue;
            mbr.XMax = decimal.MinValue;
            mbr.YMax = decimal.MinValue;

            foreach (Urzadzenie dev in devices)
            {
                mbr.XMin = Math.Min(mbr.XMin, dev.Szerokosc);
                mbr.YMin = Math.Min(mbr.YMin, dev.Dlugosc);
                mbr.XMax = Math.Max(mbr.XMax, dev.Szerokosc);
                mbr.YMax = Math.Max(mbr.YMax, dev.Dlugosc);
            }

        }

        public override void SplitEntries(RNode pNode, RNode kNode)
        {
            decimal maxDiff = decimal.MinValue;
            Urzadzenie devToAdd = null;
            RLeaf destination = (RLeaf)pNode;

            foreach (Urzadzenie dev in devices)
            {
                decimal distP = pNode.mbr.EnlargedBy(dev.Szerokosc, dev.Dlugosc).Area() - pNode.mbr.Area();
                decimal distK = kNode.mbr.EnlargedBy(dev.Szerokosc, dev.Dlugosc).Area() - kNode.mbr.Area();
                decimal diff = Math.Abs(distP - distK);

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
            foreach (Urzadzenie dev in devices)
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

        public override List<Urzadzenie> SearchBy(Rectangle rect)
        {
            List<Urzadzenie> result = new List<Urzadzenie>();

            foreach (Urzadzenie dev in devices)
            {
                if (rect.Contains(dev.Szerokosc, dev.Dlugosc))
                {
                    result.Add(dev);
                }
            }
            return result;
        }

        public override Urzadzenie SearchBy(decimal x, decimal y)
        {
            Urzadzenie result = null;
            foreach (Urzadzenie dev in devices)
            {
                if (Equals(x, dev.Szerokosc) && Equals(y, dev.Dlugosc))
                {
                    result = dev;
                }
            }
            return result;
        }

    }
}
