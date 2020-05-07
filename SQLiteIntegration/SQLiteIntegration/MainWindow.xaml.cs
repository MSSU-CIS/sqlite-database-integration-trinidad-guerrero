using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace SQLiteIntegration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      List<string> HipHopList = new List<string>();
      public MainWindow()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (HasValidInput())
            {
                int torrents = int.Parse(TorrentsTextBox.Text);
                int tags = int.Parse(TagsTextBox.Text);

                //MARKER - Probably an error here
                DataTable dt = new DataTable();

                string datasource = @"Data Source=../../whatcd-hiphop.sqlite;";
                //Batting.'2B' is the number of torrents a player hit in a season
                //Batting.'3B' is the number of tags a player hit in a season
                string sql = $"SELECT namefirst, namelast,Sum (Batting.'2B'),Sum (Batting.'3B') FROM Master INNER JOIN Batting ON Batting.playerid = Master.playerid GROUP BY Master.playerid HAVING Sum (Batting.'2B') > {torrents} AND Sum (Batting.'3B')> {tags};";
                using (SQLiteConnection conn = new SQLiteConnection(datasource))
                {
                    conn.Open();
                    SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn);
                    da.Fill(dt);
                    conn.Close();
                }

                HipHopList.Clear();
                foreach (DataRow row in dt.Rows)
                {
               // MARKER: Making something different show up
               string playerRow = $"{row​[0].ToString()} {row​[1].ToString()} - 2B = {row​[2].ToString()}, 3B = {row​[3].ToString()}";
               HipHopList.Add(playerRow);
                }
                populateList();
            }
        }

        private void populateList()
        {
            HiphopView.Items.Clear();
            foreach (string s in HipHopList)
            {
                HiphopView.Items.Add(s);
            }
        }

        private string ValidateTextBox(TextBox box, string name)
        {
            string message = "";
            string text = box.Text;
            int amount = 0;

            if (text == "")
                box.Text = "0";
            else
            {
                bool isNumber = int.TryParse(text, out amount);
                if (!isNumber)
                {
                    message += $"Invalid Input - {name} is not a number! ";
                }
                if (amount < 0)
                {
                    message += $"Invalid Input - {name} is negative! ";
                }
            }
            return message;
        }

        private bool HasValidInput()
        {
            string message = ValidateTextBox(TorrentsTextBox, "Torrents") +
                ValidateTextBox(TagsTextBox, "tags");

            if (message == "")
            {
                return true;
            }
            else
            {
                MessageBox.Show(message);
                return false;
            }
        }
    }
}
