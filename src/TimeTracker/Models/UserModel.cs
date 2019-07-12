using TimeTracker.Domain;

namespace TimeTracker.Models
{
    public class UserModel
    {
        private UserModel()
        {
            //Sad ne mozemo napraviti instancu user metoda osim kroz metod FromUser
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal HourRate { get; set; }

        public static UserModel FromUser (User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Name = user.Name,
                HourRate = user.HourRate
            };
        }

    }
}
