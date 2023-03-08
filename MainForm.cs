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

namespace Five_testing
{
    //главное окно программы
    public partial class MainForm : Form
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        public User current_user; // текущий пользователь
        List <User> all_users; // все пользователи системы (заполняется только если Админ запросил)
        List <Group> Groups; // все группы студентов
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
            }
        }

        //выбор вкладки
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2) // вкладка Администрирование
                Refresh_user_list();
        }

        //кнопка Удалить пользователя
        private void button4_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
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
                u.is_student = true;
            if (comboBox1.SelectedIndex == 1)
                u.is_prepod = true;
            if (comboBox1.SelectedIndex == 2)
                u.is_admin = true;
           
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
           
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

       

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            //выбор пользователя в дереве
            User temp = null;
            if (treeView1.SelectedNode != null)
            {
                foreach (User u in all_users)
                    if (u.ToString() == treeView1.SelectedNode.Text)
                        temp = u;
                if (temp != null)
                {
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    textBox6.Enabled = true;
                    comboBox1.Enabled = true;
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
                    if (temp.group_id !=0)
                    {
                        foreach (Group group in Groups)
                        {
                            if (group.idgroup == temp.group_id)
                                temp.group = group;
                        }
                        comboBox2.Enabled = true;
                        comboBox2.SelectedItem = temp.group.Name;
                    }
                    if(temp.is_student==true)
                        comboBox1.SelectedIndex = 0;
                    if (temp.is_prepod == true)
                        comboBox1.SelectedIndex = 1;
                    if (temp.is_admin == true)
                        comboBox1.SelectedIndex = 2;
                }
            }
        }
       
        //редактирование групп
        private void button1_Click(object sender, EventArgs e)
        {
            Groups_editing groups_Editing = new Groups_editing(connectionString);
            groups_Editing.ShowDialog();
            Refresh_user_list();
        }


        #endregion
    }
} 
