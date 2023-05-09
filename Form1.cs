using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginData
{
    public partial class Form1 : Form
    {
        string chemin = @"C:\Users\Ragot_Prod\AppData\Local\Google\Chrome\User Data\Profile 2\Login Data";
        string browser = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ComboBox1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                //MessageBox.Show("Chrome");
                label1.Text = "Chrome";
                browser = "Chrome";
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                //MessageBox.Show("Edge");
                label1.Text = "Edge";
                browser = "Edge";
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                //MessageBox.Show("FireFox");
                label1.Text = "FireFox";
                browser = "FireFox";
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                //MessageBox.Show("Opéra");
                label1.Text = "Opéra";
                browser = "Opera";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Historique");
            label1.Text = "Historique";
            MessageBox.Show("Cette rubrique est en maintenance");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Identifiants Admin");
            label1.Text = "Identifiants Admin";
            Remplissage(browser);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Cookies Admin");
            label1.Text = "Cookies Admin";
            MessageBox.Show("Cette rubrique est en maintenance");
        }

        public void Remplissage(string browser)
        {
            MsedgeReader msedgeReader = new MsedgeReader();
            ChromeReader chromeReader = new ChromeReader();
            OperaReader operaReader = new OperaReader();

            IEnumerable<CredentialModel> credentialModels = new List<CredentialModel>();
            
            if(browser.Equals("Chrome") ) 
            {
                dataGridView1.Rows.Clear();
                credentialModels = chromeReader.ReadPasswords();

                foreach (var str in credentialModels)
                {
                    //MessageBox.Show("UserName : "+ str.Username+" et PassWord :  "+ str.Password) ;

                    if (str.Username.Equals("") && str.Password.Equals(""))
                    {
                        //dataGridView1.Rows.Add("Opéra", str.Url, str.Username, str.Password);
                    }
                    else
                    {
                        dataGridView1.Rows.Add("Chrome", str.Url, str.Username, str.Password);
                    }
                }
            }
            else if (browser.Equals("Edge"))
            {
                dataGridView1.Rows.Clear();
                credentialModels = msedgeReader.ReadPasswords();
                
                foreach (var str in credentialModels)
                {
                    //MessageBox.Show("UserName : "+ str.Username+" et PassWord :  "+ str.Password) ;

                    if (str.Username.Equals("") && str.Password.Equals(""))
                    {
                        //dataGridView1.Rows.Add("Opéra", str.Url, str.Username, str.Password);
                    }
                    else
                    {
                        dataGridView1.Rows.Add("Edge", str.Url, str.Username, str.Password);
                    }
                }

            }
            else if (browser.Equals("Opera"))
            {
                dataGridView1.Rows.Clear();
                credentialModels = operaReader.ReadPasswords();
             
                foreach (var str in credentialModels)
                {
                    //MessageBox.Show("UserName : "+ str.Username+" et PassWord :  "+ str.Password) ;

                    if (str.Username.Equals("") && str.Password.Equals(""))
                    {
                        //dataGridView1.Rows.Add("Opéra", str.Url, str.Username, str.Password);
                    }
                    else
                    {
                        dataGridView1.Rows.Add("Opéra", str.Url, str.Username, str.Password);
                    }
                }

            }
            else if (browser.Equals("FireFox"))
            {
                dataGridView1.Rows.Clear();
                MessageBox.Show("Fonction traité sous d'autre cieux");
            }
            else
            {
                MessageBox.Show("Faite un choix de navigateur ");
            }
        }
    }
}
