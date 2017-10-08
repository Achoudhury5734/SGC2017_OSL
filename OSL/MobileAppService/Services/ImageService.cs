using System;
using System.Collections.Generic;  
using System.IO;  
using System.Linq;  
using System.Threading.Tasks;  
using System.Web;
using Microsoft.WindowsAzure.Storage;  
using Microsoft.WindowsAzure.Storage.Blob;

namespace OSL.MobileAppService.Services
{
    public class ImageService
    {
        public async Task<string> UploadImageAsync(string imageToUploadBase64  
        {  
            string imageFullPath = null;  
            if (imageToUploadBase64 == "" || imageToUploadBase64 == null)  
            {  
                return null;  
            }  
            try  
            {  
                var credentials = new StorageCredentials(
                    configuration["BlobStorage:AccountName"],
                    configuration["BlobStorage:AccessKey"]
                );
                var account = new CloudStorageAccount(credentials, useHttps: true);
                var client = account.CreateCloudBlobClient(); 
                var container = client.GetContainerReference(configuration["BlobStorage:ContainerName"]);  

                if (await container.CreateIfNotExistsAsync())  
                {  
                    await container.SetPermissionsAsync(new BlobContainerPermissions {  
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
