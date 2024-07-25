<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_About
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_About))
        Me.rdtxtbInstalledComponents = New Telerik.WinControls.UI.RadTextBox()
        Me.RadPanel1 = New Telerik.WinControls.UI.RadPanel()
        Me.rdLabelProgram = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblVersion = New Telerik.WinControls.UI.RadLabel()
        Me.rdllbProductGroup = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblInstallProducts = New Telerik.WinControls.UI.RadLabel()
        CType(Me.rdtxtbInstalledComponents, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadPanel1.SuspendLayout()
        CType(Me.rdLabelProgram, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblVersion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdllbProductGroup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblInstallProducts, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdtxtbInstalledComponents
        '
        resources.ApplyResources(Me.rdtxtbInstalledComponents, "rdtxtbInstalledComponents")
        Me.rdtxtbInstalledComponents.BackColor = System.Drawing.Color.Transparent
        Me.rdtxtbInstalledComponents.ForeColor = System.Drawing.Color.Black
        Me.rdtxtbInstalledComponents.Multiline = True
        Me.rdtxtbInstalledComponents.Name = "rdtxtbInstalledComponents"
        Me.rdtxtbInstalledComponents.ReadOnly = True
        '
        '
        '
        Me.rdtxtbInstalledComponents.RootElement.AccessibleDescription = resources.GetString("rdtxtbInstalledComponents.RootElement.AccessibleDescription")
        Me.rdtxtbInstalledComponents.RootElement.AccessibleName = resources.GetString("rdtxtbInstalledComponents.RootElement.AccessibleName")
        Me.rdtxtbInstalledComponents.RootElement.Alignment = CType(resources.GetObject("rdtxtbInstalledComponents.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdtxtbInstalledComponents.RootElement.AngleTransform = CType(resources.GetObject("rdtxtbInstalledComponents.RootElement.AngleTransform"), Single)
        Me.rdtxtbInstalledComponents.RootElement.FlipText = CType(resources.GetObject("rdtxtbInstalledComponents.RootElement.FlipText"), Boolean)
        Me.rdtxtbInstalledComponents.RootElement.Margin = CType(resources.GetObject("rdtxtbInstalledComponents.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdtxtbInstalledComponents.RootElement.Text = resources.GetString("rdtxtbInstalledComponents.RootElement.Text")
        Me.rdtxtbInstalledComponents.RootElement.TextOrientation = CType(resources.GetObject("rdtxtbInstalledComponents.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'RadPanel1
        '
        resources.ApplyResources(Me.RadPanel1, "RadPanel1")
        Me.RadPanel1.Controls.Add(Me.rdLabelProgram)
        Me.RadPanel1.Controls.Add(Me.rdlblVersion)
        Me.RadPanel1.Controls.Add(Me.rdllbProductGroup)
        Me.RadPanel1.Controls.Add(Me.rdtxtbInstalledComponents)
        Me.RadPanel1.Controls.Add(Me.rdlblInstallProducts)
        Me.RadPanel1.Name = "RadPanel1"
        '
        '
        '
        Me.RadPanel1.RootElement.AccessibleDescription = resources.GetString("RadPanel1.RootElement.AccessibleDescription")
        Me.RadPanel1.RootElement.AccessibleName = resources.GetString("RadPanel1.RootElement.AccessibleName")
        Me.RadPanel1.RootElement.Alignment = CType(resources.GetObject("RadPanel1.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RadPanel1.RootElement.AngleTransform = CType(resources.GetObject("RadPanel1.RootElement.AngleTransform"), Single)
        Me.RadPanel1.RootElement.FlipText = CType(resources.GetObject("RadPanel1.RootElement.FlipText"), Boolean)
        Me.RadPanel1.RootElement.Margin = CType(resources.GetObject("RadPanel1.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RadPanel1.RootElement.Text = resources.GetString("RadPanel1.RootElement.Text")
        Me.RadPanel1.RootElement.TextOrientation = CType(resources.GetObject("RadPanel1.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdLabelProgram
        '
        resources.ApplyResources(Me.rdLabelProgram, "rdLabelProgram")
        Me.rdLabelProgram.BackColor = System.Drawing.Color.Transparent
        Me.rdLabelProgram.ForeColor = System.Drawing.Color.White
        Me.rdLabelProgram.Name = "rdLabelProgram"
        '
        '
        '
        Me.rdLabelProgram.RootElement.AccessibleDescription = resources.GetString("rdLabelProgram.RootElement.AccessibleDescription")
        Me.rdLabelProgram.RootElement.AccessibleName = resources.GetString("rdLabelProgram.RootElement.AccessibleName")
        Me.rdLabelProgram.RootElement.Alignment = CType(resources.GetObject("rdLabelProgram.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdLabelProgram.RootElement.AngleTransform = CType(resources.GetObject("rdLabelProgram.RootElement.AngleTransform"), Single)
        Me.rdLabelProgram.RootElement.FlipText = CType(resources.GetObject("rdLabelProgram.RootElement.FlipText"), Boolean)
        Me.rdLabelProgram.RootElement.Margin = CType(resources.GetObject("rdLabelProgram.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdLabelProgram.RootElement.Text = resources.GetString("rdLabelProgram.RootElement.Text")
        Me.rdLabelProgram.RootElement.TextOrientation = CType(resources.GetObject("rdLabelProgram.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdlblVersion
        '
        resources.ApplyResources(Me.rdlblVersion, "rdlblVersion")
        Me.rdlblVersion.BackColor = System.Drawing.Color.Transparent
        Me.rdlblVersion.Name = "rdlblVersion"
        '
        '
        '
        Me.rdlblVersion.RootElement.AccessibleDescription = resources.GetString("rdlblVersion.RootElement.AccessibleDescription")
        Me.rdlblVersion.RootElement.AccessibleName = resources.GetString("rdlblVersion.RootElement.AccessibleName")
        Me.rdlblVersion.RootElement.Alignment = CType(resources.GetObject("rdlblVersion.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdlblVersion.RootElement.AngleTransform = CType(resources.GetObject("rdlblVersion.RootElement.AngleTransform"), Single)
        Me.rdlblVersion.RootElement.FlipText = CType(resources.GetObject("rdlblVersion.RootElement.FlipText"), Boolean)
        Me.rdlblVersion.RootElement.Margin = CType(resources.GetObject("rdlblVersion.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdlblVersion.RootElement.Text = resources.GetString("rdlblVersion.RootElement.Text")
        Me.rdlblVersion.RootElement.TextOrientation = CType(resources.GetObject("rdlblVersion.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdllbProductGroup
        '
        resources.ApplyResources(Me.rdllbProductGroup, "rdllbProductGroup")
        Me.rdllbProductGroup.BackColor = System.Drawing.Color.Transparent
        Me.rdllbProductGroup.Name = "rdllbProductGroup"
        '
        '
        '
        Me.rdllbProductGroup.RootElement.AccessibleDescription = resources.GetString("rdllbProductGroup.RootElement.AccessibleDescription")
        Me.rdllbProductGroup.RootElement.AccessibleName = resources.GetString("rdllbProductGroup.RootElement.AccessibleName")
        Me.rdllbProductGroup.RootElement.Alignment = CType(resources.GetObject("rdllbProductGroup.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdllbProductGroup.RootElement.AngleTransform = CType(resources.GetObject("rdllbProductGroup.RootElement.AngleTransform"), Single)
        Me.rdllbProductGroup.RootElement.FlipText = CType(resources.GetObject("rdllbProductGroup.RootElement.FlipText"), Boolean)
        Me.rdllbProductGroup.RootElement.Margin = CType(resources.GetObject("rdllbProductGroup.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdllbProductGroup.RootElement.Text = resources.GetString("rdllbProductGroup.RootElement.Text")
        Me.rdllbProductGroup.RootElement.TextOrientation = CType(resources.GetObject("rdllbProductGroup.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdlblInstallProducts
        '
        resources.ApplyResources(Me.rdlblInstallProducts, "rdlblInstallProducts")
        Me.rdlblInstallProducts.BackColor = System.Drawing.Color.Transparent
        Me.rdlblInstallProducts.Name = "rdlblInstallProducts"
        '
        '
        '
        Me.rdlblInstallProducts.RootElement.AccessibleDescription = resources.GetString("rdlblInstallProducts.RootElement.AccessibleDescription")
        Me.rdlblInstallProducts.RootElement.AccessibleName = resources.GetString("rdlblInstallProducts.RootElement.AccessibleName")
        Me.rdlblInstallProducts.RootElement.Alignment = CType(resources.GetObject("rdlblInstallProducts.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdlblInstallProducts.RootElement.AngleTransform = CType(resources.GetObject("rdlblInstallProducts.RootElement.AngleTransform"), Single)
        Me.rdlblInstallProducts.RootElement.FlipText = CType(resources.GetObject("rdlblInstallProducts.RootElement.FlipText"), Boolean)
        Me.rdlblInstallProducts.RootElement.Margin = CType(resources.GetObject("rdlblInstallProducts.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdlblInstallProducts.RootElement.Text = resources.GetString("rdlblInstallProducts.RootElement.Text")
        Me.rdlblInstallProducts.RootElement.TextOrientation = CType(resources.GetObject("rdlblInstallProducts.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'usctrl_About
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RadPanel1)
        Me.Name = "usctrl_About"
        CType(Me.rdtxtbInstalledComponents, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadPanel1.ResumeLayout(False)
        Me.RadPanel1.PerformLayout()
        CType(Me.rdLabelProgram, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblVersion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdllbProductGroup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblInstallProducts, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rdtxtbInstalledComponents As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents RadPanel1 As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdllbProductGroup As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdlblVersion As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdlblInstallProducts As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdLabelProgram As Telerik.WinControls.UI.RadLabel

End Class
