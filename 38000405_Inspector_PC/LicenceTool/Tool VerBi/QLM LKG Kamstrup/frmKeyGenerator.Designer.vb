<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMainLicense
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMainLicense))
        Me.RadStatusStrip1 = New Telerik.WinControls.UI.RadStatusStrip()
        Me.rdElementQLMUrl = New Telerik.WinControls.UI.RadLabelElement()
        Me.rdElementVersion = New Telerik.WinControls.UI.RadLabelElement()
        Me.rdElementUser = New Telerik.WinControls.UI.RadLabelElement()
        Me.RadPageView1 = New Telerik.WinControls.UI.RadPageView()
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPageView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadStatusStrip1
        '
        Me.RadStatusStrip1.Items.AddRange(New Telerik.WinControls.RadItem() {Me.rdElementQLMUrl, Me.rdElementVersion, Me.rdElementUser})
        Me.RadStatusStrip1.Location = New System.Drawing.Point(0, 382)
        Me.RadStatusStrip1.Name = "RadStatusStrip1"
        Me.RadStatusStrip1.Size = New System.Drawing.Size(869, 26)
        Me.RadStatusStrip1.TabIndex = 1
        Me.RadStatusStrip1.Text = "RadStatusStrip1"
        '
        'rdElementQLMUrl
        '
        Me.rdElementQLMUrl.AccessibleDescription = "rdElementQLMUrl"
        Me.rdElementQLMUrl.AccessibleName = "rdElementQLMUrl"
        Me.rdElementQLMUrl.Name = "rdElementQLMUrl"
        Me.RadStatusStrip1.SetSpring(Me.rdElementQLMUrl, False)
        Me.rdElementQLMUrl.Text = "QLM URL"
        Me.rdElementQLMUrl.TextWrap = True
        '
        'rdElementVersion
        '
        Me.rdElementVersion.AccessibleDescription = "rdElementVersion"
        Me.rdElementVersion.AccessibleName = "rdElementVersion"
        Me.rdElementVersion.Alignment = System.Drawing.ContentAlignment.TopLeft
        Me.rdElementVersion.Name = "rdElementVersion"
        Me.RadStatusStrip1.SetSpring(Me.rdElementVersion, False)
        Me.rdElementVersion.Text = "Version"
        Me.rdElementVersion.TextAlignment = System.Drawing.ContentAlignment.TopLeft
        Me.rdElementVersion.TextWrap = True
        '
        'rdElementUser
        '
        Me.rdElementUser.AccessibleDescription = "rdElementUser"
        Me.rdElementUser.AccessibleName = "rdElementUser"
        Me.rdElementUser.Name = "rdElementUser"
        Me.RadStatusStrip1.SetSpring(Me.rdElementUser, False)
        Me.rdElementUser.Text = "User"
        Me.rdElementUser.TextWrap = True
        '
        'RadPageView1
        '
        Me.RadPageView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadPageView1.Location = New System.Drawing.Point(0, 0)
        Me.RadPageView1.Name = "RadPageView1"
        Me.RadPageView1.Size = New System.Drawing.Size(869, 386)
        Me.RadPageView1.TabIndex = 0
        Me.RadPageView1.Text = "RadPageView1"
        '
        'frmMainLicense
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(869, 408)
        Me.Controls.Add(Me.RadStatusStrip1)
        Me.Controls.Add(Me.RadPageView1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMainLicense"
        Me.Text = "Wigersma & Sikkema. QLM licenses"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.RadStatusStrip1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPageView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RadStatusStrip1 As Telerik.WinControls.UI.RadStatusStrip
    Friend WithEvents rdElementQLMUrl As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents rdElementVersion As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents rdElementUser As Telerik.WinControls.UI.RadLabelElement
    Friend WithEvents RadPageView1 As Telerik.WinControls.UI.RadPageView
End Class
