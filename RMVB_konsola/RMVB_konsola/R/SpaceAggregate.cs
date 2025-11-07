using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    public class SpaceAggregate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int spaceAggregateId { get; set; }

        public decimal xMin { get; set; }
        public decimal yMin { get; set; }
        public decimal xMax { get; set; }
        public decimal yMax { get; set; }

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
