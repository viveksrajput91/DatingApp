using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using API.Helpers;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            Account account=new Account(
                config.Value.CloudName,
                config.Value.APIKey,
                config.Value.APISecret
            );

            _cloudinary=new Cloudinary(account);
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            DeletionParams deletionParams=new DeletionParams(publicId);
            DeletionResult deletionResult= await _cloudinary.DestroyAsync(deletionParams);
            return deletionResult;
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile image)
        {
            var uploadResult=new ImageUploadResult();
            if(image.Length >0)
            {
                using var imageInStream= image.OpenReadStream();

                ImageUploadParams imageUploadParams=new ImageUploadParams()
                {
                    File=new FileDescription(image.FileName,imageInStream),
                    Transformation=new Transformation().Height(500).Width(500).Crop("fill").Gravity(Gravity.Face)
                };

                uploadResult= await _cloudinary.UploadAsync(imageUploadParams);
            }
            return uploadResult;
        }
    }
}