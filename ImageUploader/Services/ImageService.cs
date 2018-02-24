﻿using ImageUploader.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ImageUploader.Services
{
    public class ImageService
    {
        private readonly string _imageRootPath;
        private readonly string _containerName;
        private readonly string _blobStorageConnectionString;
        public ImageService()
        {
            _imageRootPath = ConfigurationManager.AppSettings["ImageRootPath"];
            _containerName = ConfigurationManager.AppSettings["ImagesContainer"];
            _blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
        }

        public async Task<UploadedImage> CreateUploadedImage(HttpPostedFileBase file)
        {
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                byte[] fileBytes = new byte[file.ContentLength];
                await file.InputStream.ReadAsync(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                return new UploadedImage
                {
                    ContentType = file.ContentType,
                    Data = fileBytes,
                    Name = file.FileName,
                    Url = $"{_imageRootPath}{file.FileName}"
                };
            }
            return null;
        }

        public async Task AddImageToBlobStorageAsync(UploadedImage image)
        {
            //  get the container reference
            var container = GetImagesBlobContainer();
            // using the container reference, get a block blob reference and set its type
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(image.Name);
            blockBlob.Properties.ContentType = image.ContentType;
            // finally, upload the image into blob storage using the block blob reference
            var fileBytes = image.Data;
            await blockBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);
        }
        private CloudBlobContainer GetImagesBlobContainer()
        {
            // use the connection string to get the storage account
            var storageAccount = CloudStorageAccount.Parse(_blobStorageConnectionString);
            // using the storage account, create the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();
            // finally, using the blob client, get a reference to our container
            var container = blobClient.GetContainerReference(_containerName);
            // if we had not created the container in the portal, this would automatically create it for us at run time
            container.CreateIfNotExists();
            // by default, blobs are private and would require your access key to download.
            //   You can allow public access to the blobs by making the container public.   
            container.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            return container;
        }

        public List<UploadedImage> GetAllImages()
        {
            var container = GetImagesBlobContainer();
            var lista = new List<UploadedImage>();
            foreach (var item in container.ListBlobs())
            {
               lista.Add(new UploadedImage() {
                   Url = item.Uri.ToString()
               });
            }

            return lista;
        }
    }
}