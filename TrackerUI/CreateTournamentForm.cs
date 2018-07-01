using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();
        }
        private void WireUpLists()
        {
            SelectTeamDropDown.DataSource = null;
            SelectTeamDropDown.DataSource = availableTeams;
            SelectTeamDropDown.DisplayMember = "TeamName";
            tournamentTeamsListbox.DataSource = null;
            tournamentTeamsListbox.DataSource = selectedTeams;
            tournamentTeamsListbox.DisplayMember = "TeamName";
            PrizesListBox.DataSource = null;
            PrizesListBox.DataSource = selectedPrizes;
            PrizesListBox.DisplayMember = "PlaceName";
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)SelectTeamDropDown.SelectedItem;
            if ( t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);
                WireUpLists();
            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
        }

        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);
            WireUpLists();
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
            WireUpLists();
        }

        private void removeSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            //PersonModel p = (PersonModel)teamMembersListbox.SelectedItem;
            // if (p != null)
            // {
            //     selectedTeamMembers.Remove(p);
            //     availableTeamMembers.Add(p);
            //     WireUpLists();
            // } 
            TeamModel t = (TeamModel)tournamentTeamsListbox.SelectedItem;
            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);
                WireUpLists();
            }
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)PrizesListBox.SelectedItem;
            if (p != null)
            {
                selectedPrizes.Remove(p);
                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            //Validate Data 
            decimal fee = 0;
            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text, out fee);
            if (!feeAcceptable)
            {
                MessageBox.Show("Please enter a valid entry fee", "invalid fee",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return;
            }
            // Create our tournament model 
            TournamentModel tm = new TournamentModel();
            tm.TournamentName = tournamentNameValue.Text;
            tm.EntryFee = fee;
            tm.EnteredTeams = selectedTeams;
            // Create our matchups
            //TODO -- Wireup matchup  
            TournamentLogic.CreateRounds(tm);
            // Create Tournament Entry 

            // Create all of the prizes enteries 
            tm.Prizes = selectedPrizes;
            tm.EnteredTeams = selectedTeams;
            // Create all of the team entries 
            GlobalConfig.Connection.CreateTournament(tm);
            
            TournamentViewerForm frm = new TournamentViewerForm(tm);
            frm.Show();
            this.Close();
        }

        private void tournamentTeamsListbox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PrizesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
