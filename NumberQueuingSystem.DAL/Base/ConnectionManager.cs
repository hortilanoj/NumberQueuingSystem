
namespace NumberQueuingSystem.DAL.Base
{
    public static class ConnectionManager
    {
        public static string ConnectionString { get; set; }

        //private static MySql.Data.MySqlClient.MySqlConnection _Connection;

        public static MySql.Data.MySqlClient.MySqlConnection Connection
        {
            get
            {
                MySql.Data.MySqlClient.MySqlConnection Conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                Conn.Open();
                return Conn; 
            }
        }

        public static bool Check_Connection()
        {
            try
            {
                MySql.Data.MySqlClient.MySqlConnection Connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                Connection.Open();
                Connection.Close();
                Connection.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Check_Connection(string connection_string)
        {
            try
            {
                MySql.Data.MySqlClient.MySqlConnection Connection = new MySql.Data.MySqlClient.MySqlConnection(connection_string);
                Connection.Open();
                Connection.Close();
                Connection.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
