<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uscrtl_ChartSettings

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uscrtl_ChartSettings))
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series3 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.rdDropDownPalletteColor = New Telerik.WinControls.UI.RadDropDownList()
        Me.rDropDownChartBackGroundColor = New Telerik.WinControls.UI.RadDropDownList()
        Me.Chart1 = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.RadLabel1 = New Telerik.WinControls.UI.RadLabel()
        Me.rDropDownStyle = New Telerik.WinControls.UI.RadDropDownList()
        Me.rDropDownAlignment = New Telerik.WinControls.UI.RadDropDownList()
        Me.rDropDownDock = New Telerik.WinControls.UI.RadDropDownList()
        Me.RadLabel4 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel3 = New Telerik.WinControls.UI.RadLabel()
        Me.RadLabel2 = New Telerik.WinControls.UI.RadLabel()
        Me.rdPanelChart = New Telerik.WinControls.UI.RadPanel()
        Me.rdInsideChart = New Telerik.WinControls.UI.RadLabel()
        Me.rDropDownInsideChart = New Telerik.WinControls.UI.RadDropDownList()
        Me.rdGroupLegend = New Telerik.WinControls.UI.RadGroupBox()
        Me.RadLabel5 = New Telerik.WinControls.UI.RadLabel()
        Me.rDropDownChartLineSize = New Telerik.WinControls.UI.RadDropDownList()
        CType(Me.rdDropDownPalletteColor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownChartBackGroundColor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownStyle, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownAlignment, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownDock, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdPanelChart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdPanelChart.SuspendLayout()
        CType(Me.rdInsideChart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownInsideChart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rdGroupLegend, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.rdGroupLegend.SuspendLayout()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rDropDownChartLineSize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdDropDownPalletteColor
        '
        resources.ApplyResources(Me.rdDropDownPalletteColor, "rdDropDownPalletteColor")
        Me.rdDropDownPalletteColor.Name = "rdDropDownPalletteColor"
        '
        'rDropDownChartBackGroundColor
        '
        resources.ApplyResources(Me.rDropDownChartBackGroundColor, "rDropDownChartBackGroundColor")
        Me.rDropDownChartBackGroundColor.Name = "rDropDownChartBackGroundColor"
        '
        'Chart1
        '
        Me.Chart1.BackColor = System.Drawing.Color.Transparent
        ChartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray
        ChartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray
        ChartArea1.BackColor = System.Drawing.Color.Black
        ChartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid
        ChartArea1.Name = "Default"
        Me.Chart1.ChartAreas.Add(ChartArea1)
        resources.ApplyResources(Me.Chart1, "Chart1")
        Legend1.BackColor = System.Drawing.Color.Transparent
        Legend1.DockedToChartArea = "Default"
        Legend1.IsDockedInsideChartArea = False
        Legend1.Name = "Default"
        Me.Chart1.Legends.Add(Legend1)
        Me.Chart1.Name = "Chart1"
        Me.Chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright
        Series1.ChartArea = "Default"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series1.Legend = "Default"
        Series1.Name = "Series1"
        Series2.ChartArea = "Default"
        Series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series2.Legend = "Default"
        Series2.Name = "Series2"
        Series3.ChartArea = "Default"
        Series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series3.Legend = "Default"
        Series3.Name = "Series3"
        Me.Chart1.Series.Add(Series1)
        Me.Chart1.Series.Add(Series2)
        Me.Chart1.Series.Add(Series3)
        '
        'RadLabel1
        '
        resources.ApplyResources(Me.RadLabel1, "RadLabel1")
        Me.RadLabel1.Name = "RadLabel1"
        '
        'rDropDownStyle
        '
        resources.ApplyResources(Me.rDropDownStyle, "rDropDownStyle")
        Me.rDropDownStyle.Name = "rDropDownStyle"
        '
        '
        '
        Me.rDropDownStyle.RootElement.AccessibleDescription = resources.GetString("rDropDownStyle.RootElement.AccessibleDescription")
        Me.rDropDownStyle.RootElement.AccessibleName = resources.GetString("rDropDownStyle.RootElement.AccessibleName")
        Me.rDropDownStyle.RootElement.Alignment = CType(resources.GetObject("rDropDownStyle.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rDropDownStyle.RootElement.AngleTransform = CType(resources.GetObject("rDropDownStyle.RootElement.AngleTransform"), Single)
        Me.rDropDownStyle.RootElement.FlipText = CType(resources.GetObject("rDropDownStyle.RootElement.FlipText"), Boolean)
        Me.rDropDownStyle.RootElement.Margin = CType(resources.GetObject("rDropDownStyle.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rDropDownStyle.RootElement.Padding = CType(resources.GetObject("rDropDownStyle.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.rDropDownStyle.RootElement.Text = resources.GetString("rDropDownStyle.RootElement.Text")
        Me.rDropDownStyle.RootElement.TextOrientation = CType(resources.GetObject("rDropDownStyle.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rDropDownAlignment
        '
        resources.ApplyResources(Me.rDropDownAlignment, "rDropDownAlignment")
        Me.rDropDownAlignment.Name = "rDropDownAlignment"
        '
        '
        '
        Me.rDropDownAlignment.RootElement.AccessibleDescription = resources.GetString("rDropDownAlignment.RootElement.AccessibleDescription")
        Me.rDropDownAlignment.RootElement.AccessibleName = resources.GetString("rDropDownAlignment.RootElement.AccessibleName")
        Me.rDropDownAlignment.RootElement.Alignment = CType(resources.GetObject("rDropDownAlignment.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rDropDownAlignment.RootElement.AngleTransform = CType(resources.GetObject("rDropDownAlignment.RootElement.AngleTransform"), Single)
        Me.rDropDownAlignment.RootElement.FlipText = CType(resources.GetObject("rDropDownAlignment.RootElement.FlipText"), Boolean)
        Me.rDropDownAlignment.RootElement.Margin = CType(resources.GetObject("rDropDownAlignment.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rDropDownAlignment.RootElement.Padding = CType(resources.GetObject("rDropDownAlignment.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.rDropDownAlignment.RootElement.Text = resources.GetString("rDropDownAlignment.RootElement.Text")
        Me.rDropDownAlignment.RootElement.TextOrientation = CType(resources.GetObject("rDropDownAlignment.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rDropDownDock
        '
        resources.ApplyResources(Me.rDropDownDock, "rDropDownDock")
        Me.rDropDownDock.Name = "rDropDownDock"
        '
        'RadLabel4
        '
        resources.ApplyResources(Me.RadLabel4, "RadLabel4")
        Me.RadLabel4.Name = "RadLabel4"
        '
        '
        '
        Me.RadLabel4.RootElement.AccessibleDescription = resources.GetString("RadLabel4.RootElement.AccessibleDescription")
        Me.RadLabel4.RootElement.AccessibleName = resources.GetString("RadLabel4.RootElement.AccessibleName")
        Me.RadLabel4.RootElement.Alignment = CType(resources.GetObject("RadLabel4.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RadLabel4.RootElement.AngleTransform = CType(resources.GetObject("RadLabel4.RootElement.AngleTransform"), Single)
        Me.RadLabel4.RootElement.FlipText = CType(resources.GetObject("RadLabel4.RootElement.FlipText"), Boolean)
        Me.RadLabel4.RootElement.Margin = CType(resources.GetObject("RadLabel4.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RadLabel4.RootElement.Padding = CType(resources.GetObject("RadLabel4.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.RadLabel4.RootElement.Text = resources.GetString("RadLabel4.RootElement.Text")
        Me.RadLabel4.RootElement.TextOrientation = CType(resources.GetObject("RadLabel4.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'RadLabel3
        '
        resources.ApplyResources(Me.RadLabel3, "RadLabel3")
        Me.RadLabel3.Name = "RadLabel3"
        '
        '
        '
        Me.RadLabel3.RootElement.AccessibleDescription = resources.GetString("RadLabel3.RootElement.AccessibleDescription")
        Me.RadLabel3.RootElement.AccessibleName = resources.GetString("RadLabel3.RootElement.AccessibleName")
        Me.RadLabel3.RootElement.Alignment = CType(resources.GetObject("RadLabel3.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RadLabel3.RootElement.AngleTransform = CType(resources.GetObject("RadLabel3.RootElement.AngleTransform"), Single)
        Me.RadLabel3.RootElement.FlipText = CType(resources.GetObject("RadLabel3.RootElement.FlipText"), Boolean)
        Me.RadLabel3.RootElement.Margin = CType(resources.GetObject("RadLabel3.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RadLabel3.RootElement.Padding = CType(resources.GetObject("RadLabel3.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.RadLabel3.RootElement.Text = resources.GetString("RadLabel3.RootElement.Text")
        Me.RadLabel3.RootElement.TextOrientation = CType(resources.GetObject("RadLabel3.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'RadLabel2
        '
        resources.ApplyResources(Me.RadLabel2, "RadLabel2")
        Me.RadLabel2.Name = "RadLabel2"
        '
        'rdPanelChart
        '
        resources.ApplyResources(Me.rdPanelChart, "rdPanelChart")
        Me.rdPanelChart.Controls.Add(Me.Chart1)
        Me.rdPanelChart.ForeColor = System.Drawing.Color.Black
        Me.rdPanelChart.Name = "rdPanelChart"
        '
        'rdInsideChart
        '
        resources.ApplyResources(Me.rdInsideChart, "rdInsideChart")
        Me.rdInsideChart.Name = "rdInsideChart"
        '
        '
        '
        Me.rdInsideChart.RootElement.AccessibleDescription = resources.GetString("rdInsideChart.RootElement.AccessibleDescription")
        Me.rdInsideChart.RootElement.AccessibleName = resources.GetString("rdInsideChart.RootElement.AccessibleName")
        Me.rdInsideChart.RootElement.Alignment = CType(resources.GetObject("rdInsideChart.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rdInsideChart.RootElement.AngleTransform = CType(resources.GetObject("rdInsideChart.RootElement.AngleTransform"), Single)
        Me.rdInsideChart.RootElement.FlipText = CType(resources.GetObject("rdInsideChart.RootElement.FlipText"), Boolean)
        Me.rdInsideChart.RootElement.Margin = CType(resources.GetObject("rdInsideChart.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rdInsideChart.RootElement.Padding = CType(resources.GetObject("rdInsideChart.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.rdInsideChart.RootElement.Text = resources.GetString("rdInsideChart.RootElement.Text")
        Me.rdInsideChart.RootElement.TextOrientation = CType(resources.GetObject("rdInsideChart.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rDropDownInsideChart
        '
        resources.ApplyResources(Me.rDropDownInsideChart, "rDropDownInsideChart")
        Me.rDropDownInsideChart.Name = "rDropDownInsideChart"
        '
        '
        '
        Me.rDropDownInsideChart.RootElement.AccessibleDescription = resources.GetString("rDropDownInsideChart.RootElement.AccessibleDescription")
        Me.rDropDownInsideChart.RootElement.AccessibleName = resources.GetString("rDropDownInsideChart.RootElement.AccessibleName")
        Me.rDropDownInsideChart.RootElement.Alignment = CType(resources.GetObject("rDropDownInsideChart.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rDropDownInsideChart.RootElement.AngleTransform = CType(resources.GetObject("rDropDownInsideChart.RootElement.AngleTransform"), Single)
        Me.rDropDownInsideChart.RootElement.FlipText = CType(resources.GetObject("rDropDownInsideChart.RootElement.FlipText"), Boolean)
        Me.rDropDownInsideChart.RootElement.Margin = CType(resources.GetObject("rDropDownInsideChart.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rDropDownInsideChart.RootElement.Padding = CType(resources.GetObject("rDropDownInsideChart.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.rDropDownInsideChart.RootElement.Text = resources.GetString("rDropDownInsideChart.RootElement.Text")
        Me.rDropDownInsideChart.RootElement.TextOrientation = CType(resources.GetObject("rDropDownInsideChart.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rdGroupLegend
        '
        Me.rdGroupLegend.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me.rdGroupLegend.Controls.Add(Me.rDropDownInsideChart)
        Me.rdGroupLegend.Controls.Add(Me.RadLabel2)
        Me.rdGroupLegend.Controls.Add(Me.rdInsideChart)
        Me.rdGroupLegend.Controls.Add(Me.rDropDownDock)
        Me.rdGroupLegend.Controls.Add(Me.rDropDownStyle)
        Me.rdGroupLegend.Controls.Add(Me.RadLabel3)
        Me.rdGroupLegend.Controls.Add(Me.RadLabel4)
        Me.rdGroupLegend.Controls.Add(Me.rDropDownAlignment)
        resources.ApplyResources(Me.rdGroupLegend, "rdGroupLegend")
        Me.rdGroupLegend.Name = "rdGroupLegend"
        '
        '
        '
        Me.rdGroupLegend.RootElement.Padding = CType(resources.GetObject("rdGroupLegend.RootElement.Padding"), System.Windows.Forms.Padding)
        '
        'RadLabel5
        '
        resources.ApplyResources(Me.RadLabel5, "RadLabel5")
        Me.RadLabel5.Name = "RadLabel5"
        '
        '
        '
        Me.RadLabel5.RootElement.AccessibleDescription = resources.GetString("RadLabel5.RootElement.AccessibleDescription")
        Me.RadLabel5.RootElement.AccessibleName = resources.GetString("RadLabel5.RootElement.AccessibleName")
        Me.RadLabel5.RootElement.Alignment = CType(resources.GetObject("RadLabel5.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.RadLabel5.RootElement.AngleTransform = CType(resources.GetObject("RadLabel5.RootElement.AngleTransform"), Single)
        Me.RadLabel5.RootElement.FlipText = CType(resources.GetObject("RadLabel5.RootElement.FlipText"), Boolean)
        Me.RadLabel5.RootElement.Margin = CType(resources.GetObject("RadLabel5.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.RadLabel5.RootElement.Padding = CType(resources.GetObject("RadLabel5.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.RadLabel5.RootElement.Text = resources.GetString("RadLabel5.RootElement.Text")
        Me.RadLabel5.RootElement.TextOrientation = CType(resources.GetObject("RadLabel5.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'rDropDownChartLineSize
        '
        Me.rDropDownChartLineSize.DropDownStyle  = Telerik.WinControls.RadDropDownStyle.DropDownList
        resources.ApplyResources(Me.rDropDownChartLineSize, "rDropDownChartLineSize")
        Me.rDropDownChartLineSize.Name = "rDropDownChartLineSize"
        '
        '
        '
        Me.rDropDownChartLineSize.RootElement.AccessibleDescription = resources.GetString("rDropDownChartLineSize.RootElement.AccessibleDescription")
        Me.rDropDownChartLineSize.RootElement.AccessibleName = resources.GetString("rDropDownChartLineSize.RootElement.AccessibleName")
        Me.rDropDownChartLineSize.RootElement.Alignment = CType(resources.GetObject("rDropDownChartLineSize.RootElement.Alignment"), System.Drawing.ContentAlignment)
        Me.rDropDownChartLineSize.RootElement.AngleTransform = CType(resources.GetObject("rDropDownChartLineSize.RootElement.AngleTransform"), Single)
        Me.rDropDownChartLineSize.RootElement.FlipText = CType(resources.GetObject("rDropDownChartLineSize.RootElement.FlipText"), Boolean)
        Me.rDropDownChartLineSize.RootElement.Margin = CType(resources.GetObject("rDropDownChartLineSize.RootElement.Margin"), System.Windows.Forms.Padding)
        Me.rDropDownChartLineSize.RootElement.Padding = CType(resources.GetObject("rDropDownChartLineSize.RootElement.Padding"), System.Windows.Forms.Padding)
        Me.rDropDownChartLineSize.RootElement.Text = resources.GetString("rDropDownChartLineSize.RootElement.Text")
        Me.rDropDownChartLineSize.RootElement.TextOrientation = CType(resources.GetObject("rDropDownChartLineSize.RootElement.TextOrientation"), System.Windows.Forms.Orientation)
        '
        'uscrtl_ChartSettings
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RadLabel5)
        Me.Controls.Add(Me.rDropDownChartLineSize)
        Me.Controls.Add(Me.rdGroupLegend)
        Me.Controls.Add(Me.rdPanelChart)
        Me.Controls.Add(Me.RadLabel1)
        Me.Controls.Add(Me.rDropDownChartBackGroundColor)
        Me.Controls.Add(Me.rdDropDownPalletteColor)
        Me.Name = "uscrtl_ChartSettings"
        CType(Me.rdDropDownPalletteColor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownChartBackGroundColor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownStyle, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownAlignment, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownDock, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdPanelChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdPanelChart.ResumeLayout(False)
        CType(Me.rdInsideChart, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownInsideChart, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rdGroupLegend, System.ComponentModel.ISupportInitialize).EndInit()
        Me.rdGroupLegend.ResumeLayout(False)
        Me.rdGroupLegend.PerformLayout()
        CType(Me.RadLabel5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rDropDownChartLineSize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdDropDownPalletteColor As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rDropDownChartBackGroundColor As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents Chart1 As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents RadLabel1 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel4 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel3 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadLabel2 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rDropDownStyle As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rDropDownAlignment As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rDropDownDock As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rdPanelChart As Telerik.WinControls.UI.RadPanel
    Friend WithEvents rdInsideChart As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rDropDownInsideChart As Telerik.WinControls.UI.RadDropDownList
    Friend WithEvents rdGroupLegend As Telerik.WinControls.UI.RadGroupBox
    Friend WithEvents RadLabel5 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rDropDownChartLineSize As Telerik.WinControls.UI.RadDropDownList

End Class
