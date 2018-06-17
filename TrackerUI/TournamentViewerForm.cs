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
    public partial class TournamentViewerForm : Form
    {
        private string storageChoice;
        DatabaseType db = new DatabaseType();
        private TournamentModel tournament;
        
        BindingList<int> rounds = new BindingList<int>();
        BindingList<MatchupModel> selectedMatchups = new BindingList<MatchupModel>();
        // Lookup binding source documentation later; findout why it was the old way of doing things 
        // Also try to figure out why this or the binding list did not fix the issue. Tim fixes this later
        // but it is not clear what was the root cause. 
        //BindingSource roundsBinding = new BindingSource();
        //BindingSource matchupsBinding = new BindingSource();
        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();
            tournament = tournamentModel;
            WireUpRoundsLists();
            WireUpMatchUpsLists();
            LoadFormData();
            LoadRounds();

        }
        private void LoadFormData ()
        {
            tournamentName.Text = tournament.TournamentName;
        }
        private void WireUpRoundsLists()
        {
            //roundDropDown.DataSource = null;
            roundsBinding.DataSource = rounds;
            roundDropDown.DataSource = roundsBinding;
            roundDropDown.DataSource = rounds;

        }
        private void WireUpMatchUpsLists()
        {
            //matchupListbox.DataSource = null;
            matchupsBinding.DataSource = selectedMatchups;
            matchupListbox.DataSource = matchupsBinding;
            matchupListbox.DataSource = selectedMatchups;
            matchupListbox.DisplayMember = "DisplayName";
        }
        private void LoadRounds()
        {
            rounds = new List<int>();
            rounds.Add(1);
           
            int currRound = 1;
            
            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currRound)
                {
                    currRound = matchups.First().MatchupRound;
                    rounds.Add(currRound);
                    
                }
            }
            roundsBinding.ResetBindings(false);
            //WireUpRoundsLists();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            //I added this to implement the storage pull down menu
            storageChoice = (string)comboBox.SelectedItem;
            //if (storageChoice.Equals(DatabaseType.Sql)) db = DatabaseType.Sql;
            //if (storageChoice.Equals(DatabaseType.TextFile)) db = DatabaseType.TextFile;
            //if (storageChoice.Equals(DatabaseType.All)) db = DatabaseType.All;
            //or I can say...
            //if (storageChoice.Equals(DatabaseType.All)) db = DatabaseType.Sql & DatabaseType.TextFile;

        }
        private void InitialComboBox()
        {
            string[] storageOptions = new string[] { "Sql", "TextFile", "All" };
            comboBox2.Items.AddRange(storageOptions);
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(comboBox2_SelectedIndexChanged);
            foreach (string choice in storageOptions)
            {
                DatabaseType dbb = new DatabaseType();
                if (Enum.TryParse(choice, true, out dbb))
                    db = dbb;
            }           
            //TODO - write an error message to the screen if the result is false above
        }
        // The method below will update the event every time the user changes / chooses a different item from 
        // the list
        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }
        private void LoadMatchups()
        {
            int round = (int)roundDropDown.SelectedItem;
            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    selectedMatchups = matchups;

                }
            }
            // true / false parameter resets or not the metadata in binging speciffied 
            matchupsBinding.ResetBindings(false);
            //WireUpMatchUpsLists();
        }
        private void tournamentName_Click(object sender, EventArgs e)
        {

        }
        private void LoadMatchup()
        {
            MatchupModel m = (MatchupModel)matchupListbox.SelectedItem;
            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i==0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                        teamTwoName.Text = "<bye>";
                        teamTwoScoreValue.Text = "0";
                    }
                    else
                    {
                        teamOneName.Text = "Team Name Not yet set";
                        teamOneScoreValue.Text = "";
                    }
                }
                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = m.Entries[0].Score.ToString();
                    }
                    else
                    {
                        teamTwoName.Text = "Team Name Not yet set";
                        teamTwoScoreValue.Text = "";
                    }
                }
            }
        }

        private void matchupListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup();
        }
    }
}
