<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_BluetoothDongle

    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    '<System.Diagnostics.DebuggerNonUserCode()> _
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_BluetoothDongle))
        Me.rDropDownBluetoothDriver = New Telerik.WinControls.UI.RadDropDownList()
        Me.RadLabel5 = New Telerik.WinControls.UI.RadLabel()
        Me.rDropDownBluetoothAddress = New Telerik.WinControls.UI.RadDropDownList()
        Me.rdLbl = New Telerik.WinControls.UI.RadLabel()
        Me.rdCheckUnpairBefore = New Telerik.WinControls.UI.RadCheckBox()
        CType(Me.rDropDownBluetoothDriver, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownBluetoothAddress, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdLbl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdCheckUnpairBefore, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rDropDownBluetoothDriver
        '
        Me.rDropDownBluetoothDriver.DropDownStyle  = Telerik.WinControls.RadDropDownStyle.DropDownList
        resources.ApplyResources(Me.rDropDownBluetoothDriver, "rDropDownBluetoothDriver")
        Me.rDropDownBluetoothDriver.Name = "rDropDownBluetoothDriver"
        '
        '
        '
        Me.rDropDownBluetoothDriver.RootElement.AccessibleDescription = resources.GetString("rDropDownBluetoothDriver.RootElement.AccessibleDescription")
        Me.rDropDownBluetoothDriver.RootElement.AccessibleName = resources.GetString("rDropDownBluetoothDriver.RootElement.AccessibleName")
        Me.rDropDownBluetoothDriver.RootElement.Alignment = CType(resources.GetObject("rDropDownBluetoothDriver.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rDropDownBluetoothDriver.RootElement.AngleTransform = CType(resources.GetObject("rDropDownBluetoothDriver.RootElement.AngleTransform"), Single)
        Me.rDropDownBluetoothDriver.RootElement.FlipText = CType(resources.GetObject("rDropDownBluetoothDriver.RootElement.FlipText"), Boolean)
        Me.rDropDownBluetoothDriver.RootElement.Margin = CType(resources.GetObject("rDropDownBluetoothDriver.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rDropDownBluetoothDriver.RootElement.Text = resources.GetString("rDropDownBluetoothDriver.RootElement.Text")
        Me.rDropDownBluetoothDriver.RootElement.TextOrientation = CType(resources.GetObject("rDropDownBluetoothDriver.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'RadLabel5
        '
        resources.ApplyResources(Me.RadLabel5, "RadLabel5")
        Me.RadLabel5.Name = "RadLabel5"
        '
        '
        '
        Me.RadLabel5.RootElement.AccessibleDescription = resources.GetString("RadLabel5.RootElement.AccessibleDescription")
        Me.RadLabel5.RootElement.AccessibleName = resources.GetString("RadLabel5.RootElement.AccessibleName")
        Me.RadLabel5.RootElement.Alignment = CType(resources.GetObject("RadLabel5.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RadLabel5.RootElement.AngleTransform = CType(resources.GetObject("RadLabel5.RootElement.AngleTransform"), Single)
        Me.RadLabel5.RootElement.FlipText = CType(resources.GetObject("RadLabel5.RootElement.FlipText"), Boolean)
        Me.RadLabel5.RootElement.Margin = CType(resources.GetObject("RadLabel5.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RadLabel5.RootElement.Text = resources.GetString("RadLabel5.RootElement.Text")
        Me.RadLabel5.RootElement.TextOrientation = CType(resources.GetObject("RadLabel5.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rDropDownBluetoothAddress
        '
        Me.rDropDownBluetoothAddress.DropDownStyle  = Telerik.WinControls.RadDropDownStyle.DropDownList
        resources.ApplyResources(Me.rDropDownBluetoothAddress, "rDropDownBluetoothAddress")
        Me.rDropDownBluetoothAddress.Name = "rDropDownBluetoothAddress"
        '
        '
        '
        Me.rDropDownBluetoothAddress.RootElement.AccessibleDescription = resources.GetString("rDropDownBluetoothAddress.RootElement.AccessibleDescription")
        Me.rDropDownBluetoothAddress.RootElement.AccessibleName = resources.GetString("rDropDownBluetoothAddress.RootElement.AccessibleName")
        Me.rDropDownBluetoothAddress.RootElement.Alignment = CType(resources.GetObject("rDropDownBluetoothAddress.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rDropDownBluetoothAddress.RootElement.AngleTransform = CType(resources.GetObject("rDropDownBluetoothAddress.RootElement.AngleTransform"), Single)
        Me.rDropDownBluetoothAddress.RootElement.FlipText = CType(resources.GetObject("rDropDownBluetoothAddress.RootElement.FlipText"), Boolean)
        Me.rDropDownBluetoothAddress.RootElement.Margin = CType(resources.GetObject("rDropDownBluetoothAddress.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rDropDownBluetoothAddress.RootElement.Text = resources.GetString("rDropDownBluetoothAddress.RootElement.Text")
        Me.rDropDownBluetoothAddress.RootElement.TextOrientation = CType(resources.GetObject("rDropDownBluetoothAddress.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdLbl
        '
        Me.rdLbl.ForeColor = System.Drawing.Color.Black
        resources.ApplyResources(Me.rdLbl, "rdLbl")
        Me.rdLbl.Name = "rdLbl"
        '
        '
        '
        Me.rdLbl.RootElement.AccessibleDescription = resources.GetString("rdLbl.RootElement.AccessibleDescription")
        Me.rdLbl.RootElement.AccessibleName = resources.GetString("rdLbl.RootElement.AccessibleName")
        Me.rdLbl.RootElement.Alignment = CType(resources.GetObject("rdLbl.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdLbl.RootElement.AngleTransform = CType(resources.GetObject("rdLbl.RootElement.AngleTransform"), Single)
        Me.rdLbl.RootElement.FlipText = CType(resources.GetObject("rdLbl.RootElement.FlipText"), Boolean)
        Me.rdLbl.RootElement.Margin = CType(resources.GetObject("rdLbl.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdLbl.RootElement.Text = resources.GetString("rdLbl.RootElement.Text")
        Me.rdLbl.RootElement.TextOrientation = CType(resources.GetObject("rdLbl.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdCheckUnpairBefore
        '
        resources.ApplyResources(Me.rdCheckUnpairBefore, "rdCheckUnpairBefore")
        Me.rdCheckUnpairBefore.Name = "rdCheckUnpairBefore"
        '
        'usctrl_BluetoothDongle
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.rdCheckUnpairBefore)
        Me.Controls.Add(Me.rDropDownBluetoothDriver)
        Me.Controls.Add(Me.RadLabel5)
        Me.Controls.Add(Me.rDropDownBluetoothAddress)
        Me.Controls.Add(Me.rdLbl)
        Me.Name = "usctrl_BluetoothDongle"
        CType(Me.rDropDownBluetoothDriver, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownBluetoothAddress, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdLbl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdCheckUnpairBefore, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rDropDownBluetoothDriver As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents RadLabel5 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rDropDownBluetoothAddress As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rdLbl As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rdCheckUnpairBefore As Telerik.WinControls.UI.RadCheckBox

End Class
