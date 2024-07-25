using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Kamstrup.LicenseKeyGenerator.Controller;
using Kamstrup.LicenseKeyGenerator.Model;

namespace Kamstrup.LicenseKeyGenerator.GUI.Controls
{
  public partial class Maintenance : UserControl
  {
    readonly List<RadioButton> _trailProductControlList = new List<RadioButton>();
    public Maintenance()
    {
      InitializeComponent();
      initTrialTab();
#if DEBUGMAINTENANCE|| RELEASEMAINTENANCE
      tabControl_Maintenance.TabPages.Remove(tab_ExtraTrail);
#endif
    }

    private void initTrialTab()
    {
      #region Adding products radio buttons
      List<CatalogItem> list = CatalogItemController.Instance.GetCatalogItems();

      foreach (CatalogItem t in list)
      {
        RadioButton radioButton = new RadioButton { Text = t.ProductDescription, Tag = t.ProductId, Visible = true, AutoSize = true };
        _trailProductControlList.Add(radioButton);
        flowPanel_Trial_Products.Controls.Add(radioButton);
      }
      #endregion

      lbl_dateCounter.Text = "Expires on " +
                             DateTime.Now.AddDays((double)numUpDown_ExtraTrialDays.Value).ToShortDateString();
    }

    private void btn_Trial_CreateTrialKey_Click(object sender, EventArgs e)
    {
      bool noProductChosen = true;
      foreach (RadioButton product in _trailProductControlList)
      {
        if (product.Checked)
        {
          noProductChosen = false;
          ProductItem item = ProductController.Instance.GetSpecificProduct((int)product.Tag);
          CatalogItemController.Instance.DefineProduct(item);
          string trialKey = MaintenanceController.CreateTrialKey(item, (int)numUpDown_ExtraTrialDays.Value, txt_OrderNo.Text.Trim());
          if (string.IsNullOrEmpty(trialKey))
            return;
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(trialKey + ";" + trialKey);

          SaveFileDialog fileDialog = new SaveFileDialog
          {
            Filter = "license file (*.license)|*.license",
            FilterIndex = 1,
            DefaultExt = "license",
            AddExtension = true,
            FileName = (int)product.Tag + ".license",
            OverwritePrompt = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
          };

          if (fileDialog.ShowDialog() == DialogResult.OK)
          {
            try
            {
              File.WriteAllText(fileDialog.FileName, stringBuilder.ToString(), Encoding.Unicode);
              EqatecMonitor.EqatecMonitor.SendFeatureUse("CreateTrialKeyDays", numUpDown_ExtraTrialDays.Value.ToString());
            }
            catch (Exception ex)
            {
              MessageBox.Show("An error happend when saving the file." + Environment.NewLine + ex);
            }
          }
        }
      }
      if (noProductChosen)
        MessageBox.Show(this, "Remember to choose a product", "Missing product", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void numUpDown_ExtraTrialDays_ValueChanged(object sender, EventArgs e)
    {
      lbl_dateCounter.Text = "Expires on " +
                             DateTime.Now.AddDays((double)numUpDown_ExtraTrialDays.Value).ToShortDateString();
    }

    private void orderNumberOK(object sender, EventArgs e)
    {
      grpBox_CreateTrialKey.Enabled = UITool.OrderNumberFormatApproved(txt_OrderNo.Text);
    }

    private void btn_Search_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(txt_SearchOrderNo.Text.Trim()) &&
        string.IsNullOrEmpty(txt_SearchLicenseKey.Text.Trim()) &&
        string.IsNullOrEmpty(txt_SearchComputerName.Text.Trim()) &&
        string.IsNullOrEmpty(txt_SearchComputerKey.Text.Trim()))
      {
        MessageBox.Show(this, "Remember to enter search text", "Missing text", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
        return;
      }

      double result;
      if (!string.IsNullOrEmpty(txt_SearchOrderNo.Text.Trim()) && !double.TryParse(txt_SearchOrderNo.Text.Trim(), out result))
      {
        MessageBox.Show(this, "Order number has to be numeric.", "Order number", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
        return;
      }

      Cursor tmpCursor = Cursor;
      Cursor = Cursors.WaitCursor;
      List<License> list = MaintenanceController.FindKeys(txt_SearchOrderNo.Text.Trim(), txt_SearchLicenseKey.Text.Trim().Replace("-", ""), txt_SearchComputerName.Text, txt_SearchComputerKey.Text.Trim().Replace("-", ""));
      list = insertHyphensInKeys(list);
      dgv_LicenseResult.DataSource = list;
      if (dgv_LicenseResult.RowCount > 0)
        dgv_LicenseResult.Rows[0].Selected = true;
      EqatecMonitor.EqatecMonitor.SendFeatureUse("MaintenanceTab", "SearchButtonClicked");
      Cursor = tmpCursor;
    }

    private static List<License> insertHyphensInKeys(List<License> licenseList)
    {
      foreach (License license in licenseList)
      {
        int ackKeyBlocks = license.ActivationKey.Length/5;
        int index = 5;
        for (int i = 0; i < ackKeyBlocks; i++)
        {
          if(index >= license.ActivationKey.Length - 3)
            break;
          license.ActivationKey = license.ActivationKey.Insert(index, "-");
          index += 6;
        }

        int compKeyBlocks = license.ComputerKey.Length / 5;
        index = 5;
        for (int i = 0; i < compKeyBlocks; i++)
        {
          if (index >= license.ComputerKey.Length - 3)
            break;
          license.ComputerKey = license.ComputerKey.Insert(index, "-");
          index += 6;
        }
      }
      return licenseList;
    }

    private void dgv_LicenseResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      grpBox_Action.Enabled = true;
    }

    private void btn_Unlock_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(txt_Comment.Text))
      {
        if (
          MessageBox.Show(this,
                          "Are you sure you want to unlock this license without inputting a comment?" +
                          Environment.NewLine + "It could be usefull at a later time...", "No comment?",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
          DialogResult.No)
          return;
      }

      DataGridViewSelectedRowCollection rows = dgv_LicenseResult.SelectedRows;
      if (rows.Count != 1)
        return;
      string activationKey = (string)rows[0].Cells[col_ActivationKey.Name].Value;
      string computerID = (string)rows[0].Cells[col_ComputerID.Name].Value;
      try
      {
        string orderNo = (string)rows[0].Cells[col_OrderNo.Name].Value;
        MaintenanceController.ReleaseLicense(activationKey, computerID, orderNo, txt_Comment.Text);
        MessageBox.Show(this, string.Format("License '{0}' has been unlocked.", activationKey), "Unlocked successful", MessageBoxButtons.OK,
                        MessageBoxIcon.None);
      }
      catch (Exception exception)
      {
        MessageBox.Show(this,
                        string.Format("An error happend when unlocking the license '{0}'.",
                                      activationKey) + Environment.NewLine + exception, "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.None);
      }
      EqatecMonitor.EqatecMonitor.SendFeatureUse("MaintenanceTab", "UnlockButtonClicked");
      txt_Comment.Text = string.Empty;
      btn_Search_Click(null, e);
    }

    private void btn_Delete_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(txt_Comment.Text))
      {
        if (
          MessageBox.Show(this,
                          "Are you sure you want to delete this license without inputting a comment?" +
                          Environment.NewLine + "It could be usefull at a later time...", "No comment?",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
          DialogResult.No)
          return;
      }

      DataGridViewSelectedRowCollection rows = dgv_LicenseResult.SelectedRows;
      if (rows.Count != 1)
        return;
      string activationKey = (string)rows[0].Cells[col_ActivationKey.Name].Value;
      string computerID = (string)rows[0].Cells[col_ComputerID.Name].Value;
      string orderNo = (string)rows[0].Cells[col_OrderNo.Name].Value;

      DialogResult dialogResult = MessageBox.Show(this,
                      string.Format("Are you sure you want to delete license '{0}' on order number '{1}'?", activationKey, orderNo),
                      "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes)
      {
        try
        {
          MaintenanceController.DeleteLicense(activationKey, computerID, orderNo, txt_Comment.Text);
          MessageBox.Show(this, string.Format("License '{0}' has been deleted.", activationKey), "Delete successful",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.None);
        }
        catch (Exception exception)
        {
          MessageBox.Show(this, string.Format("An error happend when deleting the license '{0}'.", activationKey) + Environment.NewLine + exception, "Error",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.None);
        }
      }
      EqatecMonitor.EqatecMonitor.SendFeatureUse("MaintenanceTab", "DeleteButtonClicked");
      txt_Comment.Text = string.Empty;
      btn_Search_Click(null, e);
    }

    private void txt_Search_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == (int)Keys.Enter)
      {
        btn_Search_Click(btn_Search, e);
      }
    }

