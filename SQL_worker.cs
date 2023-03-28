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
    public  class SQL_worker
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        public  User current_user; // текущий пользователь
        IDbConnection db;

        public SQL_worker(User u)
        {
            current_user = u;
            db = new MySqlConnection(connectionString);
            db.Open();
        }

        //удаление вопроса из БД по ИД
        public void Delete_question(int id)
        {
            string del_ans = $"DELETE from five_test_debug.answers where question_id = {id}";
            string del_from_set = $"DELETE FROM five_test_debug.test_set where id_question = {id}";
            string del_ques = $"DELETE FROM five_test_debug.questions where idquestion = {id}";
            db.Execute(del_ans);
            db.Execute(del_from_set);
            db.Execute(del_ques);
        }
    }
}
