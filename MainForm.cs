using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper;
using System.Drawing.Text;

namespace Five_testing
{
    //главное окно программы
    public partial class MainForm : Form
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        public User current_user; // текущий пользователь
        List <User> all_users; // все пользователи системы (заполняется только если Админ запросил)
        List <Group> Groups; // все группы студентов
        List<Test> all_tests; // пакет всех тестов
        public Test current_test;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //загружаем авторизацию
            Login login = new Login();
            if (login.ShowDialog() == DialogResult.Cancel)
                this.Close();
            current_user = login.Fill_user(); // передаем текущего пользователя
            
            //сокрытие лишних вкладок в зависимости от роли пользователя
            if (current_user.is_student == true)
            {
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Remove(tabPage3);
            }
            if (current_user.is_prepod == true)
                tabControl1.TabPages.Remove(tabPage3);
        }

        //выбор вкладки
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2) // вкладка Администрирование
                Refresh_user_list();
            if (tabControl1.SelectedIndex == 1) // вкладка Редактирование тестов
                Refresh_test_list();
            if (tabControl1.SelectedIndex == 0) // вкладка Тестирование
            {

            }
        }

        #region Вкладка Редактирование тестов
        /// <summary>
        /// обновление списка тестов
        /// </summary>
        private void Refresh_test_list()
        {
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                listBox1.Items.Clear();
                all_tests = db.Query<Test>("SELECT * FROM five_test_debug.test join info on test.idtest = info.test_id;").ToList();
                foreach (Test test in all_tests)
                {
                    listBox1.Items.Add(test);
                    test.questions = db.Query<Question>($@"SELECT * FROM test_set join questions on test_set.id_question = questions.idquestion 
                                                        join question_theme on question_theme.idtheme = questions.id_question_theme
                                                        WHERE test_set.idtest = {test.idtest}").ToList();
                    foreach (Question question in test.questions)
                    {
                        question.Answers = db.Query<Answer>($"SELECT * FROM five_test_debug.answers WHERE question_id = {question.idquestion};").ToList();
                    }
                }
            }
        }

        //выбор теста в редактировании
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            current_test = null;
            if (listBox1.SelectedItems != null)
            {
                current_test = listBox1.SelectedItem as Test;
                textBox7.Text = current_test.info;
                textBox8.Text = current_test.text;
                textBox9.Text = current_test.date.ToShortDateString();
                listBox2.Items.Clear();
                foreach (Question question in current_test.questions)
                {
                    listBox2.Items.Add(question);
                }
            }
        }

        //выбор вопроса у выбранного теста
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_test != null && listBox2.SelectedItem != null)
            {
                Question q = listBox2.SelectedItem as Question;
                textBox11.Text = q.text;
                richTextBox1.Clear();
                foreach (Answer answer in q.Answers)
                {
                    if (q.correct_answer_id == answer.Idanswers)
                    {
                        //жирный текст не работает...
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                        richTextBox1.Text += ($"{answer.Text} - correct\n");
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                    }
                    else
                    {
                        richTextBox1.Text += $"{answer.Text}\n";
                    }
                }
            }
        }

        //удалить тест
        private void button7_Click(object sender, EventArgs e)
        {

        }

        //экспорт теста
        private void button8_Click(object sender, EventArgs e)
        {

        }

        //создать новый тест
        private void button6_Click(object sender, EventArgs e)
        {
            Test t = new Test(current_user);
            Test_editing test_Editing = new Test_editing(t);
            test_Editing.ShowDialog();
        }

        //редактировать тест
        private void button5_Click(object sender, EventArgs e)
        {
            if (current_test != null)
            {
                Test_editing test_Editing = new Test_editing(current_test);
                test_Editing.ShowDialog();
            }
        }

        #endregion


        #region Вкладка Администрирование

        //обновить список пользователей, групп
        private void Refresh_user_list()
        {
            //пользуем Dapper
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                all_users = db.Query<User>("SELECT * FROM users").ToList();
                Groups = db.Query<Group>("SELECT * FROM five_test_debug.groups").ToList();
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add("Ученики");
                treeView1.Nodes.Add("Преподаватели");
                treeView1.Nodes.Add("Администраторы");
                foreach (User u in all_users)
                {
                    if (u.is_student == true)
                        treeView1.Nodes[0].Nodes.Add(new TreeNode(u.ToString()));
                    if (u.is_prepod == true)
                        treeView1.Nodes[1].Nodes.Add(new TreeNode(u.ToString()));
                    if (u.is_admin == true)
                        treeView1.Nodes[2].Nodes.Add(new TreeNode(u.ToString()));
                }
                
                comboBox2.Items.Clear();
                foreach (Group g in Groups)
                {
                    comboBox2.Items.Add(g.ToString());
                }
                Clear_Fields_amd();
            }
        }

       

        //очистить поля в админке
        private void Clear_Fields_amd()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }

        //кнопка Удалить пользователя
        private void button4_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode!=null)
            {
                int id_to_del=0;
                foreach (User u in all_users)
                    if (u.ToString() == treeView1.SelectedNode.Text)
                        id_to_del = u.Id;
                if (id_to_del != 0)
                {
                    try
                    {
                        using (MySqlConnection db = new MySqlConnection(connectionString))
                        {
                            db.Open();
                            MySqlCommand Command = new MySqlCommand($"DELETE FROM users WHERE id = {id_to_del}");
                            Command.Connection = db;
                            DialogResult result = MessageBox.Show("Внимание! Отменить это действие будет невозможно!", "Удаление Пользователя", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (result == DialogResult.OK)
                                Command.ExecuteNonQuery();
                            Refresh_user_list();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                    MessageBox.Show("Не могу удалить пользователя");
            }
        }

        //кнопка сохранить изменения
        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode !=null)
            {
                {
                    int id_to_del = 0;
                    bool admin = false;
                    bool prepod = false;
                    bool student = true;
                    int group_id=0;
                    foreach (User u in all_users)
                        if (u.ToString() == treeView1.SelectedNode.Text)
                        {
                            id_to_del = u.Id;
                        }
                    if (comboBox1.SelectedIndex == 0)
                    {
                        student = true;
                        prepod = false;
                        admin = false;
                    }
                    if (comboBox1.SelectedIndex == 1)
                    {
                        prepod = true;
                        student = false;
                        admin=false;
                    }
                    if (comboBox1.SelectedIndex == 2)
                    {
                        admin = true;
                        student=false;
                        prepod=false;
                    }
                    foreach (Group g in Groups)
                    {
                        if (g.Name == comboBox2.Text)
                            group_id = g.idgroup;
                    }
                    if (id_to_del != 0)
                    {
                        try
                        {
                            using (MySqlConnection db = new MySqlConnection(connectionString))
                            {
                                db.Open();
                                MySqlCommand Command = new MySqlCommand($"UPDATE five_test_debug.users SET name = '{textBox5.Text}', surname = '{textBox6.Text}', username = '{textBox1.Text}', password='{textBox2.Text}', email='{textBox4.Text}', telephone = '{textBox3.Text}', age = {Convert.ToInt32(numericUpDown1.Value)}, is_prepod={prepod}, is_student={student}, is_admin={admin}, group_id={group_id} WHERE id = {id_to_del}");
                                Command.Connection = db;
                                Command.ExecuteNonQuery();
                                Refresh_user_list();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                        MessageBox.Show("Не могу внести изменения");
                }
            }
        }

        //кнопка Новый пользователь
        private void button3_Click(object sender, EventArgs e)
        {
            User u = new User();
            comboBox1.Enabled = true;
            u.username = textBox1.Text;
            u.password = textBox2.Text; 
            u.email = textBox4.Text;
            u.phone = textBox3.Text;
            u.age = Convert.ToInt32(numericUpDown1.Value);
            u.name = textBox5.Text;
            u.surname = textBox6.Text;
            if (comboBox1.SelectedIndex == 0)
            {
                u.is_student = true;
                u.is_prepod = false;
                u.is_admin = false;
            }
               
            if (comboBox1.SelectedIndex == 1)
            {
                u.is_prepod = true;
                u.is_admin = false;
                u.is_student = false;
            }
                
            if (comboBox1.SelectedIndex == 2)
            {
                u.is_admin = true;
                u.is_student = false;
                u.is_prepod = false;
            }
           
            foreach (Group g in Groups)
            {
                if (g.Name == comboBox2.Text)
                u.group_id = g.idgroup;
            }
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0 && textBox5.Text.Length > 0 && textBox6.Text.Length > 0)
            {
                using(MySqlConnection db = new MySqlConnection(connectionString))
                {
                    db.Open();
                    MySqlCommand Command = new MySqlCommand($"INSERT INTO five_test_debug.users (username, password, email, name, surname, is_prepod, is_student, is_admin, age, telephone, group_id) VALUES ('{u.username}', '{u.password}', '{u.email}', '{u.name}', '{u.surname}', {u.is_prepod}, {u.is_student}, {u.is_admin}, {u.age}, '{u.phone}', {u.group_id})");
                    Command.Connection = db;
                    Command.ExecuteNonQuery();
                    Refresh_user_list();
                }
            }
            else
                MessageBox.Show("Не все обязательные поля заполнены");
        }

        //выбор пользователя в дереве
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            User temp = null;
            if (treeView1.SelectedNode.Nodes.Count == 0)
            {
                foreach (User u in all_users)
                    if (u.ToString() == treeView1.SelectedNode.Text)
                        temp = u;
                if (temp != null)
                {
                    textBox1.Text = temp.username;
                    textBox2.Text = temp.password;
                    textBox3.Text = temp.phone;
                    textBox4.Text = temp.email;
                    textBox5.Text = temp.name;
                    textBox6.Text = temp.surname;
                    if (temp.age > 0)
                    {
                        numericUpDown1.Enabled = true;
                        numericUpDown1.Value = Convert.ToDecimal(temp.age);
                    }
                    if (temp.group_id != 0)
                    {
                        foreach (Group group in Groups)
                        {
                            if (group.idgroup == temp.group_id)
                                temp.group = group;
                        }
                        comboBox2.Enabled = true;
                        comboBox2.SelectedItem = temp.group.Name;
                    }
                    if (temp.is_student == true)
                        comboBox1.SelectedIndex = 0;
                    if (temp.is_prepod == true)
                        comboBox1.SelectedIndex = 1;
                    if (temp.is_admin == true)
                        comboBox1.SelectedIndex = 2;
                }
            }
            else
                Clear_Fields_amd();
        }
        

        //показать пароль в текстбоксе
        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '\0';
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }

        //редактирование групп
        private void button1_Click(object sender, EventArgs e)
        {
            Groups_editing groups_Editing = new Groups_editing(connectionString);
            groups_Editing.ShowDialog();
            Refresh_user_list();
        }


        #endregion

        //пробуем настройки графики
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics formGraphics = e.Graphics;
            formGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            formGraphics.TextContrast = 0;
        }

        //двойной щелчок на выбранном тесте
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button5_Click(sender, e);
        }
    }
} 
