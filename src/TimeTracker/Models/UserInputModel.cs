using System;
using TimeTracker.Domain;

namespace TimeTracker.Models
{
    public class UserInputModel
    {
        public string Name { get; set; }
        public decimal HourRate { get; set; }

        //Koristimo MapTo u UsersController da bi napravili novog korisnika u bazi sa podacima koje korisnik unese u UserInputModel
        public void MapTo(User user)
        {
            user.Name = Name;
            user.HourRate = HourRate;
        }
    }
}
