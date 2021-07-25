using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemeMarketeerApp.Data;
using MemeMarketeerApp.Models;
using MemeMarketeerApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MemeMarketeerApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AddCriteria(int clientId, bool isPost, bool isStory, bool isPostPlusStory, bool takeLast30Days, double budget, double targetImpression, double profitmargin, int isPostBack)
        {
            int postTypeId = 0;
            if (isPostBack == 1)
            {
                double costToMeetProfitMargin = budget - ((budget * profitmargin) / 100);
                double ratePerTHousandImpression = (costToMeetProfitMargin / targetImpression) * 1000;
                var usedAffiliates = await _context.Posts
                    .Where(m => m.Campaign.ClientId == clientId && m.Campaign.CreationDate >= DateTime.Now.AddDays(-30) && m.Affiliate.Status == 1)
                    .Select(o => o.Affiliate).Distinct()
                    .Include(m => m.Posts)
                    .ToListAsync();

                var allActiveAffiliates = await _context.Affiliates.Where(m => m.Status == 1).ToListAsync();
                if (!takeLast30Days)
                {
                    allActiveAffiliates = allActiveAffiliates.Except(usedAffiliates).ToList();
                }


                List<AffiliateSuggestionViewModel> allAffiliatesWithDetails = new List<AffiliateSuggestionViewModel>();
                List<AffiliateSuggestionViewModel> campaignAffiliateListOne = new List<AffiliateSuggestionViewModel>();
                List<AffiliateSuggestionViewModel> campaignAffiliateListTwo = new List<AffiliateSuggestionViewModel>();
                bool hasPrev = false;
                allAffiliatesWithDetails = GetDetailed(allActiveAffiliates);

                allAffiliatesWithDetails = GetTHeList(allAffiliatesWithDetails, isPost, isStory, isPostPlusStory);
                double gainedImpressionListOne = 0;
                double costHappenedListOne = 0;
                foreach (var affiliate in allAffiliatesWithDetails)
                {
                    if (targetImpression > gainedImpressionListOne && costToMeetProfitMargin > costHappenedListOne)
                    {
                        if (affiliate.SuggestedPostType == 1)
                        {
                            gainedImpressionListOne += affiliate.AverageImpressionPost;
                            costHappenedListOne += affiliate.Affiliate.PaymentForPost;
                        }
                        else if (affiliate.SuggestedPostType == 2)
                        {
                            gainedImpressionListOne += affiliate.AverageImpressionStory;
                            costHappenedListOne += affiliate.Affiliate.PaymentForStory;
                        }
                        else if (affiliate.SuggestedPostType == 3)
                        {
                            gainedImpressionListOne += affiliate.AverageImpressionPostPlusStory;
                            costHappenedListOne += affiliate.Affiliate.PaymentForPostPlusStory;
                        }

                        AffiliateSuggestionViewModel tempAff = new AffiliateSuggestionViewModel
                        {
                            Affiliate = affiliate.Affiliate,
                            AverageImpressionPost = affiliate.AverageImpressionPost,
                            AverageImpressionStory = affiliate.AverageImpressionStory,
                            AverageImpressionPostPlusStory = affiliate.AverageImpressionPostPlusStory,
                            RatePerThousandImpressionPost = affiliate.RatePerThousandImpressionPost,
                            RatePerThousandImpressionStory = affiliate.RatePerThousandImpressionStory,
                            RatePerThousandImpressionPostPlusStory = affiliate.RatePerThousandImpressionPostPlusStory,
                            SuggestedPostType = affiliate.SuggestedPostType
                        };


                        campaignAffiliateListOne.Add(tempAff);
                    }
                    else
                    {
                        break;
                    }

                }
                double gainedImpressionListTwo = 0;
                double targetImoressionToAchieve = targetImpression;
                double costHappenedListTwo = 0;
                for (int i = 0; i < 100; i++)
                {
                    if (targetImpression > gainedImpressionListTwo && costToMeetProfitMargin > costHappenedListTwo)
                    {

                        if (allAffiliatesWithDetails.Count != 0)
                        {
                            var closestAffiliatePost = allAffiliatesWithDetails.OrderBy(m => targetImoressionToAchieve - m.AverageImpressionPost).Where(m => m.AverageImpressionPost != 0 && (targetImoressionToAchieve - m.AverageImpressionPost > 0)).FirstOrDefault();

                            if (closestAffiliatePost == null)
                            {
                                closestAffiliatePost = allAffiliatesWithDetails.OrderBy(m => targetImoressionToAchieve - m.AverageImpressionPost).Where(m => m.AverageImpressionPost != 0).FirstOrDefault();
                            }



                            var closestAffiliateStory = allAffiliatesWithDetails.OrderBy(m => targetImoressionToAchieve - m.AverageImpressionStory).Where(m => m.AverageImpressionStory != 0 && (targetImoressionToAchieve - m.AverageImpressionStory > 0)).FirstOrDefault();
                            if (closestAffiliateStory == null)
                            {
                                closestAffiliateStory = allAffiliatesWithDetails.OrderBy(m => targetImoressionToAchieve - m.AverageImpressionStory).Where(m => m.AverageImpressionStory != 0).FirstOrDefault();
                            }


                            var closestAffiliatePostPlusStory = allAffiliatesWithDetails.OrderBy(m => targetImoressionToAchieve - m.AverageImpressionPostPlusStory).Where(m => m.AverageImpressionPostPlusStory != 0 && (targetImoressionToAchieve - m.AverageImpressionPostPlusStory > 0)).FirstOrDefault();

                            if (closestAffiliatePostPlusStory == null)
                            {
                                closestAffiliatePostPlusStory = allAffiliatesWithDetails.OrderBy(m => targetImoressionToAchieve - m.AverageImpressionPostPlusStory).Where(m => m.AverageImpressionPostPlusStory != 0).FirstOrDefault();
                            }


                            List<TempVM> temps = new List<TempVM>();


                            if (closestAffiliatePost != null)
                            {
                                if (closestAffiliatePost.AverageImpressionPost != 0 && isPost)
                                {
                                    TempVM tempPost = new TempVM
                                    {
                                        Type = 1,
                                        Value = targetImoressionToAchieve - closestAffiliatePost.AverageImpressionPost
                                    };
                                    temps.Add(tempPost);

                                }
                            }
                            if (closestAffiliateStory != null)
                            {
                                if (closestAffiliateStory.AverageImpressionStory != 0 && isStory)
                                {
                                    TempVM tempStory = new TempVM
                                    {
                                        Type = 2,
                                        Value = targetImoressionToAchieve - closestAffiliateStory.AverageImpressionStory
                                    };
                                    temps.Add(tempStory);


                                }
                            }
                            if (closestAffiliatePostPlusStory != null)
                            {
                                if (closestAffiliatePostPlusStory.AverageImpressionPostPlusStory != 0 && isPostPlusStory)
                                {
                                    TempVM tempPostPlusStory = new TempVM
                                    {
                                        Type = 3,
                                        Value = targetImoressionToAchieve - closestAffiliatePostPlusStory.AverageImpressionPostPlusStory
                                    };
                                    temps.Add(tempPostPlusStory);

                                }

                            }

                            var finalTemp = temps.OrderByDescending(m => m.Value).Where(m => m.Value != 0).FirstOrDefault();
                            if (finalTemp.Value < 0)
                            {
                                finalTemp = temps.OrderBy(m => m.Value).Where(m => m.Value > 0).FirstOrDefault();
                            }
                            if (finalTemp == null)
                            {
                                finalTemp = temps.OrderByDescending(m => m.Value).Where(m => m.Value != 0).FirstOrDefault();
                            }


                            if (finalTemp != null)
                            {
                                if (finalTemp.Type == 1)
                                {

                                    closestAffiliatePost.SuggestedPostType = 1;
                                    campaignAffiliateListTwo.Add(closestAffiliatePost);
                                    gainedImpressionListTwo += closestAffiliatePost.AverageImpressionPost;
                                    costHappenedListTwo += closestAffiliatePost.Affiliate.PaymentForPost;
                                    targetImoressionToAchieve = targetImpression - gainedImpressionListTwo;
                                    allAffiliatesWithDetails.Remove(closestAffiliatePost);
                                }
                                else if (finalTemp.Type == 2)
                                {
                                    closestAffiliateStory.SuggestedPostType = 2;
                                    campaignAffiliateListTwo.Add(closestAffiliateStory);
                                    gainedImpressionListTwo += closestAffiliateStory.AverageImpressionStory;
                                    costHappenedListTwo += closestAffiliateStory.Affiliate.PaymentForStory;
                                    targetImoressionToAchieve = targetImpression - gainedImpressionListTwo;
                                    allAffiliatesWithDetails.Remove(closestAffiliateStory);
                                }
                                else if (finalTemp.Type == 3)
                                {
                                    closestAffiliatePostPlusStory.SuggestedPostType = 3;
                                    campaignAffiliateListTwo.Add(closestAffiliatePostPlusStory);
                                    gainedImpressionListTwo += closestAffiliatePostPlusStory.AverageImpressionPostPlusStory;
                                    costHappenedListTwo += closestAffiliatePostPlusStory.Affiliate.PaymentForPostPlusStory;
                                    targetImoressionToAchieve = targetImpression - gainedImpressionListTwo;
                                    allAffiliatesWithDetails.Remove(closestAffiliatePostPlusStory);
                                }

                            }
                            else
                            {
                                continue;
                            }


                        }
                        else
                        {
                            break;
                        }



                    }
                    else
                    {
                        break;
                    }

                }


                if (campaignAffiliateListTwo.Count != 0)
                {
                    double extra = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (gainedImpressionListTwo > targetImpression)
                        {
                            extra = gainedImpressionListTwo - targetImpression;
                            foreach (var affiliateItem in campaignAffiliateListTwo)
                            {
                                if (affiliateItem.SuggestedPostType == 1)
                                {

                                    if (affiliateItem.AverageImpressionPost < extra)
                                    {
                                        gainedImpressionListTwo -= affiliateItem.AverageImpressionPost;
                                        campaignAffiliateListTwo.Remove(affiliateItem);
                                        break;
                                    }
                                }
                                if (affiliateItem.SuggestedPostType == 2)
                                {

                                    if (affiliateItem.AverageImpressionStory < extra)
                                    {
                                        gainedImpressionListTwo -= affiliateItem.AverageImpressionStory;
                                        campaignAffiliateListTwo.Remove(affiliateItem);
                                        break;
                                    }
                                }

                                if (affiliateItem.AverageImpressionPostPlusStory == 3)
                                {

                                    if (affiliateItem.AverageImpressionPostPlusStory < extra)
                                    {
                                        gainedImpressionListTwo -= affiliateItem.AverageImpressionPostPlusStory;
                                        campaignAffiliateListTwo.Remove(affiliateItem);
                                        break;
                                    }
                                }
                                
                            }



                        }
                    }

                }

                if (campaignAffiliateListOne.Count == 0)
                {
                    ViewData["ListOneNotFound"] = true;
                }
                else
                {
                    ViewData["ListOneNotFound"] = false;
                }
                if (campaignAffiliateListTwo.Count == 0)
                {
                    ViewData["ListTwoNotFound"] = true;
                }
                else
                {
                    ViewData["ListTwoNotFound"] = false;

                }
                ViewData["ListOne"] = campaignAffiliateListOne;
                ViewData["ListTwo"] = campaignAffiliateListTwo;
                ViewData["CostToGetPM"] = costToMeetProfitMargin;


                ViewData["ClientId"] = new SelectList(_context.Clients.Where(m => m.Status == 1), "Id", "Name");
                ViewData["PostTypeId"] = new SelectList(_context.PostTypes, "Id", "Name");
                ViewData["IsPostBack"] = isPostBack;
                ViewData["TotalBughet"] = budget;

                return View();
            }
            else
            {
                ViewData["ClientId"] = new SelectList(_context.Clients.Where(m => m.Status == 1), "Id", "Name");
                ViewData["PostTypeId"] = new SelectList(_context.PostTypes, "Id", "Name");
                return View();
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCriteria(ProposeReportViewModel proposeReportViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("AddCriteria", new { clientId = proposeReportViewModel.ClientId, isPost = proposeReportViewModel.IsPost, isStory = proposeReportViewModel.IsStory, isPostPlusStory = proposeReportViewModel.IsPostPlusStory, takeLast30Days = proposeReportViewModel.TakeLast30Days, budget = proposeReportViewModel.Budget, targetImpression = proposeReportViewModel.TargetImpression, profitmargin = proposeReportViewModel.ProfitMargin, isPostBack = 1 });
            }
            else
            {
                ViewData["ClientId"] = new SelectList(_context.Clients.Where(m => m.Status == 1), "Id", "Name", proposeReportViewModel.ClientId);

                return View(proposeReportViewModel);
            }
        }

        private List<AffiliateSuggestionViewModel> GetDetailed(List<Affiliate> affiliates)
        {
            List<AffiliateSuggestionViewModel> uViewModels = new List<AffiliateSuggestionViewModel>();

            foreach (var uAffiliate in affiliates)
            {
                AffiliateSuggestionViewModel uViewModel = new AffiliateSuggestionViewModel();
                uViewModel.Affiliate = uAffiliate;

                double totalPostReach = _context.Posts.Where(m => m.AffiliateId == uAffiliate.Id && m.Campaign.Status == 1 && m.PostReached != 0).Sum(m => m.ImpressionGenerated);

                double totalPost = _context.Posts.Where(m => m.AffiliateId == uAffiliate.Id && m.Campaign.Status == 1 && m.PostReached != 0).Count();


                double totalStoryView = _context.Posts.Where(m => m.AffiliateId == uAffiliate.Id && m.Campaign.Status == 1 && m.StoryViewed != 0).Sum(m => m.StoryViewed);

                double totalStory = _context.Posts.Where(m => m.AffiliateId == uAffiliate.Id && m.Campaign.Status == 1 && m.StoryViewed != 0).Count();

                double totalPostPlusStoryImpression = _context.Posts.Where(m => m.AffiliateId == uAffiliate.Id && m.PostTypeId == 3 && m.Campaign.Status == 1 && m.ImpressionGenerated != 0).Sum(m => m.ImpressionGenerated);

                double totalPostPlusStory = _context.Posts.Where(m => m.AffiliateId == uAffiliate.Id && m.PostTypeId == 3 && m.Campaign.Status == 1 && m.ImpressionGenerated != 0).Count();


                double averagePostReach = 0;
                double averageStoryView = 0;
                double averagePostPlusStoryImpression = 0;

                if (totalPostReach != 0 && totalPost != 0)
                {
                    averagePostReach = totalPostReach / totalPost;
                }
                if (totalStoryView != 0 && totalStory != 0)
                {
                    averageStoryView = totalStoryView / totalStory;
                }
                if (averagePostReach != 0 && averageStoryView != 0)
                {
                    averagePostPlusStoryImpression = averagePostReach + averageStoryView;
                }

                if (averagePostReach != 0)
                {

                    uViewModel.AverageImpressionPost = averagePostReach;
                    uViewModel.RatePerThousandImpressionPost = (uAffiliate.PaymentForPost / uViewModel.AverageImpressionPost) * 1000;

                }
                if (averageStoryView != 0)
                {
                    uViewModel.AverageImpressionStory = averageStoryView;
                    uViewModel.RatePerThousandImpressionStory = (uAffiliate.PaymentForStory / uViewModel.AverageImpressionStory) * 1000;

                }
                if (averagePostPlusStoryImpression != 0)
                {
                    uViewModel.AverageImpressionPostPlusStory = averagePostPlusStoryImpression;
                    uViewModel.RatePerThousandImpressionPostPlusStory = (uAffiliate.PaymentForPostPlusStory / uViewModel.AverageImpressionPostPlusStory) * 1000;

                }
                if (averagePostReach != 0 || averageStoryView != 0 || averagePostPlusStoryImpression != 0)
                {
                    uViewModels.Add(uViewModel);
                }

            }
            return uViewModels;
        }


        private List<AffiliateSuggestionViewModel> GetTHeList(List<AffiliateSuggestionViewModel> all, bool isPost, bool isStory, bool isPostPlusStory)
        {
            List<AffiliateSuggestionViewModel> returnableAffiliateList = new List<AffiliateSuggestionViewModel>();
            if (isPost && isStory && isPostPlusStory)
            {
                all = all.OrderBy(m => m.RatePerThousandImpressionPost).ThenBy(m => m.RatePerThousandImpressionStory).ThenBy(m => m.RatePerThousandImpressionPostPlusStory).ToList();
            }
            else if (isStory && isPostPlusStory)
            {
                all = all.OrderBy(m => m.RatePerThousandImpressionStory).ThenBy(m => m.RatePerThousandImpressionPostPlusStory).ToList();
            }
            else if (isPost && isPostPlusStory)
            {
                all = all.OrderBy(m => m.RatePerThousandImpressionPost).ThenBy(m => m.RatePerThousandImpressionPostPlusStory).ToList();
            }
            else if (isPost && isStory)
            {
                all = all.OrderBy(m => m.RatePerThousandImpressionPost).ThenBy(m => m.RatePerThousandImpressionStory).ToList();
            }

            else if (isPost)
            {
                all = all.OrderBy(m => m.RatePerThousandImpressionPost).ToList();
            }
            else if (isStory)
            {
                all = all.OrderBy(m => m.RatePerThousandImpressionStory).ToList();
            }
            else if (isPostPlusStory)
            {
                all = all.OrderBy(m => m.RatePerThousandImpressionPostPlusStory).ToList();
            }

            foreach (var item in all)
            {
                if (isPost && isStory && isPostPlusStory)
                {
                    List<TempVM> temps = new List<TempVM>();
                    TempVM tempPost = new TempVM
                    {
                        Type = 1,
                        Value = item.RatePerThousandImpressionPost
                    };
                    TempVM tempStory = new TempVM
                    {
                        Type = 2,
                        Value = item.RatePerThousandImpressionStory
                    };
                    TempVM tempPostPlusStory = new TempVM
                    {
                        Type = 3,
                        Value = item.RatePerThousandImpressionPostPlusStory
                    };

                    temps.Add(tempPost);
                    temps.Add(tempStory);
                    temps.Add(tempPostPlusStory);

                    var finalTemp = temps.OrderBy(m => m.Value).Where(m => m.Value != 0).FirstOrDefault();
                    item.SuggestedPostType = finalTemp.Type;
                    returnableAffiliateList.Add(item);
                }
                else if (isStory && isPostPlusStory)
                {
                    if ((item.RatePerThousandImpressionStory != 0 || item.RatePerThousandImpressionPostPlusStory != 0) && item.RatePerThousandImpressionStory > item.RatePerThousandImpressionPostPlusStory)
                    {
                        item.SuggestedPostType = 2;
                        returnableAffiliateList.Add(item);
                    }
                    else if ((item.RatePerThousandImpressionStory != 0 || item.RatePerThousandImpressionPostPlusStory != 0) && item.RatePerThousandImpressionStory < item.RatePerThousandImpressionPostPlusStory)
                    {
                        item.SuggestedPostType = 3;
                        returnableAffiliateList.Add(item);
                    }

                }
                else if (isPost && isPostPlusStory)
                {
                    if ((item.RatePerThousandImpressionPost != 0 || item.RatePerThousandImpressionPostPlusStory != 0) && item.RatePerThousandImpressionPost > item.RatePerThousandImpressionPostPlusStory)
                    {
                        item.SuggestedPostType = 1;
                        returnableAffiliateList.Add(item);
                    }
                    else if ((item.RatePerThousandImpressionPost != 0 || item.RatePerThousandImpressionPostPlusStory != 0) && item.RatePerThousandImpressionPost < item.RatePerThousandImpressionPostPlusStory)
                    {
                        item.SuggestedPostType = 3;
                        returnableAffiliateList.Add(item);
                    }


                }
                else if (isPost && isStory)
                {
                    if ((item.RatePerThousandImpressionPost != 0 || item.RatePerThousandImpressionStory != 0) && item.RatePerThousandImpressionPost > item.RatePerThousandImpressionStory)
                    {
                        item.SuggestedPostType = 1;
                        returnableAffiliateList.Add(item);
                    }
                    else if ((item.RatePerThousandImpressionPost != 0 || item.RatePerThousandImpressionStory != 0) && item.RatePerThousandImpressionPost < item.RatePerThousandImpressionStory)
                    {

                        item.SuggestedPostType = 2;
                        returnableAffiliateList.Add(item);
                    }


                }


                else if (isPost && item.RatePerThousandImpressionPost != 0)
                {
                    item.SuggestedPostType = 1;
                    returnableAffiliateList.Add(item);
                }
                else if (isStory && item.RatePerThousandImpressionStory != 0)
                {
                    item.SuggestedPostType = 2;
                    returnableAffiliateList.Add(item);
                }
                else if (isPostPlusStory && item.RatePerThousandImpressionPostPlusStory != 0)
                {
                    item.SuggestedPostType = 3;
                    returnableAffiliateList.Add(item);
                }

            }
            return returnableAffiliateList;
        }
        //private List<AffiliateSuggestionViewModel> GetWithAll(List<AffiliateSuggestionViewModel> all, int orderby, double targetImpression)
        //{
        //    List<AffiliateSuggestionViewModel> returnableAffiliateList = new List<AffiliateSuggestionViewModel>();
        //    if (orderby == 1)
        //    {
        //        all = all.OrderByDescending(m => m.AverageImpression).ThenBy(m => m.RatePerThousandImpressions).ToList();
        //    }
        //    else
        //    {
        //        all = all.OrderBy(m => m.AverageImpression).ThenBy(m => m.RatePerThousandImpressions).ToList();
        //    }
        //    double impressionCount = 0;
        //    for (int i = 0; impressionCount < targetImpression; i++)
        //    {

        //        foreach (var allAffiliate in all)
        //        {
        //            if (impressionCount < targetImpression)
        //            {
        //                if (allAffiliate.AverageImpression != 0)
        //                {
        //                    returnableAffiliateList.Add(allAffiliate);
        //                    impressionCount += allAffiliate.AverageImpression;
        //                }

        //                continue;

        //            }
        //            else
        //            {
        //                break;
        //            }

        //        }

        //    }
        //    return returnableAffiliateList;
        //}
        private List<AffiliateSuggestionViewModel> RecalculatePreferredRate(List<AffiliateSuggestionViewModel> all, double totalCostforProfit, double proposedRatePerThousandImpression)
        {

            //    var affiliateRate = all.Max(x => x.RatePerThousandImpressions);

            //    double sumOfTotal = all.Sum(m => m.PreferredRate);
            //    if ( sumOfTotal > totalCostforProfit)
            //    {
            //        var affiliates = all.Where(m => m.RatePerThousandImpressions == affiliateRate).ToList();

            //        double averageDecreament = (affiliateRate - proposedRatePerThousandImpression) / 2;


            //        if (affiliates.Count != 0)
            //        {
            //            foreach (var item in affiliates)
            //            {
            //                if (affiliateRate - proposedRatePerThousandImpression < 1)
            //                {
            //                    item.RatePerThousandImpressions = (item.RatePerThousandImpressions - 1);
            //                }
            //                else
            //                {
            //                    item.RatePerThousandImpressions = (item.RatePerThousandImpressions - averageDecreament);
            //                }

            //                item.PreferredRate = (item.RatePerThousandImpressions * (item.AverageImpression/1000));
            //                all.Remove(item);
            //                all.Add(item);

            //            }

            //        }
            //        if (all.Sum(m => m.PreferredRate) > totalCostforProfit)
            //        {
            //           return  RecalculatePreferredRate(all, totalCostforProfit, proposedRatePerThousandImpression);
            //        }
            //        else
            //        {
            //            return all;
            //        }



            //    }
            //    else
            //    {
            //       
            //    }

            return all;
        }
    }
}