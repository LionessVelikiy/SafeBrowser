using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp12
{
    public partial class Form2 : Form
    {


        Form1 mainForm;
        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        public string dirTitle = "";

        private void button1_Click(object sender, EventArgs e)
        {
   
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
            this.mainForm.newTab(dataGridView1.SelectedCells[0].Value.ToString());

        }

        private void Form2_Load(object sender, EventArgs e)
        {


            string[] history = User.ReadHistoryUrl();

            foreach(string item in history)
            {
                string[] itemData = item.Split('~');
                int rowId = dataGridView1.Rows.Add();

                dataGridView1.Rows[rowId].Cells[0].Value = itemData[0];
                dataGridView1.Rows[rowId].Cells[1].Value = itemData[1];
            }


        }
    }
}
