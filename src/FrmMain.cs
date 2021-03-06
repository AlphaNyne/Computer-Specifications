﻿/*
 * Copyright (c) 2018-2019 AlphaNyne
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

using ByteSizeLib;

namespace ComputerSpecifications
{
  public partial class FrmMain : Form
  {
    public FrmMain()
    {
      this.InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.UseWaitCursor = true;

      try {

        // Motherboard
        string memorySize = this.Searcher("Win32_OperatingSystem", "TotalVisibleMemorySize");
        this.textBoxProcessName.Text = this.Searcher("Win32_Processor", "Name");
        this.textBoxProcessCore.Text = this.Searcher("Win32_Processor", "NumberOfCores");
        this.textBoxOSName.Text = this.Searcher("Win32_OperatingSystem", "Name").Split('|')[0];
        this.textBoxRam.Text = ByteSize.FromKiloBytes(double.Parse(memorySize)).ToString();

        // Graphic
        string adapterRAM = this.Searcher("Win32_VideoController", "AdapterRAM");
        this.textBoxVideoName.Text = this.Searcher("Win32_VideoController", "Name");
        this.textBoxVideoRam.Text = ByteSize.FromBytes(double.Parse(adapterRAM)).ToString();

        this.textBoxMonitor.Text = $"{Screen.PrimaryScreen.Bounds.Width} x {Screen.PrimaryScreen.Bounds.Height}";
        this.textBoxMonitorPrimary.Text = this.Searcher("Win32_DesktopMonitor", "Description");
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        this.UseWaitCursor = false;
      }
    }

    private string Searcher(string query, string name, string scope = null)
    {
      ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(scope, $"select * from {query}");

      foreach (ManagementBaseObject data in objectSearcher.Get()) {
        if (data[name] != null) {
          return data[name].ToString();
        }
      }

      return "";
    }

    private void printScreenToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try {
        SaveFileDialog SFD = new SaveFileDialog {
          FileName = DateTime.Now.ToString("yyyy-MM-dd"),
          Filter = "*.png|*.png"
        };

        if (SFD.ShowDialog(this) == DialogResult.OK) {
          using (Bitmap bmp = new Bitmap(this.Width, this.Height)) {
            this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Width, this.Height));
            bmp.Save(SFD.FileName);
            bmp.Dispose();
          }
        }
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
