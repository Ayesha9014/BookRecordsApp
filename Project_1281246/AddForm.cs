using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_1281246
{
    public partial class AddForm : Form
    {
        List<AddDetails> details = new List<AddDetails>();
        string currentFile = "";
        public AddForm()
        {
            InitializeComponent();
        }
        public MainForm TheForm {  get; set; }

        private void AddForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            details.Add(new AddDetails
            {
                ChapterNo = textBox3.Text,
                Topic = textBox2.Text,
                TotalPages = (int)numericUpDown2.Value
            });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex== 3)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1 .DataSource = null;
                dataGridView1.DataSource = details;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using(SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using(SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = @"..\..\Pictures\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes = ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                        cmd.CommandText = "INSERT INTO Books(Title, Price, PublishDate, IsEBookAvailable, Picture) VALUES (@t, @p, @pd, @eb, @pic);SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.AddWithValue("@t", textBox1.Text);
                        cmd.Parameters.AddWithValue("@p", numericUpDown1.Value);
                        cmd.Parameters.AddWithValue("@pd", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@eb", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@pic", f);
                        try
                        {
                            var bid = cmd.ExecuteScalar();
                            foreach(var toc in details)
                            {
                                cmd.CommandText = "INSERT INTO TableOfContents (ChapterNo, Topic, TotalPages, BookId) VALUES (@cn, @tc, @tp, @i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@cn", toc.ChapterNo);
                                cmd.Parameters.AddWithValue("@tc", toc.Topic);
                                cmd.Parameters.AddWithValue("tp", toc.TotalPages);
                                cmd.Parameters.AddWithValue("@i", bid);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();

                            TheForm.LoadBindingSource();
                            MessageBox.Show("Data Saved", "Success");
                            details.Clear();
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = details;
                            textBox1.Clear();
                            numericUpDown1.Value = 0;
                            dateTimePicker1.Value = DateTime.Now;
                            checkBox1.Checked = false;
                            pictureBox1.Image = Image.FromFile(@"..\..\Pictures\upload-picture.jpg");
                            textBox2.Clear();
                            textBox3.Clear();
                            numericUpDown2.Value = 0;
                        }
                        catch
                        {
                            trx.Rollback();
                        }
                    }
                }
            }
        }
    }
    public class AddDetails
    {
        public string ChapterNo { get; set; }
        public string Topic { get; set; }
        public int TotalPages { get; set; }
    }
}
