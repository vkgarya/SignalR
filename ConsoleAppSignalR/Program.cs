using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSignalR
{
    // https://stackoverflow.com/questions/18143599/can-signalr-be-used-with-asp-net-webforms
    // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/detecting-changes-with-sqldependency
    // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/enabling-query-notifications
    class Program
    {
        static string connectionString = @"Data Source=VKRISHNA-T470P;Initial Catalog=SignalRDb;Integrated Security=True;";
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Page_Load();
            Console.ReadKey();
        }

        public void Page_Load()
        {
            SqlDependency.Start(connectionString);
            GetDataWithSqlDependency();
            Console.WriteLine("Waiting for Data Changes");
            // Console.WriteLine("Waiting for data changes");
            //Console.WriteLine("Press enter to quit");
            // Console.WriteLine(CanRequestNotifications().ToString());
            //Console.ReadLine();
            SqlDependency.Stop(connectionString);
        }
        static DataTable GetDataWithSqlDependency()
        {
            //Console.WriteLine("GetDataWithSqlDependency");
            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT [Id] ,[Name] ,[City] FROM [SignalRDb].[dbo].[User_Tbl]", connection))
            {
                var dt = new DataTable();
                // Create dependency for this command and add event handler
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(OnSqlDependencyChange);
                // execute command to get data
                connection.Open();
                dt.Load(cmd.ExecuteReader(CommandBehavior.CloseConnection));
                Console.WriteLine(dt.Rows.Count.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.WriteLine($"{dt.Rows[i]["Id"].ToString()} - {dt.Rows[i]["Name"].ToString()} - {dt.Rows[i]["City"].ToString()}");
                }

                return dt;
            }

        }
        static void OnSqlDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            Console.WriteLine($"OnChange Event fired. SqlNotificationEventArgs: Info={e.Info}, Source={e.Source}, Type={e.Type}.");
            if ((e.Info != SqlNotificationInfo.Invalid) && (e.Type != SqlNotificationType.Subscribe))
            {
                //resubscribe
                var dt = GetDataWithSqlDependency();
                Console.WriteLine($"Data changed. {dt.Rows.Count} rows returned.");
            }
            else
            {
                Console.WriteLine("SqlDependency not restarted");
            }


        }
    }
}
