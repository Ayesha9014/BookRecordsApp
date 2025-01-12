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
    public partial class MainForm : Form
    {
        BindingSource bsB = new BindingSource();
        BindingSource bsTOC = new BindingSource();
        DataSet ds;

        public string ConfiguraionManager { get; private set; }

        public MainForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dataGridView2.AutoGenerateColumns = false;
            LoadBindingSource();
        }

        public void LoadBindingSource()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                using(SqlDataAdapter da = new SqlDataAdapter("SELECT* FROM Books", con))
                {
                    ds = new DataSet();
                    da.Fill(ds, "Books");
                    da.SelectCommand.CommandText = @"SELECT * FROM TableOfContents";
                    da.Fill(ds, "TableOfContents");

                    ds.Tables["Books"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for(var i=0;i< ds.Tables["Books"].Rows.Count; i++)
                    {
                        ds.Tables["Books"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Pictures\{ds.Tables["Books"].Rows[i]["Picture"]}");
                    }

                    DataRelation rel = new DataRelation("FK_B_TOC", ds.Tables["Books"].Columns["BookId"], ds.Tables["TableOfContents"].Columns["BookId"]);
                    ds.Relations.Add(rel);
                    bsB.DataSource = ds;
                    bsB.DataMember = "Books";
                    bsTOC.DataSource = bsB;
                    bsTOC.DataMember = "FK_B_TOC";
                    dataGridView2.DataSource = bsTOC;
                    AddDataBindings();
                }
            }
        }

        private void AddDataBindings()
        {
            labelid.DataBindings.Clear();
            labelid.DataBindings.Add(new Binding("Text", bsB, "BookId"));
            labelTitle.DataBindings.Clear();
            labelTitle.DataBindings.Add(new Binding("Text", bsB, "Title"));
            labelPrice.DataBindings.Clear();
            Binding bp = new Binding("Text", bsB, "Price", true);
            bp.Format += Bp_Format;
            labelPrice.DataBindings.Add(bp);
            pictureBox1.DataBindings.Clear();
            pictureBox1.DataBindings.Add(new Binding("Image", bsB, "image", true));
            Binding bm = new Binding("Text", bsB, "PublishDate", true);
            bm.Format += Bm_Format;
            labelDate.DataBindings.Clear();
            labelDate.DataBindings.Add(bm);
            checkBox1.DataBindings.Clear();
            checkBox1.DataBindings.Add("Checked", bsB, "IsEBookAvailable", true);
        }

        private void Bm_Format(object sender, ConvertEventArgs e)
        {
            DateTime d = (DateTime)e.Value;
            e.Value = d.ToString("dd-MM-yyyy");
        }

        private void Bp_Format(object sender, ConvertEventArgs e)
        {
            decimal d = (decimal)e.Value;
            e.Value = d.ToString("0.00");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bsB.Position < bsB.Count - 1)
            {
                bsB.MoveNext();
            }
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(bsB.Position > 0)
            {
                bsB.MovePrevious();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bsB.MoveLast();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bsB.MoveFirst();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddForm { TheForm = this }.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsB.Current as DataRowView).Row[0].ToString());
            new EditForm { TheForm = this, IdToEdit = v }.ShowDialog();
        }

        private void exitDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsB.Current as DataRowView).Row[0].ToString());
            new EditForm { TheForm = this, IdToEdit = v }.ShowDialog();
        }

        private void report1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReportForm1().Show();
        }
    }
}
