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
        string connectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        public User current_user; // текущий пользователь
        List <User> all_users; // все пользователи системы (заполняется только если Админ запросил)
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

        //обновить список пользователей
        private void Refresh_user_list()
        {
            //пользуем Dapper
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                all_users = db.Query<User>("SELECT * FROM users").ToList();
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
                        using (IDbConnection db = new MySqlConnection(connectionString))
                        {
                            db.Open();
                            MySqlCommand Command = new MySqlCommand($"DELETE FROM users WHERE id = {id_to_del}");
                            DialogResult result = MessageBox.Show("Внимание! Отменить это действие будет невозможно!", "Удаление Пользователя", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (result == DialogResult.OK)
                                Command.BeginExecuteNonQuery();
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

        }

        //кнопка Новый пользователь
        private void button3_Click(object sender, EventArgs e)
        {

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

        #endregion

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
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
                    textBox1.Text = temp.username;
                    textBox2.Text = temp.password;
                }
            }
        }
    }
}
