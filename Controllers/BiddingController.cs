using Microsoft.AspNetCore.Mvc;
using RTB.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace RTB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BiddingController : ControllerBase
    {
        private readonly ILogger<BiddingController> _logger;

        public BiddingController(ILogger<BiddingController> logger)
        {
            _logger = logger;
        }

        private readonly List<Advertisement> _advertisements = new List<Advertisement>
        {
            new Advertisement { Country = "france", Category = "food", Price = 3, UpTo = 7 },
            new Advertisement { Country = "france", Category = "food", Price = 4, UpTo = 9  },

            new Advertisement { Country = "france", Category = "tech", Price = 5, UpTo = 5 },

            new Advertisement { Country = "belgium", Category = "food", Price = 6, UpTo = 7 },
            new Advertisement { Country = "belgium", Category = "food", Price = 1, UpTo = 9 },
            new Advertisement { Country = "belgium", Category = "tech", Price = 3, UpTo = 6 },

            new Advertisement { Country = "USA", Category = "tech", Price = 2, UpTo = 12 },

            new Advertisement { Country = "USA", Category = "tech", Price = 5, UpTo = 9 },

            new Advertisement { Country = "USA", Category = "tech", Price = 6, UpTo = 7 }
        };

        [HttpPost]
        [Route("ad")]
        public ActionResult<Advertisement> GetHighestPricedAd([FromBody] UserInfo userInfo)
        {
            _logger.LogInformation("Received POST request to /Bidding/ad");

            if (userInfo == null)
            {
                _logger.LogError("User information is missing in the request body.");
                return BadRequest("User information is missing in the request body.");
            }

            _logger.LogInformation("User information received: {@UserInfo}", userInfo);

            var matchingAds = GetMatchingAdvertisements(userInfo);
            if (matchingAds.Any())
            {
                var highestPricedAd = GetHighestPricedAdvertisement(matchingAds);
                _logger.LogInformation("Highest priced advertisement found: {@Advertisement}", highestPricedAd);
                return highestPricedAd;
            }

            _logger.LogInformation("No matching advertisements found.");
            return NotFound("No matching advertisements found.");
        }

        private List<Advertisement> GetMatchingAdvertisements(UserInfo user)
        {
            return _advertisements.Where(ad =>
                ad.Country.ToLower() == user.Country.ToLower() &&
                ad.Category.ToLower() == user.Category.ToLower()).ToList();
        }

        private Advertisement GetHighestPricedAdvertisement(List<Advertisement> advertisements)
        {
            // Sort advertisements by UpTo value descending and then by price descending
            advertisements = advertisements.OrderByDescending(ad => ad.UpTo).ThenByDescending(ad => ad.Price).ToList();

            // Get the highest priced advertisement
            var highestPricedAd = advertisements.FirstOrDefault();

            // Check if the highest priced advertisement also has the highest UpTo value among all matching advertisements
            if (highestPricedAd.UpTo == advertisements.Max(ad => ad.UpTo))
            {
                // If yes, check if the highest priced advertisement also has the highest price
                if (highestPricedAd.Price == advertisements.Max(ad => ad.Price))
                {
                    // If yes, return the advertisement with only the price
                    highestPricedAd = new Advertisement
                    {
                        Country = highestPricedAd.Country,
                        Category = highestPricedAd.Category,
                        Price = highestPricedAd.Price
                    };
                }
                else
                {
                    // If no, return the advertisement with only the UpTo value
                    highestPricedAd = new Advertisement
                    {
                        Country = highestPricedAd.Country,
                        Category = highestPricedAd.Category,
                        UpTo = highestPricedAd.UpTo
                    };
                }
            }

            return highestPricedAd;
        }

    }
}

