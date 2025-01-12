using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Project_1281246
{
    public partial class EditForm : Form
    {
        List<AddDetails> details = new List<AddDetails>();
        string currentFile = "";
        string oldFile = "";
        public EditForm()
        {
            InitializeComponent();
        }
        public MainForm TheForm { get; set; }
        public int IdToEdit { get; set; }
        

        private void EditForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadInForm();
        }

        private void LoadInForm()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Books WHERE BookId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetString(1);
                        numericUpDown1.Value = dr.GetDecimal(2);
                        dateTimePicker1.Value = dr.GetDateTime(3).Date;
                        checkBox1.Checked = dr.GetBoolean(4);
                        pictureBox1.Image = Image.FromFile(@"..\..\Pictures\" + dr.GetString(5));
                        oldFile = dr.GetString(5);
                    }
                    dr.Close();
                    cmd.CommandText = @"SELECT * FROM TableOfContents WHERE BookId = @i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        details.Add(new AddDetails { ChapterNo = dr2.GetString(1), Topic = dr2.GetString(2), TotalPages = dr2.GetInt32(3) });
                    }
                    SetDataSources();
                    con.Close();
                }
            }
        }

        private void SetDataSources()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
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

        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string f = oldFile;
                        if (currentFile != "")
                        {
                            string ext = Path.GetExtension(currentFile);
                            f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                            string savePath = @"..\..\Pictures\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                        cmd.CommandText = "UPDATE Books SET Title = @t, Price = @p, PublishDate = @pd, IsEBookAvailable = @eb, Picture = @pic WHERE BookId=@id ";
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        cmd.Parameters.AddWithValue("@t", textBox1.Text);
                        cmd.Parameters.AddWithValue("@p", numericUpDown1.Value);
                        cmd.Parameters.AddWithValue("@pd", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@eb", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@pic", f);
                        try
                        {
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "DELETE FROM TableOfContents WHERE BookId = @id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            foreach (var toc in details)
                            {
                                cmd.CommandText = "INSERT INTO TableOfContents (ChapterNo, Topic, TotalPages, BookId) VALUES (@cn, @tc, @tp, @i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@cn", toc.ChapterNo);
                                cmd.Parameters.AddWithValue("@tc", toc.Topic);
                                cmd.Parameters.AddWithValue("@tp", toc.TotalPages);
                                cmd.Parameters.AddWithValue("@i", IdToEdit);
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = details;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    string sql = @"DELETE  TableOfContents
                                    WHERE BookId=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, con, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "DELETE FROM Books WHERE BookId=@id";
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            MessageBox.Show("Data Deleted", "Success");
                            TheForm.LoadBindingSource();
                            this.Close();
                        }
                        catch
                        {
                            tran.Rollback();
                            MessageBox.Show("Failed to Delete", "Error");
                        }
                        con.Close();
                    }
                }
            }
        }
    }
}

    
 
