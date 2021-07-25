using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models
{
    public class Affiliate
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string  WANumber { get; set; }
        public string InstaLink { get; set; }
        public string TotalFollwers { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }

        public double PaymentForPost { get; set; }
        public double PaymentForStory { get; set; }
        public double PaymentForPostPlusStory { get; set; }

        [NotMapped]
        public int FromHome { get; set; }


        public int? AffiliateLanguageId { get; set; }
        public AffiliateLanguage AffiliateLanguage { get; set; }

        public int? CatagoryId { get; set; }
        public Catagory Catagory { get; set; }

        public DateTime AddintionDate { get; set; }
        public List<Post> Posts { get; set; }

    }
}
