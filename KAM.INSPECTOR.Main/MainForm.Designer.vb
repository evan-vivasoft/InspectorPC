<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits Telerik.WinControls.UI.RadForm

    'Form overrides dispose to clean up the component list.
    ' ''<System.Diagnostics.DebuggerNonUserCode()> _
    ' ''Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    ' ''    Try
    ' ''        If disposing AndAlso components IsNot Nothing Then
    ' ''            components.Dispose()
    ' ''        End If
    ' ''    Finally
    ' ''        MyBase.Dispose(disposing)
    ' ''    End Try
    ' ''End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.rdDockMain = New Telerik.WinControls.UI.Docking.RadDock()
        Me.MainDocumentContainer = New Telerik.WinControls.UI.Docking.DocumentContainer()
        Me.DocumentTabStrip2 = New Telerik.WinControls.UI.Docking.DocumentTabStrip()
        Me.DocWindowPRS = New Telerik.WinControls.UI.Docking.DocumentWindow()
        Me.DocWindowInspection = New Telerik.WinControls.UI.Docking.DocumentWindow()
        Me.DocWindowPLEXOR = New Telerik.WinControls.UI.Docking.DocumentWindow()
        Me.DocWindowResults = New Telerik.WinControls.UI.Docking.DocumentWindow()
        Me.ToolTabStrip1 = New Telerik.WinControls.UI.Docking.ToolTabStrip()
        Me.toolWindowRemarks = New Telerik.WinControls.UI.Docking.ToolWindow()
        Me.rdMenuExtra = New Telerik.WinControls.UI.RadMenuItem()
        Me.rdMenuSettings = New Telerik.WinControls.UI.RadMenuItem()
        Me.RadMenuSeparatorItem1 = New Telerik.WinControls.UI.RadMenuSeparatorItem()
        Me.rdMenuExit = New Telerik.WinControls.UI.RadMenuItem()
        Me.RadMenu1 = New Telerik.WinControls.UI.RadMenu()
        Me.rdMenuHelp = New Telerik.WinControls.UI.RadMenuItem()
        Me.rdMenuAbout = New Telerik.WinControls.UI.RadMenuItem()
        Me.HelpProvider1 = New System.Windows.Forms.HelpProvider()
        Me.DocWindowSync = New Telerik.WinControls.UI.Docking.DocumentWindow()
        CType(Me.rdDockMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdDockMain.SuspendLayout()
        CType(Me.MainDocumentContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MainDocumentContainer.SuspendLayout()
        CType(Me.DocumentTabStrip2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DocumentTabStrip2.SuspendLayout()
        CType(Me.ToolTabStrip1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ToolTabStrip1.SuspendLayout()
        CType(Me.RadMenu1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdDockMain
        '
        Me.rdDockMain.ActiveWindow = Me.DocWindowSync
        Me.rdDockMain.CausesValidation = False
        Me.rdDockMain.Controls.Add(Me.MainDocumentContainer)
        Me.rdDockMain.Controls.Add(Me.ToolTabStrip1)
        resources.ApplyResources(Me.rdDockMain, "rdDockMain")
        Me.rdDockMain.IsCleanUpTarget = True
        Me.rdDockMain.MainDocumentContainer = Me.MainDocumentContainer
        Me.rdDockMain.Name = "rdDockMain"
        '
        '
        '
        Me.rdDockMain.RootElement.AccessibleDescription = resources.GetString("rdDockMain.RootElement.AccessibleDescription")
        Me.rdDockMain.RootElement.AccessibleName = resources.GetString("rdDockMain.RootElement.AccessibleName")
        Me.rdDockMain.RootElement.Alignment = CType(resources.GetObject("rdDockMain.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdDockMain.RootElement.AngleTransform = CType(resources.GetObject("rdDockMain.RootElement.AngleTransform"), Single)
        Me.rdDockMain.RootElement.FlipText = CType(resources.GetObject("rdDockMain.RootElement.FlipText"), Boolean)
        Me.rdDockMain.RootElement.Margin = CType(resources.GetObject("rdDockMain.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdDockMain.RootElement.MinSize = New System.Drawing.Size(0, 0)
        Me.rdDockMain.RootElement.Text = resources.GetString("rdDockMain.RootElement.Text")
        Me.rdDockMain.RootElement.TextOrientation = CType(resources.GetObject("rdDockMain.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.rdDockMain.TabStop = False
        '
        'MainDocumentContainer
        '
        Me.MainDocumentContainer.CausesValidation = False
        Me.MainDocumentContainer.Controls.Add(Me.DocumentTabStrip2)
        Me.MainDocumentContainer.Name = "MainDocumentContainer"
        '
        '
        '
        Me.MainDocumentContainer.RootElement.AccessibleDescription = resources.GetString("MainDocumentContainer.RootElement.AccessibleDescription")
        Me.MainDocumentContainer.RootElement.AccessibleName = resources.GetString("MainDocumentContainer.RootElement.AccessibleName")
        Me.MainDocumentContainer.RootElement.Alignment = CType(resources.GetObject("MainDocumentContainer.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.MainDocumentContainer.RootElement.AngleTransform = CType(resources.GetObject("MainDocumentContainer.RootElement.AngleTransform"), Single)
        Me.MainDocumentContainer.RootElement.FlipText = CType(resources.GetObject("MainDocumentContainer.RootElement.FlipText"), Boolean)
        Me.MainDocumentContainer.RootElement.Margin = CType(resources.GetObject("MainDocumentContainer.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.MainDocumentContainer.RootElement.MinSize = New System.Drawing.Size(0, 0)
        Me.MainDocumentContainer.RootElement.Text = resources.GetString("MainDocumentContainer.RootElement.Text")
        Me.MainDocumentContainer.RootElement.TextOrientation = CType(resources.GetObject("MainDocumentContainer.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.MainDocumentContainer.SizeInfo.AbsoluteSize = New System.Drawing.Size(858, 440)
        Me.MainDocumentContainer.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Fill
        Me.MainDocumentContainer.SizeInfo.SplitterCorrection = New System.Drawing.Size(7, 100)
        '
        'DocumentTabStrip2
        '
        Me.DocumentTabStrip2.CanUpdateChildIndex = True
        Me.DocumentTabStrip2.CausesValidation = False
        Me.DocumentTabStrip2.Controls.Add(Me.DocWindowSync)
        Me.DocumentTabStrip2.Controls.Add(Me.DocWindowPRS)
        Me.DocumentTabStrip2.Controls.Add(Me.DocWindowInspection)
        Me.DocumentTabStrip2.Controls.Add(Me.DocWindowPLEXOR)
        Me.DocumentTabStrip2.Controls.Add(Me.DocWindowResults)
        resources.ApplyResources(Me.DocumentTabStrip2, "DocumentTabStrip2")
        Me.DocumentTabStrip2.Name = "DocumentTabStrip2"
        '
        '
        '
        Me.DocumentTabStrip2.RootElement.AccessibleDescription = resources.GetString("DocumentTabStrip2.RootElement.AccessibleDescription")
        Me.DocumentTabStrip2.RootElement.AccessibleName = resources.GetString("DocumentTabStrip2.RootElement.AccessibleName")
        Me.DocumentTabStrip2.RootElement.Alignment = CType(resources.GetObject("DocumentTabStrip2.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.DocumentTabStrip2.RootElement.AngleTransform = CType(resources.GetObject("DocumentTabStrip2.RootElement.AngleTransform"), Single)
        Me.DocumentTabStrip2.RootElement.FlipText = CType(resources.GetObject("DocumentTabStrip2.RootElement.FlipText"), Boolean)
        Me.DocumentTabStrip2.RootElement.Margin = CType(resources.GetObject("DocumentTabStrip2.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.DocumentTabStrip2.RootElement.MinSize = New System.Drawing.Size(0, 0)
        Me.DocumentTabStrip2.RootElement.Text = resources.GetString("DocumentTabStrip2.RootElement.Text")
        Me.DocumentTabStrip2.RootElement.TextOrientation = CType(resources.GetObject("DocumentTabStrip2.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.DocumentTabStrip2.SelectedIndex = 0
        Me.DocumentTabStrip2.TabStop = False
        '
        'DocWindowPRS
        '
        Me.DocWindowPRS.DocumentButtons = Telerik.WinControls.UI.Docking.DocumentStripButtons.None
        resources.ApplyResources(Me.DocWindowPRS, "DocWindowPRS")
        Me.DocWindowPRS.ForeColor = System.Drawing.Color.Black
        Me.DocWindowPRS.Name = "DocWindowPRS"
        Me.DocWindowPRS.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument
        Me.HelpProvider1.SetShowHelp(Me.DocWindowPRS, CType(resources.GetObject("DocWindowPRS.ShowHelp"), Boolean))
        '
        'DocWindowInspection
        '
        Me.DocWindowInspection.DocumentButtons = Telerik.WinControls.UI.Docking.DocumentStripButtons.None
        resources.ApplyResources(Me.DocWindowInspection, "DocWindowInspection")
        Me.DocWindowInspection.Name = "DocWindowInspection"
        Me.DocWindowInspection.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument
        '
        'DocWindowPLEXOR
        '
        Me.DocWindowPLEXOR.DocumentButtons = Telerik.WinControls.UI.Docking.DocumentStripButtons.None
        resources.ApplyResources(Me.DocWindowPLEXOR, "DocWindowPLEXOR")
        Me.DocWindowPLEXOR.Name = "DocWindowPLEXOR"
        Me.DocWindowPLEXOR.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument
        '
        'DocWindowResults
        '
        Me.DocWindowResults.DocumentButtons = Telerik.WinControls.UI.Docking.DocumentStripButtons.None
        resources.ApplyResources(Me.DocWindowResults, "DocWindowResults")
        Me.DocWindowResults.Name = "DocWindowResults"
        Me.DocWindowResults.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument
        '
        'ToolTabStrip1
        '
        Me.ToolTabStrip1.CanUpdateChildIndex = True
        Me.ToolTabStrip1.Controls.Add(Me.toolWindowRemarks)
        resources.ApplyResources(Me.ToolTabStrip1, "ToolTabStrip1")
        Me.ToolTabStrip1.Name = "ToolTabStrip1"
        '
        '
        '
        Me.ToolTabStrip1.RootElement.MinSize = New System.Drawing.Size(0, 0)
        Me.ToolTabStrip1.SelectedIndex = 0
        Me.ToolTabStrip1.SizeInfo.AbsoluteSize = New System.Drawing.Size(193, 100)
        Me.ToolTabStrip1.SizeInfo.SplitterCorrection = New System.Drawing.Size(-7, -100)
        Me.ToolTabStrip1.TabStop = False
        '
        'toolWindowRemarks
        '
        Me.toolWindowRemarks.Caption = Nothing
        resources.ApplyResources(Me.toolWindowRemarks, "toolWindowRemarks")
        Me.toolWindowRemarks.Name = "toolWindowRemarks"
        Me.toolWindowRemarks.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.Docked
        '
        'rdMenuExtra
        '
        resources.ApplyResources(Me.rdMenuExtra, "rdMenuExtra")
        Me.rdMenuExtra.Items.AddRange(New Telerik.WinControls.RadItem() {Me.rdMenuSettings, Me.RadMenuSeparatorItem1, Me.rdMenuExit})
        Me.rdMenuExtra.Name = "rdMenuExtra"
        '
        'rdMenuSettings
        '
        resources.ApplyResources(Me.rdMenuSettings, "rdMenuSettings")
        Me.rdMenuSettings.Name = "rdMenuSettings"
        '
        'RadMenuSeparatorItem1
        '
        Me.RadMenuSeparatorItem1.Name = "RadMenuSeparatorItem1"
        resources.ApplyResources(Me.RadMenuSeparatorItem1, "RadMenuSeparatorItem1")
        '
        'rdMenuExit
        '
        resources.ApplyResources(Me.rdMenuExit, "rdMenuExit")
        Me.rdMenuExit.Name = "rdMenuExit"
        '
        'RadMenu1
        '
        Me.RadMenu1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.rdMenuExtra, Me.rdMenuHelp})
        resources.ApplyResources(Me.RadMenu1, "RadMenu1")
        Me.RadMenu1.Name = "RadMenu1"
        '
        '
        '
        Me.RadMenu1.RootElement.Alignment = CType(resources.GetObject("RadMenu1.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RadMenu1.RootElement.AngleTransform = CType(resources.GetObject("RadMenu1.RootElement.AngleTransform"), Single)
        Me.RadMenu1.RootElement.FlipText = CType(resources.GetObject("RadMenu1.RootElement.FlipText"), Boolean)
        Me.RadMenu1.RootElement.Margin = CType(resources.GetObject("RadMenu1.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RadMenu1.RootElement.Text = resources.GetString("RadMenu1.RootElement.Text")
        Me.RadMenu1.RootElement.TextOrientation = CType(resources.GetObject("RadMenu1.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdMenuHelp
        '
        resources.ApplyResources(Me.rdMenuHelp, "rdMenuHelp")
        Me.rdMenuHelp.Items.AddRange(New Telerik.WinControls.RadItem() {Me.rdMenuAbout})
        Me.rdMenuHelp.Name = "rdMenuHelp"
        '
        'rdMenuAbout
        '
        resources.ApplyResources(Me.rdMenuAbout, "rdMenuAbout")
        Me.rdMenuAbout.Name = "rdMenuAbout"
        '
        'HelpProvider1
        '
        resources.ApplyResources(Me.HelpProvider1, "HelpProvider1")
        '
        'DocWindowSync
        '
        Me.DocWindowSync.DocumentButtons = Telerik.WinControls.UI.Docking.DocumentStripButtons.None
        resources.ApplyResources(Me.DocWindowSync, "DocWindowSync")
        Me.DocWindowSync.Name = "DocWindowSync"
        Me.DocWindowSync.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument
        '
        'MainForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.rdDockMain)
        Me.Controls.Add(Me.RadMenu1)
        Me.Name = "MainForm"
        '
        '
        '
        Me.RootElement.AccessibleDescription = resources.GetString("MainForm.RootElement.AccessibleDescription")
        Me.RootElement.AccessibleName = resources.GetString("MainForm.RootElement.AccessibleName")
        Me.RootElement.Alignment = CType(resources.GetObject("MainForm.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RootElement.AngleTransform = CType(resources.GetObject("MainForm.RootElement.AngleTransform"), Single)
        Me.RootElement.ApplyShapeToControl = True
        Me.RootElement.FlipText = CType(resources.GetObject("MainForm.RootElement.FlipText"), Boolean)
        Me.RootElement.Margin = CType(resources.GetObject("MainForm.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RootElement.Text = resources.GetString("MainForm.RootElement.Text")
        Me.RootElement.TextOrientation = CType(resources.GetObject("MainForm.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        CType(Me.rdDockMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdDockMain.ResumeLayout(False)
        CType(Me.MainDocumentContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MainDocumentContainer.ResumeLayout(False)
        CType(Me.DocumentTabStrip2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DocumentTabStrip2.ResumeLayout(False)
        CType(Me.ToolTabStrip1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ToolTabStrip1.ResumeLayout(False)
        CType(Me.RadMenu1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdDockMain As Telerik.WinControls.UI.Docking.RadDock
    Friend WithEvents MainDocumentContainer As Telerik.WinControls.UI.Docking.DocumentContainer
    Friend WithEvents DocWindowInspection As Telerik.WinControls.UI.Docking.DocumentWindow
    Friend WithEvents rdMenuExtra As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rdMenuSettings As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents DocWindowPLEXOR As Telerik.WinControls.UI.Docking.DocumentWindow
    Friend WithEvents DocumentTabStrip2 As Telerik.WinControls.UI.Docking.DocumentTabStrip
    Friend WithEvents DocWindowPRS As Telerik.WinControls.UI.Docking.DocumentWindow
    Friend WithEvents RadMenu1 As Telerik.WinControls.UI.RadMenu
    Friend WithEvents toolWindowRemarks As Telerik.WinControls.UI.Docking.ToolWindow
    Friend WithEvents ToolTabStrip1 As Telerik.WinControls.UI.Docking.ToolTabStrip
    Friend WithEvents RadMenuSeparatorItem1 As Telerik.WinControls.UI.RadMenuSeparatorItem
    Friend WithEvents rdMenuExit As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rdMenuHelp As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rdMenuAbout As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents DocWindowResults As Telerik.WinControls.UI.Docking.DocumentWindow
    Friend WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
    Friend WithEvents DocWindowSync As Telerik.WinControls.UI.Docking.DocumentWindow
End Class

