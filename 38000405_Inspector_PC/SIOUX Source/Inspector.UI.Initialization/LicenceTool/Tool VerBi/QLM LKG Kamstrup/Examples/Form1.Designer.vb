<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.label6 = New System.Windows.Forms.Label()
        Me.txtActivationKey = New System.Windows.Forms.TextBox()
        Me.label4 = New System.Windows.Forms.Label()
        Me.btnActivate = New System.Windows.Forms.Button()
        Me.label2 = New System.Windows.Forms.Label()
        Me.groupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtComputerKey = New System.Windows.Forms.TextBox()
        Me.txtOutput2 = New System.Windows.Forms.TextBox()
        Me.txtUrlQlmWeb = New System.Windows.Forms.TextBox()
        Me.label8 = New System.Windows.Forms.Label()
        Me.label9 = New System.Windows.Forms.Label()
        Me.groupBox1 = New System.Windows.Forms.GroupBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.label3 = New System.Windows.Forms.Label()
        Me.txtUrlGetActivationKey = New System.Windows.Forms.TextBox()
        Me.label5 = New System.Windows.Forms.Label()
        Me.btnCreateActivationKey = New System.Windows.Forms.Button()
        Me.btnGo = New System.Windows.Forms.Button()
        Me.txtOutput = New System.Windows.Forms.TextBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.RadGridView1 = New Telerik.WinControls.UI.RadGridView()
        Me.groupBox2.SuspendLayout()
        Me.groupBox1.SuspendLayout()
        CType(Me.RadGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'label6
        '
        Me.label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label6.Location = New System.Drawing.Point(16, 32)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(893, 118)
        Me.label6.TabIndex = 9
        Me.label6.Text = resources.GetString("label6.Text")
        '
        'txtActivationKey
        '
        Me.txtActivationKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtActivationKey.Location = New System.Drawing.Point(19, 199)
        Me.txtActivationKey.Multiline = True
        Me.txtActivationKey.Name = "txtActivationKey"
        Me.txtActivationKey.Size = New System.Drawing.Size(689, 22)
        Me.txtActivationKey.TabIndex = 10
        Me.txtActivationKey.Text = "<Enter the activation key and then click on the Activate key button>"
        '
        'label4
        '
        Me.label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label4.Location = New System.Drawing.Point(20, 232)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(692, 43)
        Me.label4.TabIndex = 15
        Me.label4.Text = "The results of a call to activate a license key is an XML fragment. You need to p" & _
    "arse the XML fragment to extract the required information."
        '
        'btnActivate
        '
        Me.btnActivate.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnActivate.Location = New System.Drawing.Point(726, 199)
        Me.btnActivate.Name = "btnActivate"
        Me.btnActivate.Size = New System.Drawing.Size(145, 28)
        Me.btnActivate.TabIndex = 11
        Me.btnActivate.Text = "&Activate key"
        Me.btnActivate.UseVisualStyleBackColor = True
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label2.Location = New System.Drawing.Point(21, 194)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(478, 17)
        Me.label2.TabIndex = 17
        Me.label2.Text = "Your customer should receive the activation key by e-mail upon purchase. "
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.label6)
        Me.groupBox2.Controls.Add(Me.txtComputerKey)
        Me.groupBox2.Controls.Add(Me.txtActivationKey)
        Me.groupBox2.Controls.Add(Me.label4)
        Me.groupBox2.Controls.Add(Me.btnActivate)
        Me.groupBox2.Controls.Add(Me.txtOutput2)
        Me.groupBox2.Controls.Add(Me.txtUrlQlmWeb)
        Me.groupBox2.Controls.Add(Me.label8)
        Me.groupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.groupBox2.Location = New System.Drawing.Point(16, 300)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(926, 398)
        Me.groupBox2.TabIndex = 21
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Step 2 - In your application"
        '
        'txtComputerKey
        '
        Me.txtComputerKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtComputerKey.Location = New System.Drawing.Point(24, 366)
        Me.txtComputerKey.Multiline = True
        Me.txtComputerKey.Name = "txtComputerKey"
        Me.txtComputerKey.Size = New System.Drawing.Size(689, 22)
        Me.txtComputerKey.TabIndex = 10
        '
        'txtOutput2
        '
        Me.txtOutput2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOutput2.Location = New System.Drawing.Point(24, 279)
        Me.txtOutput2.Multiline = True
        Me.txtOutput2.Name = "txtOutput2"
        Me.txtOutput2.ReadOnly = True
        Me.txtOutput2.Size = New System.Drawing.Size(689, 83)
        Me.txtOutput2.TabIndex = 14
        '
        'txtUrlQlmWeb
        '
        Me.txtUrlQlmWeb.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUrlQlmWeb.Location = New System.Drawing.Point(19, 170)
        Me.txtUrlQlmWeb.Multiline = True
        Me.txtUrlQlmWeb.Name = "txtUrlQlmWeb"
        Me.txtUrlQlmWeb.Size = New System.Drawing.Size(689, 22)
        Me.txtUrlQlmWeb.TabIndex = 12
        Me.txtUrlQlmWeb.Text = "http://quicklicensemanager.com/kamstrup/qlm/qlmservice.asmx"
        '
        'label8
        '
        Me.label8.AutoSize = True
        Me.label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label8.Location = New System.Drawing.Point(16, 150)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(192, 17)
        Me.label8.TabIndex = 13
        Me.label8.Text = "URL to QlmWeb web service:"
        '
        'label9
        '
        Me.label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label9.Location = New System.Drawing.Point(20, 110)
        Me.label9.Name = "label9"
        Me.label9.Size = New System.Drawing.Size(418, 28)
        Me.label9.TabIndex = 16
        Me.label9.Text = "Customize the following arguments: is_productid, is_majorversion, is_minorversion" & _
    ", vendor, according to the products you have defined."
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.Button1)
        Me.groupBox1.Controls.Add(Me.label2)
        Me.groupBox1.Controls.Add(Me.label3)
        Me.groupBox1.Controls.Add(Me.label9)
        Me.groupBox1.Controls.Add(Me.txtUrlGetActivationKey)
        Me.groupBox1.Controls.Add(Me.label5)
        Me.groupBox1.Controls.Add(Me.btnCreateActivationKey)
        Me.groupBox1.Controls.Add(Me.btnGo)
        Me.groupBox1.Controls.Add(Me.txtOutput)
        Me.groupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.groupBox1.Location = New System.Drawing.Point(16, 62)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(926, 232)
        Me.groupBox1.TabIndex = 20
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Step1 - Configuration at your eCommerce Provider"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(735, 34)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(160, 40)
        Me.Button1.TabIndex = 18
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'label3
        '
        Me.label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label3.Location = New System.Drawing.Point(18, 27)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(569, 48)
        Me.label3.TabIndex = 2
        Me.label3.Text = resources.GetString("label3.Text")
        '
        'txtUrlGetActivationKey
        '
        Me.txtUrlGetActivationKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUrlGetActivationKey.Location = New System.Drawing.Point(21, 141)
        Me.txtUrlGetActivationKey.Name = "txtUrlGetActivationKey"
        Me.txtUrlGetActivationKey.Size = New System.Drawing.Size(687, 23)
        Me.txtUrlGetActivationKey.TabIndex = 4
        Me.txtUrlGetActivationKey.Text = "http://quicklicensemanager.com/kamstrup/qlm/qlmservice.asmx/GetActivationKey?is_p" & _
    "roductid=8&is_majorversion=5&is_minorversion=0"
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label5.Location = New System.Drawing.Point(20, 93)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(416, 17)
        Me.label5.TabIndex = 5
        Me.label5.Text = "Example: URL to the QLM web service GetActivationKey method:"
        '
        'btnCreateActivationKey
        '
        Me.btnCreateActivationKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateActivationKey.Location = New System.Drawing.Point(726, 178)
        Me.btnCreateActivationKey.Name = "btnCreateActivationKey"
        Me.btnCreateActivationKey.Size = New System.Drawing.Size(194, 45)
        Me.btnCreateActivationKey.TabIndex = 6
        Me.btnCreateActivationKey.Text = "&Generate Activation Key using CreateActivationKey"
        Me.btnCreateActivationKey.UseVisualStyleBackColor = True
        '
        'btnGo
        '
        Me.btnGo.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGo.Location = New System.Drawing.Point(726, 127)
        Me.btnGo.Name = "btnGo"
        Me.btnGo.Size = New System.Drawing.Size(194, 45)
        Me.btnGo.TabIndex = 6
        Me.btnGo.Text = "&Generate Activation Key using Url"
        Me.btnGo.UseVisualStyleBackColor = True
        '
        'txtOutput
        '
        Me.txtOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOutput.Location = New System.Drawing.Point(21, 165)
        Me.txtOutput.Multiline = True
        Me.txtOutput.Name = "txtOutput"
        Me.txtOutput.ReadOnly = True
        Me.txtOutput.Size = New System.Drawing.Size(687, 22)
        Me.txtOutput.TabIndex = 7
        Me.txtOutput.Text = "<click on Generate Activation Key to generate an activation key>"
        '
        'label1
        '
        Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label1.Location = New System.Drawing.Point(16, 9)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(920, 50)
        Me.label1.TabIndex = 19
        Me.label1.Text = resources.GetString("label1.Text")
        '
        'RadGridView1
        '
        Me.RadGridView1.Location = New System.Drawing.Point(16, 704)
        Me.RadGridView1.Name = "RadGridView1"
        Me.RadGridView1.Size = New System.Drawing.Size(926, 148)
        Me.RadGridView1.TabIndex = 16
        Me.RadGridView1.Text = "RadGridView1"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(958, 842)
        Me.Controls.Add(Me.RadGridView1)
        Me.Controls.Add(Me.groupBox2)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.label1)
        Me.Name = "Form1"
        Me.Text = "QLM Pro Sample"
        Me.groupBox2.ResumeLayout(False)
        Me.groupBox2.PerformLayout()
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        CType(Me.RadGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents label6 As System.Windows.Forms.Label
    Private WithEvents txtActivationKey As System.Windows.Forms.TextBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents btnActivate As System.Windows.Forms.Button
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents groupBox2 As System.Windows.Forms.GroupBox
    Private WithEvents txtOutput2 As System.Windows.Forms.TextBox
    Private WithEvents txtUrlQlmWeb As System.Windows.Forms.TextBox
    Private WithEvents label8 As System.Windows.Forms.Label
    Private WithEvents label9 As System.Windows.Forms.Label
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents txtUrlGetActivationKey As System.Windows.Forms.TextBox
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents btnGo As System.Windows.Forms.Button
    Private WithEvents txtOutput As System.Windows.Forms.TextBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents btnCreateActivationKey As System.Windows.Forms.Button
    Private WithEvents txtComputerKey As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents RadGridView1 As Telerik.WinControls.UI.RadGridView

End Class
