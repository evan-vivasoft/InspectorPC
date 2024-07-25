<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_GeneralUI
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_GeneralUI))
        Me.rdLbl = New Telerik.WinControls.UI.RadLabel()
        Me.AquaTheme1 = New Telerik.WinControls.Themes.AquaTheme()
        Me.BreezeTheme1 = New Telerik.WinControls.Themes.BreezeTheme()
        Me.DesertTheme1 = New Telerik.WinControls.Themes.DesertTheme()
        Me.VisualStudio2012LightTheme1 = New Telerik.WinControls.Themes.VisualStudio2012LightTheme()
        Me.HighContrastBlackTheme1 = New Telerik.WinControls.Themes.HighContrastBlackTheme()
        Me.Office2007BlackTheme1 = New Telerik.WinControls.Themes.Office2007BlackTheme()
        Me.Office2007SilverTheme1 = New Telerik.WinControls.Themes.Office2007SilverTheme()
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.Office2010BlueTheme1 = New Telerik.WinControls.Themes.Office2010BlueTheme()
        Me.Office2010SilverTheme1 = New Telerik.WinControls.Themes.Office2010SilverTheme()
        Me.TelerikMetroTheme1 = New Telerik.WinControls.Themes.TelerikMetroTheme()
        Me.TelerikMetroBlueTheme1 = New Telerik.WinControls.Themes.TelerikMetroBlueTheme()
        Me.TelerikMetroTouchTheme1 = New Telerik.WinControls.Themes.TelerikMetroTouchTheme()
        Me.Windows7Theme1 = New Telerik.WinControls.Themes.Windows7Theme()
        Me.rDropDownThemesSelection = New Telerik.WinControls.UI.RadDropDownList()
        Me.rdBrowsCommunicatorPath = New Telerik.WinControls.UI.RadBrowseEditor()
        Me.rdlblPathCommunicator = New Telerik.WinControls.UI.RadLabel()
        Me.RadGroupBox1 = New Telerik.WinControls.UI.RadGroupBox()
        Me.rdchkInspectorPda = New Telerik.WinControls.UI.RadCheckBox()
        Me.rdchkInspectorPc = New Telerik.WinControls.UI.RadCheckBox()
        CType(Me.rdLbl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownThemesSelection, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdBrowsCommunicatorPath, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblPathCommunicator, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadGroupBox1.SuspendLayout()
        CType(Me.rdchkInspectorPda, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdchkInspectorPc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdLbl
        '
        Me.rdLbl.ForeColor = System.Drawing.Color.Black
        resources.ApplyResources(Me.rdLbl, "rdLbl")
        Me.rdLbl.Name = "rdLbl"
        '
        'rDropDownThemesSelection
        '
        Me.rDropDownThemesSelection.AutoCompleteDisplayMember = Nothing
        Me.rDropDownThemesSelection.AutoCompleteValueMember = Nothing
        resources.ApplyResources(Me.rDropDownThemesSelection, "rDropDownThemesSelection")
        Me.rDropDownThemesSelection.Name = "rDropDownThemesSelection"
        '
        '
        '
        Me.rDropDownThemesSelection.RootElement.AccessibleDescription = resources.GetString("rDropDownThemesSelection.RootElement.AccessibleDescription")
        Me.rDropDownThemesSelection.RootElement.AccessibleName = resources.GetString("rDropDownThemesSelection.RootElement.AccessibleName")
        Me.rDropDownThemesSelection.RootElement.Alignment = CType(resources.GetObject("rDropDownThemesSelection.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rDropDownThemesSelection.RootElement.AngleTransform = CType(resources.GetObject("rDropDownThemesSelection.RootElement.AngleTransform"), Single)
        Me.rDropDownThemesSelection.RootElement.FlipText = CType(resources.GetObject("rDropDownThemesSelection.RootElement.FlipText"), Boolean)
        Me.rDropDownThemesSelection.RootElement.Margin = CType(resources.GetObject("rDropDownThemesSelection.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rDropDownThemesSelection.RootElement.Text = resources.GetString("rDropDownThemesSelection.RootElement.Text")
        Me.rDropDownThemesSelection.RootElement.TextOrientation = CType(resources.GetObject("rDropDownThemesSelection.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdBrowsCommunicatorPath
        '
        resources.ApplyResources(Me.rdBrowsCommunicatorPath, "rdBrowsCommunicatorPath")
        Me.rdBrowsCommunicatorPath.DialogType = Telerik.WinControls.UI.BrowseEditorDialogType.FolderBrowseDialog
        Me.rdBrowsCommunicatorPath.Name = "rdBrowsCommunicatorPath"
        '
        'rdlblPathCommunicator
        '
        Me.rdlblPathCommunicator.ForeColor = System.Drawing.Color.Black
        resources.ApplyResources(Me.rdlblPathCommunicator, "rdlblPathCommunicator")
        Me.rdlblPathCommunicator.Name = "rdlblPathCommunicator"
        '
        '
        '
        Me.rdlblPathCommunicator.RootElement.AccessibleDescription = resources.GetString("rdlblPathCommunicator.RootElement.AccessibleDescription")
        Me.rdlblPathCommunicator.RootElement.AccessibleName = resources.GetString("rdlblPathCommunicator.RootElement.AccessibleName")
        Me.rdlblPathCommunicator.RootElement.Alignment = CType(resources.GetObject("rdlblPathCommunicator.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdlblPathCommunicator.RootElement.AngleTransform = CType(resources.GetObject("rdlblPathCommunicator.RootElement.AngleTransform"), Single)
        Me.rdlblPathCommunicator.RootElement.FlipText = CType(resources.GetObject("rdlblPathCommunicator.RootElement.FlipText"), Boolean)
        Me.rdlblPathCommunicator.RootElement.Margin = CType(resources.GetObject("rdlblPathCommunicator.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdlblPathCommunicator.RootElement.Text = resources.GetString("rdlblPathCommunicator.RootElement.Text")
        Me.rdlblPathCommunicator.RootElement.TextOrientation = CType(resources.GetObject("rdlblPathCommunicator.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'RadGroupBox1
        '
        Me.RadGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me.RadGroupBox1.Controls.Add(Me.rdchkInspectorPda)
        Me.RadGroupBox1.Controls.Add(Me.rdchkInspectorPc)
        resources.ApplyResources(Me.RadGroupBox1, "RadGroupBox1")
        Me.RadGroupBox1.Name = "RadGroupBox1"
        '
        'rdchkInspectorPda
        '
        resources.ApplyResources(Me.rdchkInspectorPda, "rdchkInspectorPda")
        Me.rdchkInspectorPda.Name = "rdchkInspectorPda"
        '
        '
        '
        Me.rdchkInspectorPda.RootElement.AccessibleDescription = resources.GetString("rdchkInspectorPda.RootElement.AccessibleDescription")
        Me.rdchkInspectorPda.RootElement.AccessibleName = resources.GetString("rdchkInspectorPda.RootElement.AccessibleName")
        Me.rdchkInspectorPda.RootElement.Alignment = CType(resources.GetObject("rdchkInspectorPda.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdchkInspectorPda.RootElement.AngleTransform = CType(resources.GetObject("rdchkInspectorPda.RootElement.AngleTransform"), Single)
        Me.rdchkInspectorPda.RootElement.FlipText = CType(resources.GetObject("rdchkInspectorPda.RootElement.FlipText"), Boolean)
        Me.rdchkInspectorPda.RootElement.Margin = CType(resources.GetObject("rdchkInspectorPda.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdchkInspectorPda.RootElement.Text = resources.GetString("rdchkInspectorPda.RootElement.Text")
        Me.rdchkInspectorPda.RootElement.TextOrientation = CType(resources.GetObject("rdchkInspectorPda.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdchkInspectorPc
        '
        resources.ApplyResources(Me.rdchkInspectorPc, "rdchkInspectorPc")
        Me.rdchkInspectorPc.Name = "rdchkInspectorPc"
        '
        '
        '
        Me.rdchkInspectorPc.RootElement.AccessibleDescription = resources.GetString("rdchkInspectorPc.RootElement.AccessibleDescription")
        Me.rdchkInspectorPc.RootElement.AccessibleName = resources.GetString("rdchkInspectorPc.RootElement.AccessibleName")
        Me.rdchkInspectorPc.RootElement.Alignment = CType(resources.GetObject("rdchkInspectorPc.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdchkInspectorPc.RootElement.AngleTransform = CType(resources.GetObject("rdchkInspectorPc.RootElement.AngleTransform"), Single)
        Me.rdchkInspectorPc.RootElement.FlipText = CType(resources.GetObject("rdchkInspectorPc.RootElement.FlipText"), Boolean)
        Me.rdchkInspectorPc.RootElement.Margin = CType(resources.GetObject("rdchkInspectorPc.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdchkInspectorPc.RootElement.Text = resources.GetString("rdchkInspectorPc.RootElement.Text")
        Me.rdchkInspectorPc.RootElement.TextOrientation = CType(resources.GetObject("rdchkInspectorPc.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'usctrl_GeneralUI
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.RadGroupBox1)
        Me.Controls.Add(Me.rdlblPathCommunicator)
        Me.Controls.Add(Me.rdBrowsCommunicatorPath)
        Me.Controls.Add(Me.rDropDownThemesSelection)
        Me.Controls.Add(Me.rdLbl)
        Me.Name = "usctrl_GeneralUI"
        CType(Me.rdLbl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownThemesSelection, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdBrowsCommunicatorPath, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblPathCommunicator, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadGroupBox1.ResumeLayout(False)
        Me.RadGroupBox1.PerformLayout()
        CType(Me.rdchkInspectorPda, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdchkInspectorPc, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdLbl As Telerik.WinControls.UI.RadLabel
    Friend WithEvents AquaTheme1 As Telerik.WinControls.Themes.AquaTheme
    Friend WithEvents BreezeTheme1 As Telerik.WinControls.Themes.BreezeTheme
    Friend WithEvents DesertTheme1 As Telerik.WinControls.Themes.DesertTheme
    Friend WithEvents VisualStudio2012LightTheme1 As Telerik.WinControls.Themes.VisualStudio2012LightTheme
    Friend WithEvents HighContrastBlackTheme1 As Telerik.WinControls.Themes.HighContrastBlackTheme
    Friend WithEvents Office2007BlackTheme1 As Telerik.WinControls.Themes.Office2007BlackTheme
    Friend WithEvents Office2007SilverTheme1 As Telerik.WinControls.Themes.Office2007SilverTheme
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents Office2010BlueTheme1 As Telerik.WinControls.Themes.Office2010BlueTheme
    Friend WithEvents Office2010SilverTheme1 As Telerik.WinControls.Themes.Office2010SilverTheme
    Friend WithEvents TelerikMetroTheme1 As Telerik.WinControls.Themes.TelerikMetroTheme
    Friend WithEvents TelerikMetroBlueTheme1 As Telerik.WinControls.Themes.TelerikMetroBlueTheme
    Friend WithEvents TelerikMetroTouchTheme1 As Telerik.WinControls.Themes.TelerikMetroTouchTheme
    Friend WithEvents Windows7Theme1 As Telerik.WinControls.Themes.Windows7Theme
    Friend WithEvents rDropDownThemesSelection As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rdBrowsCommunicatorPath As Telerik.WinControls.UI.RadBrowseEditor
    Friend WithEvents rdlblPathCommunicator As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadGroupBox1 As Telerik.WinControls.UI.RadGroupBox
    Friend WithEvents rdchkInspectorPda As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents rdchkInspectorPc As Telerik.WinControls.UI.RadCheckBox

End Class
