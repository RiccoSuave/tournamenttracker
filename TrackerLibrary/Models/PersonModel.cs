using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {
        /// <summary>
        /// Team member's first name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Team member's last name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Team member's email address.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Team member's cell phone number.
        /// </summary>
        public string CellPhoneNumber { get; set; }


    }
}
