using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.R
{
    public class Rectangle
    {
        public decimal XMin;

        public decimal YMin;

        public decimal XMax;

        public decimal YMax;

        public Rectangle(decimal yMin, decimal xMin, decimal yMax, decimal xMax)
        {
            if ((xMin > xMax) || (yMin > yMax)) {
                throw new ArgumentException("\"min\" coordinates must be less than the \"max\" ones");
            }
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
        }

        public bool Contains(Rectangle other)
        {
            return (XMin <= other.XMin) && (YMin <= other.YMin) && (XMax >= other.XMax) && (YMax >= other.YMax);
        }

        public bool Contains(Decimal x, Decimal y)
        {
            return (XMin <= x) && (YMin <= y) && (XMax >= x) && (YMax >= y);
        }

        public bool Intersects(Rectangle other)
        {
            return (XMin < other.XMax) && (XMax > other.XMin) && (YMin < other.YMax) && (YMax > other.YMin);
        }


        public decimal Distance(Rectangle r)
        {
            decimal xDistance = Math.Max(0, Math.Max(XMin, r.XMin) - Math.Min(XMax, r.XMax));
            decimal yDistance = Math.Max(0, Math.Max(YMin, r.YMin) - Math.Min(YMax, r.YMax));
            return xDistance + yDistance;
        }

        public Rectangle EnlargedBy(Rectangle other)
        {
            return new Rectangle(
                Math.Min(XMin, other.XMin),
                Math.Min(YMin, other.YMin),
                Math.Max(XMax, other.XMax),
                Math.Max(YMax, other.YMax)
            );
        }

        public Rectangle EnlargedBy(Decimal x, Decimal y)
        {
            return new Rectangle(
                Math.Min(XMin, x),
                Math.Min(YMin, y),
                Math.Max(XMax, x),
                Math.Max(YMax, y)
            );
        }

        public void Enlarge (Rectangle other) {
            XMin = Math.Min(XMin, other.XMin);
            YMin = Math.Min(YMin, other.YMin);
            XMax = Math.Max(XMax, other.XMax);
            YMax = Math.Max(YMax, other.YMax);
        }

        public void Enlarge(Decimal x, Decimal y)
        {
            XMin = Math.Min(XMin, x);
            YMin = Math.Min(YMin, y);
            XMax = Math.Max(XMax, x);
            YMax = Math.Max(YMax, y);
        }

        public decimal Area()
        {
            return (XMax - XMin) * (YMax - YMin);
        }


    }
}
