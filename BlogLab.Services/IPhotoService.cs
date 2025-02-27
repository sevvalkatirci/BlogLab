﻿using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace BlogLab.Services
{
    public interface IPhotoService
    {
        public Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        public Task<DeletionResult> DeletePhotoAsyn(string publicId);

    }
}