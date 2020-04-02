using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.DTO
{
    public class PromotionRequest
    {
        [Required]
        public string studies { get; set; }
        [Required]
        public int semester { get; set; }
    }
}
