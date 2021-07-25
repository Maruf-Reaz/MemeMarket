using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public int? CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public double Amount { get; set; }

        public DateTime ExpenceDate { get; set; }
    }
}
