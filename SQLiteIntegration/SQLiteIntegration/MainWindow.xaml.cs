using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SQLiteIntegration
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      List<string> TrackList;
      List<string> DetailList;
      public MainWindow()
      {
         TrackList = new List<string>();
         DetailList = new List<string>();
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         SearchTracks();
      }

      private void populateList()
      {
         TracksView.Items.Clear();
         foreach (string s in TrackList)
         {
            TracksView.Items.Add(s);
         }
      }

      private string ValidateTextBox(TextBox box, string name)
      {
         string message = "";
         string text = box.Text.Trim();
         int amount = 0;

         if (0 < text.Length)
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
         return true;
      }

      private void AlbumTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
      {
         if (Key.Enter == e.Key)
         {
            SearchTracks();
         }
      }

      private void ArtistTextBox_KeyDown(object sender, KeyEventArgs e)
      {
         if (Key.Enter == e.Key)
         {
            SearchTracks();
         }
      }

      private void SearchTracks()
      {

         if (HasValidInput())
         {
            DataTable dt = new DataTable();

            string datasource = @"Data Source=..\..\Chinook.sqlite;";
            string sql = $"select ab.`Title`, count(t.`TrackId`), at.`Name`, sum(t.`UnitPrice`) from Track t inner join Album ab on t.`AlbumId` = ab.`AlbumId` inner join Artist at on ab.`ArtistId` = at.`ArtistId` where ab.`Title` like '%{AlbumTextBox.Text.Trim()}%' AND at.`Name` like '%{ArtistTextBox.Text.Trim()}%' group by ab.`AlbumId`";
            using (SQLiteConnection conn = new SQLiteConnection(datasource))
            {
               conn.Open();
               SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn);
               da.Fill(dt);
               conn.Close();
            }

            TrackList.Clear();
            DetailList.Clear();
            foreach (DataRow row in dt.Rows)
            {
               TrackList.Add(row​[0].ToString());
               DetailList.Add($"No of Tracks {row[1].ToString()}\nArtist Name: {row[2].ToString()}\nPrice of Track: {row[3].ToString()}");
            }
            populateList();
         }
      }

      private void TracksView_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         int i = TracksView.SelectedIndex;
         if (-1 < i)
         {
            MessageBox.Show(DetailList[i], "Track Details");
         }
      }
   }
}
