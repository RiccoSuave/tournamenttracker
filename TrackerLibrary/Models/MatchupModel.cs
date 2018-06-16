using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    {
        /// <summary>
        /// Represents Id of each matchup. 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// A list of competing teams.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// The id from the db which will be used to identify the winner
        /// </summary>
        public int WinnerId { get; set; }
        /// <summary>
        /// The winner of each match.
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Specifies which round we are in.
        /// </summary>
        public int MatchupRound { get; set; }

        public string DisplayName
        {
            get
            {
                string output = "";
                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. { me.TeamCompeting.TeamName }";
                        }
                    }
                    else
                    {
                        output = $"Matchup not yet determined";
                        break;
                    }
                    
                }
                return output;
            }
        }
    }
}
