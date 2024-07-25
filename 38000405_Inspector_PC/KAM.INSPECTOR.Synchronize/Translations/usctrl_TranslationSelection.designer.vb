<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class usctrl_TranslationSelection
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(usctrl_TranslationSelection))
        Me.rdListTranslationSelection = New Telerik.WinControls.UI.RadListControl()
        CType(Me.rdListTranslationSelection, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdListTranslationSelection
        '
        Me.rdListTranslationSelection.AutoSizeItems = True
        resources.ApplyResources(Me.rdListTranslationSelection, "rdListTranslationSelection")
        Me.rdListTranslationSelection.Name = "rdListTranslationSelection"
        '
        'usctrl_TranslationSelection
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.rdListTranslationSelection)
        Me.Name = "usctrl_TranslationSelection"
        CType(Me.rdListTranslationSelection, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rdListTranslationSelection As Telerik.WinControls.UI.RadListControl

End Class
