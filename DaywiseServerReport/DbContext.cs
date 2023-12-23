using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaywiseServerReport
{
    public class DbContext
    {
        public string conStr = "";
        public List<string> FindKioskIds(string project, string dateFrom, string dateTo)
        {
            string query = $"select DISTINCT KioskID from Payments where PaymentDate >='{dateFrom} 00:00:00.000' and PaymentDate <='{dateTo} 00:00:00.000'";
            List<string> ids = new List<string>();
            if (project == "150")
                conStr = $"Server=10.10.2.39;Database=WelkinATP_150Machine;User Id=sa;Password=Wb&210$0919@KiosK(3)welKIN#;";
            else if (project == "210")
                conStr = $"Server=10.10.2.39;Database=WelkinATP;User Id=sa;Password=Wb&210$0919@KiosK(3)welKIN#;";


            SqlConnection con = new SqlConnection(conStr);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Find Kios Ids\nDescription : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
            return ids;
        }
        public string FindDetails(string kioskId, string project, string date)
        {
            decimal totalAmt = 0;
            int rcptStart = 0;
            int rcptEnd = 0;

            if(project == "150")
                conStr = $"Server=10.10.2.39;Database=WelkinATP_150Machine;User Id=sa;Password=Wb&210$0919@KiosK(3)welKIN#;";
            else if(project == "210")
                conStr = $"Server=10.10.2.39;Database=WelkinATP;User Id=sa;Password=Wb&210$0919@KiosK(3)welKIN#;";

            string query = $"select ReceiptNO, Amount from Payments where PaymentDate ='{date} 00:00:00.000' and kioskID={kioskId} order by ReceiptNO asc";
            SqlConnection con = new SqlConnection(conStr);

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    totalAmt += reader.GetDecimal(1);
                    if (rcptStart == 0)
                        rcptStart = reader.GetInt32(0);
                    else
                        rcptEnd = reader.GetInt32(0);
                }
            }catch (Exception ex)
            {
                MessageBox.Show($"Find Details of Kiosk Id\nDescription: {ex.Message}", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
            int numberOfRcpt = rcptEnd - rcptStart + 1;
            string output = kioskId +"\t"+rcptStart+"\t"+rcptEnd+"\t"+numberOfRcpt+"\t"+totalAmt;

            return output;
        }
        public DbContext() { }
    }
}
