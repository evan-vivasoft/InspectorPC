'===============================================================================
'Copyright Wigersma 2012
'All rights reserved.
'===============================================================================
Imports System.Windows.Forms.DataVisualization.Charting
Imports System
Imports System.Drawing
Imports System.IO
Imports KAM.INSPECTOR.Main.My.Resources
Imports KAM.INSPECTOR.Infra
Imports KAM.Infra
Public Class uscrtl_ChartSettings
#Region "Class members"
    'MOD 10 Const chartSettingsFile As String = "config\ChartSettings.xml"


    'ReadOnly chartSettingsFile As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WS-gas\CONNEXION V5.x\INSPECTORPC", "ChartSettings.xml")
    Dim chartColorPalette1 As Object
#End Region
#Region "Constructor"
    ''' <summary>
    ''' Initialize form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ''ChartColorPalette1 = New ChartColorPalette
        ''For Each Me.chartColorPalette1 In [Enum].GetValues(GetType(ChartColorPalette))
        ''    rdDropDownPalletteColor.Items.Add(chartColorPalette1.ToString)
        ''Next

        rDropDownChartBackGroundColor.Items.Add(INSPECTORMainResx.str_GraphBackColorBlack)
        rDropDownChartBackGroundColor.Items.Add(INSPECTORMainResx.str_GraphBackColorWhite)

        'MOD 05
        'Set the line sizes
        For i As Integer = 1 To 5
            rDropDownChartLineSize.Items.Add(i)
        Next


        LoadChartTemplate(chartSettingsFile)
        Chart1.Series.Clear()
        'Create new series
        Dim series1 As New Series()
        Dim series2 As New Series()
        Dim series3 As New Series()

        'Define line types
        series1.ChartType = SeriesChartType.FastLine
        series2.ChartType = SeriesChartType.FastLine
        series3.ChartType = SeriesChartType.FastLine

        'Create lines
        For i As Integer = 0 To 100
            series1.Points.Add(i)
            series2.Points.Add(i + 2)
            series3.Points.Add(i + 4)
        Next

        'Add the series to the chart
        Chart1.Series.Add(series1)
        Chart1.Series.Add(series2)
        Chart1.Series.Add(series3)
    End Sub
    ''' <summary>
    ''' Loading form setting values 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub uscrtl_ChartSettings_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim backColor As String = ""
        backColor = ModuleSettings.SettingFile.GetSetting(GsSectionChart, GsSettingChartBackColor)
        If backColor = "White" Then
            rDropDownChartBackGroundColor.Text = INSPECTORMainResx.str_GraphBackColorWhite
        Else
            rDropDownChartBackGroundColor.Text = INSPECTORMainResx.str_GraphBackColorBlack
        End If
        'Styles
        For Each styleName As String In [Enum].GetNames(GetType(LegendStyle))
            If styleName = LegendStyle.Table.ToString Then rDropDownStyle.Items.Add(INSPECTORMainResx.str_legend_Style_Table)
            If styleName = LegendStyle.Row.ToString Then rDropDownStyle.Items.Add(INSPECTORMainResx.str_legend_Style_Row)
            If styleName = LegendStyle.Column.ToString Then rDropDownStyle.Items.Add(INSPECTORMainResx.str_legend_Style_Column)
        Next
        Select Case Chart1.Legends("Default").LegendStyle
            Case LegendStyle.Table
                rDropDownStyle.Text = INSPECTORMainResx.str_legend_Style_Table
            Case LegendStyle.Row
                rDropDownStyle.Text = INSPECTORMainResx.str_legend_Style_Row
            Case LegendStyle.Column
                rDropDownStyle.Text = INSPECTORMainResx.str_legend_Style_Column
        End Select


        'Docking
        For Each dockName As String In [Enum].GetNames(GetType(Docking))
            If dockName = Docking.Bottom.ToString Then rDropDownDock.Items.Add(INSPECTORMainResx.str_legend_Docking_Bottom)
            If dockName = Docking.Left.ToString Then rDropDownDock.Items.Add(INSPECTORMainResx.str_legend_Docking_Left)
            If dockName = Docking.Right.ToString Then rDropDownDock.Items.Add(INSPECTORMainResx.str_legend_Docking_Right)
            If dockName = Docking.Top.ToString Then rDropDownDock.Items.Add(INSPECTORMainResx.str_legend_Docking_Top)
        Next
        Select Case Chart1.Legends("Default").Docking
            Case Docking.Bottom
                rDropDownDock.Text = INSPECTORMainResx.str_legend_Docking_Bottom
            Case Docking.Left
                rDropDownDock.Text = INSPECTORMainResx.str_legend_Docking_Left
            Case Docking.Right
                rDropDownDock.Text = INSPECTORMainResx.str_legend_Docking_Right
            Case Docking.Top
                rDropDownDock.Text = INSPECTORMainResx.str_legend_Docking_Top
        End Select


        'Alignment
        For Each alignName As String In [Enum].GetNames(GetType(StringAlignment))
            If alignName = StringAlignment.Center.ToString Then rDropDownAlignment.Items.Add(INSPECTORMainResx.str_legend_Alignment_Center)
            If alignName = StringAlignment.Far.ToString Then rDropDownAlignment.Items.Add(INSPECTORMainResx.str_legend_Alignment_Far)
            If alignName = StringAlignment.Near.ToString Then rDropDownAlignment.Items.Add(INSPECTORMainResx.str_legend_Alignment_Near)
        Next
        Select Case Chart1.Legends("Default").Alignment
            Case StringAlignment.Center
                rDropDownAlignment.Text = INSPECTORMainResx.str_legend_Alignment_Center
            Case StringAlignment.Far
                rDropDownAlignment.Text = INSPECTORMainResx.str_legend_Alignment_Far
            Case StringAlignment.Near
                rDropDownAlignment.Text = INSPECTORMainResx.str_legend_Alignment_Near

        End Select
        rDropDownInsideChart.Items.Add(INSPECTORMainResx.str_legend_DockinChart_True)
        rDropDownInsideChart.Items.Add(INSPECTORMainResx.str_legend_DockinChart_False)

        'Docking
        Select Case Chart1.Legends("Default").IsDockedInsideChartArea
            Case True
                rDropDownInsideChart.Text = INSPECTORMainResx.str_legend_DockinChart_True
            Case False
                rDropDownInsideChart.Text = INSPECTORMainResx.str_legend_DockinChart_False
        End Select

        'MOD 05
        'Set the selected line size
        Dim LineSize As Integer
        LineSize = ModuleSettings.SettingFile.GetSetting(GsSectionChart, GsSettingChartSeriesLineSize)
        rDropDownChartLineSize.Text = LineSize
        Chart1.Series("Series1").BorderWidth = LineSize
        Chart1.Series("Series2").BorderWidth = LineSize
        Chart1.Series("Series3").BorderWidth = LineSize
    End Sub
