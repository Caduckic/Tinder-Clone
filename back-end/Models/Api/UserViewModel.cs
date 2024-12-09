using back_end.Models.Entities;

namespace back_end.Models.Api
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string? Bio {  get; set; }
        public string? Image {  get; set; }
        public UserViewModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Age = user.Age;
            Gender = user.Gender;
            Bio = user.Bio;
            Image = user.Image;
        }
    }
}
