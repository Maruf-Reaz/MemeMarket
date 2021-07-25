using MemeMarketeerApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MemeMarketeerApp.Models
{
   


    public class Campaign
    {
        

        public int Id { get; set; }
        public string Title { get; set; }
        public double TargetImpression { get; set; }
        public double TotalBudget { get; set; }
        public double? TotalLinkClicked { get; set; }
        public double? TotalFollowerIncreament { get; set; }

        public string BitlyLink { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime FinishingDate { get; set; }
        public string Remarks { get; set; }
        [NotMapped]
        public int FromHome { get; set; }

        public List<Post> Posts { get; set; }

        public int Status { get; set; }

        public static double GetExpenseForCampaign(int campaignId, ApplicationDbContext _context)
        {

            var posts = _context.Posts.
                Where(m => m.CampaignId == campaignId).
                ToList();
            
            double expenseTotal = _context.Posts.Where(m => m.CampaignId == campaignId).Sum(m => m.PaidAmount);
            expenseTotal += _context.Expenses.Where(m => m.CampaignId == campaignId).Sum(m => m.Amount);


            return expenseTotal;
        }
        public static double GetImpressionsGenerated(int campaignId, ApplicationDbContext _context)
        {

            var posts = _context.Posts.
                Where(m => m.CampaignId == campaignId).
                ToList();
            
            double impressionTotal = _context.Posts.Where(m => m.CampaignId == campaignId).Sum(m => m.ImpressionGenerated);


            return impressionTotal;
        }

    }






}
