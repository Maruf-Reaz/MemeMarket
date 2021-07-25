using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models.ViewModels
{
    public class ClientCampaign
    {
        public Campaign Campaign { get; set; }
        public List<Affiliate> Affiliates { get; set; }
    }
}
