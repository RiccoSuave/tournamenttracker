using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        // = ... initializes the List; it is short cut since C# v 6.0 
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        
        // The otherway to initialize the list of person is
        /*
         * 
         */
        public TeamModel()
        {
            TeamMembers = new List<PersonModel>();
        }
    }

}
