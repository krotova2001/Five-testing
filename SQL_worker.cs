using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper;
using System.Data;

//класс для работы с данными в БД

namespace Five_testing
{
    public static class SQL_worker
    {
        readonly static string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        public static User current_user; // текущий пользователь
        static IDbConnection db;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="u">текущий пользователь</param>
        static SQL_worker()
        {
            db = new MySqlConnection(connectionString);
            db.Open();
        }

        //удаление вопроса из БД по ИД
        public static void Delete_question(int id)
        {
            string del_ans = $"DELETE from five_test_debug.answers where question_id = {id}";
            string del_from_set = $"DELETE FROM five_test_debug.test_set where id_question = {id}";
            string del_ques = $"DELETE FROM five_test_debug.questions where idquestion = {id}";
            db.Execute(del_ans);
            db.Execute(del_from_set);
            db.Execute(del_ques);
        }

        /// <summary>
        /// создание нового теста
        /// </summary>
        /// <returns>ид теста из БАзы данных</returns>
        public static int Create_new_test(Test t) 
        {
            return db.Query<int>($@"INSERT INTO five_test_debug.test (info, author_id, name)
                                    VALUES ('{t.info}', {current_user.Id}, '{t.name}');
                                        SELECT LAST_INSERT_ID();" ).First();
        }
    }
}
