using CloudinaryDotNet;

namespace back_end.Services
{
    public class CloudinaryService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary? _cloudinary;
        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;

            var name = _configuration["CloudinaryConfig:Name"];
            var key = _configuration["CloudinaryConfig:Key"];
            var secret = _configuration["CloudinaryConfig:Secret"];

            _cloudinary = new Cloudinary($"cloudinary://{key}:{secret}@{name}");
            _cloudinary.Api.Secure = true;
        }
        public Cloudinary? Cloudinary => _cloudinary;
    }
}
