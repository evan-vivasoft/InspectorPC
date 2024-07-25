<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SplashForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SplashForm))
        Me.rdlblStatus = New Telerik.WinControls.UI.RadLabel()
        Me.RadWaitingBar1 = New Telerik.WinControls.UI.RadWaitingBar()
        Me.rdlblAppName = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblWebServer = New Telerik.WinControls.UI.RadLabel()
        Me.rdlblVersion = New Telerik.WinControls.UI.RadLabel()
        Me.RadPanel1 = New Telerik.WinControls.UI.RadPanel()
        CType(Me.rdlblStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadWaitingBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblAppName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblWebServer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdlblVersion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'rdlblStatus
        '
        Me.rdlblStatus.BackColor = System.Drawing.Color.Transparent
        Me.rdlblStatus.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdlblStatus.Location = New System.Drawing.Point(1, 334)
        Me.rdlblStatus.Name = "rdlblStatus"
        '
        '
        '
        Me.rdlblStatus.RootElement.ControlBounds = New System.Drawing.Rectangle(1, 334, 100, 18)
        Me.rdlblStatus.Size = New System.Drawing.Size(56, 25)
        Me.rdlblStatus.TabIndex = 0
        Me.rdlblStatus.Text = "Status"
        '
        'RadWaitingBar1
        '
        Me.RadWaitingBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadWaitingBar1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.RadWaitingBar1.Location = New System.Drawing.Point(1, 359)
        Me.RadWaitingBar1.Name = "RadWaitingBar1"
        '
        '
        '
        Me.RadWaitingBar1.RootElement.ControlBounds = New System.Drawing.Rectangle(1, 359, 130, 24)
        Me.RadWaitingBar1.Size = New System.Drawing.Size(319, 15)
        Me.RadWaitingBar1.TabIndex = 1
        Me.RadWaitingBar1.Text = "rdWaitingBar"
        Me.RadWaitingBar1.WaitingStep = 3
        '
        'rdlblAppName
        '
        Me.rdlblAppName.BackColor = System.Drawing.Color.Transparent
        Me.rdlblAppName.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdlblAppName.ForeColor = System.Drawing.Color.White
        Me.rdlblAppName.Location = New System.Drawing.Point(11, 9)
        Me.rdlblAppName.Name = "rdlblAppName"
        '
        '
        '
        Me.rdlblAppName.RootElement.ControlBounds = New System.Drawing.Rectangle(11, 9, 100, 18)
        Me.rdlblAppName.Size = New System.Drawing.Size(184, 25)
        Me.rdlblAppName.TabIndex = 2
        Me.rdlblAppName.Text = "W&&S BV; QLM licenses"
        '
        'rdlblWebServer
        '
        Me.rdlblWebServer.BackColor = System.Drawing.Color.Transparent
        Me.rdlblWebServer.Font = New System.Drawing.Font("Segoe UI", 6.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdlblWebServer.Location = New System.Drawing.Point(6, 380)
        Me.rdlblWebServer.Name = "rdlblWebServer"
        '
        '
        '
        Me.rdlblWebServer.RootElement.ControlBounds = New System.Drawing.Rectangle(6, 380, 100, 18)
        Me.rdlblWebServer.Size = New System.Drawing.Size(51, 15)
        Me.rdlblWebServer.TabIndex = 3
        Me.rdlblWebServer.Text = "webServer"
        '
        'rdlblVersion
        '
        Me.rdlblVersion.BackColor = System.Drawing.Color.Transparent
        Me.rdlblVersion.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdlblVersion.ForeColor = System.Drawing.Color.White
        Me.rdlblVersion.Location = New System.Drawing.Point(108, 40)
        Me.rdlblVersion.Name = "rdlblVersion"
        '
        '
        '
        Me.rdlblVersion.RootElement.ControlBounds = New System.Drawing.Rectangle(108, 40, 100, 18)
        Me.rdlblVersion.Size = New System.Drawing.Size(46, 18)
        Me.rdlblVersion.TabIndex = 4
        Me.rdlblVersion.Text = "Version"
        '
        'RadPanel1
        '
        Me.RadPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RadPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.RadPanel1.BackgroundImage = CType(resources.GetObject("RadPanel1.BackgroundImage"), System.Drawing.Image)
        Me.RadPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.RadPanel1.Controls.Add(Me.rdlblVersion)
        Me.RadPanel1.Controls.Add(Me.rdlblAppName)
        Me.RadPanel1.Location = New System.Drawing.Point(1, 3)
        Me.RadPanel1.Name = "RadPanel1"
        '
        '
        '
        Me.RadPanel1.RootElement.ControlBounds = New System.Drawing.Rectangle(1, 3, 200, 100)
        Me.RadPanel1.Size = New System.Drawing.Size(319, 325)
        Me.RadPanel1.TabIndex = 5
        '
        'SplashForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ClientSize = New System.Drawing.Size(321, 411)
        Me.Controls.Add(Me.RadPanel1)
        Me.Controls.Add(Me.RadWaitingBar1)
        Me.Controls.Add(Me.rdlblWebServer)
        Me.Controls.Add(Me.rdlblStatus)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "SplashForm"
        Me.Opacity = 0.9R
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Wigersma & Sikkema BV; QLM licenses"
        Me.TransparencyKey = System.Drawing.Color.Silver
        CType(Me.rdlblStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadWaitingBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblAppName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblWebServer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdlblVersion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadPanel1.ResumeLayout(False)
        Me.RadPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents rdlblStatus As Telerik.WinControls.UI.RadLabel
    Private WithEvents RadWaitingBar1 As Telerik.WinControls.UI.RadWaitingBar
    Private WithEvents rdlblAppName As Telerik.WinControls.UI.RadLabel
    Private WithEvents rdlblWebServer As Telerik.WinControls.UI.RadLabel
    Private WithEvents rdlblVersion As Telerik.WinControls.UI.RadLabel
    Private WithEvents RadPanel1 As Telerik.WinControls.UI.RadPanel
End Class
