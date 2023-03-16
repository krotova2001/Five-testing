using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Five_testing
{
    /// <summary>
    /// Класс для редактирования отельного теста
    /// </summary>
    public partial class Test_editing : Form
    {
        public Test temp_test; //временный тест для редактирования
        public Question temp_question; // временный вопрос для редактиврования
        public Answer temp_answer; // временный ответ для редактирования
        
        public Test_editing()
        {
            InitializeComponent();
            temp_test = new Test();
        }

        public Test_editing(Test t)
        {
            InitializeComponent();
            temp_test = t;
            Refresh_questions();
        }

        //оновить список вопросов
        private void Refresh_questions()
        {
            listBox1.Items.Clear();
            foreach (Question q in temp_test)
                listBox1.Items.Add(q);
        }

        //кнопка отмена
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Cancel;
            this.Close();
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        //выбор вопроса
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                temp_question = (Question)listBox1.SelectedItem;
                richTextBox1.Text = temp_question.text;
                listBox2.Items.Clear();
                foreach (Answer a in temp_question.Answers)
                    listBox2.Items.Add(a);
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

        //удалить вопрос
        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
                listBox2.Items.Remove(listBox2.SelectedItem);
        }

        //добавить вопрос
        private void button6_Click(object sender, EventArgs e)
        {
            Answer answer = new Answer();
            listBox2.Items.Add(answer);
        }

        //выбор ответа
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                temp_answer = listBox2.SelectedItem as Answer;
                richTextBox2.Text = temp_answer.Text;
                if (temp_question.correct_answer_id == temp_answer.Idanswers)
                    checkBox1.Checked = true;
            }
        }

        //галочка - правильный ответ
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            temp_answer.is_correct = checkBox1.Checked;
        }
    }
}
