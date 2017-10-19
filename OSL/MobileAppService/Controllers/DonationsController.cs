using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OSL.MobileAppService.Models;
using OSL.MobileAppService.Services;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OSL.MobileAppService.Controllers
{
    [Route("api/[controller]")]
    public class DonationsController : Controller
    {
        private readonly DonationRepository donationRepository;
        private readonly UserRepository userRepository;
        //ImageService imageService = new ImageService();

        public DonationsController(DonationRepository donationRepository, UserRepository userRepository)
        {
            this.donationRepository = donationRepository;
            this.userRepository = userRepository;
        }

        // GET: api/values
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user)) {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.Get();
            foreach (var donation in donations) 
            {
                donation.Donor = userRepository.GetById(donation.DonorId);
                if (donation.RecipientId.HasValue) {
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
                }
            }
            return Ok(donations);
        }

        // GET: api/values/donor/13
        [Authorize]
        [HttpGet("donor/{donorId}")]
        public IActionResult GetUserDonations(int donorId)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.GetByUserId(donorId);
            foreach (var donation in donations)
            {
                donation.Donor = userRepository.GetById(donation.DonorId);
                if (donation.RecipientId.HasValue)
                {
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
                }
            }
            return Ok(donations);
        }

        // GET: api/values/donor/me
        [Authorize]
        [HttpGet("donor/me")]
        public IActionResult GetDonations()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.GetByUserId(user.Id);
            foreach (var donation in donations)
            {
                donation.Donor = userRepository.GetById(donation.DonorId);
                if (donation.RecipientId.HasValue)
                {
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
                }
            }
            return Ok(donations);
        }

        // GET api/values/5
        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user)) {
                return new UnauthorizedResult();
            }
            var donation = donationRepository.GetById(Id);

            if (donation != null) {
                donation.Donor = userRepository.GetById(donation.DonorId);
                if (donation.RecipientId.HasValue) {
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
                }
                return Ok(donation);
            } else {
                return new NotFoundResult();
            }
        }

        // POST api/values
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Donation donation)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user)) {
                return new UnauthorizedResult();
            } else {
               // donation.PictureUrl = await imageService.UploadImageAsync(donation.Image);
                donation.DonorId = user.Id;
                donation.Created = DateTime.Now;
                donation.Updated = DateTime.Now;
                donation.StatusUpdated = DateTime.Now;
                if (donation.PictureUrl == null) {
                    donation.PictureUrl = "Empty";
                }
                if (donation.Expiration == null) {
                    var expires = DateTime.Now;
                    donation.Expiration = expires.AddHours(2);
                }
                var insertedDonation = donationRepository.Create(donation);
                if (insertedDonation != null) {
                    return Ok(insertedDonation);
                } else {
                    return BadRequest("Invalid donation data.");
                }
            }
        }

        // POST api/Donation/5/status/PendingPickup
        [Authorize]
        [HttpPost("{Id}/status/{Status}")]
        public IActionResult POST(int Id, string Status)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user)) {
                return new UnauthorizedResult();
            } else {
                var originalDonation = donationRepository.GetById(Id);
                if (originalDonation == null) {
                    return new NotFoundResult();
                }
                //
                // Process Status property property HERE.
                // Only Donor can change to Wasted, Completed, or Canceled
                // ...

            }
            return BadRequest("Invalid donation data.");

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
