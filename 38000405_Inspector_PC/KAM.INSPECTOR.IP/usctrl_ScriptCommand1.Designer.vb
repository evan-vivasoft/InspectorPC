<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_scriptCommand1
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_scriptCommand1))
        Me.rdpInstruction = New Telerik.WinControls.UI.RadPanel()
        Me.rdrtbInstruction = New Telerik.WinControls.UI.RadRichTextEditor()
        CType(Me.rdpInstruction,System.ComponentModel.ISupportInitialize).BeginInit
        Me.rdpInstruction.SuspendLayout
        CType(Me.rdrtbInstruction,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'rdpInstruction
        '
        resources.ApplyResources(Me.rdpInstruction, "rdpInstruction")
        Me.rdpInstruction.Controls.Add(Me.rdrtbInstruction)
        Me.rdpInstruction.Name = "rdpInstruction"
        '
        '
        '
        Me.rdpInstruction.RootElement.AccessibleDescription = resources.GetString("rdpInstruction.RootElement.AccessibleDescription")
        Me.rdpInstruction.RootElement.AccessibleName = resources.GetString("rdpInstruction.RootElement.AccessibleName")
        Me.rdpInstruction.RootElement.Alignment = CType(resources.GetObject("rdpInstruction.RootElement.Alignment"),System.Drawing.ContentAlignment)
        Me.rdpInstruction.RootElement.AngleTransform = CType(resources.GetObject("rdpInstruction.RootElement.AngleTransform"),Single)
        Me.rdpInstruction.RootElement.FlipText = CType(resources.GetObject("rdpInstruction.RootElement.FlipText"),Boolean)
        Me.rdpInstruction.RootElement.Margin = CType(resources.GetObject("rdpInstruction.RootElement.Margin"),System.Windows.Forms.Padding)
        Me.rdpInstruction.RootElement.Text = resources.GetString("rdpInstruction.RootElement.Text")
        Me.rdpInstruction.RootElement.TextOrientation = CType(resources.GetObject("rdpInstruction.RootElement.TextOrientation"),System.Windows.Forms.Orientation)
        '
        'rdrtbInstruction
        '
        resources.ApplyResources(Me.rdrtbInstruction, "rdrtbInstruction")
        Me.rdrtbInstruction.BorderColor = System.Drawing.Color.FromArgb(CType(CType(156, Byte), Integer), CType(CType(189, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.rdrtbInstruction.Name = "rdrtbInstruction"
        Me.rdrtbInstruction.SelectionFill = System.Drawing.Color.FromArgb(CType(CType(128,Byte),Integer), CType(CType(78,Byte),Integer), CType(CType(158,Byte),Integer), CType(CType(255,Byte),Integer))
        '
        'usctrl_scriptCommand1
        '
        resources.ApplyResources(Me, "$this")
        Me.Controls.Add(Me.rdpInstruction)
        Me.Name = "usctrl_scriptCommand1"
        Me.Controls.SetChildIndex(Me.rdpInstruction, 0)
        CType(Me.rdpInstruction,System.ComponentModel.ISupportInitialize).EndInit
        Me.rdpInstruction.ResumeLayout(false)
        CType(Me.rdrtbInstruction,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents rdpInstruction As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdrtbInstruction As Telerik.WinControls.UI.RadRichTextEditor

End Class
