using back_end.Models.Entities;

namespace back_end.Models.Api
{
    public class CurrentUserViewModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public CurrentUserViewModel(User user)
        {
            Name = user.Name;
            Email = user.Email;
            Age = user.Age;
            Gender = user.Gender;
        }
    }
}
