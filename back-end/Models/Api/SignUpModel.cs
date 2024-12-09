using back_end.Models.Entities;

namespace back_end.Models.Api
{
    public class SignUpModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public List<Gender>? GenderPreferences { get; set; }
    }
}
