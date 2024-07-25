using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Kamstrup.LicenseKeyGenerator.Controller;
using Kamstrup.LicenseKeyGenerator.GUI.Controls;
using Kamstrup.LicenseKeyGenerator.Model;

namespace Kamstrup.LicenseKeyGenerator.GUI
{
  public partial class frmKeyGenerator : Form
  {
    /// <summary>
    /// Remember Equatec version EqatecMonitor.EqatecMonitorConstants.LKG_Version
    /// </summary>
#if DEBUGNL || RELEASENL
    private const string Version = "5098-813";
#elif DEBUGMAINTENANCE|| RELEASEMAINTENANCE
    private const string Version = "5098-850";
#else
    private const string Version = "5098-573";
#endif
    private readonly List<RadioButton> _featureControlList = new List<RadioButton>();
    private readonly List<RadioButton> _productOfflineControlList = new List<RadioButton>();
    private readonly List<RadioButton> _productOnlineControlList = new List<RadioButton>();
    private string _feature;
    private string _product;

    public frmKeyGenerator()
    {
      InitializeComponent();
      Maintenance maintenance = new Maintenance();
      tabPage_Maintenance.Controls.Add(maintenance);
      Text += " - " + Version + " E1";
#if (DEBUG || DEBUGNL || DEBUGMAINTENANCE)
      {
        Text += " ***DEBUG***";
        lbl_DebugInfo.Text = string.Format("Debug: {0}", SettingsController.GetConfigFileSettings().licenseServerUrl);
      }
#endif
      textBox_itemNumber.Select();
      initProducts();

#if DEBUGMAINTENANCE|| RELEASEMAINTENANCE
      tabControl.TabPages.Remove(tabPage_ActivationKey);
#endif
      EqatecMonitor.EqatecMonitor.SendFeatureUseClean("Version", Version);
      EqatecMonitor.EqatecMonitor.SendFeatureUseClean("Version_Country", Version + "_" + System.Globalization.RegionInfo.CurrentRegion.EnglishName);
    }

    public override sealed string Text
    {
      get { return base.Text; }
      set { base.Text = value; }
    }

    #region Events

    private void radioButtonProductOnlineCheckedChanged(object sender, EventArgs e)
    {
      foreach (RadioButton t in _productOnlineControlList)
      {
        if (t.Checked)
        {
          CatalogItemLookupCollection collection =
            CatalogItemController.Instance.GetFeaturesForProducts(
              ProductController.Instance.GetSpecificProduct((int)t.Tag));

          //This is a hack (I know) but PcTermPro and PcImportExport is treated the same license data and the user doesn't have to know it
          if ((((RadioButton)sender).Text.Equals("PcTermPro III") ||
               ((RadioButton)sender).Text.Equals("PcImportExport III")) && collection.Count == 2)
          {
            foreach (CatalogItemLookupObject catalogItem in collection)
            {
              if (((RadioButton)sender).Text.Equals("PcTermPro III") &&
                  catalogItem.Catalog.ProductDescription.Equals("PcImportExport III"))
              {
                collection.Remove(catalogItem);
                break;
              }
              if (((RadioButton)sender).Text.Equals("PcImportExport III") &&
                  catalogItem.Catalog.ProductDescription.Equals("PcTermPro III"))
              {
                collection.Remove(catalogItem);
                break;
              }
            }
          }

          init(collection);
          button_createKey.Enabled = true;
        }
      }
    }

