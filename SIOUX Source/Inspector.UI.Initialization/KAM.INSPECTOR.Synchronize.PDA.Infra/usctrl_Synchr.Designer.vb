<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_Synchr

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_Synchr))
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Dim TableViewDefinition2 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.rdWaitingSync = New Telerik.WinControls.UI.RadWaitingBar()
        Me.rdProgressCopyfile = New Telerik.WinControls.UI.RadProgressBar()
        Me.rdGridSyncStatus = New Telerik.WinControls.UI.RadGridView()
        Me.rdGridPDA = New Telerik.WinControls.UI.RadGridView()
        Me.pictureConnectStatus = New System.Windows.Forms.PictureBox()
        CType(Me.rdWaitingSync, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdProgressCopyfile, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridSyncStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridSyncStatus.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridPDA, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGridPDA.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pictureConnectStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdWaitingSync
        '
        resources.ApplyResources(Me.rdWaitingSync, "rdWaitingSync")
        Me.rdWaitingSync.Name = "rdWaitingSync"
        Me.rdWaitingSync.WaitingSpeed = 100
        Me.rdWaitingSync.WaitingStep = 2
        '
        'rdProgressCopyfile
        '
        resources.ApplyResources(Me.rdProgressCopyfile, "rdProgressCopyfile")
        Me.rdProgressCopyfile.Name = "rdProgressCopyfile"
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
        'rdGridPDA
        '
        resources.ApplyResources(Me.rdGridPDA, "rdGridPDA")
        '
        '
        '
        Me.rdGridPDA.MasterTemplate.ViewDefinition = TableViewDefinition2
        Me.rdGridPDA.Name = "rdGridPDA"
        '
        'pictureConnectStatus
        '
        resources.ApplyResources(Me.pictureConnectStatus, "pictureConnectStatus")
        Me.pictureConnectStatus.Name = "pictureConnectStatus"
        Me.pictureConnectStatus.TabStop = False
        '
        'usctrl_Synchr
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pictureConnectStatus)
        Me.Controls.Add(Me.rdGridPDA)
        Me.Controls.Add(Me.rdProgressCopyfile)
        Me.Controls.Add(Me.rdGridSyncStatus)
        Me.Controls.Add(Me.rdWaitingSync)
        Me.Name = "usctrl_Synchr"
        CType(Me.rdWaitingSync, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdProgressCopyfile, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridSyncStatus.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridSyncStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridPDA.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGridPDA, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pictureConnectStatus, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TextBox1 As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents rdWaitingSync As Telerik.WinControls.UI.RadWaitingBar
    Friend WithEvents rdProgressCopyfile As Telerik.WinControls.UI.RadProgressBar
    Friend WithEvents rdGridSyncStatus As Telerik.WinControls.UI.RadGridView
    Friend WithEvents rdGridPDA As Telerik.WinControls.UI.RadGridView
    Friend WithEvents pictureConnectStatus As System.Windows.Forms.PictureBox

End Class
