using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public partial class CreatePrizeForm : Form
    {
        public CreatePrizeForm()
        {
            InitializeComponent();
        }

        private void prizePercentageValueLabel_TextChanged(object sender, EventArgs e)
        {

        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidatForm())
            {
                PrizeModel model = new PrizeModel(
                    placeNumberValue.Text, 
                    palceNameValue.Text,
                    prizeAmountValue.Text,
                    prizePercentageValue.Text);
                GlobalConfig.Connection.CreatePrize(model);
                placeNumberValue.Text = "";
                palceNameValue.Text = "";
                prizeAmountValue.Text = "0";
                prizePercentageValue.Text = "0";
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please fix" +
                    "and try again.");
            }

        }
        private bool ValidatForm()
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValidNumber = int.TryParse(placeNumberValue.Text, out placeNumber);
            if (placeNumberValidNumber == false)
            {
                output = false;
            }
            if (placeNumber < 1)
            {
                output = false;
            }
            if (palceNameValue.Text.Length == 0)
            {
                output = false;
            }
            decimal prizeAmount=0;
            double prizePercentage=0;
            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageValue.Text, out prizePercentage);
            if (prizeAmountValid == false || prizePercentageValid == false)
            {
                output = false;
            }
            if (prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
            }
            if (prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }
            return output;
        }
    }
}
