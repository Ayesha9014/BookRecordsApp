using Project_1281246.Reports;
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
    public partial class ReportForm1 : Form
    {
        public ReportForm1()
        {
            InitializeComponent();
        }

        private void ReportForm1_Load(object sender, EventArgs e)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT* FROM Books", con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "Books1");
                    da.SelectCommand.CommandText = @"SELECT * FROM TableOfContents";
                    da.Fill(ds, "TableOfContents");

                    ds.Tables["Books1"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (var i = 0; i < ds.Tables["Books1"].Rows.Count; i++)
                    {
                        ds.Tables["Books1"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Pictures\{ds.Tables["Books1"].Rows[i]["Picture"]}");
                    }

                    Report1 rpt = new Report1();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
