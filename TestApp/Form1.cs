using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AppUpdater;
using AppUpdater.Log;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private AutoUpdater autoUpdater;

        public Form1()
        {
            InitializeComponent();

            Logger.LoggerProvider = (type) => new FormLog(DoLog, type);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            autoUpdater = new AutoUpdater(UpdateManager.Default);
            autoUpdater.SecondsBetweenChecks = 10;
            autoUpdater.Updated += new EventHandler(autoUpdater_Updated);
            autoUpdater.Start();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnUpdate.Enabled = false;
        }

        void autoUpdater_Updated(object sender, EventArgs e)
        {
            MessageBox.Show("Updated");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            autoUpdater.Stop();

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnUpdate.Enabled = true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateManager manager = UpdateManager.Default;
            UpdateInfo info;
            if (manager.CheckForUpdate(out info))
            {
                manager.DoUpdate(info);
                MessageBox.Show("Updated");
            }
            else
            {
                MessageBox.Show("No update available.");
            }
        }

        private void DoLog(string logLevel, Type type, string message, object[] values)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(new Action<string, Type, string, object[]>(DoLog), logLevel, type, message, values);
            }
            else
            {
                string msg = String.Format(message, values);
                msg = String.Format("{0:HH:mm:ss} - [{1}] - {2} - {3}\r\n", DateTime.Now, logLevel, type.FullName, msg);
                txtLog.AppendText(msg);
            }
        }

        public class FormLog : ILog
        {
            private Action<string, Type, string, object[]> doLog;
            private Type type;

            public FormLog(Action<string, Type, string, object[]> doLog, Type type)
            {
                this.doLog = doLog;
                this.type = type;
            }

            public void Info(string message, params object[] values)
            {
                doLog("Info", type, message, values);
            }

            public void Warn(string message, params object[] values)
            {
                doLog("Warn", type, message, values);
            }

            public void Error(string message, params object[] values)
            {
                doLog("Error", type, message, values);
            }

            public void Debug(string message, params object[] values)
            {
                doLog("Debug", type, message, values);
            }
        }
    }
}
