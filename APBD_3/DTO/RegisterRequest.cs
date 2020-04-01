using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.DTO
{
    public class RegisterRequest
    {
        [Required]
        public int idStudent { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        public DateTime birthDate { get; set; }
        [Required]
        public string nameOfStudies { get; set; }
    }
}
