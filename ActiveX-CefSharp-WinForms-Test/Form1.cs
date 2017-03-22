using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ActiveX_CefSharp_WinForms_Test
{
    public partial class Form1 : Form
    {
        public ChromiumWebBrowser Browser { get; private set; }
        public MOSHost MOSHost { get; private set; }

        private Random random = new Random();
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Form1()
        {
            String page = string.Format(@"{0}\html-resources\two-way-comms.html", Application.StartupPath);

            if (!File.Exists(page))
            {
                MessageBox.Show("Error The html file doesn't exist: " + page);
            }

            MOSHost = new MOSHost(this);

            Browser = new ChromiumWebBrowser(page)
            {
                Dock = DockStyle.Fill
            };

            Browser.LoadingStateChanged += Browser_LoadingStateChanged;
            Browser.ConsoleMessage += Browser_ConsoleMessage;

            var settings = new BrowserSettings()
            {
                UniversalAccessFromFileUrls = CefState.Enabled,
                FileAccessFromFileUrls = CefState.Enabled
            };

            Browser.BrowserSettings = settings;
         
            InitializeComponent();

            splitContainer1.Panel2.Controls.Add(Browser);
            splitContainer1.Panel2.Controls.SetChildIndex(lblLoading, 0);
            Browser.RegisterJsObject("mosHost", MOSHost);

            dgvLog.DataSource = MOSHost.LogMessages;
            dgvLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dgvLog.Columns[0].DefaultCellStyle.Format = "HH:mm:ss";

            //dgvLog.ColumnCount = 2;
            //dgvLog.Columns[0].DataPropertyName = "Time";
            //dgvLog.Columns[1].DataPropertyName = "Message";
        }

        private void Browser_ConsoleMessage(object sender, CefSharp.ConsoleMessageEventArgs e)
        {
            Console.WriteLine("Browser logged: " + e.Message);
        }

        private void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            Console.WriteLine("Loading state changed");

            if (!e.IsLoading)
            {
                this.Invoke((MethodInvoker) delegate
                {
                    lblLoading.Visible = false;
                    Browser.Visible = true;
                });   
            }
        }

        private void btnSendToJS_Click(object sender, EventArgs e)
        {
            var message = HttpUtility.JavaScriptStringEncode(RandomString(64));
            Browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("receiveMessageFromCSharp('" + message + "');");
        }
    }
}
