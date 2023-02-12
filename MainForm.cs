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

        //кнопка Обновить
        private void button1_Click(object sender, EventArgs e)
        {
            Refresh_user_list();
        }

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
                        treeView1.Nodes[0].Nodes.Add(new TreeNode (u.ToString()));
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
            if (tabControl1.SelectedIndex == 2)
                Refresh_user_list();
        }
    }
}
