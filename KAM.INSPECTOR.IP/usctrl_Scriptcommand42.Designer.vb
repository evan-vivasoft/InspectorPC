<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_Scriptcommand42
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_Scriptcommand42))
        Me.radTxtBRemarks = New Telerik.WinControls.UI.RadTextBox()
        CType(Me.radTxtBRemarks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'radTxtBRemarks
        '
        Me.radTxtBRemarks.AcceptsReturn = True
        resources.ApplyResources(Me.radTxtBRemarks, "radTxtBRemarks")
        Me.radTxtBRemarks.MaxLength = 255
        Me.radTxtBRemarks.Multiline = True
        Me.radTxtBRemarks.Name = "radTxtBRemarks"
        Me.radTxtBRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.radTxtBRemarks.TabStop = False
        '
        'usctrl_Scriptcommand42
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.radTxtBRemarks)
        Me.Name = "usctrl_Scriptcommand42"
        CType(Me.radTxtBRemarks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents radTxtBRemarks As Telerik.WinControls.UI.RadTextBox

End Class
