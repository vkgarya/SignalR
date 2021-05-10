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
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;
using WebApplicationSignalR_NEW.Models;
using WebApplicationSignalR_NEW.SignalRHubs;

namespace WebApplicationSignalR_NEW
{
    public partial class _Default : Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["UserConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            var myLog = new List<string>();
            myLog.Add(string.Format("{0} - Logging Started", DateTime.UtcNow));
            LogListView.DataSource = myLog;
            LogListView.DataBind();

            UserGridView.DataSource = GetUserData();
            UserGridView.DataBind();

            //User u = new User()
            //{
            //    Id = 1,
            //    Name = "TestName1",
            //    City = "TestCity1"
            //};
            //InsertUser(u);
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
                using (SqlCommand command = new SqlCommand(@"SELECT [Id] ,[Name] ,[City] FROM [SignalRDb].[dbo].[User_Tbl]", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(OnSqlDependencyChange);

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
        private void OnSqlDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            // HttpContext.Current.Response.Write($"OnChange Event fired. SqlNotificationEventArgs: Info={e.Info}, Source={e.Source}, Type={e.Type}.");
            if ((e.Info != SqlNotificationInfo.Invalid) && (e.Type != SqlNotificationType.Subscribe))
            {
                //resubscribe
                HttpContext.Current.Response.Write($"Data changed.");

                // get change data
                var mapper = new ModelToTableMapper<User>();
                mapper.AddMapping(c => c.Id, "Id");
                mapper.AddMapping(c => c.Name, "Name");
                mapper.AddMapping(c => c.City, "City");
                using (var dep = new SqlTableDependency<User>(connectionString, "User_Tbl", mapper: mapper))
                {
                    dep.OnChanged += Changed;
                    dep.Start();
                    dep.Stop();
                }
            }
            else
            {
                // HttpContext.Current.Response.Write("SqlDependency not restarted");
            }

            UserHub.Show();
        }
        
        public void Changed(object sender, RecordChangedEventArgs<User> e)
        {
            var changedEntity = e.Entity;
            HttpContext.Current.Response.Write("e.ChangeType " + e.ChangeType);
            HttpContext.Current.Response.Write("changedEntity.Id " + changedEntity.Id);
            HttpContext.Current.Response.Write("changedEntity.Name " + changedEntity.Name);
            HttpContext.Current.Response.Write("changedEntity.City " + changedEntity.City);

            InsertUser(changedEntity);

        }

        private void InsertUser(User changedEntity)
        {
            string query = "INSERT INTO dbo.User_ChangeData_Tbl (Id, Name, City) " +
                   "VALUES (@id, @name, @city) ";
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = changedEntity.Id;
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 50).Value = changedEntity.Name;
                cmd.Parameters.Add("@city", SqlDbType.VarChar, 50).Value = changedEntity.City;

                // open connection, execute INSERT, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

            }
        }
    }
}