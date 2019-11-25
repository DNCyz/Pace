using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Pace;
using Pace.Common;

namespace PaceTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                //Engine engine = Engine.Init();
                //object obj= engine.Execute("获取当前时间", new object[] {"阿三"});
                // MessageBox.Show("DN:" + obj.ToString());

                //string txt = this.txtParameters.Text;
                //txt = txt.Replace("\r\n", "¤");
                string[] txtArray = Tool.GetFromTextBox(this.txtParameters);

                string strScript = this.txtScript.Text;
                //string str = "";
                foreach (string str in txtArray)
                {
                    string[] strArrray = str.Split('=');
                    strScript = strScript.Replace("<" + strArrray[0] + ">", strArrray[1]);
                }
                //this.txtScript.Text = strScript;
                //MessageBox.Show(str);
                string[] txtScriptArray = Tool.GetFromString(strScript);

                //string strMsg = "";
                //foreach(string str in txtScriptArray)
                //{
                //    strMsg += (str+"\r\n");
                //}
                //MessageBox.Show(strMsg);
                /*
                
                */
                //string[] s1 = txtScriptArray[0].Split('◎');
                //string[] s2 = txtScriptArray[1].Split('◎');
                //MessageBox.Show(s1.Length.ToString() + " " + s2.Length.ToString());

                string strResult = Engine.Init().Run(txtScriptArray);
                this.lblResult.Text = strResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Err:" + ex.ToString());
            }
        }

        private void lblResult_Click(object sender, EventArgs e)
        {

        }

        private void txtScript_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.txtParameters.Text = this.txtParameters.Text.Replace("2019-09-30", DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch(Exception ex)
            {
                MessageBox.Show("Err:" + ex.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
