<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_Licensed
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_Licensed))
        Me.txtBoxLicenseKey = New Telerik.WinControls.UI.RadTextBox()
        Me.RadLabel4 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel1 = New Telerik.WinControls.UI.RadLabel()
        CType(Me.txtBoxLicenseKey, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtBoxLicenseKey
        '
        resources.ApplyResources(Me.txtBoxLicenseKey, "txtBoxLicenseKey")
        Me.txtBoxLicenseKey.Name = "txtBoxLicenseKey"
        Me.txtBoxLicenseKey.ReadOnly = True
        Me.txtBoxLicenseKey.TabStop = False
        '
        'RadLabel4
        '
        Me.RadLabel4.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.RadLabel4, "RadLabel4")
        Me.RadLabel4.ForeColor = System.Drawing.Color.Black
        Me.RadLabel4.Name = "RadLabel4"
        '
        'RadLabel1
        '
        Me.RadLabel1.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.RadLabel1, "RadLabel1")
        Me.RadLabel1.ForeColor = System.Drawing.Color.Black
        Me.RadLabel1.Name = "RadLabel1"
        '
        'usctrl_Licensed
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RadLabel1)
        Me.Controls.Add(Me.txtBoxLicenseKey)
        Me.Controls.Add(Me.RadLabel4)
        Me.Name = "usctrl_Licensed"
        CType(Me.txtBoxLicenseKey, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtBoxLicenseKey As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents RadLabel4 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel1 As Telerik.WinControls.UI.RadLabel

End Class
