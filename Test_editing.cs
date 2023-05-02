using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using NAudio;
using NAudio.Wave;
using NAudio.Lame;
using System.Threading;

namespace Five_testing
{
    /// <summary>
    /// Класс для редактирования или создания нового отдельного теста 
    /// </summary>
    public partial class Test_editing : Form
    {
        readonly string conn = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        public Test temp_test; //временный тест для редактирования
        public List<Thema> all_themas; // все темы
        public Question temp_question; // временный вопрос для редактиврования
        public Answer temp_answer; // временный ответ для редактирования
        public User user;
        SoundPlayer soundPlayer;
        IDbConnection db;
        SQL_worker worker;

        private void Test_editing_Load(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            this.Name = $"Редактирование теста - {temp_test.name}";
            db = new MySqlConnection(conn);
            db.Open();
            all_themas = db.Query<Thema>("SELECT * FROM five_test_debug.question_theme").ToList();
            foreach (Thema thema in all_themas)
                listBox3.Items.Add(thema);
            worker = new SQL_worker(user);
            if (temp_test.idtest!=0)
            {
                textBox1.Text = temp_test.name;
                richTextBox3.Text = temp_test.info;
            }
        }

        //создание нового
        public Test_editing(User u)
        {
            InitializeComponent();
            temp_test = new Test();
            user = u;
        }

        //редактирование существующего
        public Test_editing(Test t, User u)
        {
            InitializeComponent();
            temp_test = t;
            Refresh_questions();
            user = u;
        }

        //обновить список вопросов
        private void Refresh_questions()
        {
            listBox1.Items.Clear();
            foreach (Question q in temp_test)
            {
                listBox1.Items.Add(q);
                foreach (Answer answer in q)
                {
                    if (q.correct_answer_id != null && q.correct_answer_id==answer.Idanswers)
                    {
                        answer.is_correct=true;
                    }
                }
            }
        }

        //кнопка отмена
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //кнопка сохранить. Вот тут главная жесть...
        private void button1_Click(object sender, EventArgs e)
        {
            if(temp_test.idtest==0) // если это новый тест
                temp_test.idtest = SQL_worker.Create_new_test(temp_test); // получение id свежему тесту
            foreach (Question question in temp_test)
            {
                if (question.idquestion == 0) //если это новый вопрос
                {
                    question.idquestion = db.Query<int>($@"insert into five_test_debug.questions
                                                    (text, level, id_question_theme, author_id) values 
                                                    ('{question.text}', {question.level}, {question.theme.idtheme}, {user.Id});
                                                        SELECT LAST_INSERT_ID();").First();
                    foreach (Answer answer in question)
                    {
                        string insert_query = $@"INSERT INTO five_test_debug.answers (text, question_id)
                                                    VALUES ('{answer.Text}', {question.idquestion}) ";
                        if (answer.is_correct)
                        {
                            question.correct_answer_id = db.Query<int>(insert_query + "; SELECT LAST_INSERT_ID();").FirstOrDefault();
                            db.Execute($"UPDATE five_test_debug.questions SET correct_answer_id = {question.correct_answer_id} WHERE idquestion = {question.idquestion}");
                        }
                        else
                            db.Execute(insert_query);
                    }
                    db.Execute($"INSERT INTO five_test_debug.test_set (idtest, id_question) VALUES ({temp_test.idtest}, {question.idquestion})");
                }
                else
                {
                    //если вопрос уже существует
                    foreach (Answer answer in question)
                    {
                        string update_ans = $@"UPDATE five_test_debug.answers SET text='{answer.Text}' WHERE idanswers={answer.Idanswers}";
                        string insert_ans = $@"INSERT INTO five_test_debug.answers (text, question_id)
                                                    VALUES ('{answer.Text}', {answer.Question_id}) ";
                        if (answer.Idanswers == 0) // если это новый добавленный ответ
                            answer.Idanswers= db.Query<int>(insert_ans + "; SELECT LAST_INSERT_ID();").First();
                        else // если существующий
                            db.Execute(update_ans);
                    }
                    string update_quest = $@"UPDATE five_test_debug.questions SET 
                                            text='{question.text}', 
                                            level={question.level},                                                    
                                            correct_answer_id = {question.correct_answer_id},
                                            id_question_theme = {question.id_question_theme}
                                            WHERE idquestion={question.idquestion}";
                    db.Execute(update_quest);
                }
            }
            Refresh_questions();
        }

        //выбор вопроса
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                temp_question = (Question)listBox1.SelectedItem;
                richTextBox1.Text = temp_question.text;
                listBox2.Items.Clear();
                richTextBox2.Clear();
                numericUpDown1.Value = temp_question.level;
                foreach (Answer a in temp_question.Answers)
                    listBox2.Items.Add(a); 
                if(temp_question.audio_file != null)
                {
                    button12.Enabled = true;
                    button13.Enabled = true;
                }
                foreach (Thema thema in all_themas)
                {
                    if (thema.idtheme == temp_question.id_question_theme)
                        listBox3.SelectedItem = thema;
                }
            }
        }

