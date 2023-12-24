using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Five_testing
{
    public partial class Theme_editing : Form
    {
        public Theme_editing()
        {
            InitializeComponent();
            Refresh_groups();
        }

        private void Theme_editing_Load(object sender, EventArgs e)
        {

        }
        readonly string cs = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        Thema current_thema; // текущая выбранная тема
        List<Thema> Themas; // ссылка на уже загруженные в класс группы с БД
       
        private void Refresh_groups()
        {
            using (IDbConnection db = new MySqlConnection(cs))
            {
                Themas = db.Query<Thema>("SELECT * FROM five_test_debug.question_theme").ToList();
                listBox1.Items.Clear();
                foreach (Thema g in Themas)
                {
                    listBox1.Items.Add(g);
                }
            }
        }

        //кнопка закрыть
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Добавить
        private void button4_Click(object sender, EventArgs e)
        {
            Thema gr = new Thema();
            gr.Theme = textBox1.Text;
            using (MySqlConnection db = new MySqlConnection(cs))
            {
                db.Open();
                MySqlCommand Command = new MySqlCommand($"INSERT INTO five_test_debug.question_theme (theme) VALUES ('{gr.Theme}') ");
                Command.Connection = db;
                Command.ExecuteNonQuery();
            }
            Refresh_groups();
        }

        //Сохранить
        private void button3_Click(object sender, EventArgs e)
        {
            using (MySqlConnection db = new MySqlConnection(cs))
            {
                db.Open();
                int? id_to_del = current_thema.idtheme;
                MySqlCommand Command = new MySqlCommand($"UPDATE five_test_debug.question_theme SET theme = '{textBox1.Text}' WHERE idtheme = {id_to_del}");
                Command.Connection = db;
                Command.ExecuteNonQuery();
                Refresh_groups();
            }
        }

        //удалить
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogresult = MessageBox.Show("Удалить выбранную тему?", "Удаление темы", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogresult == DialogResult.OK)
            {
                if (current_thema != null)
                {
                    int? id_to_del = current_thema.idtheme;
                    if (id_to_del != 0)
                    {
                        try
                        {
                            using (MySqlConnection db = new MySqlConnection(cs))
                            {
                                db.Open();
                                MySqlCommand Command = new MySqlCommand($"DELETE FROM five_test_debug.question_theme WHERE idtheme = {id_to_del}");
                                Command.Connection = db;
                                Command.ExecuteNonQuery();
                                Refresh_groups();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                        MessageBox.Show("Не могу удалить тему");
                }
            }
        }

        //клик на группе
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            current_thema = listBox1.SelectedItems[0] as Thema;
            textBox1.Text = current_thema.Theme;
        }
    }
}
