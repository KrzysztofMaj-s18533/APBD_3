using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.DTO
{
    public class PromotionResponse
    {
        public int idEnrollment { get; set; }
        public int semester { get; set; }
        public int idStudy { get; set; }
        public DateTime startDate { get; set; }
    }
}
