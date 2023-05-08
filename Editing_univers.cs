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
    /// Универсальная форма для редактирования различных небольших табличек
    /// </summary>
    public partial class Editing_univers : Form
    {
        public Editing_univers(string query)
        {
            InitializeComponent();
            if (query == "questions")
                dataGridView1.DataSource = SQL_worker.Get_all_questions();
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
        }

        //Отмена
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Update();
            this.Close();
        }
    }
}
