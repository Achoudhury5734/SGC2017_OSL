using System;
using System.Collections.Generic;  
using System.IO;  
using System.Linq;  
using System.Threading.Tasks;  
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace OSL.MobileAppService.Services
{
    public class ImageService
    {
        public ImageService(IConfigurationRoot configurationRoot)
        {
            Configuration = configurationRoot;
        }

        public IConfigurationRoot Configuration { get; }

        public async Task<string> UploadImageAsync(byte[] imageToUploadBytes)  
        {  
            string imageFullPath = null;  
            try  
            {  
                var credentials = new StorageCredentials(
                    Configuration["BlobStorage:AccountName"],
                    Configuration["BlobStorage:AccessKey"]
                );
                var storageAccount = new CloudStorageAccount(credentials, useHttps: true);
                var blobClient = storageAccount.CreateCloudBlobClient(); 
                var container = blobClient.GetContainerReference(Configuration["BlobStorage:ContainerName"]);  

                if (await container.CreateIfNotExistsAsync())  
                {  
                    await container.SetPermissionsAsync(new BlobContainerPermissions {  
                        PublicAccess = BlobContainerPublicAccessType.Blob  
                    });  
                }  
                string imageName = Guid.NewGuid().ToString() + ".jpg";  

                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(imageName);  

                await cloudBlockBlob.UploadFromByteArrayAsync(imageToUploadBytes, 0, imageToUploadBytes.Length);  

                imageFullPath = cloudBlockBlob.Uri.ToString();  
            }  
            catch (Exception ex)  
            {  

            }  
            return imageFullPath;  
        }
    }
}
