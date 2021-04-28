using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationSignalR_NEW.Models;
using WebApplicationSignalR_NEW.SignalRHubs;

namespace WebApplicationSignalR_NEW
{
    public partial class _Default : Page
    {
        //private DataSet myDataSet = null;
        //private SqlConnection connection;
        //private SqlCommand command = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            //EnoughPermission();
            //string ssql = "SELECT [Id] ,[Name] ,[City] FROM [SignalRDb].[dbo].[UserTbl]";

            //SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString.ToString());
            //if (command == null)
            //    command = new SqlCommand(ssql, connection);

            //if (myDataSet == null)
            //    myDataSet = new DataSet();
            //GetAdvtData();

            var myLog = new List<string>();
            myLog.Add(string.Format("{0} - Logging Started", DateTime.UtcNow));
            LogListView.DataSource = myLog;
            LogListView.DataBind();

            UserGridView.DataSource = GetUserData();
            UserGridView.DataBind();

        }

        // Vijay::Step - 8
        [WebMethod]
        public static List<User> GetUserDataWebMethod()
        {
            _Default defaultObj = new _Default();
            return defaultObj.GetUserData();
        }

        // Vijay::Step - 9
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

        //Vijay::Step - 9
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            HttpContext.Current.Response.Write($"OnChange Event fired. SqlNotificationEventArgs: Info={e.Info}, Source={e.Source}, Type={e.Type}.");
            if ((e.Info != SqlNotificationInfo.Invalid) && (e.Type != SqlNotificationType.Subscribe))
            {
                //resubscribe
                //var dt = getDataWithSqlDependency();
                //HttpContext.Current.Response.Write($"Data changed. {dt.Rows.Count} rows returned.");
                HttpContext.Current.Response.Write($"Data changed.");
            }
            else
            {
                HttpContext.Current.Response.Write("SqlDependency not restarted");
            }

            UserHub.Show();
        }

        //private bool EnoughPermission()
        //{
        //    SqlClientPermission perm = new SqlClientPermission(PermissionState.Unrestricted);
        //    try
        //    {
        //        perm.Demand();
        //        return true;
        //    }
        //    catch (System.Exception)
        //    {
        //        return false;
        //    }
        //}

        //SqlDependency dependency;
        //private void GetAdvtData()
        //{
        //    myDataSet.Clear();
        //    command.Notification = null;
        //    dependency = new SqlDependency(command);
        //    //Label1.Text = System.DateTime.Now.ToString();
        //    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //    {
        //        adapter.Fill(myDataSet, "dbo.UserTbl");
        //        UserGridView.DataSource = myDataSet;
        //        UserGridView.DataMember = "dbo.UserTbl";
        //        UserGridView.DataBind();
        //    }
        //}

        //private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        //{
        //    try
        //    {
        //        RefreshData();
        //       // UpdatePanel1.Update();
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    SqlDependency dependency = (SqlDependency)sender;
        //    dependency.OnChange -= dependency_OnChange;
        //}

        //private void RefreshData()
        //{
        //    //Label1.Text = "Database had some changes and are applied in the Grid";
        //    GetAdvtData();
        //}
    }
}