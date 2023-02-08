﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
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
            string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
            // Создание подключения
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand($@"SELECT * from users WHERE username = '{username}' and password = '{pass}'", connection);
                connection.Open();
                MySqlDataReader sqldatareader = command.ExecuteReader();
                if (sqldatareader.HasRows)
                {
                    User user = new User();
                    while (sqldatareader.Read())
                    {
                        user.Id = sqldatareader.GetInt32(0);
                        user.Name = sqldatareader.GetString(4);
                        user.Surname = sqldatareader.GetString(5);
                        user.IsStuden = sqldatareader.GetBoolean(7);
                        user.IsPrepod = sqldatareader.GetBoolean(6);
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