    private void dgv_LicenseResult_SelectionChanged(object sender, EventArgs e)
    {
      if (dgv_LicenseResult.RowCount > 0)
        grpBox_Action.Enabled = true;
    }

    private void btn_GetLog_Click(object sender, EventArgs e)
    {
      Cursor cursor = Cursor.Current;
      Cursor = Cursors.WaitCursor;
      string csvLog;
      try
      {
        csvLog = MaintenanceController.GetLog();
        EqatecMonitor.EqatecMonitor.SendFeatureUse("MaintenanceTab", "GetLogClicked");
      }
      catch (Exception ex)
      {
        MessageBox.Show("An error happened when getting the log from the license server" + Environment.NewLine + ex);
        Cursor = cursor;
        return;
      }

      Cursor = cursor;

      SaveFileDialog fileDialog = new SaveFileDialog
      {
        Filter = "(*.csv)|*.csv",
        FilterIndex = 1,
        DefaultExt = "csv",
        AddExtension = true,
        FileName = "License Key Generator Log",
        OverwritePrompt = true,
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
      };

      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        try
        {
          File.WriteAllText(fileDialog.FileName, csvLog, Encoding.Unicode);
          EqatecMonitor.EqatecMonitor.SendFeatureUse("MaintenanceTab", "GetLogButtonClicked");
        }
        catch (Exception ex)
        {
          MessageBox.Show("An error happend when saving the file." + Environment.NewLine + ex);
        }
      }
    }
  }
}