    private void button_search_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textBox_itemNumber.Text.Trim()))
        return;
      UseWaitCursor = true;
      resetCreateKeyGUI();
      string catalogNumber;
      UITool.CorrectItemNumber(textBox_itemNumber.Text, out catalogNumber);
      CatalogItemLookupObject obj = CatalogItemController.Instance.LookupStoredItem(catalogNumber);
      displaySearchResult(obj, catalogNumber);
      EqatecMonitor.EqatecMonitor.SendFeatureUse("CreateKeyTab", "SearchButtonClicked");
      UseWaitCursor = false;
    }

    private void button_reset_Click(object sender, EventArgs e)
    {
      resetCreateKeyGUI();
      EqatecMonitor.EqatecMonitor.SendFeatureUse("CreateKeyTab", "ResetAllButtonClicked");
    }

    private void button_resetOffline_Click(object sender, EventArgs e)
    {
      UseWaitCursor = true;
      button_selectDirectoryLicense.Enabled = false;
      button_resetOffline.Enabled = false;
      button_generateComputerKey.Enabled = false;
      textBox_activationKey.Text = string.Empty;
      textBox_computerId.Text = string.Empty;
      textBox_computerName.Text = string.Empty;
      richTextBox_output.Text = string.Empty;
      textBox_path.Text = string.Empty;
      foreach (RadioButton radioButton in _productOfflineControlList)
      {
        radioButton.Checked = false;
      }
      EqatecMonitor.EqatecMonitor.SendFeatureUse("OfflineActivationTab", "ResetAllButtonClicked");
      button_copyOffline.Enabled = false;
      UseWaitCursor = false;
    }

    private void button_makeKey_Click(object sender, EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      try
      {
        Boolean wasError = false;
        foreach (RadioButton t in _productOnlineControlList)
        {
          if (t.Checked)
          {
            CatalogItemController.Instance.DefineProduct(ProductController.Instance.GetSpecificProduct((int)t.Tag));
            _product = t.Text;
          }
        }
        foreach (RadioButton b in _featureControlList)
        {
          if (b.Checked)
          {
            _feature = ((FeatureItem)b.Tag).ProductDescription;
            textBox_outputActivationKey.Text = string.Empty;
            //If this line below is not printet at the top of textBox_outputActivationKey anymore, there should be made at change in button_SaveKeys_Click
            textBox_outputActivationKey.Text = string.Format("Activation key for {0} with {1}{2}", _product, _feature,
                                                             Environment.NewLine);
            try
            {
              for (int index = 0; index < numUpDown_NoOfKeys.Value; index++)
              {
                string activationKey = CatalogItemController.Instance.GenerateActivationKey((FeatureItem)b.Tag,
                                                                                            txt_OrderNo.Text.Trim());
                textBox_outputActivationKey.AppendText(activationKey + Environment.NewLine);
              }
            }
            catch (Exception ex)
            {
              MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
              wasError = true;
            }
          }
        }
        lbl_info.Text = string.Empty;
        if (!wasError)
        {
          foreach (RadioButton t in _productOnlineControlList)
          {
            if (t.Checked)
            {
              CatalogItemController.Instance.saveFile(textBox_outputActivationKey.Text);
            }
          }
        }
      }
      catch (Exception)
      {
        MessageBox.Show(this, "Catalog not implemented", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }

      EqatecMonitor.EqatecMonitor.SendFeatureUse("CreateKeyTab", "CreateKeyClicked");
      EqatecMonitor.EqatecMonitor.SendFeatureUse("WhoCreatesActivationKeys", Environment.UserName + " - " + System.Globalization.RegionInfo.CurrentRegion.EnglishName);
      
      Cursor.Current = Cursors.Default;
      if (string.IsNullOrEmpty(textBox_outputActivationKey.Text))
      {
        button_copy.Enabled = false;
        button_SaveKeys.Enabled = false;
      }
      else
      {
        if (numUpDown_NoOfKeys.Value == 1)
          button_copy.Enabled = true;
        button_SaveKeys.Enabled = true;
      }
    }

    private void button_selectDirectory_Click(object sender, EventArgs e)
    {
      openFileDialog_LicenseFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      openFileDialog_LicenseFile.Filter = ("Kamstrup License files") + " (*.KAMlicense)|*.KAMlicense|" + ("All files") +
                                          " (*.*)|*.*";
      openFileDialog_LicenseFile.FilterIndex = 1;
      openFileDialog_LicenseFile.RestoreDirectory = true;
      DialogResult dr = openFileDialog_LicenseFile.ShowDialog();
      if (dr == DialogResult.OK)
      {
        try
        {
          button_generateComputerKey.Enabled = true;
          button_resetOffline.Enabled = true;
          textBox_path.Text = openFileDialog_LicenseFile.FileName;
          string activationKey, computerID, computerName;
          CatalogItemController.Instance.ReadLicenseFile(textBox_path.Text, out activationKey, out computerID,
                                                         out computerName);
          if (string.IsNullOrEmpty(textBox_activationKey.Text) && !string.IsNullOrEmpty(activationKey))
            textBox_activationKey.Text = activationKey;
          textBox_computerId.Text = computerID;
          textBox_computerName.Text = computerName;
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
      }
    }

    private void button_generateComputerKey_Click(object sender, EventArgs e)
    {
      UseWaitCursor = true;
      if (textBox_activationKey.Text == string.Empty)
      {
        MessageBox.Show(this, "Activation key missing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else if (textBox_computerId.Text == string.Empty)
      {
        MessageBox.Show(this, "Computer ID missing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else if (textBox_computerName.Text == string.Empty)
      {
        MessageBox.Show(this, "Computername missing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else
      {
        bool found = false;
        foreach (RadioButton radioButton in _productOfflineControlList)
        {
          if (radioButton.Checked)
          {
            CatalogItemController.Instance.DefineProduct(
              ProductController.Instance.GetSpecificProduct((int)radioButton.Tag));
            found = true;
            break;
          }
        }
        if (!found)
        {
          MessageBox.Show(this, "Catalog not defined", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          UseWaitCursor = false;
          button_copyOffline.Enabled = true;
          return;
        }
        try
        {
          string computerKey = CatalogItemController.Instance.GenerateComputerKey(textBox_activationKey.Text.ToUpper(),
                                                                                  textBox_computerId.Text,
                                                                                  textBox_computerName.Text);
          string splitComputerKey = string.Empty;
          for (int i = 0; i < computerKey.Length; i++)
          {
            if (i % 5 == 0 && i != 0)
            {
              splitComputerKey += "-";
            }
            splitComputerKey += computerKey[i];
          }
          richTextBox_output.Text = splitComputerKey;
        }
        catch (Exception ex)
        {
          MessageBox.Show(this,
                          string.Format("{0}{1}Have you chosen the correct product for the activation key?", ex.Message,
                                        Environment.NewLine), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          UseWaitCursor = false;
          button_copyOffline.Enabled = true;
        }
      }
      EqatecMonitor.EqatecMonitor.SendFeatureUse("OfflineActivationTab", "GenerateComputerKeyButtonClicked");
      EqatecMonitor.EqatecMonitor.SendFeatureUse("WhoCreatesComputerKeys", Environment.UserName + " - " + System.Globalization.RegionInfo.CurrentRegion.EnglishName);
      UseWaitCursor = false;
      button_copyOffline.Enabled = true;
    }

    private void radioButton_productOffline_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        button_selectDirectoryLicense.Enabled = true;
      }
      catch (Exception ex)
      {
        foreach (RadioButton button in _productOfflineControlList)
        {
          button.Checked = false;
        }
        button_resetOffline_Click(sender, e);
        MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }

    private void button_copy_Click(object sender, EventArgs e)
    {
      textBox_outputActivationKey.SelectAll();
      string tmpText = textBox_outputActivationKey.SelectedText;
      string[] splitter = { Environment.NewLine };
      string[] arrSplitted = tmpText.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

      Clipboard.SetDataObject(arrSplitted[1], true);

      EqatecMonitor.EqatecMonitor.SendFeatureUse("CreateKeyTab", "CopyButtonClicked");
    }

    private void button_copyOffline_Click(object sender, EventArgs e)
    {
      richTextBox_output.SelectAll();
      Clipboard.SetDataObject(richTextBox_output.SelectedText, true);
      EqatecMonitor.EqatecMonitor.SendFeatureUse("OfflineActivationTab", "CopyButtonClicked");
    }

    private void button_Paste_Click(object sender, EventArgs e)
    {
      if (Clipboard.ContainsText())
      {
        textBox_activationKey.Text = Clipboard.GetText();
        EqatecMonitor.EqatecMonitor.SendFeatureUse("OfflineActivationTab", "PasteButtonClicked");
      }
    }

    private void numUpDown_NoOfKeys_ValueChanged(object sender, EventArgs e)
    {
      if (numUpDown_NoOfKeys.Value > 1)
      {
        button_createKey.Text = "Create keys";
        button_SaveKeys.Visible = true;
        button_copy.Enabled = false;
      }
      else
      {
        button_createKey.Text = "Create key";
        button_SaveKeys.Visible = false;
        button_copy.Enabled = true;
      }
    }

    private void button_SaveKeys_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(textBox_outputActivationKey.Text))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Catalog,\tFeature,\tActivation key");
      string[] keys = textBox_outputActivationKey.Text.Split(new[] { Environment.NewLine },
                                                             StringSplitOptions.RemoveEmptyEntries);
      for (int i = 1; i < keys.Length; i++)
      {
        StringBuilder tmp = new StringBuilder(_product);
        tmp.Append(",\t").Append(_feature.Replace(Environment.NewLine, string.Empty)).Append(",\t").Append(keys[i]);
        stringBuilder.AppendLine(tmp.ToString());
      }

      SaveFileDialog fileDialog = new SaveFileDialog
                         {
                           Filter = "csv file (*.csv)|*.csv|All files (*.*)|*.*",
                           FilterIndex = 1,
                           DefaultExt = "csv",
                           AddExtension = true,
                           FileName = "*.csv",
                           OverwritePrompt = true,
                           InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                         };

      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        try
        {
          File.WriteAllText(fileDialog.FileName, stringBuilder.ToString(), Encoding.Unicode);
          EqatecMonitor.EqatecMonitor.SendFeatureUse("CreateKeyTab", "SaveKeysButtonClicked");
        }
        catch (Exception ex)
        {
          MessageBox.Show("An error happend when saving the file." + Environment.NewLine + ex);
        }
      }
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Environment.Exit(0);
    }

    private void danishToolStripMenuItem_Click(object sender, EventArgs e)
    {
      showOnlineHelp("5512871_A1_DK.chm");
    }

    private void englishToolStripMenuItem_Click(object sender, EventArgs e)
    {
      showOnlineHelp("5512872_A1_GB.chm");
    }

    private void textBox_itemNumber_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == (int)Keys.Enter)
      {
        button_search_Click(sender, e);
      }
    }

    #endregion

    #region Private helper methods

    private void resetCreateKeyGUI()
    {
      foreach (RadioButton radioButton in _productOnlineControlList)
      {
        radioButton.Checked = false;
      }
      foreach (RadioButton radioButton in _featureControlList)
      {
        radioButton.Checked = false;
        radioButton.Visible = false;
      }
      textBox_outputActivationKey.Text = string.Empty;
      button_createKey.Enabled = false;
      button_copy.Enabled = false;
      button_SaveKeys.Enabled = false;
      numUpDown_NoOfKeys.Value = 1;
    }

    private void initProducts()
    {
      List<CatalogItem> list = CatalogItemController.Instance.GetCatalogItems();

      foreach (CatalogItem t in list)
      {
        RadioButton radioButtonOnline = new RadioButton { Text = t.ProductDescription, Tag = t.ProductId, Visible = true, AutoSize = true };
        radioButtonOnline.CheckedChanged += radioButtonProductOnlineCheckedChanged;
        _productOnlineControlList.Add(radioButtonOnline);
        flowPanel_CreateKey_Products.Controls.Add(radioButtonOnline);

        RadioButton radioButtonOffline = new RadioButton { Text = t.ProductDescription, Tag = t.ProductId, Visible = true, AutoSize = true };
        radioButtonOffline.CheckedChanged += radioButton_productOffline_CheckedChanged;
        _productOfflineControlList.Add(radioButtonOffline);
        flowPanel_Offline_Products.Controls.Add(radioButtonOffline);
      }
    }

    private void init(CatalogItemLookupCollection collection)
    {
      //This is a hack (I know) but PcTermPro and PcImportExport use the same license data and the user doesn't have to know it
      if ((collection[0].Catalog.ProductDescription.Equals("PcTermPro III") ||
           collection[0].Catalog.ProductDescription.Equals("PcImportExport III")) && collection.Count == 2)
      {
        collection.RemoveAt(1);
      }

      resetFeatures();

      foreach (CatalogItemLookupObject t in collection)
      {
        RadioButton radioButton = new RadioButton();
        radioButton.Text = t.Feature.ProductDescription;
        radioButton.Visible = true;
        radioButton.Tag = t.Feature;
        _featureControlList.Add(radioButton);
        flowPanel_CreateKey_Features.Controls.Add(radioButton);
      }

      if (flowPanel_CreateKey_Features.Controls.Count > 0)
        ((RadioButton)flowPanel_CreateKey_Features.Controls[0]).Checked = true;
    }

    private void resetFeatures()
    {
      _featureControlList.Clear();
      flowPanel_CreateKey_Features.Controls.Clear();
    }

    private void orderNumberOK(object sender, EventArgs e)
    {
      grpBox_CreateKey.Enabled = UITool.OrderNumberFormatApproved(txt_OrderNo.Text);
    }

    private void displaySearchResult(CatalogItemLookupObject obj, string catalogNumber)
    {
      if (obj != null)
      {
        foreach (RadioButton t in _productOnlineControlList)
        {
          if (t.Tag != null)
          {
            ProductItem item = ProductController.Instance.GetSpecificProduct((int)t.Tag);
            if (obj.Catalog.ProductId == item.ProductId && obj.Catalog.ProductDescription.Equals(t.Text))
            {
              t.Checked = true;
              displayFeatureSearchResult(catalogNumber);
            }
          }
        }
      }
      else
        MessageBox.Show(this, string.Format("Varenummer/Catalog number '{0}' was not found", textBox_itemNumber.Text),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    private void displayFeatureSearchResult(string catalogNumber)
    {
      foreach (RadioButton t in _productOnlineControlList)
      {
        if (t.Checked)
        {
          init(
            CatalogItemController.Instance.GetFeaturesForProducts(
              ProductController.Instance.GetSpecificProduct((int)t.Tag)));
          foreach (RadioButton rb in _featureControlList)
          {
            if (rb.Tag != null && rb.Tag.GetType() == typeof(FeatureItem))
            {
              if (((FeatureItem)rb.Tag).CatalogNumbers.Contains(catalogNumber))
              {
                rb.Checked = true;
                return;
              }
            }
          }
        }
      }

      MessageBox.Show(this, "Varenummer/Catalog number was not found", "Error", MessageBoxButtons.OK,
                      MessageBoxIcon.Hand);
    }

    private static void showOnlineHelp(string docName)
    {
      string path = Application.StartupPath + Path.DirectorySeparatorChar;
      string name = path + docName;
      if (!File.Exists(name))
        name = path + "5512872_A1_GB.chm"; //attempt to find the english online help in lack of the translated one

      if (File.Exists(name))
      {
        // ShowHelp is called with a 'Parent' parameter. Using this causes PcBase to be the 'owner' of the help window.
        // At first glance this seems right. However, the help window will shown 'always on top' of the parent window.
        // To work around this (see Mantis 4483), create an invisible dummy form and use that as the help window's parent.
        Help.ShowHelp(new Form(), name);
      }
      else
        MessageBox.Show(
          String.Format("Cannot find the manual for License Key Generator. The file '{0}' is missing.", name), "Help",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    #endregion
  }
}