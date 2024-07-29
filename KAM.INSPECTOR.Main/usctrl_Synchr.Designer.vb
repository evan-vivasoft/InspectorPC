<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class usctrl_Synchr
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_Synchr))
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.pictureConnectStatus = New System.Windows.Forms.PictureBox()
        Me.SyncButton = New System.Windows.Forms.Button()
        Me.rdGridSyncStatus = New Telerik.WinControls.UI.RadGridView()
        Me.rdProgressCopyfile = New Telerik.WinControls.UI.RadProgressBar()
        Me.rdWaitingSync = New Telerik.WinControls.UI.RadWaitingBar()
        CType(Me.pictureConnectStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridSyncStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridSyncStatus.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdProgressCopyfile, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdWaitingSync, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pictureConnectStatus
        '
        resources.ApplyResources(Me.pictureConnectStatus, "pictureConnectStatus")
        Me.pictureConnectStatus.Name = "pictureConnectStatus"
        Me.pictureConnectStatus.TabStop = False
        '
        'SyncButton
        '
        Me.SyncButton.BackColor = System.Drawing.SystemColors.Highlight
        resources.ApplyResources(Me.SyncButton, "SyncButton")
        Me.SyncButton.Name = "SyncButton"
        Me.SyncButton.UseVisualStyleBackColor = False
        '
        'rdGridSyncStatus
        '
        resources.ApplyResources(Me.rdGridSyncStatus, "rdGridSyncStatus")
        '
        '
        '
        Me.rdGridSyncStatus.MasterTemplate.ViewDefinition = TableViewDefinition1
        Me.rdGridSyncStatus.Name = "rdGridSyncStatus"
        '
        '
        '
        Me.rdGridSyncStatus.RootElement.AccessibleDescription = resources.GetString("rdGridSyncStatus.RootElement.AccessibleDescription")
        Me.rdGridSyncStatus.RootElement.AccessibleName = resources.GetString("rdGridSyncStatus.RootElement.AccessibleName")
        Me.rdGridSyncStatus.RootElement.Alignment = CType(resources.GetObject("rdGridSyncStatus.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdGridSyncStatus.RootElement.AngleTransform = CType(resources.GetObject("rdGridSyncStatus.RootElement.AngleTransform"), Single)
        Me.rdGridSyncStatus.RootElement.FlipText = CType(resources.GetObject("rdGridSyncStatus.RootElement.FlipText"), Boolean)
        Me.rdGridSyncStatus.RootElement.Margin = CType(resources.GetObject("rdGridSyncStatus.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdGridSyncStatus.RootElement.Text = resources.GetString("rdGridSyncStatus.RootElement.Text")
        Me.rdGridSyncStatus.RootElement.TextOrientation = CType(resources.GetObject("rdGridSyncStatus.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdProgressCopyfile
        '
        resources.ApplyResources(Me.rdProgressCopyfile, "rdProgressCopyfile")
        Me.rdProgressCopyfile.Name = "rdProgressCopyfile"
        '
        '
        '
        Me.rdProgressCopyfile.RootElement.AccessibleDescription = resources.GetString("rdProgressCopyfile.RootElement.AccessibleDescription")
        Me.rdProgressCopyfile.RootElement.AccessibleName = resources.GetString("rdProgressCopyfile.RootElement.AccessibleName")
        Me.rdProgressCopyfile.RootElement.Alignment = CType(resources.GetObject("rdProgressCopyfile.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdProgressCopyfile.RootElement.AngleTransform = CType(resources.GetObject("rdProgressCopyfile.RootElement.AngleTransform"), Single)
        Me.rdProgressCopyfile.RootElement.FlipText = CType(resources.GetObject("rdProgressCopyfile.RootElement.FlipText"), Boolean)
        Me.rdProgressCopyfile.RootElement.Margin = CType(resources.GetObject("rdProgressCopyfile.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdProgressCopyfile.RootElement.Text = resources.GetString("rdProgressCopyfile.RootElement.Text")
        Me.rdProgressCopyfile.RootElement.TextOrientation = CType(resources.GetObject("rdProgressCopyfile.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdWaitingSync
        '
        resources.ApplyResources(Me.rdWaitingSync, "rdWaitingSync")
        Me.rdWaitingSync.Name = "rdWaitingSync"
        '
        '
        '
        Me.rdWaitingSync.RootElement.AccessibleDescription = resources.GetString("rdWaitingSync.RootElement.AccessibleDescription")
        Me.rdWaitingSync.RootElement.AccessibleName = resources.GetString("rdWaitingSync.RootElement.AccessibleName")
        Me.rdWaitingSync.RootElement.Alignment = CType(resources.GetObject("rdWaitingSync.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdWaitingSync.RootElement.AngleTransform = CType(resources.GetObject("rdWaitingSync.RootElement.AngleTransform"), Single)
        Me.rdWaitingSync.RootElement.FlipText = CType(resources.GetObject("rdWaitingSync.RootElement.FlipText"), Boolean)
        Me.rdWaitingSync.RootElement.Margin = CType(resources.GetObject("rdWaitingSync.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdWaitingSync.RootElement.Text = resources.GetString("rdWaitingSync.RootElement.Text")
        Me.rdWaitingSync.RootElement.TextOrientation = CType(resources.GetObject("rdWaitingSync.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.rdWaitingSync.WaitingSpeed = 100
        Me.rdWaitingSync.WaitingStep = 2
        '
        'usctrl_Synchr
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.rdProgressCopyfile)
        Me.Controls.Add(Me.rdWaitingSync)
        Me.Controls.Add(Me.rdGridSyncStatus)
        Me.Controls.Add(Me.SyncButton)
        Me.Controls.Add(Me.pictureConnectStatus)
        Me.Name = "usctrl_Synchr"
        CType(Me.pictureConnectStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridSyncStatus.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridSyncStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdProgressCopyfile, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdWaitingSync, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents pictureConnectStatus As System.Windows.Forms.PictureBox
    Friend WithEvents SyncButton As Button
    Friend WithEvents rdGridSyncStatus As Telerik.WinControls.UI.RadGridView
    Friend WithEvents rdProgressCopyfile As Telerik.WinControls.UI.RadProgressBar
    Friend WithEvents rdWaitingSync As Telerik.WinControls.UI.RadWaitingBar
End Class
