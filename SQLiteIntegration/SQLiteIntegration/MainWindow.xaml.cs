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
      List<string> SalariesList = new List<string>();
      public MainWindow()
      {

      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         if (HasValidInput())
         {
            int BasePay = int.Parse(BasePayTextBox.Text);
            string EmployeeName = EmployeeNameTextBox.Text;

            //MARKER - Probably an error here
            DataTable dt = new DataTable();

            string datasource = @"Data Source=../../database1.sqlite;";
            //string datasource = @"Data Source=../../whatcd-hiphop.sqlite;";
            //Batting.'2B' is the number of torrents a player hit in a season
            //Batting.'3B' is the number of tags a player hit in a season
            string sql = $"SELECT EmployeeName, JobTitle, Description, Sum (Salaries.BasePay) As BasePay,Sum (Salaries.OverTimePay),Sum (Salaries.Benefits),Sum (Salaries.TotalPay),Sum (Salaries.TotalPayBenefits) FROM Salaries INNER JOIN Jobs ON Salaries.JobTitle = Jobs.Job Where EmployeeName Like '%{EmployeeName}%' GROUP BY Salaries.EmployeeName, Salaries.JobTitle, Jobs.Description HAVING Sum (Salaries.BasePay) >= {BasePay};";
            using (SQLiteConnection conn = new SQLiteConnection(datasource))
            {
               conn.Open();
               SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn);
               da.Fill(dt);
               conn.Close();
            }

            SalariesList.Clear();
            foreach (DataRow row in dt.Rows)
            {
               // MARKER: Making something different show up
               string EmployeeRow = $"{row​[0].ToString()}, {row​[1].ToString()} - {row​[2].ToString()}, {row​[3].ToString()}";
               SalariesList.Add(EmployeeRow);
            }
            populateList();
         }
      }

      private void populateList()
      {
         SalariesView.Items.Clear();
         foreach (string s in SalariesList)
         {
            SalariesView.Items.Add(s);
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
            if (name.Equals("EmployeeName"))
            {
               if (text.Equals(""))
                  message += $"Invalid Input - {name} cannot be empty! ";
            }
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
         }
         return message;
      }

      private bool HasValidInput()
      {
         string message = ValidateTextBox(EmployeeNameTextBox, "EmployeeName") +
             ValidateTextBox(BasePayTextBox, "BasePay");

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