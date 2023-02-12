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
    //главное окно программы
    public partial class MainForm : Form
    {
        public User current_user; // текущий пользователь
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
            if (current_user.IsPrepod == false)
                tabControl1.TabPages.Remove(tabPage2);
            if (current_user.IsAdmin == false)
                tabControl1.TabPages.Remove(tabPage3);

        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        //обновить список пользователей
        private void Refresh_user_list()
        {

        }
    }
}
