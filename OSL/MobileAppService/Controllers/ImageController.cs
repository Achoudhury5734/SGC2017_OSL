using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OSL.MobileAppService.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

        public async Task<string> UploadImageAsync(HttpPostedFileBase imageToUpload)  
        {  
            string imageFullPath = null;  
            if (imageToUpload == null || imageToUpload.ContentLength == 0)  
            {  
                return null;  
            }  
            try  
            {  
                CloudStorageAccount cloudStorageAccount = ConnectionString.GetConnectionString();  
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();  
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("DonationImage");  

                if (await cloudBlobContainer.CreateIfNotExistsAsync())  
                {  
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions {  
                        PublicAccess = BlobContainerPublicAccessType.Blob  
                    });  
                }  
                string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);  

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);  
                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;  
                await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);  

                imageFullPath = cloudBlockBlob.Uri.ToString();  
            }  
            catch (Exception ex)  
            {  

            }  
            return imageFullPath;  
        }  
    }
}
