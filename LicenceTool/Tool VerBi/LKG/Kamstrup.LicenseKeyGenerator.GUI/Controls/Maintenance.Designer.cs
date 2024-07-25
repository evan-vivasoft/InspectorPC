namespace Kamstrup.LicenseKeyGenerator.GUI.Controls
{
  partial class Maintenance
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Maintenance));
      this.tabControl_Maintenance = new System.Windows.Forms.TabControl();
      this.tab_ReleaseDelete = new System.Windows.Forms.TabPage();
      this.grpBox_SearchCriteria = new System.Windows.Forms.GroupBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txt_SearchComputerName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.btn_Search = new System.Windows.Forms.Button();
      this.lbl_AndOr = new System.Windows.Forms.Label();
      this.txt_SearchLicenseKey = new System.Windows.Forms.TextBox();
      this.lbl_LicenseKey = new System.Windows.Forms.Label();
      this.lbl_Info = new System.Windows.Forms.Label();
      this.txt_SearchOrderNo = new System.Windows.Forms.TextBox();
      this.lbl_OrderNo = new System.Windows.Forms.Label();
      this.grpBox_Action = new System.Windows.Forms.GroupBox();
      this.grpBox_Comment = new System.Windows.Forms.GroupBox();
      this.txt_Comment = new System.Windows.Forms.RichTextBox();
      this.grpBox_Action_Delete = new System.Windows.Forms.GroupBox();
      this.btn_Delete = new System.Windows.Forms.Button();
      this.lbl_DeleteLicenseInfo = new System.Windows.Forms.Label();
      this.grpBox_Action_Unlock = new System.Windows.Forms.GroupBox();
      this.btn_Unlock = new System.Windows.Forms.Button();
      this.lbl_Unlock = new System.Windows.Forms.Label();
      this.grpBox_FoundData = new System.Windows.Forms.GroupBox();
      this.dgv_LicenseResult = new System.Windows.Forms.DataGridView();
      this.col_ActivationKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.col_ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.col_OrderNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.col_ReleaseCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.col_ComputerKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.col_ComputerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.col_ComputerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.col_UserData = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tab_ExtraTrail = new System.Windows.Forms.TabPage();
      this.grpBox_EnterOrderNumber = new System.Windows.Forms.GroupBox();
      this.btn_OrderNumberOK = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txt_OrderNo = new System.Windows.Forms.TextBox();
      this.grpBox_CreateTrialKey = new System.Windows.Forms.GroupBox();
      this.lbl_dateCounter = new System.Windows.Forms.Label();
      this.lbl_Trial = new System.Windows.Forms.Label();
      this.lbl_Trial_Info = new System.Windows.Forms.Label();
      this.grpBox_TrialProducts = new System.Windows.Forms.GroupBox();
      this.flowPanel_Trial_Products = new System.Windows.Forms.FlowLayoutPanel();
      this.numUpDown_ExtraTrialDays = new System.Windows.Forms.NumericUpDown();
      this.lbl_Trial_NoOfDays = new System.Windows.Forms.Label();
      this.btn_Trial_CreateTrialKey = new System.Windows.Forms.Button();
      this.btn_GetLog = new System.Windows.Forms.Button();
      this.txt_SearchComputerKey = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.tabControl_Maintenance.SuspendLayout();
      this.tab_ReleaseDelete.SuspendLayout();
      this.grpBox_SearchCriteria.SuspendLayout();
      this.grpBox_Action.SuspendLayout();
      this.grpBox_Comment.SuspendLayout();
      this.grpBox_Action_Delete.SuspendLayout();
      this.grpBox_Action_Unlock.SuspendLayout();
      this.grpBox_FoundData.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_LicenseResult)).BeginInit();
      this.tab_ExtraTrail.SuspendLayout();
      this.grpBox_EnterOrderNumber.SuspendLayout();
      this.grpBox_CreateTrialKey.SuspendLayout();
      this.grpBox_TrialProducts.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numUpDown_ExtraTrialDays)).BeginInit();
      this.SuspendLayout();
      // 
      // tabControl_Maintenance
      // 
      this.tabControl_Maintenance.Controls.Add(this.tab_ReleaseDelete);
      this.tabControl_Maintenance.Controls.Add(this.tab_ExtraTrail);
      this.tabControl_Maintenance.Location = new System.Drawing.Point(3, 3);
      this.tabControl_Maintenance.Name = "tabControl_Maintenance";
      this.tabControl_Maintenance.SelectedIndex = 0;
      this.tabControl_Maintenance.Size = new System.Drawing.Size(468, 562);
      this.tabControl_Maintenance.TabIndex = 0;
      // 
      // tab_ReleaseDelete
      // 
      this.tab_ReleaseDelete.Controls.Add(this.grpBox_SearchCriteria);
      this.tab_ReleaseDelete.Controls.Add(this.grpBox_Action);
      this.tab_ReleaseDelete.Controls.Add(this.grpBox_FoundData);
      this.tab_ReleaseDelete.Location = new System.Drawing.Point(4, 22);
      this.tab_ReleaseDelete.Name = "tab_ReleaseDelete";
      this.tab_ReleaseDelete.Padding = new System.Windows.Forms.Padding(3);
      this.tab_ReleaseDelete.Size = new System.Drawing.Size(460, 536);
      this.tab_ReleaseDelete.TabIndex = 0;
      this.tab_ReleaseDelete.Text = "Release/Delete";
      this.tab_ReleaseDelete.UseVisualStyleBackColor = true;
      // 
      // grpBox_SearchCriteria
      // 
      this.grpBox_SearchCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpBox_SearchCriteria.Controls.Add(this.label5);
      this.grpBox_SearchCriteria.Controls.Add(this.label4);
      this.grpBox_SearchCriteria.Controls.Add(this.txt_SearchComputerKey);
      this.grpBox_SearchCriteria.Controls.Add(this.label3);
      this.grpBox_SearchCriteria.Controls.Add(this.txt_SearchComputerName);
      this.grpBox_SearchCriteria.Controls.Add(this.label2);
      this.grpBox_SearchCriteria.Controls.Add(this.btn_Search);
      this.grpBox_SearchCriteria.Controls.Add(this.lbl_AndOr);
      this.grpBox_SearchCriteria.Controls.Add(this.txt_SearchLicenseKey);
      this.grpBox_SearchCriteria.Controls.Add(this.lbl_LicenseKey);
      this.grpBox_SearchCriteria.Controls.Add(this.lbl_Info);
      this.grpBox_SearchCriteria.Controls.Add(this.txt_SearchOrderNo);
      this.grpBox_SearchCriteria.Controls.Add(this.lbl_OrderNo);
      this.grpBox_SearchCriteria.Location = new System.Drawing.Point(6, 6);
      this.grpBox_SearchCriteria.Name = "grpBox_SearchCriteria";
      this.grpBox_SearchCriteria.Size = new System.Drawing.Size(448, 154);
      this.grpBox_SearchCriteria.TabIndex = 0;
      this.grpBox_SearchCriteria.TabStop = false;
      this.grpBox_SearchCriteria.Text = "Search Criteria";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(367, 74);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(39, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "and/or";
      // 
      // txt_SearchComputerName
      // 
      this.txt_SearchComputerName.Location = new System.Drawing.Point(93, 102);
      this.txt_SearchComputerName.Name = "txt_SearchComputerName";
      this.txt_SearchComputerName.Size = new System.Drawing.Size(268, 20);
      this.txt_SearchComputerName.TabIndex = 8;
      this.txt_SearchComputerName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Search_KeyPress);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 105);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(81, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "Computer name";
      // 
      // btn_Search
      // 
      this.btn_Search.Location = new System.Drawing.Point(370, 126);
      this.btn_Search.Name = "btn_Search";
      this.btn_Search.Size = new System.Drawing.Size(75, 23);
      this.btn_Search.TabIndex = 12;
      this.btn_Search.Text = "Search";
      this.btn_Search.UseVisualStyleBackColor = true;
      this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
      // 
      // lbl_AndOr
      // 
      this.lbl_AndOr.AutoSize = true;
      this.lbl_AndOr.Location = new System.Drawing.Point(367, 48);
      this.lbl_AndOr.Name = "lbl_AndOr";
      this.lbl_AndOr.Size = new System.Drawing.Size(39, 13);
      this.lbl_AndOr.TabIndex = 3;
      this.lbl_AndOr.Text = "and/or";
      // 
      // txt_SearchLicenseKey
      // 
      this.txt_SearchLicenseKey.Location = new System.Drawing.Point(93, 71);
      this.txt_SearchLicenseKey.Name = "txt_SearchLicenseKey";
      this.txt_SearchLicenseKey.Size = new System.Drawing.Size(268, 20);
      this.txt_SearchLicenseKey.TabIndex = 5;
      this.txt_SearchLicenseKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Search_KeyPress);
      // 
      // lbl_LicenseKey
      // 
      this.lbl_LicenseKey.AutoSize = true;
      this.lbl_LicenseKey.Location = new System.Drawing.Point(6, 74);
      this.lbl_LicenseKey.Name = "lbl_LicenseKey";
      this.lbl_LicenseKey.Size = new System.Drawing.Size(64, 13);
      this.lbl_LicenseKey.TabIndex = 4;
      this.lbl_LicenseKey.Text = "License key";
      // 
      // lbl_Info
      // 
      this.lbl_Info.AutoSize = true;
      this.lbl_Info.Location = new System.Drawing.Point(6, 16);
      this.lbl_Info.Name = "lbl_Info";
      this.lbl_Info.Size = new System.Drawing.Size(344, 26);
      this.lbl_Info.TabIndex = 0;
      this.lbl_Info.Text = "To find license key information enter license key and/or order no. below\r\nand cli" +
          "ck search.";
      // 
      // txt_SearchOrderNo
      // 
      this.txt_SearchOrderNo.Location = new System.Drawing.Point(93, 45);
      this.txt_SearchOrderNo.Name = "txt_SearchOrderNo";
      this.txt_SearchOrderNo.Size = new System.Drawing.Size(268, 20);
      this.txt_SearchOrderNo.TabIndex = 2;
      this.txt_SearchOrderNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Search_KeyPress);
      // 
      // lbl_OrderNo
      // 
      this.lbl_OrderNo.AutoSize = true;
      this.lbl_OrderNo.Location = new System.Drawing.Point(6, 48);
      this.lbl_OrderNo.Name = "lbl_OrderNo";
      this.lbl_OrderNo.Size = new System.Drawing.Size(53, 13);
      this.lbl_OrderNo.TabIndex = 1;
      this.lbl_OrderNo.Text = "Order No.";
      // 
      // grpBox_Action
      // 
      this.grpBox_Action.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpBox_Action.Controls.Add(this.grpBox_Comment);
      this.grpBox_Action.Controls.Add(this.grpBox_Action_Delete);
      this.grpBox_Action.Controls.Add(this.grpBox_Action_Unlock);
      this.grpBox_Action.Enabled = false;
      this.grpBox_Action.Location = new System.Drawing.Point(6, 313);
      this.grpBox_Action.Name = "grpBox_Action";
      this.grpBox_Action.Size = new System.Drawing.Size(448, 219);
      this.grpBox_Action.TabIndex = 2;
      this.grpBox_Action.TabStop = false;
      this.grpBox_Action.Text = "Action";
      // 
      // grpBox_Comment
      // 
      this.grpBox_Comment.Controls.Add(this.txt_Comment);
      this.grpBox_Comment.Location = new System.Drawing.Point(7, 127);
      this.grpBox_Comment.Name = "grpBox_Comment";
      this.grpBox_Comment.Size = new System.Drawing.Size(435, 86);
      this.grpBox_Comment.TabIndex = 0;
      this.grpBox_Comment.TabStop = false;
      this.grpBox_Comment.Text = "Comment (optional)";
      // 
      // txt_Comment
      // 
      this.txt_Comment.Location = new System.Drawing.Point(7, 20);
      this.txt_Comment.Name = "txt_Comment";
      this.txt_Comment.Size = new System.Drawing.Size(422, 60);
      this.txt_Comment.TabIndex = 0;
      this.txt_Comment.Text = "";
      // 
      // grpBox_Action_Delete
      // 
      this.grpBox_Action_Delete.Controls.Add(this.btn_Delete);
      this.grpBox_Action_Delete.Controls.Add(this.lbl_DeleteLicenseInfo);
      this.grpBox_Action_Delete.Location = new System.Drawing.Point(232, 21);
      this.grpBox_Action_Delete.Name = "grpBox_Action_Delete";
      this.grpBox_Action_Delete.Size = new System.Drawing.Size(210, 100);
      this.grpBox_Action_Delete.TabIndex = 2;
      this.grpBox_Action_Delete.TabStop = false;
      this.grpBox_Action_Delete.Text = "Delete License";
      // 
      // btn_Delete
      // 
      this.btn_Delete.AutoSize = true;
      this.btn_Delete.Location = new System.Drawing.Point(37, 63);
      this.btn_Delete.Name = "btn_Delete";
      this.btn_Delete.Size = new System.Drawing.Size(127, 23);
      this.btn_Delete.TabIndex = 1;
      this.btn_Delete.Text = "Delete selected license";
      this.btn_Delete.UseVisualStyleBackColor = true;
      this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
      // 
      // lbl_DeleteLicenseInfo
      // 
      this.lbl_DeleteLicenseInfo.AutoSize = true;
      this.lbl_DeleteLicenseInfo.Location = new System.Drawing.Point(7, 20);
      this.lbl_DeleteLicenseInfo.Name = "lbl_DeleteLicenseInfo";
      this.lbl_DeleteLicenseInfo.Size = new System.Drawing.Size(168, 39);
      this.lbl_DeleteLicenseInfo.TabIndex = 0;
      this.lbl_DeleteLicenseInfo.Text = "Deleting a license means it will be \r\ngone for good.\r\nIt can not be used or found" +
          " again.";
      // 
      // grpBox_Action_Unlock
      // 
      this.grpBox_Action_Unlock.Controls.Add(this.btn_Unlock);
      this.grpBox_Action_Unlock.Controls.Add(this.lbl_Unlock);
      this.grpBox_Action_Unlock.Location = new System.Drawing.Point(7, 20);
      this.grpBox_Action_Unlock.Name = "grpBox_Action_Unlock";
      this.grpBox_Action_Unlock.Size = new System.Drawing.Size(210, 100);
      this.grpBox_Action_Unlock.TabIndex = 1;
      this.grpBox_Action_Unlock.TabStop = false;
      this.grpBox_Action_Unlock.Text = "Unlock License";
      // 
      // btn_Unlock
      // 
      this.btn_Unlock.AutoSize = true;
      this.btn_Unlock.Location = new System.Drawing.Point(35, 63);
      this.btn_Unlock.Name = "btn_Unlock";
      this.btn_Unlock.Size = new System.Drawing.Size(130, 23);
      this.btn_Unlock.TabIndex = 1;
      this.btn_Unlock.Text = "Unlock selected license";
      this.btn_Unlock.UseVisualStyleBackColor = true;
      this.btn_Unlock.Click += new System.EventHandler(this.btn_Unlock_Click);
      // 
      // lbl_Unlock
      // 
      this.lbl_Unlock.AutoSize = true;
      this.lbl_Unlock.Location = new System.Drawing.Point(7, 20);
      this.lbl_Unlock.Name = "lbl_Unlock";
      this.lbl_Unlock.Size = new System.Drawing.Size(181, 26);
      this.lbl_Unlock.TabIndex = 0;
      this.lbl_Unlock.Text = "Unlocking a license means it can be \r\nreused on a different computer.";
      // 
      // grpBox_FoundData
      // 
      this.grpBox_FoundData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpBox_FoundData.Controls.Add(this.dgv_LicenseResult);
      this.grpBox_FoundData.Location = new System.Drawing.Point(6, 166);
      this.grpBox_FoundData.Name = "grpBox_FoundData";
      this.grpBox_FoundData.Size = new System.Drawing.Size(448, 141);
      this.grpBox_FoundData.TabIndex = 1;
      this.grpBox_FoundData.TabStop = false;
      this.grpBox_FoundData.Text = "Found data";
      // 
      // dgv_LicenseResult
      // 
      this.dgv_LicenseResult.AllowUserToAddRows = false;
      this.dgv_LicenseResult.AllowUserToDeleteRows = false;
      this.dgv_LicenseResult.AllowUserToResizeRows = false;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dgv_LicenseResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dgv_LicenseResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_LicenseResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ActivationKey,
            this.col_ProductName,
            this.col_OrderNo,
            this.col_ReleaseCount,
            this.col_ComputerKey,
            this.col_ComputerName,
            this.col_ComputerID,
            this.col_UserData});
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dgv_LicenseResult.DefaultCellStyle = dataGridViewCellStyle2;
      this.dgv_LicenseResult.Location = new System.Drawing.Point(7, 20);
      this.dgv_LicenseResult.MultiSelect = false;
      this.dgv_LicenseResult.Name = "dgv_LicenseResult";
      this.dgv_LicenseResult.ReadOnly = true;
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dgv_LicenseResult.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
      this.dgv_LicenseResult.RowHeadersVisible = false;
      this.dgv_LicenseResult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_LicenseResult.Size = new System.Drawing.Size(435, 115);
      this.dgv_LicenseResult.TabIndex = 0;
      this.dgv_LicenseResult.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_LicenseResult_CellContentClick);
      this.dgv_LicenseResult.SelectionChanged += new System.EventHandler(this.dgv_LicenseResult_SelectionChanged);
      // 
      // col_ActivationKey
      // 
      this.col_ActivationKey.DataPropertyName = "ActivationKey";
      this.col_ActivationKey.HeaderText = "License key";
      this.col_ActivationKey.Name = "col_ActivationKey";
      this.col_ActivationKey.ReadOnly = true;
      // 
      // col_ProductName
      // 
      this.col_ProductName.DataPropertyName = "ProductName";
      this.col_ProductName.HeaderText = "Product";
      this.col_ProductName.Name = "col_ProductName";
      this.col_ProductName.ReadOnly = true;
      // 
      // col_OrderNo
      // 
      this.col_OrderNo.DataPropertyName = "OrderNo";
      this.col_OrderNo.HeaderText = "Order no";
      this.col_OrderNo.Name = "col_OrderNo";
      this.col_OrderNo.ReadOnly = true;
      // 
      // col_ReleaseCount
      // 
      this.col_ReleaseCount.DataPropertyName = "ReleaseCount";
      this.col_ReleaseCount.HeaderText = "Unlock count";
      this.col_ReleaseCount.Name = "col_ReleaseCount";
      this.col_ReleaseCount.ReadOnly = true;
      // 
      // col_ComputerKey
      // 
      this.col_ComputerKey.DataPropertyName = "ComputerKey";
      this.col_ComputerKey.HeaderText = "Offline key";
      this.col_ComputerKey.Name = "col_ComputerKey";
      this.col_ComputerKey.ReadOnly = true;
      // 
      // col_ComputerName
      // 
      this.col_ComputerName.DataPropertyName = "ComputerName";
      this.col_ComputerName.HeaderText = "Computer name";
      this.col_ComputerName.Name = "col_ComputerName";
      this.col_ComputerName.ReadOnly = true;
      // 
      // col_ComputerID
      // 
      this.col_ComputerID.DataPropertyName = "ComputerID";
      this.col_ComputerID.HeaderText = "ComputerID";
      this.col_ComputerID.Name = "col_ComputerID";
      this.col_ComputerID.ReadOnly = true;
      this.col_ComputerID.Visible = false;
      // 
      // col_UserData
      // 
      this.col_UserData.DataPropertyName = "UserData";
      this.col_UserData.HeaderText = "UserData";
      this.col_UserData.Name = "col_UserData";
      this.col_UserData.ReadOnly = true;
      this.col_UserData.Visible = false;
      // 
      // tab_ExtraTrail
      // 
      this.tab_ExtraTrail.Controls.Add(this.grpBox_EnterOrderNumber);
      this.tab_ExtraTrail.Controls.Add(this.grpBox_CreateTrialKey);
      this.tab_ExtraTrail.Location = new System.Drawing.Point(4, 22);
      this.tab_ExtraTrail.Name = "tab_ExtraTrail";
      this.tab_ExtraTrail.Padding = new System.Windows.Forms.Padding(3);
      this.tab_ExtraTrail.Size = new System.Drawing.Size(460, 512);
      this.tab_ExtraTrail.TabIndex = 1;
      this.tab_ExtraTrail.Text = "Extra trial days";
      this.tab_ExtraTrail.UseVisualStyleBackColor = true;
      // 
      // grpBox_EnterOrderNumber
      // 
      this.grpBox_EnterOrderNumber.Controls.Add(this.btn_OrderNumberOK);
      this.grpBox_EnterOrderNumber.Controls.Add(this.label1);
      this.grpBox_EnterOrderNumber.Controls.Add(this.txt_OrderNo);
      this.grpBox_EnterOrderNumber.Location = new System.Drawing.Point(6, 6);
      this.grpBox_EnterOrderNumber.Name = "grpBox_EnterOrderNumber";
      this.grpBox_EnterOrderNumber.Size = new System.Drawing.Size(448, 82);
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
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(346, 26);
      this.label1.TabIndex = 0;
      this.label1.Text = "An order number has to be entered before you can create a license key.\r\nNB: Pleas" +
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
      // grpBox_CreateTrialKey
      // 
      this.grpBox_CreateTrialKey.Controls.Add(this.lbl_dateCounter);
      this.grpBox_CreateTrialKey.Controls.Add(this.lbl_Trial);
      this.grpBox_CreateTrialKey.Controls.Add(this.lbl_Trial_Info);
      this.grpBox_CreateTrialKey.Controls.Add(this.grpBox_TrialProducts);
      this.grpBox_CreateTrialKey.Controls.Add(this.numUpDown_ExtraTrialDays);
      this.grpBox_CreateTrialKey.Controls.Add(this.lbl_Trial_NoOfDays);
      this.grpBox_CreateTrialKey.Controls.Add(this.btn_Trial_CreateTrialKey);
      this.grpBox_CreateTrialKey.Enabled = false;
      this.grpBox_CreateTrialKey.Location = new System.Drawing.Point(7, 94);
      this.grpBox_CreateTrialKey.Name = "grpBox_CreateTrialKey";
      this.grpBox_CreateTrialKey.Size = new System.Drawing.Size(447, 340);
      this.grpBox_CreateTrialKey.TabIndex = 1;
      this.grpBox_CreateTrialKey.TabStop = false;
      this.grpBox_CreateTrialKey.Text = "Create trial key";
      // 
      // lbl_dateCounter
      // 
      this.lbl_dateCounter.AutoSize = true;
      this.lbl_dateCounter.Enabled = false;
      this.lbl_dateCounter.Location = new System.Drawing.Point(223, 217);
      this.lbl_dateCounter.Name = "lbl_dateCounter";
      this.lbl_dateCounter.Size = new System.Drawing.Size(0, 13);
      this.lbl_dateCounter.TabIndex = 4;
      // 
      // lbl_Trial
      // 
      this.lbl_Trial.AutoSize = true;
      this.lbl_Trial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbl_Trial.Location = new System.Drawing.Point(6, 16);
      this.lbl_Trial.Name = "lbl_Trial";
      this.lbl_Trial.Size = new System.Drawing.Size(324, 78);
      this.lbl_Trial.TabIndex = 0;
      this.lbl_Trial.Text = resources.GetString("lbl_Trial.Text");
      // 
      // lbl_Trial_Info
      // 
      this.lbl_Trial_Info.AutoSize = true;
      this.lbl_Trial_Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbl_Trial_Info.ForeColor = System.Drawing.SystemColors.ControlText;
      this.lbl_Trial_Info.Location = new System.Drawing.Point(6, 237);
      this.lbl_Trial_Info.Name = "lbl_Trial_Info";
      this.lbl_Trial_Info.Size = new System.Drawing.Size(403, 65);
      this.lbl_Trial_Info.TabIndex = 6;
      this.lbl_Trial_Info.Text = resources.GetString("lbl_Trial_Info.Text");
      // 
      // grpBox_TrialProducts
      // 
      this.grpBox_TrialProducts.Controls.Add(this.flowPanel_Trial_Products);
      this.grpBox_TrialProducts.Location = new System.Drawing.Point(5, 97);
      this.grpBox_TrialProducts.Name = "grpBox_TrialProducts";
      this.grpBox_TrialProducts.Size = new System.Drawing.Size(436, 110);
      this.grpBox_TrialProducts.TabIndex = 1;
      this.grpBox_TrialProducts.TabStop = false;
      this.grpBox_TrialProducts.Text = "Products";
      // 
      // flowPanel_Trial_Products
      // 
      this.flowPanel_Trial_Products.AutoScroll = true;
      this.flowPanel_Trial_Products.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowPanel_Trial_Products.Location = new System.Drawing.Point(6, 14);
      this.flowPanel_Trial_Products.Name = "flowPanel_Trial_Products";
      this.flowPanel_Trial_Products.Size = new System.Drawing.Size(424, 90);
      this.flowPanel_Trial_Products.TabIndex = 0;
      // 
      // numUpDown_ExtraTrialDays
      // 
      this.numUpDown_ExtraTrialDays.Increment = new decimal(new int[] {
            7,
            0,
            0,
            0});
      this.numUpDown_ExtraTrialDays.Location = new System.Drawing.Point(97, 214);
      this.numUpDown_ExtraTrialDays.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
      this.numUpDown_ExtraTrialDays.Name = "numUpDown_ExtraTrialDays";
      this.numUpDown_ExtraTrialDays.Size = new System.Drawing.Size(120, 20);
      this.numUpDown_ExtraTrialDays.TabIndex = 3;
      this.numUpDown_ExtraTrialDays.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
      this.numUpDown_ExtraTrialDays.ValueChanged += new System.EventHandler(this.numUpDown_ExtraTrialDays_ValueChanged);
      // 
      // lbl_Trial_NoOfDays
      // 
      this.lbl_Trial_NoOfDays.AutoSize = true;
      this.lbl_Trial_NoOfDays.Location = new System.Drawing.Point(6, 217);
      this.lbl_Trial_NoOfDays.Name = "lbl_Trial_NoOfDays";
      this.lbl_Trial_NoOfDays.Size = new System.Drawing.Size(84, 13);
      this.lbl_Trial_NoOfDays.TabIndex = 2;
      this.lbl_Trial_NoOfDays.Text = "Number of days:";
      // 
      // btn_Trial_CreateTrialKey
      // 
      this.btn_Trial_CreateTrialKey.AutoSize = true;
      this.btn_Trial_CreateTrialKey.Location = new System.Drawing.Point(349, 211);
      this.btn_Trial_CreateTrialKey.Name = "btn_Trial_CreateTrialKey";
      this.btn_Trial_CreateTrialKey.Size = new System.Drawing.Size(92, 23);
      this.btn_Trial_CreateTrialKey.TabIndex = 5;
      this.btn_Trial_CreateTrialKey.Text = "Create Trial Key";
      this.btn_Trial_CreateTrialKey.UseVisualStyleBackColor = true;
      this.btn_Trial_CreateTrialKey.Click += new System.EventHandler(this.btn_Trial_CreateTrialKey_Click);
      // 
      // btn_GetLog
      // 
      this.btn_GetLog.AutoSize = true;
      this.btn_GetLog.Location = new System.Drawing.Point(113, 589);
      this.btn_GetLog.Name = "btn_GetLog";
      this.btn_GetLog.Size = new System.Drawing.Size(249, 36);
      this.btn_GetLog.TabIndex = 1;
      this.btn_GetLog.Text = "Get log\r\n(released and deleted keys + created trial keys)\r\n";
      this.btn_GetLog.UseVisualStyleBackColor = true;
      this.btn_GetLog.Click += new System.EventHandler(this.btn_GetLog_Click);
      // 
      // txt_SearchComputerKey
      // 
      this.txt_SearchComputerKey.Location = new System.Drawing.Point(93, 128);
      this.txt_SearchComputerKey.Name = "txt_SearchComputerKey";
      this.txt_SearchComputerKey.Size = new System.Drawing.Size(268, 20);
      this.txt_SearchComputerKey.TabIndex = 11;
      this.txt_SearchComputerKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Search_KeyPress);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(367, 105);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(39, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "and/or";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 135);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(57, 13);
      this.label5.TabIndex = 10;
      this.label5.Text = "Offline key";
      // 
      // Maintenance
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btn_GetLog);
      this.Controls.Add(this.tabControl_Maintenance);
      this.Name = "Maintenance";
      this.Size = new System.Drawing.Size(474, 647);
      this.tabControl_Maintenance.ResumeLayout(false);
      this.tab_ReleaseDelete.ResumeLayout(false);
      this.grpBox_SearchCriteria.ResumeLayout(false);
      this.grpBox_SearchCriteria.PerformLayout();
      this.grpBox_Action.ResumeLayout(false);
      this.grpBox_Comment.ResumeLayout(false);
      this.grpBox_Action_Delete.ResumeLayout(false);
      this.grpBox_Action_Delete.PerformLayout();
      this.grpBox_Action_Unlock.ResumeLayout(false);
      this.grpBox_Action_Unlock.PerformLayout();
      this.grpBox_FoundData.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgv_LicenseResult)).EndInit();
      this.tab_ExtraTrail.ResumeLayout(false);
      this.grpBox_EnterOrderNumber.ResumeLayout(false);
      this.grpBox_EnterOrderNumber.PerformLayout();
      this.grpBox_CreateTrialKey.ResumeLayout(false);
      this.grpBox_CreateTrialKey.PerformLayout();
      this.grpBox_TrialProducts.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.numUpDown_ExtraTrialDays)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl_Maintenance;
    private System.Windows.Forms.TabPage tab_ReleaseDelete;
    private System.Windows.Forms.GroupBox grpBox_SearchCriteria;
    private System.Windows.Forms.Button btn_Search;
    private System.Windows.Forms.Label lbl_AndOr;
    private System.Windows.Forms.TextBox txt_SearchLicenseKey;
    private System.Windows.Forms.Label lbl_LicenseKey;
    private System.Windows.Forms.Label lbl_Info;
    private System.Windows.Forms.TextBox txt_SearchOrderNo;
    private System.Windows.Forms.Label lbl_OrderNo;
    private System.Windows.Forms.GroupBox grpBox_Action;
    private System.Windows.Forms.GroupBox grpBox_Action_Delete;
    private System.Windows.Forms.GroupBox grpBox_Action_Unlock;
    private System.Windows.Forms.GroupBox grpBox_FoundData;
    private System.Windows.Forms.TabPage tab_ExtraTrail;
    private System.Windows.Forms.NumericUpDown numUpDown_ExtraTrialDays;
    private System.Windows.Forms.Button btn_Trial_CreateTrialKey;
    private System.Windows.Forms.Label lbl_Trial_NoOfDays;
    private System.Windows.Forms.Label lbl_Trial;
    private System.Windows.Forms.GroupBox grpBox_TrialProducts;
    private System.Windows.Forms.FlowLayoutPanel flowPanel_Trial_Products;
    private System.Windows.Forms.Label lbl_Trial_Info;
    private System.Windows.Forms.Label lbl_dateCounter;
    private System.Windows.Forms.GroupBox grpBox_CreateTrialKey;
    private System.Windows.Forms.GroupBox grpBox_EnterOrderNumber;
    private System.Windows.Forms.Button btn_OrderNumberOK;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txt_OrderNo;
    private System.Windows.Forms.DataGridView dgv_LicenseResult;
    private System.Windows.Forms.Label lbl_Unlock;
    private System.Windows.Forms.Label lbl_DeleteLicenseInfo;
    private System.Windows.Forms.Button btn_Unlock;
    private System.Windows.Forms.Button btn_Delete;
    private System.Windows.Forms.GroupBox grpBox_Comment;
    private System.Windows.Forms.RichTextBox txt_Comment;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txt_SearchComputerName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_ActivationKey;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_ProductName;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_OrderNo;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_ReleaseCount;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_ComputerKey;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_ComputerName;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_ComputerID;
    private System.Windows.Forms.DataGridViewTextBoxColumn col_UserData;
    private System.Windows.Forms.Button btn_GetLog;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txt_SearchComputerKey;
  }
}
