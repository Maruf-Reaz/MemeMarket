using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WANumber { get; set; }
        public string Link { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        [NotMapped]
        public int FromHome { get; set; }

        public DateTime AddintionDate { get; set; }

    }
}
