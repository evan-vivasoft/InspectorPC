<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_ScriptCommand41_ControlList

    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_ScriptCommand41_ControlList))
        Dim GridViewTextBoxColumn1 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewTextBoxColumn2 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewTextBoxColumn3 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewTextBoxColumn4 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewTextBoxColumn5 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Me.RadGroupBox1 = New Telerik.WinControls.UI.RadGroupBox()
        Me.rdCheckListResult = New Telerik.WinControls.UI.RadCheckBox()
        Me.rdCheckOneSelectionAllowed = New Telerik.WinControls.UI.RadCheckBox()
        Me.rdCheckRequired = New Telerik.WinControls.UI.RadCheckBox()
        Me.rdGridConditionCodes = New Telerik.WinControls.UI.RadGridView()
        Me.rdtbInstruction = New Telerik.WinControls.UI.RadTextBox()
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadGroupBox1.SuspendLayout()
        CType(Me.rdCheckListResult, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdCheckOneSelectionAllowed, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdCheckRequired, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridConditionCodes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdGridConditionCodes.SuspendLayout()
        CType(Me.rdtbInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadGroupBox1
        '
        Me.RadGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        resources.ApplyResources(Me.RadGroupBox1, "RadGroupBox1")
        Me.RadGroupBox1.Controls.Add(Me.rdCheckListResult)
        Me.RadGroupBox1.Controls.Add(Me.rdCheckOneSelectionAllowed)
        Me.RadGroupBox1.Controls.Add(Me.rdCheckRequired)
        Me.RadGroupBox1.Name = "RadGroupBox1"
        '
        '
        '
        Me.RadGroupBox1.RootElement.Padding = CType(resources.GetObject("RadGroupBox1.RootElement.Padding"), System.Windows.Forms.Padding)
        '
        'rdCheckListResult
        '
        resources.ApplyResources(Me.rdCheckListResult, "rdCheckListResult")
        Me.rdCheckListResult.Name = "rdCheckListResult"
        '
        'rdCheckOneSelectionAllowed
        '
        resources.ApplyResources(Me.rdCheckOneSelectionAllowed, "rdCheckOneSelectionAllowed")
        Me.rdCheckOneSelectionAllowed.Name = "rdCheckOneSelectionAllowed"
        '
        'rdCheckRequired
        '
        resources.ApplyResources(Me.rdCheckRequired, "rdCheckRequired")
        Me.rdCheckRequired.Name = "rdCheckRequired"
        '
        'rdGridConditionCodes
        '
        resources.ApplyResources(Me.rdGridConditionCodes, "rdGridConditionCodes")
        Me.rdGridConditionCodes.Controls.Add(Me.RadGroupBox1)
        '
        'rdGridConditionCodes
        '
        Me.rdGridConditionCodes.MasterTemplate.AllowAddNewRow = False
        Me.rdGridConditionCodes.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill
        resources.ApplyResources(GridViewTextBoxColumn1, "GridViewTextBoxColumn1")
        GridViewTextBoxColumn1.Name = "column1"
        GridViewTextBoxColumn1.Width = 102
        resources.ApplyResources(GridViewTextBoxColumn2, "GridViewTextBoxColumn2")
        GridViewTextBoxColumn2.Name = "column2"
        GridViewTextBoxColumn2.Width = 102
        resources.ApplyResources(GridViewTextBoxColumn3, "GridViewTextBoxColumn3")
        GridViewTextBoxColumn3.Name = "column3"
        GridViewTextBoxColumn3.ReadOnly = True
        GridViewTextBoxColumn3.Width = 102
        resources.ApplyResources(GridViewTextBoxColumn4, "GridViewTextBoxColumn4")
        GridViewTextBoxColumn4.Name = "column4"
        GridViewTextBoxColumn4.ReadOnly = True
        GridViewTextBoxColumn4.Width = 102
        resources.ApplyResources(GridViewTextBoxColumn5, "GridViewTextBoxColumn5")
        GridViewTextBoxColumn5.Name = "column5"
        GridViewTextBoxColumn5.ReadOnly = True
        GridViewTextBoxColumn5.Width = 96
        Me.rdGridConditionCodes.MasterTemplate.Columns.AddRange(New Telerik.WinControls.UI.GridViewDataColumn() {GridViewTextBoxColumn1, GridViewTextBoxColumn2, GridViewTextBoxColumn3, GridViewTextBoxColumn4, GridViewTextBoxColumn5})
        Me.rdGridConditionCodes.MasterTemplate.EnableGrouping = False
        Me.rdGridConditionCodes.Name = "rdGridConditionCodes"
        '
        '
        '
        Me.rdGridConditionCodes.RootElement.Padding = CType(resources.GetObject("rdGridConditionCodes.RootElement.Padding"), System.Windows.Forms.Padding)
        '
        'rdtbInstruction
        '
        resources.ApplyResources(Me.rdtbInstruction, "rdtbInstruction")
        Me.rdtbInstruction.Multiline = True
        Me.rdtbInstruction.Name = "rdtbInstruction"
        Me.rdtbInstruction.ReadOnly = True
        Me.rdtbInstruction.TabStop = False
        '
        'usctrl_ScriptCommand41_ControlList
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.rdtbInstruction)
        Me.Controls.Add(Me.rdGridConditionCodes)
        Me.Name = "usctrl_ScriptCommand41_ControlList"
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadGroupBox1.ResumeLayout(False)
        Me.RadGroupBox1.PerformLayout()
        CType(Me.rdCheckListResult, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdCheckOneSelectionAllowed, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdCheckRequired, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridConditionCodes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdGridConditionCodes.ResumeLayout(False)
        CType(Me.rdtbInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RadGroupBox1 As Telerik.WinControls.UI.RadGroupBox
    Friend WithEvents rdCheckListResult As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents rdCheckOneSelectionAllowed As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents rdCheckRequired As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents rdGridConditionCodes As Telerik.WinControls.UI.RadGridView
    Friend WithEvents rdtbInstruction As Telerik.WinControls.UI.RadTextBox

End Class
