using TimeTracker.Domain;

namespace TimeTracker.Models
{
    public class UserModel
    {
        // Dokumentacija
        /// <summary>
        /// Represents one time tracker user.
        /// </summary>
        private UserModel()
        {
            //Sad ne mozemo napraviti instancu user metoda osim kroz metod FromUser
        }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets user name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets how much user will be paid per hour.
        /// </summary>
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
