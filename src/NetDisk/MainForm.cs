using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bzway
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        bool block = false;
        int count = 0;
        public double workMinutes;
        public double restMinutes;
        private void MainForm_Load(object sender, EventArgs e)
        {
            string path = Application.ExecutablePath + " 40";
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                registryKey.SetValue("ScreenLocker", path);
                registryKey.Close();
            }
            catch { }
            while (workMinutes <= 0 || workMinutes > 120)
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("请输入您的工作时间（分钟）：", "请输入时间", "40", 0, 0);
                double.TryParse(input, out workMinutes);
            }
            while (restMinutes <= 0 || restMinutes > workMinutes)
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("请输入您的休息时间（分钟）：", "请输入时间", "1", 0, 0);
                double.TryParse(input, out restMinutes);
            }

            this.Hide();
            Timer timerLock = new Timer();
            timerLock.Interval = 100;
            timerLock.Tick += Lock_Tick;
            timerLock.Enabled = true;
        }
        private void Lock_Tick(object sender, EventArgs e)
        {
            //锁定状态
            if (this.block)
            {
                count++;
                if (count >= 600 * restMinutes)
                {
                    //unlock
                    block = false;
                    count = 0;
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                    this.TopMost = block;
                    WorkStation.Instance.LockWorkStation(block);
                    return;
                }
                this.WindowState = FormWindowState.Maximized;
                this.Show();
                this.TopMost = block;
                WorkStation.Instance.LockWorkStation(block);
                return;
            }
            //工作状态
            count++;
            if (count >= 600 * workMinutes)
            {
                //lock
                block = true;
                count = 0;
                return;
            }
        }
    }
}