using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp12
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string CHILD_ID = textBox1.Text;
            //ЗАПРОС НА СУЩЕСТВОВАНИЕ РЕБЕНКА
            WebClient server = new WebClient();
            if (server.DownloadString("http://test.tasty-catalog.ru/users/checkChildren/?number="+CHILD_ID) == "True")
            {
                File.WriteAllText("CHILD_ID.data",CHILD_ID);
                this.Hide();
            }
            else
            {
                //ВЫВЕСТИ ОШИБКУ
                MessageBox.Show("Номер не обнаружен");
          


            }
        }
    }
}