        //кнопка добавить вопрос
        private void button3_Click(object sender, EventArgs e)
        {
            Question question = new Question();
            listBox1.Items.Add(question);
            temp_test.questions.Add(question);
            temp_question = question;
        }

        //кнопка удалить вопрос
        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
                temp_test.questions.Remove(temp_question);
                if (temp_question.idquestion != 0)
                    SQL_worker.Delete_question(temp_question.idquestion);
            }
            Refresh_questions();
        }

        //удалить ответ
        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                listBox2.Items.Remove(listBox2.SelectedItem);
                temp_question.Answers.Remove(listBox2.SelectedItem as Answer);
                if (temp_answer.Idanswers != 0) // если этот ответ взят с бд
                    db.Query($"DELETE from five_test_debug.answers WHERE idanswers = {temp_answer.Idanswers} ");
            }
        }

        //добавить ответ
        private void button6_Click(object sender, EventArgs e)
        {
            if (richTextBox2.Text.Length>0)
            {
                Answer answer = new Answer();
                answer.Text = richTextBox2.Text;
                answer.Question_id = temp_question.idquestion;
                listBox2.Items.Add(answer);
                temp_question.Answers.Add(answer);
            }
            else
            {
                label6.Enabled = true;
                Thread.Sleep(1000);
                label6.Enabled = false;
            }
           
        }

        //выбор ответа
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                temp_answer = listBox2.SelectedItem as Answer;
                richTextBox2.Text = temp_answer.Text;
                checkBox1.Checked = temp_answer.is_correct;
            }
        }

        //галочка - правильный ответ
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            temp_answer.is_correct = checkBox1.Checked;
            if (temp_answer.is_correct)
                temp_question.correct_answer_id = temp_answer.Idanswers;
            else
                temp_question.correct_answer_id = null;
        }

        //изменение текста ответа
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                temp_answer.Text = richTextBox2.Text;
            }
        }
        
        //установка уровня вопроса
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            temp_question.level = Convert.ToInt32(numericUpDown1.Value);
        }

        //добавить аудио
        private void button8_Click(object sender, EventArgs e)
        {
            if (temp_question != null)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    temp_question.audio_file = File.ReadAllBytes(openFileDialog1.FileName);
                }
                if (temp_question.audio_file != null)
                {
                    button12.Enabled = true;
                    button13.Enabled = true;
                }
                label5.Text = temp_question.audio_file.Length.ToString();   
            }
        }

        //Play
        private void button12_Click(object sender, EventArgs e)
        {
            //загрузка аудио
            if (temp_question.audio_file != null)
            {
                soundPlayer = new SoundPlayer();
                soundPlayer.Stream = new MemoryStream(temp_question.audio_file);
                soundPlayer.LoadAsync();
            }
            if (temp_question != null)
            {
                soundPlayer.Play();
            }
        }

        //Stop
        private void button13_Click(object sender, EventArgs e)
        {
            soundPlayer.Stop();
        }

        //добавить видео
        private void button10_Click(object sender, EventArgs e)
        {

        }

        //изменить темы
        private void button11_Click(object sender, EventArgs e)
        {
            Theme_editing theme_Editing = new Theme_editing();
            theme_Editing.ShowDialog();
        }

        //удалить аудио
        private void button14_Click(object sender, EventArgs e)
        {
            soundPlayer.Stop();
            temp_question.audio_file = null;
            label5.Text = "";
            button12.Enabled = false;
            button13.Enabled = false;
        }

        //изменение текста вопроса
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (temp_question != null)
                temp_question.text = richTextBox1.Text;
        }

        //выбор тематики
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (temp_question != null)
            {
                temp_question.theme = listBox3.SelectedItem as Thema;
                temp_question.id_question_theme = temp_question.theme.idtheme; // дублирование функций?
            }
        }

        private void Test_editing_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        //название теста
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            temp_test.name = textBox1.Text;
        }

        //информация о тесте
        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            temp_test.info = richTextBox3.Text;
        }
    }
}
