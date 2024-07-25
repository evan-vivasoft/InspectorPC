<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uscrtl_LicenseRequest
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uscrtl_LicenseRequest))
        Me.RadGroupBox1 = New Telerik.WinControls.UI.RadGroupBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.VerifyTokenLabel = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.VerificationTokenInput = New System.Windows.Forms.TextBox()
        Me.licenseStatusText = New System.Windows.Forms.TextBox()
        Me.expiresInText = New System.Windows.Forms.TextBox()
        Me.licenseStatusLabel = New System.Windows.Forms.Label()
        Me.expiresInLabel = New System.Windows.Forms.Label()
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadGroupBox1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'RadGroupBox1
        '
        Me.RadGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        resources.ApplyResources(Me.RadGroupBox1, "RadGroupBox1")
        Me.RadGroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.RadGroupBox1.Controls.Add(Me.expiresInLabel)
        Me.RadGroupBox1.Controls.Add(Me.licenseStatusLabel)
        Me.RadGroupBox1.Controls.Add(Me.expiresInText)
        Me.RadGroupBox1.Controls.Add(Me.licenseStatusText)
        Me.RadGroupBox1.Name = "RadGroupBox1"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.VerifyTokenLabel)
        Me.GroupBox1.Controls.Add(Me.Button1)
        Me.GroupBox1.Controls.Add(Me.VerificationTokenInput)
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'VerifyTokenLabel
        '
        resources.ApplyResources(Me.VerifyTokenLabel, "VerifyTokenLabel")
        Me.VerifyTokenLabel.Name = "VerifyTokenLabel"
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'VerificationTokenInput
        '
        resources.ApplyResources(Me.VerificationTokenInput, "VerificationTokenInput")
        Me.VerificationTokenInput.Name = "VerificationTokenInput"
        '
        'licenseStatusText
        '
        resources.ApplyResources(Me.licenseStatusText, "licenseStatusText")
        Me.licenseStatusText.Name = "licenseStatusText"
        '
        'expiresInText
        '
        resources.ApplyResources(Me.expiresInText, "expiresInText")
        Me.expiresInText.Name = "expiresInText"
        '
        'licenseStatusLabel
        '
        resources.ApplyResources(Me.licenseStatusLabel, "licenseStatusLabel")
        Me.licenseStatusLabel.Name = "licenseStatusLabel"
        '
        'expiresInLabel
        '
        resources.ApplyResources(Me.expiresInLabel, "expiresInLabel")
        Me.expiresInLabel.Name = "expiresInLabel"
        '
        'uscrtl_LicenseRequest
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.RadGroupBox1)
        Me.Name = "uscrtl_LicenseRequest"
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadGroupBox1.ResumeLayout(False)
        Me.RadGroupBox1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RadGroupBox1 As Telerik.WinControls.UI.RadGroupBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents VerificationTokenInput As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents VerifyTokenLabel As Label
    Friend WithEvents licenseStatusLabel As Label
    Friend WithEvents expiresInText As TextBox
    Friend WithEvents licenseStatusText As TextBox
    Friend WithEvents expiresInLabel As Label
End Class
