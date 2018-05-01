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

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private string storageChoice;
        DatabaseType db = new DatabaseType();
        public TournamentViewerForm()
        {
            InitializeComponent();
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

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
