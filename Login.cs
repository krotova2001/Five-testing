﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Five_testing
{
    /// <summary>
    /// форма для авторизации
    /// </summary>
    public partial class Login : Form
    {
        User cur_user; // временное хранение текущего пользователя, чтоб потом передать главной форме
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// передача авторизированного пользователя
        /// </summary>
        /// <returns></returns>
        internal User Fill_user()
        {
            return cur_user;
        }

        //красивая линия
        private void Login_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush b = new LinearGradientBrush(new Rectangle(00, 150, 500, 150), Color.DarkRed, Color.White, LinearGradientMode.Horizontal);
            Pen p = new Pen(b, 4);
            g.DrawLine(p, new Point(40, 140), new Point(500, 140));
        }

        //кнопка Отмена
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //кнопка войти
        private void button1_Click(object sender, EventArgs e)
        {
            Login_validater  L = new Login_validater();
            cur_user = L.Login(textBox1.Text, textBox2.Text);
            if (cur_user != null&&textBox1.Text.Length>0&&textBox2.Text.Length>0)
                this.DialogResult = DialogResult.OK;
            else
                label4.Text = "Неверный логин или пароль";
        }
    }
}
