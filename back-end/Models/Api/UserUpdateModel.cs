using back_end.Models.Entities;

namespace back_end.Models.Api
{
    public class UserUpdateModel
    {
        public string? Id { get; set; } // same
        public string? Email { get; set; } // passed in without being inputted by user on the frontend
        public string? Name { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public List<Gender>? GenderPreferences { get; set; }
        public string? Bio { get; set; }
        public string? Image {  get; set; }
    }
}
