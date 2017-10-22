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

        // GET: api/donations/donor/me
        [Authorize]
        [HttpGet("donor/me")]
        public IActionResult GetUserDonations()
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

        

        // PUT api/donations/5/accept
        [HttpPut("{id}/accept")]
        public IActionResult Accept(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            donationRepository.AcceptDonation(id, user.Id);
            return Ok();
        }

        // PUT api/donations/5/complete
        [HttpPut("{id}/complete")]
        public IActionResult Complete(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            //Verify that donation is owned by user
            var donation = donationRepository.GetById(id);
            if (donation.DonorId != user.Id)
            {
                return new UnauthorizedResult();
            }

            donationRepository.CompleteDonation(id);
            return Ok();
        }

        // PUT api/donations/5/waste
        [HttpPut("{id}/waste")]
        public IActionResult Waste(int id)
        {
            var user = userRepository.GetUserFromPrincipal(HttpContext.User);
            if (!userRepository.IsActiveUser(user))
            {
                return new UnauthorizedResult();
            }

            //Verify that donation is owned by user
            var donation = donationRepository.GetById(id);
            if (donation.DonorId != user.Id)
            {
                return new UnauthorizedResult();
            }

            donationRepository.WasteDonation(id);
            return Ok();
        }
    }
}
