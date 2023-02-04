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
        /// <returns>Новый объект User</returns>
        internal User Login(string username, string pass)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyMicrosoftSql"].ConnectionString;
            // Создание подключения
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand($@"SELECT * from Users WHERE username = '{username}' and password = '{pass}'", connection);
                connection.Open();
                SqlDataReader sqldatareader = command.ExecuteReader();
                if (sqldatareader.HasRows)
                {
                    User user = new User();
                    while (sqldatareader.Read())
                    {
                        user.Id = sqldatareader.GetInt32(0);
                        user.Name = sqldatareader.GetString(3);
                        user.Surname = sqldatareader.GetString(4);
                        user.IsStuden = sqldatareader.GetBoolean(6);
                        user.IsPrepod = sqldatareader.GetBoolean(7);
                        user.IsAdmin = sqldatareader.GetBoolean(8);
                    }
                    sqldatareader.Close();
                    return user;
                }
                else
                    return null;
            }
        }
    }
}
