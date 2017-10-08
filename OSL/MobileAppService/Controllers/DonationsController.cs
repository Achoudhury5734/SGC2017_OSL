using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSL.MobileAppService.Models;
using OSL.MobileAppService.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OSL.MobileAppService.Controllers
{
    [Route("api/[controller]")]
    public class DonationsController : Controller
    {
        private readonly DonationRepository donationRepository;
        private readonly UserRepository userRepository;

        public DonationsController(DonationRepository donationRepository, UserRepository userRepository)
        {
            this.donationRepository = donationRepository;
            this.userRepository = userRepository;
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            var donations = donationRepository.Get();
            foreach (var donation in donations) 
            {
                try {
                    donation.Donor = userRepository.GetById(donation.DonorId);
                    if (donation.RecipientId > 0)
                    {
                        donation.Recipient = userRepository.GetById(donation.RecipientId);

                    }
                }
                catch (Exception eerror)
                {
                    Console.WriteLine(eerror);
                }

            }
            return Ok(donations);
        }

        // GET api/values/5
        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            return Ok(donationRepository.Get(Id));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