#End Region
#Region "Form Handling"
    ''' <summary>
    ''' Handling of pallette color change
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rdDropDownPalletteColor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdDropDownPalletteColor.SelectedIndexChanged
        Chart1.Palette = [Enum].Parse(GetType(ChartColorPalette), rdDropDownPalletteColor.SelectedText)
        ApplyColoursAcrossLegend(Chart1)
    End Sub
    ''' <summary>
    ''' Handling of Back ground color change
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownGraphBackGroundColor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rDropDownChartBackGroundColor.SelectedIndexChanged
        If rDropDownChartBackGroundColor.SelectedText = INSPECTORMainResx.str_GraphBackColorBlack Then
            ModuleSettings.SettingFile.SaveSetting(GsSectionChart, GsSettingChartBackColor) = "Black"

            Chart1.ChartAreas("Default").BackColor = Color.Black
            Chart1.ChartAreas("Default").AxisX.LineColor = Color.White
            Chart1.ChartAreas("Default").AxisX.MajorGrid.LineColor = Color.Gray
            Chart1.ChartAreas("Default").AxisX2.LineColor = Color.White
            Chart1.ChartAreas("Default").AxisX2.MajorGrid.LineColor = Color.Gray

            Chart1.ChartAreas("Default").AxisY.LineColor = Color.White
            Chart1.ChartAreas("Default").AxisY.MajorGrid.LineColor = Color.Gray
            Chart1.ChartAreas("Default").AxisY2.LineColor = Color.White
            Chart1.ChartAreas("Default").AxisY2.MajorGrid.LineColor = Color.Gray

            Chart1.ChartAreas("Default").BorderColor = Color.Gray
            Chart1.Legends("Default").ForeColor = Color.Black
        ElseIf rDropDownChartBackGroundColor.SelectedText = INSPECTORMainResx.str_GraphBackColorWhite Then
            ModuleSettings.SettingFile.SaveSetting(GsSectionChart, GsSettingChartBackColor) = "White"
            Chart1.ChartAreas("Default").BackColor = Color.White
            Chart1.ChartAreas("Default").AxisX.LineColor = Color.LightGray
            Chart1.ChartAreas("Default").AxisX.MajorGrid.LineColor = Color.LightGray
            Chart1.ChartAreas("Default").AxisX2.LineColor = Color.LightGray
            Chart1.ChartAreas("Default").AxisX2.MajorGrid.LineColor = Color.LightGray

            Chart1.ChartAreas("Default").AxisY.LineColor = Color.LightGray
            Chart1.ChartAreas("Default").AxisY.MajorGrid.LineColor = Color.LightGray
            Chart1.ChartAreas("Default").AxisY2.LineColor = Color.LightGray
            Chart1.ChartAreas("Default").AxisY2.MajorGrid.LineColor = Color.LightGray

            Chart1.ChartAreas("Default").BorderColor = Color.LightGray
            Chart1.Legends("Default").ForeColor = Color.Black
        End If
        SaveChartTemplate(chartSettingsFile)
    End Sub
