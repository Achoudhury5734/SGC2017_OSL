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

            if (!userRepository.IsRecipient(user) && !userRepository.IsActiveAdmin(user)) {
                return BadRequest("You don't have priveleges to view all donations.");
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

            if (!userRepository.IsRecipient(user) && !userRepository.IsActiveAdmin(user)) {
                return BadRequest("You don't have priveleges to view all listed donations.");
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
        public IActionResult GetListedWithinDistance([FromBody] NearbyRequest request)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            if (!userRepository.IsRecipient(user) && !userRepository.IsActiveAdmin(user)) {
                return BadRequest("You don't have priveleges to view all nearby donations.");
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
                if (donation.RecipientId.HasValue)
                {
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
                }
            }
            return Ok(donations);
        }

        // GET: api/donations/donor/me/status/1
        [Authorize]
        [HttpGet("donor/me/status/{status}")]
        public IActionResult GetDonorDonations(DonationStatus status) 
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            var donations = donationRepository.GetByDonorIdWithStatus(user.Id, (int)status);
            foreach (var donation in donations)
            {
                if (donation.RecipientId.HasValue)
                    donation.Recipient = userRepository.GetById(donation.RecipientId.Value);
            }
            return Ok(donations);
        }

        //GET: api/donations/recipient/me
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
            }
            return Ok(donations);
        }

        //GET: api/donations/recipient/me/status/1
        [Authorize]
        [HttpGet("recipient/me/status/{status}")]
        public IActionResult GetRecipientDonations(DonationStatus status)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            // Recipients can only have pending and completed donations
            if (status == DonationStatus.Wasted || status == DonationStatus.Listed)
                return BadRequest();

            var donations = donationRepository.GetByRecipientIdWithStatus(user.Id, (int)status);
            foreach (var donation in donations)
            {
                donation.Donor = userRepository.GetById(donation.DonorId);
            }
            return Ok(donations);
        }

        // GET api/donations/donor/me/stats
        [Authorize]
        [HttpGet("donor/me/stats")]
        public IActionResult GetDonorStats()
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }
            var stats = donationRepository.GetDonorStats(user.Id);
            return Ok(stats);
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
            }
            if (!userRepository.IsVerifiedUser(user)) {
                return BadRequest("Your account has not been verified yet. Please try again later.");
            }
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

        //PUT api/donations/5
        [Authorize]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody]Donation edited)
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

            if (donation.DonorId != user.Id)
            {
                return new UnauthorizedResult();
            }

            if (donation.Status == DonationStatus.Completed || donation.Status == DonationStatus.Wasted)
                return new UnauthorizedResult();

            donation.Amount = edited.Amount;
            donation.Type = edited.Type;
            donation.Title = edited.Title;
            donation.Expiration = edited.Expiration;

            String oldUrl = null;
            if (edited.Image != null && edited.Image.Length > 0)
            {
                oldUrl = donation.PictureUrl;
                donation.PictureUrl = await imageService.UploadImageAsync(edited.Image);
            }

            var updated = donationRepository.Update(donation);
            if (updated)
            {
                if (!String.IsNullOrEmpty(oldUrl) && !String.Equals(oldUrl, "Empty"))
                    await imageService.DeleteImageAsync(oldUrl);
                return Ok();
            }
            else
            {
                if (!String.IsNullOrEmpty(donation.PictureUrl) && !String.Equals(donation.PictureUrl, "Empty") && oldUrl != null)
                    await imageService.DeleteImageAsync(donation.PictureUrl);
                return BadRequest();
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

            if (!userRepository.IsRecipient(user))
            {
                return BadRequest("You are not authorized to pickup donations.");
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

            if (donation.RecipientId != user.Id)
            {
                return new UnauthorizedResult();
            }

            if (donation.Status != DonationStatus.PendingPickup)
            {
                return BadRequest();
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
        public IActionResult RecipientCancel(int Id)
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

            if (donation.Status != DonationStatus.PendingPickup)
            {
                return new BadRequestResult();
            }

            DonationStatus newStatus;
            if (donation.Expiration < DateTime.Now) {
                newStatus = DonationStatus.Wasted;
            } else {
                newStatus = DonationStatus.Listed;
            }
            
            donationRepository.RemoveRecipient(Id, newStatus);
            return Ok();
        }

        //PUT api/donations/5/reset
        [Authorize]
        [HttpPut("{Id}/reset")]
        public IActionResult RemoveRecipient(int Id)
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

            if (user.Id != donation.DonorId)
            {
                return new UnauthorizedResult();
            }

            if (donation.Status != DonationStatus.PendingPickup)
            {
                return new BadRequestResult();
            }

            DonationStatus newStatus;
            if (donation.Expiration < DateTime.Now) {
                newStatus = DonationStatus.Wasted;
            } else {
                newStatus = DonationStatus.Listed;
            }

            donationRepository.RemoveRecipient(Id, newStatus);
            return Ok();
        }

        //PUT api/donations/5/relist
        [Authorize]
        [HttpPut("{Id}/relist")]
        public async Task<IActionResult> DonorRelist(int Id, [FromBody]Donation donation)
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

            if (user.Id != originalDonation.DonorId)
            {
                return new UnauthorizedResult();
            }

            if (originalDonation.Status == DonationStatus.Completed)
            {
                return BadRequest("Cannot relist completed donation");
            }

            String oldUrl = null;
            if (donation.Image != null && donation.Image.Length > 0)
            {
                oldUrl = originalDonation.PictureUrl;
                originalDonation.PictureUrl = await imageService.UploadImageAsync(donation.Image);
            }

            originalDonation.Amount = donation.Amount;
            originalDonation.Type = donation.Type;
            originalDonation.Title = donation.Title;
            originalDonation.Expiration = donation.Expiration;

            var res = donationRepository.RelistDonation(originalDonation);
            if (res)
            {
                if (!String.IsNullOrEmpty(oldUrl) && !String.Equals(oldUrl, "Empty"))
                    await imageService.DeleteImageAsync(oldUrl);
                return Ok();
            }
            else
            {
                // If database update failed delete new image upload (if any), since reference will be to old
                if (oldUrl != null && !String.IsNullOrEmpty(originalDonation.PictureUrl) && !String.Equals(originalDonation.PictureUrl, "Empty"))
                    await imageService.DeleteImageAsync(originalDonation.PictureUrl);
                return BadRequest();
            }
        }
    }
}
