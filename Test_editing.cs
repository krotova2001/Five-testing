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
        SoundPlayer soundPlayer;
        


        private void Test_editing_Load(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            this.Name = $"Редактирование теста - {temp_test.name}";
            using (IDbConnection db = new MySqlConnection(conn))
            {
                all_themas = db.Query<Thema>("SELECT * FROM five_test_debug.question_theme").ToList();
                foreach (Thema thema in all_themas)
                    listBox3.Items.Add(thema);
            }
        }

        //создание нового
        public Test_editing()
        {
            InitializeComponent();
            temp_test = new Test();
            
        }

        //редактирование существующего
        public Test_editing(Test t)
        {
            InitializeComponent();
            temp_test = t;
            Refresh_questions();
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
            using (IDbConnection db = new MySqlConnection(conn))
            {
                //если это новый тест
                if (temp_question.idquestion == 0)
                {
                    foreach (Question question in temp_test)
                    {
                        foreach (Answer answer in question)
                        {
                            string insert_query = $@"INSERT INTO five_test_debug.answers (text, question_id)
                                                        VALUES ('{answer.Text}', {answer.Question_id}) ";
                            string update_query = $@"UPDATE five_test_debug.answers SET text ='{answer.Text}'
                                                        WHERE idanswers={answer.Idanswers}";
                        }
                    }
                }

                //если не новый
                else
                {
                    foreach(Question question in temp_test)
                    {
                        //что-то надо придумать с удадением ответов из БД
                        foreach (Answer answer in question)
                        {
                            string update_ans = $@"UPDATE five_test_debug.answers SET text='{answer.Text}' WHERE idanswers={answer.Idanswers}";
                            string insert_ans = $@"INSERT INTO five_test_debug.answers (text, question_id)
                                                        VALUES ('{answer.Text}', {answer.Question_id}) ";
                            if (answer.Idanswers == 0) // если это новый добавленный ответ
                                db.Execute(insert_ans);
                            else
                                db.Execute(update_ans); // если изменить существующий ответ
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
            }
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
        }

        //кнопка удалить вопрос
        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)            
                listBox1.Items.Remove(listBox1.SelectedItem);
        }

        //удалить ответ
        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                listBox2.Items.Remove(listBox2.SelectedItem);
                temp_question.Answers.Remove(listBox2.SelectedItem as Answer);
            }
                
        }

        //добавить ответ
        private void button6_Click(object sender, EventArgs e)
        {
            Answer answer = new Answer();
            answer.Text = richTextBox2.Text;
            answer.Question_id = temp_question.idquestion;
            listBox2.Items.Add(answer);
            temp_question.Answers.Add(answer);
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
                temp_question.id_question_theme = temp_question.theme.idtheme; // дублирование функций
            }
        }
    }
}
