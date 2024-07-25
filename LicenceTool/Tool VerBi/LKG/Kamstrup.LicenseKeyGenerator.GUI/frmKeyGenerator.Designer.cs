using Kamstrup.LicenseKeyGenerator.GUI.Controls;

namespace Kamstrup.LicenseKeyGenerator.GUI
{
    partial class frmKeyGenerator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKeyGenerator));
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tabPage_ActivationKey = new System.Windows.Forms.TabPage();
      this.grpBox_CreateKey = new System.Windows.Forms.GroupBox();
      this.lbl_NoOfKeys = new System.Windows.Forms.Label();
      this.textBox_itemNumber = new System.Windows.Forms.TextBox();
      this.lbl_info = new System.Windows.Forms.Label();
      this.button_search = new System.Windows.Forms.Button();
      this.button_SaveKeys = new System.Windows.Forms.Button();
      this.groupBox_products = new System.Windows.Forms.GroupBox();
      this.flowPanel_CreateKey_Products = new System.Windows.Forms.FlowLayoutPanel();
      this.groupBox_features = new System.Windows.Forms.GroupBox();
      this.flowPanel_CreateKey_Features = new System.Windows.Forms.FlowLayoutPanel();
      this.numUpDown_NoOfKeys = new System.Windows.Forms.NumericUpDown();
      this.button_reset = new System.Windows.Forms.Button();
      this.label_CatalogNo = new System.Windows.Forms.Label();
      this.button_createKey = new System.Windows.Forms.Button();
      this.button_copy = new System.Windows.Forms.Button();
      this.textBox_outputActivationKey = new System.Windows.Forms.TextBox();
      this.grpBox_EnterOrderNumber = new System.Windows.Forms.GroupBox();
      this.btn_OrderNumberOK = new System.Windows.Forms.Button();
      this.lbl_OrderNo = new System.Windows.Forms.Label();
      this.txt_OrderNo = new System.Windows.Forms.TextBox();
      this.tabPage_Offline = new System.Windows.Forms.TabPage();
      this.flowPanel_Offline_Products = new System.Windows.Forms.FlowLayoutPanel();
      this.button_Paste = new System.Windows.Forms.Button();
      this.button_copyOffline = new System.Windows.Forms.Button();
      this.button_resetOffline = new System.Windows.Forms.Button();
      this.label_computerName = new System.Windows.Forms.Label();
      this.textBox_computerName = new System.Windows.Forms.TextBox();
      this.textBox_computerId = new System.Windows.Forms.TextBox();
      this.textBox_activationKey = new System.Windows.Forms.TextBox();
      this.label_computerId = new System.Windows.Forms.Label();
      this.label_activationKey = new System.Windows.Forms.Label();
      this.richTextBox_output = new System.Windows.Forms.RichTextBox();
      this.label_destination = new System.Windows.Forms.Label();
      this.textBox_path = new System.Windows.Forms.TextBox();
      this.button_selectDirectoryLicense = new System.Windows.Forms.Button();
      this.button_generateComputerKey = new System.Windows.Forms.Button();
      this.tabPage_Maintenance = new System.Windows.Forms.TabPage();
      this.openFileDialog_LicenseFile = new System.Windows.Forms.OpenFileDialog();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.danishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.lbl_DebugInfo = new System.Windows.Forms.Label();
      this.tabControl.SuspendLayout();
      this.tabPage_ActivationKey.SuspendLayout();
      this.grpBox_CreateKey.SuspendLayout();
      this.groupBox_products.SuspendLayout();
      this.groupBox_features.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numUpDown_NoOfKeys)).BeginInit();
      this.grpBox_EnterOrderNumber.SuspendLayout();
      this.tabPage_Offline.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.tabPage_ActivationKey);
      this.tabControl.Controls.Add(this.tabPage_Offline);
      this.tabControl.Controls.Add(this.tabPage_Maintenance);
      this.tabControl.Location = new System.Drawing.Point(12, 27);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(494, 655);
      this.tabControl.TabIndex = 0;
      // 
      // tabPage_ActivationKey
      // 
      this.tabPage_ActivationKey.Controls.Add(this.grpBox_CreateKey);
      this.tabPage_ActivationKey.Controls.Add(this.grpBox_EnterOrderNumber);
      this.tabPage_ActivationKey.Location = new System.Drawing.Point(4, 22);
      this.tabPage_ActivationKey.Name = "tabPage_ActivationKey";
      this.tabPage_ActivationKey.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_ActivationKey.Size = new System.Drawing.Size(486, 629);
      this.tabPage_ActivationKey.TabIndex = 0;
      this.tabPage_ActivationKey.Text = "Activation key";
      this.tabPage_ActivationKey.UseVisualStyleBackColor = true;
      // 
      // grpBox_CreateKey
      // 
      this.grpBox_CreateKey.Controls.Add(this.lbl_NoOfKeys);
      this.grpBox_CreateKey.Controls.Add(this.textBox_itemNumber);
      this.grpBox_CreateKey.Controls.Add(this.lbl_info);
      this.grpBox_CreateKey.Controls.Add(this.button_search);
      this.grpBox_CreateKey.Controls.Add(this.button_SaveKeys);
      this.grpBox_CreateKey.Controls.Add(this.groupBox_products);
      this.grpBox_CreateKey.Controls.Add(this.groupBox_features);
      this.grpBox_CreateKey.Controls.Add(this.numUpDown_NoOfKeys);
      this.grpBox_CreateKey.Controls.Add(this.button_reset);
      this.grpBox_CreateKey.Controls.Add(this.label_CatalogNo);
      this.grpBox_CreateKey.Controls.Add(this.button_createKey);
      this.grpBox_CreateKey.Controls.Add(this.button_copy);
      this.grpBox_CreateKey.Controls.Add(this.textBox_outputActivationKey);
      this.grpBox_CreateKey.Enabled = false;
      this.grpBox_CreateKey.Location = new System.Drawing.Point(7, 95);
      this.grpBox_CreateKey.Name = "grpBox_CreateKey";
      this.grpBox_CreateKey.Size = new System.Drawing.Size(475, 531);
      this.grpBox_CreateKey.TabIndex = 1;
      this.grpBox_CreateKey.TabStop = false;
      this.grpBox_CreateKey.Text = "Create license key";
      // 
      // lbl_NoOfKeys
      // 
      this.lbl_NoOfKeys.AutoSize = true;
      this.lbl_NoOfKeys.Location = new System.Drawing.Point(6, 16);
      this.lbl_NoOfKeys.Name = "lbl_NoOfKeys";
      this.lbl_NoOfKeys.Size = new System.Drawing.Size(162, 13);
      this.lbl_NoOfKeys.TabIndex = 0;
      this.lbl_NoOfKeys.Text = "Number of license keys to create";
      // 
      // textBox_itemNumber
      // 
      this.textBox_itemNumber.Location = new System.Drawing.Point(9, 78);
      this.textBox_itemNumber.Name = "textBox_itemNumber";
      this.textBox_itemNumber.Size = new System.Drawing.Size(192, 20);
      this.textBox_itemNumber.TabIndex = 3;
      this.textBox_itemNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_itemNumber_KeyPress);
      // 
      // lbl_info
      // 
      this.lbl_info.AutoSize = true;
      this.lbl_info.Location = new System.Drawing.Point(370, 81);
      this.lbl_info.Name = "lbl_info";
      this.lbl_info.Size = new System.Drawing.Size(0, 13);
      this.lbl_info.TabIndex = 5;
      // 
      // button_search
      // 
      this.button_search.Location = new System.Drawing.Point(207, 76);
      this.button_search.Name = "button_search";
      this.button_search.Size = new System.Drawing.Size(75, 23);
      this.button_search.TabIndex = 4;
      this.button_search.Text = "Search";
      this.button_search.UseVisualStyleBackColor = true;
      this.button_search.Click += new System.EventHandler(this.button_search_Click);
      // 
      // button_SaveKeys
      // 
      this.button_SaveKeys.AutoSize = true;
      this.button_SaveKeys.Enabled = false;
      this.button_SaveKeys.Location = new System.Drawing.Point(172, 501);
      this.button_SaveKeys.Name = "button_SaveKeys";
      this.button_SaveKeys.Size = new System.Drawing.Size(95, 23);
      this.button_SaveKeys.TabIndex = 11;
      this.button_SaveKeys.Text = "Save keys to file";
      this.button_SaveKeys.UseVisualStyleBackColor = true;
      this.button_SaveKeys.Visible = false;
      this.button_SaveKeys.Click += new System.EventHandler(this.button_SaveKeys_Click);
      // 
      // groupBox_products
      // 
      this.groupBox_products.Controls.Add(this.flowPanel_CreateKey_Products);
      this.groupBox_products.Location = new System.Drawing.Point(9, 113);
      this.groupBox_products.Name = "groupBox_products";
      this.groupBox_products.Size = new System.Drawing.Size(456, 115);
      this.groupBox_products.TabIndex = 6;
      this.groupBox_products.TabStop = false;
      this.groupBox_products.Text = "Products";
      // 
      // flowPanel_CreateKey_Products
      // 
      this.flowPanel_CreateKey_Products.AutoScroll = true;
      this.flowPanel_CreateKey_Products.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowPanel_CreateKey_Products.Location = new System.Drawing.Point(7, 19);
      this.flowPanel_CreateKey_Products.Name = "flowPanel_CreateKey_Products";
      this.flowPanel_CreateKey_Products.Size = new System.Drawing.Size(443, 90);
      this.flowPanel_CreateKey_Products.TabIndex = 0;
      // 
      // groupBox_features
      // 
      this.groupBox_features.Controls.Add(this.flowPanel_CreateKey_Features);
      this.groupBox_features.Location = new System.Drawing.Point(9, 229);
      this.groupBox_features.Name = "groupBox_features";
      this.groupBox_features.Size = new System.Drawing.Size(456, 210);
      this.groupBox_features.TabIndex = 7;
      this.groupBox_features.TabStop = false;
      this.groupBox_features.Text = "Features";
      // 
      // flowPanel_CreateKey_Features
      // 
      this.flowPanel_CreateKey_Features.AutoScroll = true;
      this.flowPanel_CreateKey_Features.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowPanel_CreateKey_Features.Location = new System.Drawing.Point(7, 19);
      this.flowPanel_CreateKey_Features.Name = "flowPanel_CreateKey_Features";
      this.flowPanel_CreateKey_Features.Size = new System.Drawing.Size(443, 184);
      this.flowPanel_CreateKey_Features.TabIndex = 0;
      // 
      // numUpDown_NoOfKeys
      // 
      this.numUpDown_NoOfKeys.Location = new System.Drawing.Point(9, 32);
      this.numUpDown_NoOfKeys.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numUpDown_NoOfKeys.Name = "numUpDown_NoOfKeys";
      this.numUpDown_NoOfKeys.Size = new System.Drawing.Size(120, 20);
      this.numUpDown_NoOfKeys.TabIndex = 1;
      this.numUpDown_NoOfKeys.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numUpDown_NoOfKeys.ValueChanged += new System.EventHandler(this.numUpDown_NoOfKeys_ValueChanged);
      // 
      // button_reset
      // 
      this.button_reset.Location = new System.Drawing.Point(393, 501);
      this.button_reset.Name = "button_reset";
      this.button_reset.Size = new System.Drawing.Size(75, 23);
      this.button_reset.TabIndex = 12;
      this.button_reset.Text = "Reset all";
      this.button_reset.UseVisualStyleBackColor = true;
      this.button_reset.Click += new System.EventHandler(this.button_reset_Click);
      // 
      // label_CatalogNo
      // 
      this.label_CatalogNo.AutoSize = true;
      this.label_CatalogNo.Location = new System.Drawing.Point(6, 62);
      this.label_CatalogNo.Name = "label_CatalogNo";
      this.label_CatalogNo.Size = new System.Drawing.Size(124, 13);
      this.label_CatalogNo.TabIndex = 2;
      this.label_CatalogNo.Text = "Varenummer/catalog no.";
      // 
      // button_createKey
      // 
      this.button_createKey.Location = new System.Drawing.Point(10, 501);
      this.button_createKey.Name = "button_createKey";
      this.button_createKey.Size = new System.Drawing.Size(75, 23);
      this.button_createKey.TabIndex = 9;
      this.button_createKey.Text = "Create key";
      this.button_createKey.UseVisualStyleBackColor = true;
      this.button_createKey.Click += new System.EventHandler(this.button_makeKey_Click);
      // 
      // button_copy
      // 
      this.button_copy.Enabled = false;
      this.button_copy.Location = new System.Drawing.Point(91, 501);
      this.button_copy.Name = "button_copy";
      this.button_copy.Size = new System.Drawing.Size(75, 23);
      this.button_copy.TabIndex = 10;
      this.button_copy.Text = "Copy";
      this.button_copy.UseVisualStyleBackColor = true;
      this.button_copy.Click += new System.EventHandler(this.button_copy_Click);
      // 
      // textBox_outputActivationKey
      // 
      this.textBox_outputActivationKey.Location = new System.Drawing.Point(10, 445);
      this.textBox_outputActivationKey.Multiline = true;
      this.textBox_outputActivationKey.Name = "textBox_outputActivationKey";
      this.textBox_outputActivationKey.ReadOnly = true;
      this.textBox_outputActivationKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox_outputActivationKey.Size = new System.Drawing.Size(456, 50);
      this.textBox_outputActivationKey.TabIndex = 8;
      // 
      // grpBox_EnterOrderNumber
      // 
      this.grpBox_EnterOrderNumber.Controls.Add(this.btn_OrderNumberOK);
      this.grpBox_EnterOrderNumber.Controls.Add(this.lbl_OrderNo);
      this.grpBox_EnterOrderNumber.Controls.Add(this.txt_OrderNo);
      this.grpBox_EnterOrderNumber.Location = new System.Drawing.Point(7, 7);
      this.grpBox_EnterOrderNumber.Name = "grpBox_EnterOrderNumber";
      this.grpBox_EnterOrderNumber.Size = new System.Drawing.Size(475, 82);
      this.grpBox_EnterOrderNumber.TabIndex = 0;
      this.grpBox_EnterOrderNumber.TabStop = false;
      this.grpBox_EnterOrderNumber.Text = "Enter order number";
      // 
      // btn_OrderNumberOK
      // 
      this.btn_OrderNumberOK.Location = new System.Drawing.Point(185, 49);
      this.btn_OrderNumberOK.Name = "btn_OrderNumberOK";
      this.btn_OrderNumberOK.Size = new System.Drawing.Size(75, 23);
      this.btn_OrderNumberOK.TabIndex = 2;
      this.btn_OrderNumberOK.Text = "OK";
      this.btn_OrderNumberOK.UseVisualStyleBackColor = true;
      this.btn_OrderNumberOK.Click += new System.EventHandler(this.orderNumberOK);
      // 
      // lbl_OrderNo
      // 
      this.lbl_OrderNo.AutoSize = true;
      this.lbl_OrderNo.Location = new System.Drawing.Point(7, 20);
      this.lbl_OrderNo.Name = "lbl_OrderNo";
      this.lbl_OrderNo.Size = new System.Drawing.Size(346, 26);
      this.lbl_OrderNo.TabIndex = 0;
      this.lbl_OrderNo.Text = "An order number has to be entered before you can create a license key.\r\nNB: Pleas" +
    "e make sure to enter it correctly";
      // 
      // txt_OrderNo
      // 
      this.txt_OrderNo.Location = new System.Drawing.Point(7, 51);
      this.txt_OrderNo.Name = "txt_OrderNo";
      this.txt_OrderNo.Size = new System.Drawing.Size(172, 20);
      this.txt_OrderNo.TabIndex = 1;
      this.txt_OrderNo.TextChanged += new System.EventHandler(this.orderNumberOK);
      // 
      // tabPage_Offline
      // 
      this.tabPage_Offline.Controls.Add(this.flowPanel_Offline_Products);
      this.tabPage_Offline.Controls.Add(this.button_Paste);
      this.tabPage_Offline.Controls.Add(this.button_copyOffline);
      this.tabPage_Offline.Controls.Add(this.button_resetOffline);
      this.tabPage_Offline.Controls.Add(this.label_computerName);
      this.tabPage_Offline.Controls.Add(this.textBox_computerName);
      this.tabPage_Offline.Controls.Add(this.textBox_computerId);
      this.tabPage_Offline.Controls.Add(this.textBox_activationKey);
      this.tabPage_Offline.Controls.Add(this.label_computerId);
      this.tabPage_Offline.Controls.Add(this.label_activationKey);
      this.tabPage_Offline.Controls.Add(this.richTextBox_output);
      this.tabPage_Offline.Controls.Add(this.label_destination);
      this.tabPage_Offline.Controls.Add(this.textBox_path);
      this.tabPage_Offline.Controls.Add(this.button_selectDirectoryLicense);
      this.tabPage_Offline.Controls.Add(this.button_generateComputerKey);
      this.tabPage_Offline.Location = new System.Drawing.Point(4, 22);
      this.tabPage_Offline.Name = "tabPage_Offline";
      this.tabPage_Offline.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_Offline.Size = new System.Drawing.Size(486, 629);
      this.tabPage_Offline.TabIndex = 1;
      this.tabPage_Offline.Text = "Offline Activation";
      this.tabPage_Offline.UseVisualStyleBackColor = true;
      // 
      // flowPanel_Offline_Products
      // 
      this.flowPanel_Offline_Products.AutoScroll = true;
      this.flowPanel_Offline_Products.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowPanel_Offline_Products.Location = new System.Drawing.Point(6, 6);
      this.flowPanel_Offline_Products.Name = "flowPanel_Offline_Products";
      this.flowPanel_Offline_Products.Size = new System.Drawing.Size(459, 90);
      this.flowPanel_Offline_Products.TabIndex = 21;
      // 
      // button_Paste
      // 
      this.button_Paste.Location = new System.Drawing.Point(390, 121);
      this.button_Paste.Name = "button_Paste";
      this.button_Paste.Size = new System.Drawing.Size(75, 23);
      this.button_Paste.TabIndex = 20;
      this.button_Paste.Text = "Paste";
      this.button_Paste.UseVisualStyleBackColor = true;
      this.button_Paste.Click += new System.EventHandler(this.button_Paste_Click);
      // 
      // button_copyOffline
      // 
      this.button_copyOffline.Enabled = false;
      this.button_copyOffline.Location = new System.Drawing.Point(6, 389);
      this.button_copyOffline.Name = "button_copyOffline";
      this.button_copyOffline.Size = new System.Drawing.Size(75, 23);
      this.button_copyOffline.TabIndex = 19;
      this.button_copyOffline.Text = "Copy";
      this.button_copyOffline.UseVisualStyleBackColor = true;
      this.button_copyOffline.Click += new System.EventHandler(this.button_copyOffline_Click);
      // 
      // button_resetOffline
      // 
      this.button_resetOffline.Enabled = false;
      this.button_resetOffline.Location = new System.Drawing.Point(140, 209);
      this.button_resetOffline.Name = "button_resetOffline";
      this.button_resetOffline.Size = new System.Drawing.Size(75, 23);
      this.button_resetOffline.TabIndex = 12;
      this.button_resetOffline.Text = "Reset all";
      this.button_resetOffline.UseVisualStyleBackColor = true;
      this.button_resetOffline.Click += new System.EventHandler(this.button_resetOffline_Click);
      // 
      // label_computerName
      // 
      this.label_computerName.AutoSize = true;
      this.label_computerName.Location = new System.Drawing.Point(6, 181);
      this.label_computerName.Name = "label_computerName";
      this.label_computerName.Size = new System.Drawing.Size(84, 13);
      this.label_computerName.TabIndex = 11;
      this.label_computerName.Text = "Computer name:";
      // 
      // textBox_computerName
      // 
      this.textBox_computerName.Location = new System.Drawing.Point(93, 178);
      this.textBox_computerName.Name = "textBox_computerName";
      this.textBox_computerName.ReadOnly = true;
      this.textBox_computerName.Size = new System.Drawing.Size(372, 20);
      this.textBox_computerName.TabIndex = 10;
      // 
      // textBox_computerId
      // 
      this.textBox_computerId.Location = new System.Drawing.Point(93, 151);
      this.textBox_computerId.Name = "textBox_computerId";
      this.textBox_computerId.ReadOnly = true;
      this.textBox_computerId.Size = new System.Drawing.Size(372, 20);
      this.textBox_computerId.TabIndex = 9;
      // 
      // textBox_activationKey
      // 
      this.textBox_activationKey.Location = new System.Drawing.Point(93, 123);
      this.textBox_activationKey.Name = "textBox_activationKey";
      this.textBox_activationKey.Size = new System.Drawing.Size(291, 20);
      this.textBox_activationKey.TabIndex = 8;
      // 
      // label_computerId
      // 
      this.label_computerId.AutoSize = true;
      this.label_computerId.Location = new System.Drawing.Point(6, 154);
      this.label_computerId.Name = "label_computerId";
      this.label_computerId.Size = new System.Drawing.Size(69, 13);
      this.label_computerId.TabIndex = 7;
      this.label_computerId.Text = "Computer ID:";
      // 
      // label_activationKey
      // 
      this.label_activationKey.AutoSize = true;
      this.label_activationKey.Location = new System.Drawing.Point(6, 126);
      this.label_activationKey.Name = "label_activationKey";
      this.label_activationKey.Size = new System.Drawing.Size(77, 13);
      this.label_activationKey.TabIndex = 6;
      this.label_activationKey.Text = "Activation key:";
      // 
      // richTextBox_output
      // 
      this.richTextBox_output.Location = new System.Drawing.Point(6, 238);
      this.richTextBox_output.Name = "richTextBox_output";
      this.richTextBox_output.ReadOnly = true;
      this.richTextBox_output.Size = new System.Drawing.Size(459, 145);
      this.richTextBox_output.TabIndex = 5;
      this.richTextBox_output.Text = "";
      // 
      // label_destination
      // 
      this.label_destination.AutoSize = true;
      this.label_destination.Location = new System.Drawing.Point(6, 100);
      this.label_destination.Name = "label_destination";
      this.label_destination.Size = new System.Drawing.Size(32, 13);
      this.label_destination.TabIndex = 4;
      this.label_destination.Text = "Path:";
      // 
      // textBox_path
      // 
      this.textBox_path.Location = new System.Drawing.Point(93, 97);
      this.textBox_path.Name = "textBox_path";
      this.textBox_path.Size = new System.Drawing.Size(372, 20);
      this.textBox_path.TabIndex = 2;
      // 
      // button_selectDirectoryLicense
      // 
      this.button_selectDirectoryLicense.Enabled = false;
      this.button_selectDirectoryLicense.Location = new System.Drawing.Point(62, 95);
      this.button_selectDirectoryLicense.Name = "button_selectDirectoryLicense";
      this.button_selectDirectoryLicense.Size = new System.Drawing.Size(25, 23);
      this.button_selectDirectoryLicense.TabIndex = 1;
      this.button_selectDirectoryLicense.Text = "...";
      this.button_selectDirectoryLicense.UseVisualStyleBackColor = true;
      this.button_selectDirectoryLicense.Click += new System.EventHandler(this.button_selectDirectory_Click);
      // 
      // button_generateComputerKey
      // 
      this.button_generateComputerKey.AutoSize = true;
      this.button_generateComputerKey.Enabled = false;
      this.button_generateComputerKey.Location = new System.Drawing.Point(6, 209);
      this.button_generateComputerKey.Name = "button_generateComputerKey";
      this.button_generateComputerKey.Size = new System.Drawing.Size(128, 23);
      this.button_generateComputerKey.TabIndex = 0;
      this.button_generateComputerKey.Text = "Generate computer key";
      this.button_generateComputerKey.UseVisualStyleBackColor = true;
      this.button_generateComputerKey.Click += new System.EventHandler(this.button_generateComputerKey_Click);
      // 
      // tabPage_Maintenance
      // 
      this.tabPage_Maintenance.Location = new System.Drawing.Point(4, 22);
      this.tabPage_Maintenance.Name = "tabPage_Maintenance";
      this.tabPage_Maintenance.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage_Maintenance.Size = new System.Drawing.Size(486, 629);
      this.tabPage_Maintenance.TabIndex = 2;
      this.tabPage_Maintenance.Text = "Maintenance";
      this.tabPage_Maintenance.UseVisualStyleBackColor = true;
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(518, 24);
      this.menuStrip.TabIndex = 1;
      this.menuStrip.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
      this.closeToolStripMenuItem.Text = "Close";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.danishToolStripMenuItem,
            this.englishToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // danishToolStripMenuItem
      // 
      this.danishToolStripMenuItem.Name = "danishToolStripMenuItem";
      this.danishToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
      this.danishToolStripMenuItem.Text = "Danish user guide";
      this.danishToolStripMenuItem.Click += new System.EventHandler(this.danishToolStripMenuItem_Click);
      // 
      // englishToolStripMenuItem
      // 
      this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
      this.englishToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
      this.englishToolStripMenuItem.Text = "English user guide";
      this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
      // 
      // lbl_DebugInfo
      // 
      this.lbl_DebugInfo.AutoSize = true;
      this.lbl_DebugInfo.Location = new System.Drawing.Point(167, 3);
      this.lbl_DebugInfo.Name = "lbl_DebugInfo";
      this.lbl_DebugInfo.Size = new System.Drawing.Size(0, 13);
      this.lbl_DebugInfo.TabIndex = 2;
      // 
      // frmKeyGenerator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(518, 695);
      this.Controls.Add(this.lbl_DebugInfo);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.menuStrip);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip;
      this.MaximizeBox = false;
      this.Name = "frmKeyGenerator";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Kamstrup License Key Generator";
      this.tabControl.ResumeLayout(false);
      this.tabPage_ActivationKey.ResumeLayout(false);
      this.grpBox_CreateKey.ResumeLayout(false);
      this.grpBox_CreateKey.PerformLayout();
      this.groupBox_products.ResumeLayout(false);
      this.groupBox_features.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.numUpDown_NoOfKeys)).EndInit();
      this.grpBox_EnterOrderNumber.ResumeLayout(false);
      this.grpBox_EnterOrderNumber.PerformLayout();
      this.tabPage_Offline.ResumeLayout(false);
      this.tabPage_Offline.PerformLayout();
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage_ActivationKey;
        private System.Windows.Forms.TabPage tabPage_Offline;
        private System.Windows.Forms.GroupBox groupBox_features;
        private System.Windows.Forms.GroupBox groupBox_products;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.TextBox textBox_itemNumber;
        private System.Windows.Forms.Button button_reset;
        private System.Windows.Forms.Button button_createKey;
        private System.Windows.Forms.Label label_destination;
        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.Button button_selectDirectoryLicense;
        private System.Windows.Forms.Button button_generateComputerKey;
        private System.Windows.Forms.OpenFileDialog openFileDialog_LicenseFile;
        private System.Windows.Forms.RichTextBox richTextBox_output;
        private System.Windows.Forms.TextBox textBox_outputActivationKey;
        private System.Windows.Forms.Label label_computerName;
        private System.Windows.Forms.TextBox textBox_computerName;
        private System.Windows.Forms.TextBox textBox_computerId;
        private System.Windows.Forms.TextBox textBox_activationKey;
        private System.Windows.Forms.Label label_computerId;
        private System.Windows.Forms.Label label_activationKey;
        private System.Windows.Forms.Button button_resetOffline;
        private System.Windows.Forms.Button button_copy;
        private System.Windows.Forms.Button button_copyOffline;
        private System.Windows.Forms.Label label_CatalogNo;
        private System.Windows.Forms.Button button_Paste;
        private System.Windows.Forms.Label lbl_NoOfKeys;
        private System.Windows.Forms.NumericUpDown numUpDown_NoOfKeys;
        private System.Windows.Forms.Button button_SaveKeys;
        private System.Windows.Forms.Label lbl_info;
        private System.Windows.Forms.FlowLayoutPanel flowPanel_CreateKey_Products;
        private System.Windows.Forms.FlowLayoutPanel flowPanel_CreateKey_Features;
        private System.Windows.Forms.FlowLayoutPanel flowPanel_Offline_Products;
        private System.Windows.Forms.TabPage tabPage_Maintenance;
        private System.Windows.Forms.GroupBox grpBox_EnterOrderNumber;
        private System.Windows.Forms.Label lbl_OrderNo;
        private System.Windows.Forms.TextBox txt_OrderNo;
        private System.Windows.Forms.GroupBox grpBox_CreateKey;
        private System.Windows.Forms.Button btn_OrderNumberOK;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem danishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.Label lbl_DebugInfo;
    }
}

