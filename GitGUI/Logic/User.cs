using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GitGUI.Logic
{
    public struct User
    {
        string _email;

        public string Name { get; set; }

        public string Email
        {
            get { return _email; }
            set
            {
                if (!ValidEmail(value))
                    throw new ArgumentOutOfRangeException("value", value, "Invalid email format");
                _email = value;
            }
        }
        public string ImagePath { get; set; }

        bool ValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is User))
                return false;
            User u = (User)obj;
            return (u.Name == Name && u.Email == Email);
        }

        public static bool operator ==(User u1, User u2)
        {
            return u1.Equals(u2);
        }

        public static bool operator !=(User u1, User u2)
        {
            return !u1.Equals(u2);
        }
    }
}
