using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;

namespace FoundationProject.Reporting
{

    public partial class ReportViewer 
        : Form
    {

        public ReportViewer()
        {
            InitializeComponent();
        }

        public void LoadReport(ReportDocument report, string connectionString, params Parameter[] parameters)
        {
            // Apply connection information.
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            foreach (Table table in report.Database.Tables)
            {
                TableLogOnInfo tableLogOnInfo = table.LogOnInfo;
                if (connectionStringBuilder.IntegratedSecurity)
                {
                    tableLogOnInfo.ConnectionInfo.UserID = "";
                    tableLogOnInfo.ConnectionInfo.Password = "";
                    tableLogOnInfo.ConnectionInfo.DatabaseName = connectionStringBuilder.InitialCatalog;
                    tableLogOnInfo.ConnectionInfo.ServerName = connectionStringBuilder.DataSource;
                    tableLogOnInfo.ConnectionInfo.IntegratedSecurity = true;
                }
                else
                {
                    tableLogOnInfo.ConnectionInfo.UserID = connectionStringBuilder.UserID;
                    tableLogOnInfo.ConnectionInfo.Password = connectionStringBuilder.Password;
                    tableLogOnInfo.ConnectionInfo.DatabaseName = connectionStringBuilder.InitialCatalog;
                    tableLogOnInfo.ConnectionInfo.ServerName = connectionStringBuilder.DataSource;
                    tableLogOnInfo.ConnectionInfo.IntegratedSecurity = false;
                }
                table.ApplyLogOnInfo(tableLogOnInfo);
            }
            // Apply parameters.
            foreach (Parameter parameter in parameters)
                report.SetParameterValue(parameter.Name, parameter.Value);
            crystalReportViewer1.ReportSource = report;
        }

    }

}
