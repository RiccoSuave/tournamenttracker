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
            WireUpLists();
            // We deleted the method below to go from having two methods of WireUpRoundsList and WireUpMatchupsList
            // to one of just one wireup method call WireUpLists();
            //WireUpMatchUpsLists();
            LoadFormData();
            LoadRounds();

        }
        private void LoadFormData ()
        {
            tournamentName.Text = tournament.TournamentName;
        }
        private void WireUpLists()
        {
            //roundDropDown.DataSource = null;
            
            roundDropDown.DataSource = rounds;
            matchupListbox.DataSource = selectedMatchups;
            matchupListbox.DisplayMember = "DisplayName";   


        }
        private void LoadRounds()
        {
            //rounds = new BindingList<int>();
            // removing the statement above, and clearing the rounds list fixed the issue of rounds not 
            // populating, so we can not create a new list, we have to clear the old one. 
            rounds.Clear();
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
            //roundsBinding.ResetBindings(false);
            //WireUpRoundsLists();
            // What adding the statement below does is it creates round one as soon as we load all the matchups 
            LoadMatchups(1);
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
            LoadMatchups((int)roundDropDown.SelectedItem);
        }
        private void LoadMatchups(int round)
        {
            //int round = (int)roundDropDown.SelectedItem;
            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    // In order to fix the type mismatch issue between a bindng list and an actual list, we 
                    // initialized a new BindingList and passed to the constructor an object of type iList,
                    // which list is of type iList.
                    // Since we foundout we could not instantiate a new rounds list, we know that we can not 
                    // instantiate a new matchups list, done below,  if we want the matchups to populate between 
                    // the dashboard and TournamentViewer form. 
                    //selectedMatchups = new BindingList<MatchupModel> (matchups);
                    // instead...
                    selectedMatchups.Clear();
                    foreach (MatchupModel m in matchups)
                    {
                        if ( m.Winner == null || !unplayedOnlyCheckbox.Checked)
                        {
                            selectedMatchups.Add(m);
                        }
                        
                        //LoadMatchup(selectedMatchups.First());
                    }
                    //LoadMatchup(selectedMatchups.First());
                }
                if (selectedMatchups.Count > 0)
                {
                    LoadMatchup(selectedMatchups.First());
                }
                DisplayMatchupInfo();
            }
            // true / false parameter resets or not the metadata in binging speciffied 
            //matchupsBinding.ResetBindings(false);
            //WireUpMatchUpsLists();
        }
        private void DisplayMatchupInfo()
        {
            bool isVisible = (selectedMatchups.Count > 0);
            teamOneName.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }
        private void tournamentName_Click(object sender, EventArgs e)
        {

        }
        private void LoadMatchup(MatchupModel m)
        {
            //MatchupModel m = (MatchupModel)matchupListbox.SelectedItem;
            //It looks like when I change to round two of GrandPrix, I get the null reference error 
            //I think it is because there are two many <bye>s
            //For some reason m is null sometimes, and I am not sure why
            if (m != null)
            {
                for (int i = 0; i < m.Entries.Count; i++)
                {
                    if (i == 0)
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
                            teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
                        }
                        else
                        {
                            teamTwoName.Text = "Team Name Not yet set";
                            teamTwoScoreValue.Text = "";
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void matchupListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListbox.SelectedItem);
        }

        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            MatchupModel m = (MatchupModel)matchupListbox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;
            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {

                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);
                        if (scoreValid)
                        {
                            m.Entries[0].Score = teamOneScore;
                        }
                        else
                        {
                            //m.Entries[0].Score = 0;
                            MessageBox.Show("Please enter a valid score for team 1");
                            return;
                        }

                    }
                }
                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        
                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);
                        if (scoreValid)
                        {
                            m.Entries[1].Score = teamTwoScore;
                        }
                        else
                        {
                            //m.Entries[0].Score = 0;
                            MessageBox.Show("Please enter a valid score for team 2");
                            return;
                        }

                    }

                }
            }
            if (teamOneScore > teamTwoScore)
            {
                // Team one wins 
                m.Winner = m.Entries[0].TeamCompeting;

            }
            else if (teamTwoScore > teamOneScore)
            {
                m.Winner = m.Entries[1].TeamCompeting;
            }
            else
            {
                MessageBox.Show("I don't handle tie games");
            }
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    foreach (MatchupEntryModel me in rm.Entries)
                    {
                        if (me.ParentMatchup != null)
                        {
                            if (me.ParentMatchup.Id == m.Id)
                            {
                                me.TeamCompeting = m.Winner;
                                GlobalConfig.Connection.UpdateMatchup(rm);
                            }
                        }
                    }
                }
            }
            LoadMatchups((int)roundDropDown.SelectedItem);
            GlobalConfig.Connection.UpdateMatchup(m);
        }

        private void teamOneScoreValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void teamTwoScoreValue_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
