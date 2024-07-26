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
        Me.rdLabelInspector = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblVersion = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblInstallProducts = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblLicense = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblLicenceStatus = New Telerik.WinControls.UI.RadLabel()
        Me.rdtxtbInstalledComponents = New Telerik.WinControls.UI.RadTextBox()
        Me.RadPanel1 = New Telerik.WinControls.UI.RadPanel()
        Me.rdlblLicenceComputerKey = New Telerik.WinControls.UI.RadLabel()
        CType(Me.rdLabelInspector, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblVersion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblInstallProducts, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblLicense, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblLicenceStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdtxtbInstalledComponents, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadPanel1.SuspendLayout()
        CType(Me.rdlblLicenceComputerKey, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdLabelInspector
        '
        Me.rdLabelInspector.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.rdLabelInspector, "rdLabelInspector")
        Me.rdLabelInspector.ForeColor = System.Drawing.Color.White
        Me.rdLabelInspector.Name = "rdLabelInspector"
        '
        'rdlblVersion
        '
        resources.ApplyResources(Me.rdlblVersion, "rdlblVersion")
        Me.rdlblVersion.BackColor = System.Drawing.Color.Transparent
        Me.rdlblVersion.Name = "rdlblVersion"
        '
        'rdlblInstallProducts
        '
        resources.ApplyResources(Me.rdlblInstallProducts, "rdlblInstallProducts")
        Me.rdlblInstallProducts.BackColor = System.Drawing.Color.Transparent
        Me.rdlblInstallProducts.Name = "rdlblInstallProducts"
        '
        'rdlblLicense
        '
        resources.ApplyResources(Me.rdlblLicense, "rdlblLicense")
        Me.rdlblLicense.BackColor = System.Drawing.Color.Transparent
        Me.rdlblLicense.Name = "rdlblLicense"
        '
        '
        '
        Me.rdlblLicense.RootElement.AccessibleDescription = resources.GetString("rdlblLicense.RootElement.AccessibleDescription")
        Me.rdlblLicense.RootElement.AccessibleName = resources.GetString("rdlblLicense.RootElement.AccessibleName")
        Me.rdlblLicense.RootElement.Alignment = CType(resources.GetObject("rdlblLicense.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdlblLicense.RootElement.AngleTransform = CType(resources.GetObject("rdlblLicense.RootElement.AngleTransform"), Single)
        Me.rdlblLicense.RootElement.FlipText = CType(resources.GetObject("rdlblLicense.RootElement.FlipText"), Boolean)
        Me.rdlblLicense.RootElement.Margin = CType(resources.GetObject("rdlblLicense.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdlblLicense.RootElement.Text = resources.GetString("rdlblLicense.RootElement.Text")
        Me.rdlblLicense.RootElement.TextOrientation = CType(resources.GetObject("rdlblLicense.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdlblLicenceStatus
        '
        resources.ApplyResources(Me.rdlblLicenceStatus, "rdlblLicenceStatus")
        Me.rdlblLicenceStatus.BackColor = System.Drawing.Color.Transparent
        Me.rdlblLicenceStatus.Name = "rdlblLicenceStatus"
        '
        '
        '
        Me.rdlblLicenceStatus.RootElement.AccessibleDescription = resources.GetString("rdlblLicenceKey.RootElement.AccessibleDescription")
        Me.rdlblLicenceStatus.RootElement.AccessibleName = resources.GetString("rdlblLicenceKey.RootElement.AccessibleName")
        Me.rdlblLicenceStatus.RootElement.Alignment = CType(resources.GetObject("rdlblLicenceKey.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdlblLicenceStatus.RootElement.AngleTransform = CType(resources.GetObject("rdlblLicenceKey.RootElement.AngleTransform"), Single)
        Me.rdlblLicenceStatus.RootElement.FlipText = CType(resources.GetObject("rdlblLicenceKey.RootElement.FlipText"), Boolean)
        Me.rdlblLicenceStatus.RootElement.Margin = CType(resources.GetObject("rdlblLicenceKey.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdlblLicenceStatus.RootElement.Text = resources.GetString("rdlblLicenceKey.RootElement.Text")
        Me.rdlblLicenceStatus.RootElement.TextOrientation = CType(resources.GetObject("rdlblLicenceKey.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
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
        Me.RadPanel1.Controls.Add(Me.rdlblLicense)
        Me.RadPanel1.Controls.Add(Me.rdlblLicenceComputerKey)
        Me.RadPanel1.Controls.Add(Me.rdtxtbInstalledComponents)
        Me.RadPanel1.Controls.Add(Me.rdLabelInspector)
        Me.RadPanel1.Controls.Add(Me.rdlblVersion)
        Me.RadPanel1.Controls.Add(Me.rdlblInstallProducts)
        Me.RadPanel1.Controls.Add(Me.rdlblLicenceStatus)
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
        'rdlblLicenceComputerKey
        '
        resources.ApplyResources(Me.rdlblLicenceComputerKey, "rdlblLicenceComputerKey")
        Me.rdlblLicenceComputerKey.BackColor = System.Drawing.Color.Transparent
        Me.rdlblLicenceComputerKey.Name = "rdlblLicenceComputerKey"
        '
        '
        '
        Me.rdlblLicenceComputerKey.RootElement.AccessibleDescription = resources.GetString("rdlblLicenceComputerKey.RootElement.AccessibleDescription")
        Me.rdlblLicenceComputerKey.RootElement.AccessibleName = resources.GetString("rdlblLicenceComputerKey.RootElement.AccessibleName")
        Me.rdlblLicenceComputerKey.RootElement.Alignment = CType(resources.GetObject("rdlblLicenceComputerKey.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdlblLicenceComputerKey.RootElement.AngleTransform = CType(resources.GetObject("rdlblLicenceComputerKey.RootElement.AngleTransform"), Single)
        Me.rdlblLicenceComputerKey.RootElement.FlipText = CType(resources.GetObject("rdlblLicenceComputerKey.RootElement.FlipText"), Boolean)
        Me.rdlblLicenceComputerKey.RootElement.Margin = CType(resources.GetObject("rdlblLicenceComputerKey.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdlblLicenceComputerKey.RootElement.Text = resources.GetString("rdlblLicenceComputerKey.RootElement.Text")
        Me.rdlblLicenceComputerKey.RootElement.TextOrientation = CType(resources.GetObject("rdlblLicenceComputerKey.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'usctrl_About
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RadPanel1)
        Me.Name = "usctrl_About"
        CType(Me.rdLabelInspector, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblVersion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblInstallProducts, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblLicense, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblLicenceStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdtxtbInstalledComponents, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadPanel1.ResumeLayout(False)
        Me.RadPanel1.PerformLayout()
        CType(Me.rdlblLicenceComputerKey, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rdLabelInspector As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdlblVersion As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdlblInstallProducts As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdlblLicense As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdlblLicenceStatus As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdtxtbInstalledComponents As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents RadPanel1 As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdlblLicenceComputerKey As Telerik.WinControls.UI.RadLabel

End Class
