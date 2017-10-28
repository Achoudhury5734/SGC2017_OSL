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
        private readonly ImageService imageService;

        public DonationsController(DonationRepository donationRepository, UserRepository userRepository, ImageService imageService)
        {
            this.donationRepository = donationRepository;
            this.userRepository = userRepository;
            this.imageService = imageService;
        }

        // GET: api/donations
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.GetAll();
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

        // GET: api/donations/status/listed
        [Authorize]
        [HttpGet("status/listed")]
        public IActionResult GetListedDonations()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user)) {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.GetListed();
            foreach (var donation in donations)
            {
                donation.Donor = userRepository.GetById(donation.DonorId);
                if (donation.RecipientId.HasValue) {
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
                }
            }
            return Ok(donations);
        }


        public class NearbyRequest
        {
            public int Miles { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
        }

        // POST: api/donations/nearby/
        [Authorize]
        [HttpPost("nearby/")]
        public IActionResult GetDonationsWithinDistance([FromBody] NearbyRequest request)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }
            double meters = request.Miles * 1609.34;

            // If no location given, use organization location
            var Lat = request.Latitude ?? user.Lat;
            var Long = request.Longitude ?? user.Long;

            var donations = donationRepository.GetListedWithinDistance(Lat, Long, meters);
            foreach(var donation in donations)
            {
                donation.Donor = userRepository.GetById(donation.DonorId);
                if (donation.RecipientId.HasValue)
                {
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
                }
            }
            return Ok(donations);
        }

        // GET: api/donations/donor/me
        [Authorize]
        [HttpGet("donor/me")]
        public IActionResult GetDonorDonations()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.GetByDonorId(user.Id);
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

        //GET: api/values/recipient/me
        [Authorize]
        [HttpGet("recipient/me")]
        public IActionResult GetRecipientDonations()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.GetByRecipientId(user.Id);
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

        // GET api/donations/5
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

        // POST api/donations
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Donation donation)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user)) {
                return new UnauthorizedResult();
            } else {
                donation.PictureUrl = await imageService.UploadImageAsync(donation.Image);
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

        //PUT api/donations/5
        [Authorize]
        [HttpPut("{Id}")]
        public IActionResult Update(int Id, [FromBody]Donation donation)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }
            else
            {
                var originalDonation = donationRepository.GetById(Id);
                if (originalDonation == null)
                {
                    return new NotFoundResult();
                }

                if (donation.DonorId != user.Id)
                {
                    return new UnauthorizedResult();
                }

                donation.Updated = DateTime.Now;
                var updated = donationRepository.Update(donation);
                if (updated)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        // PUT api/donations/5/accept
        [Authorize]
        [HttpPut("{id}/accept")]
        public IActionResult Accept(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donation = donationRepository.GetById(id);
            if (donation == null)
            {
                return new NotFoundResult();
            }

            if (donation.RecipientId.HasValue || donation.Status != DonationStatus.Listed)
            {
                return BadRequest("Donation is not available");
            }

            donationRepository.AcceptDonation(id, user.Id);
            return Ok();
        }

        // PUT api/donations/5/complete
        [Authorize]
        [HttpPut("{id}/complete")]
        public IActionResult Complete(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donation = donationRepository.GetById(id);
            if (donation == null)
            {
                return new NotFoundResult();
            }

            //Verify that donation is owned by user
            if (donation.DonorId != user.Id)
            {
                return new UnauthorizedResult();
            }

            if (donation.Status != DonationStatus.PendingPickup)
            {
                return BadRequest("Donation must be accepted before it can be completed");
            }

            donationRepository.CompleteDonation(id);
            return Ok();
        }

        // PUT api/donations/5/waste
        [Authorize]
        [HttpPut("{id}/waste")]
        public IActionResult Waste(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donation = donationRepository.GetById(id);
            if (donation == null)
            {
                return new NotFoundResult();
            }

            if (donation.DonorId != user.Id)
            {
                return new UnauthorizedResult();
            }

            donationRepository.WasteDonation(id);
            return Ok();
        }

        //PUT api/donations/5/cancel
        [Authorize]
        [HttpPut("{Id}/cancel")]
        public IActionResult RecipientRelist(int Id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donation = donationRepository.GetById(Id);
            if (donation == null)
            {
                return new NotFoundResult();
            }

            if (user.Id != donation.RecipientId)
            {
                return new UnauthorizedResult();
            }
            
            donationRepository.RelistDonation(Id);
            return Ok();
        }

        //PUT api/donations/5/relist
        [Authorize]
        [HttpPut("{Id}/relist")]
        public IActionResult DonorRelist(int Id, [FromBody]Donation donation)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var originalDonation = donationRepository.GetById(Id);
            if (originalDonation == null)
            {
                return new NotFoundResult();
            }

            if (user.Id != donation.DonorId)
            {
                return new UnauthorizedResult();
            }

            if (originalDonation.Status == DonationStatus.Completed)
            {
                return BadRequest("Cannot relist completed donation");
            }

            originalDonation.Type = donation.Type;
            originalDonation.Title = donation.Title;
            originalDonation.Expiration = donation.Expiration;
            originalDonation.Amount = donation.Amount;
            donationRepository.RelistDonation(originalDonation);
            return Ok();
        }
    }
}
