using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.R
{
    public class SpaceAggregate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int spaceAggregateId { get; set; }

        public Decimal xMin { get; set; }
        public Decimal yMin { get; set; }
        public Decimal xMax { get; set; }
        public Decimal yMax { get; set; }

        private Rectangle mbr {  get; set; }
        public DateTime sATime { get; set; }
        public Decimal sAValue { get; set; }

        public SpaceAggregate(Rectangle r, DateTime dt, Decimal v)
        {
            mbr = r;
            xMin = r.XMin; 
            yMin = r.YMin; 
            xMax = r.XMax; 
            yMax = r.YMax;
            sATime = dt;
            sAValue = v;
        }

    }
}
