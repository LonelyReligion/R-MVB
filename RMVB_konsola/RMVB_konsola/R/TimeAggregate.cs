using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.R
{
    public class TimeAggregate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimeAggregateID { get; set; }

        public Decimal tAValue { get; set; }
        public DateTime tADateTime { get; set; }

        public int DeviceId { get; set; }

        public TimeAggregate(Decimal v, DateTime dt, int devId)
        {
            tAValue = v;
            tADateTime = dt;
            DeviceId = devId;
        }
    }
}