#End Region
    ''' <summary>
    ''' Apply the selected pallette to the legend
    ''' </summary>
    ''' <param name="LineChart"></param>
    ''' <remarks></remarks>
    Private Sub ApplyColoursAcrossLegend(ByRef LineChart As Chart)
        'Apply the colors also to the legend
        Try
            For Each thisSeries As Series In LineChart.Series
                LineChart.ApplyPaletteColors()
                For Each thislegendItem As LegendItem In Chart1.Legends("Default").CustomItems
                    If thislegendItem.Name = thisSeries.Name Then
                        thislegendItem.Color = thisSeries.Color
                        Exit For
                    End If
                Next
            Next
        Catch ex As Exception
            'ErrorHandler(ex)
        End Try

        SaveChartTemplate(chartSettingsFile)
    End Sub

    ''' <summary>
    ''' Saving the chart tamplate to file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SaveChartTemplate(ByVal fileName As String)
        Chart1.Serializer.Content = SerializationContents.Default
        ' Chart1.Serializer.IsTemplateMode = True
        Chart1.Serializer.Save(fileName)
    End Sub
    ''' <summary>
    ''' Loading the chart template
    ''' The template will contains the settings of the chart
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <remarks></remarks>
    Public Sub LoadChartTemplate(ByVal fileName As String)
        Chart1.Serializer.IsResetWhenLoading = False
        Chart1.Serializer.Load(fileName)
        Chart1.Dock = DockStyle.Fill
    End Sub



    ''' <summary>
    ''' Apply the dock item to the legend.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownDock_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownDock.SelectedIndexChanged
        Dim ldock As Docking
        Select Case Me.rDropDownDock.SelectedItem.ToString()
            Case INSPECTORMainResx.str_legend_Docking_Bottom
                ldock = Docking.Bottom
            Case INSPECTORMainResx.str_legend_Docking_Left
                ldock = Docking.Left
            Case INSPECTORMainResx.str_legend_Docking_Right
                ldock = Docking.Right
            Case INSPECTORMainResx.str_legend_Docking_Top
                ldock = Docking.Top
        End Select
        Chart1.Legends("Default").Docking = ldock

        'Chart1.Legends("Default").Docking = DirectCast(Docking.Parse(GetType(Docking), Me.rDropDownDock.SelectedItem.ToString()), Docking)
    End Sub
    ''' <summary>
    ''' Apply the alignment item to the legend.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownAlignment_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownAlignment.SelectedIndexChanged
        Dim lalignment As StringAlignment
        Select Case Me.rDropDownAlignment.SelectedItem.ToString()
            Case INSPECTORMainResx.str_legend_Alignment_Center
                lalignment = StringAlignment.Center
            Case INSPECTORMainResx.str_legend_Alignment_Far
                lalignment = StringAlignment.Far
            Case INSPECTORMainResx.str_legend_Alignment_Near
                lalignment = StringAlignment.Near
        End Select
        Chart1.Legends("Default").Alignment = lalignment

        'Chart1.Legends("Default").Alignment = DirectCast(StringAlignment.Parse(GetType(StringAlignment), Me.rDropDownAlignment.SelectedItem.ToString()), StringAlignment)
    End Sub
    ''' <summary>
    ''' Apply the style item to the legend.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownStyle_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownStyle.SelectedIndexChanged
        Dim lStyle As LegendStyle
        Select Case Me.rDropDownStyle.SelectedItem.ToString()
            Case INSPECTORMainResx.str_legend_Style_Column
                lStyle = LegendStyle.Column
            Case INSPECTORMainResx.str_legend_Style_Row
                lStyle = LegendStyle.Row
            Case INSPECTORMainResx.str_legend_Style_Table
                lStyle = LegendStyle.Table
        End Select
        Chart1.Legends("Default").LegendStyle = lStyle

        'Chart1.Legends("Default").LegendStyle = DirectCast(LegendStyle.Parse(GetType(LegendStyle), Me.rDropDownStyle.SelectedItem.ToString()), LegendStyle)
    End Sub
    ''' <summary>
    ''' Apply the dock in side item to the legend.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownInsideChart_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownInsideChart.SelectedIndexChanged
        Dim ldockInside As String = ""
        Select Case Me.rDropDownInsideChart.SelectedItem.ToString()
            Case INSPECTORMainResx.str_legend_DockinChart_True
                ldockInside = "Default"
            Case INSPECTORMainResx.str_legend_DockinChart_False
                ldockInside = ""
        End Select
        Chart1.Legends("Default").IsDockedInsideChartArea = True
        Chart1.Legends("Default").InsideChartArea = ldockInside
        'Chart1.Update()
    End Sub

    'MOD05
    ''' <summary>
    ''' Set the line size
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rDropDownGraphLineSize_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rDropDownChartLineSize.SelectedIndexChanged
        Chart1.Series("Series1").BorderWidth = rDropDownChartLineSize.Text
        Chart1.Series("Series2").BorderWidth = rDropDownChartLineSize.Text
        Chart1.Series("Series3").BorderWidth = rDropDownChartLineSize.Text
        ModuleSettings.SettingFile.SaveSetting(GsSectionChart, GsSettingChartSeriesLineSize) = rDropDownChartLineSize.Text
    End Sub


    Private Sub rdDropDownPalletteColor_SelectedIndexChanged(sender As System.Object, e As Telerik.WinControls.UI.Data.PositionChangedEventArgs) Handles rdDropDownPalletteColor.SelectedIndexChanged

    End Sub
End Class



