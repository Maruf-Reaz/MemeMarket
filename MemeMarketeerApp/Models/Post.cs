using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models
{
    public class Post
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int PostTypeId { get; set; }
        public PostType PostType { get; set; }


        public int AffiliateId { get; set; }
        public Affiliate Affiliate { get; set; }

        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }


        public double PostReached { get; set; }
        public double StoryViewed { get; set; }
        public double ImpressionGenerated { get; set; }

        public string ReactionGenerated { get; set; }
        public string CommentsGenerated { get; set; }

        public double PaidAmount { get; set; }
    }
}
