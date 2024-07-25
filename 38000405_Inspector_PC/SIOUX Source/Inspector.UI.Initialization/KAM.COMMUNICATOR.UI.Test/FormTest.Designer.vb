<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormTest

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
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Button7 = New System.Windows.Forms.Button()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(6, 75)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(137, 22)
        Me.Button3.TabIndex = 5
        Me.Button3.Text = "Loading SDF file"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(6, 47)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(137, 22)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Loading xml file"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(6, 19)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(137, 22)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "MsAccess to SDF"
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(6, 19)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(137, 22)
        Me.Button4.TabIndex = 6
        Me.Button4.Text = "Result Access to Sdf"
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(169, 19)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(137, 22)
        Me.Button5.TabIndex = 7
        Me.Button5.Text = "MsAccess to xml"
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(6, 47)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(137, 22)
        Me.Button6.TabIndex = 7
        Me.Button6.Text = "Result Access to XML"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Button3)
        Me.GroupBox1.Controls.Add(Me.Button2)
        Me.GroupBox1.Controls.Add(Me.Button5)
        Me.GroupBox1.Controls.Add(Me.Button1)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(312, 221)
        Me.GroupBox1.TabIndex = 10
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "GroupBox1"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Button7)
        Me.GroupBox2.Controls.Add(Me.Button6)
        Me.GroupBox2.Controls.Add(Me.Button4)
        Me.GroupBox2.Location = New System.Drawing.Point(342, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(304, 214)
        Me.GroupBox2.TabIndex = 11
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "GroupBox2"
        '
        'Button7
        '
        Me.Button7.Location = New System.Drawing.Point(6, 75)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(137, 22)
        Me.Button7.TabIndex = 8
        Me.Button7.Text = "Result XML to XML"
        '
        'Button8
        '
        Me.Button8.Location = New System.Drawing.Point(18, 239)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(149, 49)
        Me.Button8.TabIndex = 12
        Me.Button8.Text = "Getting inspector PC information"
        '
        'FormTest
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(680, 314)
        Me.Controls.Add(Me.Button8)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "FormTest"
        Me.Text = "FormTest"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Button3 As Telerik.WinControls.UI.RadButton
    Friend WithEvents Button2 As Telerik.WinControls.UI.RadButton
    Friend WithEvents Button1 As Telerik.WinControls.UI.RadButton
    Friend WithEvents Button4 As Telerik.WinControls.UI.RadButton
    Friend WithEvents Button5 As Telerik.WinControls.UI.RadButton
    Friend WithEvents Button6 As Telerik.WinControls.UI.RadButton
    Friend WithEvents GroupBox1 As Telerik.WinControls.UI.RadGroupBox
    Friend WithEvents GroupBox2 As Telerik.WinControls.UI.RadGroupBox
    Friend WithEvents Button7 As Telerik.WinControls.UI.RadButton
    Friend WithEvents Button8 As Telerik.WinControls.UI.RadButton
End Class
