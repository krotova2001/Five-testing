﻿using System;
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
    public class SQL_worker
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        public static User current_user; // текущий пользователь
        static IDbConnection db;

        public SQL_worker()
        {
            db = new MySqlConnection(connectionString);
            db.Open();
        }

        public SQL_worker(User u):this()
        {
            current_user = u;
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
        /// <param name="t">Экземпляр теста</param>
        /// <returns>id нового теста</returns>
        public static int Create_new_test(Test t)
        {
            db = new MySqlConnection(connectionString);
            string q = $@"INSERT INTO five_test_debug.test (info, author_id, name) VALUES ('{t.info}', {current_user.Id}, '{t.name}'); SELECT LAST_INSERT_ID();";
            var res = db.Query<int>(q).FirstOrDefault();
            return res;
        }

        /// <summary>
        /// обновление теста существующего в бд
        /// </summary>
        /// <param name="t">Экземпляр теста</param>
        public static void Update_test(Test t)
        {
            string query = $@"UPDATE five_test_debug.test SET 
                            info = '{t.info}', name = {t.name}' WHERE idtest = {t.idtest})"; 
            db.Execute(query);
        }

        /// <summary>
        /// Удаление теста из бд
        /// </summary>
        /// <param name="t">Экземпляр теста</param>
        public static void Delete_test(Test t)
        {
            db = new MySqlConnection(connectionString);
            string q = $"DELETE FROM five_test_debug.test WHERE idtest = {t.idtest}";
            db.Execute(q);
        }


        /// <summary>
        /// Получить список всех вопросов
        /// </summary>
        public static List<Question> Get_all_questions()
        {
            db = new MySqlConnection(connectionString);
            string query = "SELECT * FROM five_test_debug.questions;";
            List<Question> q = db.Query<Question>(query).ToList();
            return q;
        }

        /// <summary>
        /// Добавление нового вопроса
        /// </summary>
        /// <param name="question">Сам вопрос</param>
        /// <returns>id вопроса</returns>
        public static int Create_new_question(Question q)
        {
            db = new MySqlConnection(connectionString);
            string query = $@"insert into five_test_debug.questions (text, level, id_question_theme, author_id) values('{q.text}', {q.level}, {q.theme.idtheme}, { current_user.Id}); SELECT LAST_INSERT_ID();";
            int res = db.Query<int>(query).FirstOrDefault();
            return res;
        }

        

    }
}
