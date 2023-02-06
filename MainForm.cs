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
            
            //сокрытие лишних вкладок
            if (current_user.IsPrepod == false)
                tabControl1.TabPages.Remove(tabPage2);
            if (current_user.IsAdmin == false)
                tabControl1.TabPages.Remove(tabPage3);

        }
    }
}
