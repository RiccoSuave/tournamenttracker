using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        public event EventHandler<DateTime> OnTournamentComplete;
        /// <summary>
        /// The unique identifier for the trounament 
        /// </summary> 
        public int Id { get; set; }
        /// <summary>
        /// The name os tournament 
        /// </summary>
        public string TournamentName { get; set; }
        /// <summary>
        /// How much each team has to pay to participate 
        /// </summary>
        public decimal EntryFee { get; set; }
        /// <summary>
        /// A list of teams participating in a speciffied tournament
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        /// <summary>
        /// A list of prizes for ex. $100, $50, $25,...
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>
        /// A list of rounds describing who plays with who
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();

        public void CompleteTournament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }
    }
}
