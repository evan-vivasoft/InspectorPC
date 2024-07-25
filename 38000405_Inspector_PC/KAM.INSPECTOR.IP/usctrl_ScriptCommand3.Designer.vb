<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_scriptCommand3
    Inherits INSPECTOR.IP.usctrl_MainInspectionStep

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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_scriptCommand3))
        Me.RadProgressBar1 = New Telerik.WinControls.UI.RadProgressBar()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.rdpInstruction = New Telerik.WinControls.UI.RadPanel()
        Me.rdrtbInstruction = New Telerik.WinControls.UI.RadRichTextEditor()
        CType(Me.RadProgressBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdpInstruction.SuspendLayout()
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadProgressBar1
        '
        resources.ApplyResources(Me.RadProgressBar1, "RadProgressBar1")
        Me.RadProgressBar1.Name = "RadProgressBar1"
        '
        'Timer1
        '
        '
        'rdpInstruction
        '
        resources.ApplyResources(Me.rdpInstruction, "rdpInstruction")
        Me.rdpInstruction.Controls.Add(Me.rdrtbInstruction)
        Me.rdpInstruction.Name = "rdpInstruction"
        '
        'rdrtbInstruction
        '
        resources.ApplyResources(Me.rdrtbInstruction, "rdrtbInstruction")
        Me.rdrtbInstruction.BorderColor = System.Drawing.Color.FromArgb(CType(CType(156, Byte), Integer), CType(CType(189, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.rdrtbInstruction.Name = "rdrtbInstruction"
        '
        '
        '
        Me.rdrtbInstruction.RootElement.AccessibleDescription = resources.GetString("rdrtbInstruction.RootElement.AccessibleDescription")
        Me.rdrtbInstruction.RootElement.AccessibleName = resources.GetString("rdrtbInstruction.RootElement.AccessibleName")
        Me.rdrtbInstruction.RootElement.Alignment = CType(resources.GetObject("rdrtbInstruction.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdrtbInstruction.RootElement.AngleTransform = CType(resources.GetObject("rdrtbInstruction.RootElement.AngleTransform"), Single)
        Me.rdrtbInstruction.RootElement.FlipText = CType(resources.GetObject("rdrtbInstruction.RootElement.FlipText"), Boolean)
        Me.rdrtbInstruction.RootElement.Margin = CType(resources.GetObject("rdrtbInstruction.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdrtbInstruction.RootElement.Text = resources.GetString("rdrtbInstruction.RootElement.Text")
        Me.rdrtbInstruction.RootElement.TextOrientation = CType(resources.GetObject("rdrtbInstruction.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        Me.rdrtbInstruction.SelectionFill = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(78, Byte), Integer), CType(CType(158, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'usctrl_scriptCommand3
        '
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.rdpInstruction)
        Me.Controls.Add(Me.RadProgressBar1)
        Me.Name = "usctrl_scriptCommand3"
        Me.Controls.SetChildIndex(Me.RadProgressBar1, 0)
        Me.Controls.SetChildIndex(Me.rdpInstruction, 0)
        CType(Me.RadProgressBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdpInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdpInstruction.ResumeLayout(False)
        CType(Me.rdrtbInstruction, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RadProgressBar1 As Telerik.WinControls.UI.RadProgressBar
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents rdpInstruction As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdrtbInstruction As Telerik.WinControls.UI.RadRichTextEditor

End Class
