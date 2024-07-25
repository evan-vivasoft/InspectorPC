<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm

    Inherits Telerik.WinControls.UI.RadForm

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.Office2007BlackTheme1 = New Telerik.WinControls.Themes.Office2007BlackTheme()
        Me.AquaTheme1 = New Telerik.WinControls.Themes.AquaTheme()
        Me.BreezeTheme1 = New Telerik.WinControls.Themes.BreezeTheme()
        Me.DesertTheme1 = New Telerik.WinControls.Themes.DesertTheme()
        Me.HighContrastBlackTheme1 = New Telerik.WinControls.Themes.HighContrastBlackTheme()
        Me.Office2007SilverTheme1 = New Telerik.WinControls.Themes.Office2007SilverTheme()
        Me.Office2010BlueTheme1 = New Telerik.WinControls.Themes.Office2010BlueTheme()
        Me.Office2010SilverTheme1 = New Telerik.WinControls.Themes.Office2010SilverTheme()
        Me.TelerikMetroTheme1 = New Telerik.WinControls.Themes.TelerikMetroTheme()
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.Windows7Theme1 = New Telerik.WinControls.Themes.Windows7Theme()
        Me.rdMenuExtra = New Telerik.WinControls.UI.RadMenuItem()
        Me.rdMenuSettings = New Telerik.WinControls.UI.RadMenuItem()
        Me.rdMenuHelp = New Telerik.WinControls.UI.RadMenuItem()
        Me.rdMenuAbout = New Telerik.WinControls.UI.RadMenuItem()
        Me.RadMenu1 = New Telerik.WinControls.UI.RadMenu()
        Me.RadPanel1 = New Telerik.WinControls.UI.RadPanel()
        CType(Me.RadMenu1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdMenuExtra
        '
        resources.ApplyResources(Me.rdMenuExtra, "rdMenuExtra")
        Me.rdMenuExtra.Items.AddRange(New Telerik.WinControls.RadItem() {Me.rdMenuSettings})
        Me.rdMenuExtra.Name = "rdMenuExtra"
        '
        'rdMenuSettings
        '
        resources.ApplyResources(Me.rdMenuSettings, "rdMenuSettings")
        Me.rdMenuSettings.Name = "rdMenuSettings"
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
        'RadMenu1
        '
        Me.RadMenu1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.rdMenuExtra, Me.rdMenuHelp})
        resources.ApplyResources(Me.RadMenu1, "RadMenu1")
        Me.RadMenu1.Name = "RadMenu1"
        '
        'RadPanel1
        '
        resources.ApplyResources(Me.RadPanel1, "RadPanel1")
        Me.RadPanel1.Name = "RadPanel1"
        '
        'MainForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RadPanel1)
        Me.Controls.Add(Me.RadMenu1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MainForm"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        CType(Me.RadMenu1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2007BlackTheme1 As Telerik.WinControls.Themes.Office2007BlackTheme
    Friend WithEvents AquaTheme1 As Telerik.WinControls.Themes.AquaTheme
    Friend WithEvents BreezeTheme1 As Telerik.WinControls.Themes.BreezeTheme
    Friend WithEvents DesertTheme1 As Telerik.WinControls.Themes.DesertTheme
    Friend WithEvents HighContrastBlackTheme1 As Telerik.WinControls.Themes.HighContrastBlackTheme
    Friend WithEvents Office2007SilverTheme1 As Telerik.WinControls.Themes.Office2007SilverTheme
    Friend WithEvents Office2010BlueTheme1 As Telerik.WinControls.Themes.Office2010BlueTheme
    Friend WithEvents Office2010SilverTheme1 As Telerik.WinControls.Themes.Office2010SilverTheme
    Friend WithEvents TelerikMetroTheme1 As Telerik.WinControls.Themes.TelerikMetroTheme
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents Windows7Theme1 As Telerik.WinControls.Themes.Windows7Theme
    Friend WithEvents rdMenuExtra As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rdMenuSettings As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rdMenuHelp As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rdMenuAbout As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents RadMenu1 As Telerik.WinControls.UI.RadMenu
    Friend WithEvents RadPanel1 As Telerik.WinControls.UI.RadPanel
End Class

