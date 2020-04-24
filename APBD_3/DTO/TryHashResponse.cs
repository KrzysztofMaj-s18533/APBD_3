using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.DTO
{
    public class TryHashResponse
    {
        public int idStudent { get; set; }
        public string hash { get; set; }
        public string salt { get; set; }
    }
}
