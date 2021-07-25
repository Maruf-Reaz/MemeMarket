using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models.ViewModels
{
    public class CampaignHomeViewModel
    {
       
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public double? TotalImpressionGenerated { get; set; }
        public double? TotalExpence { get; set; }


    }
}
