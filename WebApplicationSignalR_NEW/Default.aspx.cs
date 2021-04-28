using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationSignalR_NEW.Models;

namespace WebApplicationSignalR_NEW
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var myLog = new List<string>();
            myLog.Add(string.Format("{0} - Logging Started", DateTime.UtcNow));

            logListView.DataSource = myLog;
            logListView.DataBind();

            GetUserData();

        }

        private List<User> GetUserData()
        {
            List<User> users = new List<User>();

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [Id] ,[Name] ,[City] FROM [SignalRDb].[dbo].[UserTbl]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var user = new User()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            City = Convert.ToString(reader["City"]),
                        };
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            CustomerHub.Show();
        }
    }
}