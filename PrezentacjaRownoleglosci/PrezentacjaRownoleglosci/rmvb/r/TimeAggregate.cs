using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulacja_strumieni.rmvb.r
{
    public class TimeAggregate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimeAggregateID { get; set; }

        public decimal tAValue { get; set; }
        public DateTime tADateTime { get; set; }

        public int DeviceId { get; set; }

        public TimeAggregate(decimal v, DateTime dt, int devId)
        {
            tAValue = v;
            tADateTime = dt;
            DeviceId = devId;
        }
    }
}
