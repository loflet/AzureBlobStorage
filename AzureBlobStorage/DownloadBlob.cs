using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorage
{
    class DownloadBlob
    {
        static void Main(string[] args)
        {
            //Private members
            string containerName = "spfx-171-30-8";            
            //Get credentials
            ConnectionStringSettings azureCredentials = ConfigurationManager.ConnectionStrings["AzureStorage"];
            //Create connection to the storage
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureCredentials.ConnectionString);
            //Path to store the blob.
            string path = @"C:\Users\SanketGhorpade\Source\Repos\AzureBlobStorage\AzureBlobStorage\AppData\";
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            //Getting the BlockBlob list.
            IEnumerable<CloudBlockBlob> blobcontainer = container.ListBlobs().OfType<CloudBlockBlob>(); 
            //Getting the BlobDirectory list.
            IEnumerable<CloudBlobDirectory> directoryContainer = container.ListBlobs().OfType<CloudBlobDirectory>(); 

            //Iterating through the BlockBlob and storing it locally.
            foreach (CloudBlockBlob blob in blobcontainer)
            {
                string filename = Path.Combine(path, blob.Name);
                blob.DownloadToFile(filename, FileMode.CreateNew);
            }

            //Iterating through the list of the directories then creating the directory locally and saving the content of that.
            foreach (CloudBlobDirectory blob in directoryContainer)
            {
                string directoryName = blob.Prefix.Remove(blob.Prefix.Length-1);
                string directoryPath = Path.Combine(path, directoryName);
                //Creating Directory locally
                Directory.CreateDirectory(directoryPath);

                var subfiles = blob.ListBlobs().OfType<CloudBlockBlob>();
                //Iterating through the directory and storing the files locally.
                foreach (var file in subfiles)
                {                       
                    string filename = Path.Combine(directoryPath, file.Name.Split('/')[1]);
                    file.DownloadToFile(filename, FileMode.CreateNew);
                }                
            }
        }
    }
}
