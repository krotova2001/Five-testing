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
using Dapper;

namespace Five_testing
{
    /// <summary>
    /// Редактирование групп студентов
    /// </summary>
    public partial class Groups_editing : Form
    {
        Group current_group; // текущая выбранная группа
        List<Group> Groups_all; // ссылка на уже загруженные в класс группы с БД
        string cs; // строка подлючения
        public Groups_editing()
        {
            InitializeComponent();
        }

        public Groups_editing(string conn)
        {
            InitializeComponent();
            cs = conn;
            Refresh_groups();
        }

        private void Refresh_groups()
        {
            using (IDbConnection db = new MySqlConnection(cs))
            {
                Groups_all = db.Query<Group>("SELECT * FROM five_test_debug.groups").ToList();
                listBox1.Items.Clear();
                foreach (Group g in Groups_all)
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
            Group gr = new Group();
            gr.Name = textBox1.Text;
            using (MySqlConnection db = new MySqlConnection(cs))
            {
                db.Open();
                MySqlCommand Command = new MySqlCommand($"INSERT INTO five_test_debug.groups (name) VALUES ('{gr.Name}') ");
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
                int id_to_del = current_group.idgroup;
                MySqlCommand Command = new MySqlCommand($"UPDATE five_test_debug.groups SET name = '{textBox1.Text}' WHERE idgroup = {id_to_del}");
                Command.Connection = db;
                Command.ExecuteNonQuery();
                Refresh_groups();
            }
        }

        //удалить
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogresult = MessageBox.Show("Удалить выбранную группу? Студенты этой группы...", "Удаление группы", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogresult == DialogResult.OK)
            {
                if (current_group != null)
                {
                    int id_to_del = current_group.idgroup;
                    if (id_to_del != 0)
                    {
                        try
                        {
                            using (MySqlConnection db = new MySqlConnection(cs))
                            {
                                db.Open();
                                MySqlCommand Command = new MySqlCommand($"DELETE FROM five_test_debug.groups WHERE idgroup = {id_to_del}");
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
                        MessageBox.Show("Не могу удалить группу");
                }
            }
        }

        //клик на группе
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            current_group = listBox1.SelectedItems[0] as Group;
            textBox1.Text = current_group.Name;
        }
    }
}
