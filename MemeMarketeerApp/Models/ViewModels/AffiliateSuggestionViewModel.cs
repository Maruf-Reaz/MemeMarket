using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models.ViewModels
{
    public class AffiliateSuggestionViewModel
    {
        public Affiliate Affiliate { get; set; }
        public double AverageImpressionPost { get; set; }
        public double AverageImpressionStory { get; set; }
        public double AverageImpressionPostPlusStory { get; set; }

        public double RatePerThousandImpressionPost { get; set; }
        public double RatePerThousandImpressionStory { get; set; }
        public double RatePerThousandImpressionPostPlusStory { get; set; }

        public int? SuggestedPostType { get; set; }
    }
}
