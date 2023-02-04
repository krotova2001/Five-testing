using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace Five_testing
{
    /// <summary>
    /// класс, осуществляющий вход в систему
    /// </summary>
    internal class Login_validater
    {
        /// <summary>
        /// Вход в систему
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pass"></param>
        /// <returns>результат</returns>
       
        internal bool Login(string username, string pass)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyMicrosoftSql"].ConnectionString;
            // Создание подключения
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand($@"SELECT id from Users WHERE username = '{username}' and password = '{pass}'", connection);
                connection.Open();
                SqlDataReader sqldatareader = command.ExecuteReader();
                if (sqldatareader.HasRows)
                    return true;
                return false;
            }
        }
    }
}
