<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DisplayResults

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DisplayResults))
        Me.webBrResults = New System.Windows.Forms.WebBrowser()
        Me.rdbShowResults = New Telerik.WinControls.UI.RadButton()
        CType(Me.rdbShowResults, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'webBrResults
        '
        resources.ApplyResources(Me.webBrResults, "webBrResults")
        Me.webBrResults.Name = "webBrResults"
        '
        'rdbShowResults
        '
        resources.ApplyResources(Me.rdbShowResults, "rdbShowResults")
        Me.rdbShowResults.Name = "rdbShowResults"
        '
        'DisplayResults
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.rdbShowResults)
        Me.Controls.Add(Me.webBrResults)
        Me.Name = "DisplayResults"
        CType(Me.rdbShowResults, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents webBrResults As System.Windows.Forms.WebBrowser
    Friend WithEvents rdbShowResults As Telerik.WinControls.UI.RadButton

End Class
