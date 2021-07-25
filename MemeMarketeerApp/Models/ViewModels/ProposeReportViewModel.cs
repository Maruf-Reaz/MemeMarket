using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models.ViewModels
{
    public class ProposeReportViewModel
    {
        public int ClientId { get; set; }

        public double Budget { get; set; }

        public double ProfitMargin { get; set; }
        
        public bool IsPost { get; set; }
        public bool IsStory { get; set; }
        public bool IsPostPlusStory { get; set; }
        public bool TakeLast30Days { get; set; }


        public double TargetImpression { get; set; }

    }
}
