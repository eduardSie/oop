using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppHelloWorld 
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeUI(); 
        }

        private void InitializeUI()
        {
            this.Text = "Hello World App";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label label = new Label();
            label.Text = "Hello World!";
            label.Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold);
            label.AutoSize = true;
            label.Location = new System.Drawing.Point(100, 60);

            this.Controls.Add(label);
        }
    }
}