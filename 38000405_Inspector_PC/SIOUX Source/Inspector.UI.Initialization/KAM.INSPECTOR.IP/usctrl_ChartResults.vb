Imports System.Threading
Imports System
Imports System.IO
Imports System.Math
Imports System.Drawing
Imports System.Windows.Forms.DataVisualization.Charting


Imports Inspector.Model
Imports KAM.INSPECTOR.Infra
Imports KAM.INSPECTOR.IP.My.Resources
Imports KAM.INSPECTOR.Infra.ClsCastingHelpers

Public Class usctrl_ChartResults

#Region "Class members"
    Private chartAxisMinimum As Single = 10
    Private chartAxisMaximum As Single = 1

    'MOD 01
    Private showLeakageIntervals As Integer = 10
    Private textLeakageWait As String

    Private _scriptcommand5x As New InspectionProcedure.ScriptCommand5X
    Private _lastresult As InspectionReportingResults.ReportResult

    Private pointNumber As Integer

    Private lmeasurementStartTime As Date
    Private ldate As Date

    Private lowHighCorrection As Single = 1

    Private _manometermUnit As String
    'MOD 58 Private _measurementFrequency As Integer = 10
    Private _measurementResultsCompleted As Boolean = False
    Private _measurementCompleted As Boolean = False
    'MOD 82
    Private _measurementHasValue As Boolean = False
    Private _displayLeakageResult As Boolean = False
    'MOD 13
    Private minValueY1 As Double = Double.PositiveInfinity
    Private maxValueY1 As Double = Double.NegativeInfinity
    Private minValueY2 As Double = Double.PositiveInfinity
    Private maxValueY2 As Double = Double.NegativeInfinity

    'MOD 27
    Private _lastReceivedMeasurement As InspectionMeasurement.ScriptCommand5XMeasurement
    Private boundariesUnit As String = "-"
    Private leakageUnit As UnitOfMeasurement = UnitOfMeasurement.UNSET
    Private leakageType As Leakage

    'MOD 82
    Private ShowAnnotation As Boolean = False 'Show only the maximum on minimum annotation in case of scriptcommand 53, 54, 56, 57 
    Private previousIoStatus As Integer = 7

    'MOD 59
    '-----------------------------------------------
    Private annotationSet As Boolean = False
    Private annotationChartSeries As String = ""
    Private annotionPointMaxIsCurrent As Integer
    Private safetyValueTripped As Boolean = False
    '-----------------------------------------------

    Private WithEvents myTimer As New Timers.Timer
    Private ShowText As Boolean = True
    Private flashTextrdTextMeasuredValue As String
    Private flashTextrdTextCalculatedValue As String

#End Region

    Delegate Sub SetLabelText_Delegate(ByVal [controllabel] As Telerik.WinControls.UI.RadLabel, ByVal [text] As String)
    Delegate Sub SetChart_Delegate(ByVal [controlchart] As System.Windows.Forms.DataVisualization.Charting.Chart, ByVal ChartSeries As String, ByVal [value] As Single, ByVal [timeStamp] As DateTime)
    Delegate Sub SetChartRangeX_Delegate(ByVal [controlchart] As System.Windows.Forms.DataVisualization.Charting.Chart, ByVal [timeStamp] As DateTime)
    Delegate Sub SetChartRangeY_Delegate(ByVal [controlchart] As System.Windows.Forms.DataVisualization.Charting.Chart, ByVal [minValueY1] As Double, ByVal [maxValueY1] As Double, ByVal [minValueY2] As Double, ByVal [maxValueY2] As Double)

#Region "Load form"
    ''' <summary>
    ''' Loading the settings and set items
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadSettings()

        'MOD 13
        minValueY1 = Double.PositiveInfinity
        maxValueY1 = Double.NegativeInfinity
        minValueY2 = Double.PositiveInfinity
        maxValueY2 = Double.NegativeInfinity

        SetRadlabelText(rlblMeasuredValue, My.Resources.InspectionProcedureResx.str_DefaultNoValue)
        SetRadlabelText(rlblCalculatedValue, My.Resources.InspectionProcedureResx.str_DefaultNoValue)

        'MOD 27
        rdTextResultLeakageDm3h.Visible = False
        rlblResultLeakageDm3h.Visible = False
        rlblResultLeakageDm3hUnit.Visible = False

        'MOD 28
        flashTextrdTextMeasuredValue = My.Resources.InspectionProcedureResx.str_ValueCurrent

        rdTextMeasuredValue.Text = My.Resources.InspectionProcedureResx.str_ValueCurrent
        rdTextMaximumBound.Text = My.Resources.InspectionProcedureResx.str_ValuebndMax
        rdTextMinimumBound.Text = My.Resources.InspectionProcedureResx.str_ValuebndMin
        rdTextPreviousValue.Text = My.Resources.InspectionProcedureResx.str_ValuePrevious


        SetRadlabelText(rlblMeasuredValueUnit, _manometermUnit)

        'Set the axis title for Y
        msChart.ChartAreas("Default").AxisY.Title = _manometermUnit
        msChart.ChartAreas("Default").AxisY2.Title = _manometermUnit

        'Setting value and unit labels for boundaries
        boundariesUnit = "-"

        'MOD 27
        leakageUnit = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate)  'MOD 58 UnitOfMeasurement.ItemMbarMin
        leakageType = _scriptcommand5x.Leakage

        If _scriptcommand5x.StationStepObject.Boundaries IsNot Nothing Then
            Debug.Print(_scriptcommand5x.StationStepObject.Boundaries.UOV.ToString)
            Debug.Print(CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate))
            Select Case _scriptcommand5x.StationStepObject.Boundaries.UOV
                Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitHighPressure) 'MOD 58 UnitOfMeasurement.ItemBar
                    'If the leakage type in the scriptcommand is not leakage.dash then set unit to dm3/h
                    If leakageType <> Leakage.Dash And _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand51 Then leakageUnit = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage) 'MOD 58 leakageUnit = UnitOfMeasurement.ItemDm3h
                    boundariesUnit = ModPlexorUnits.UnitHighPressure  'MOD 58 InspectionProcedureResx.str_Unit_bar
                Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitLowPressure) 'MOD 58 UnitOfMeasurement.ItemMbar
                    'If the leakage type in the scriptcommand is not leakage.dash then set unit to dm3/h
                    If leakageType <> Leakage.Dash And _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand51 Then leakageUnit = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage) 'MOD 58 leakageUnit = UnitOfMeasurement.ItemDm3h
                    boundariesUnit = ModPlexorUnits.UnitLowPressure 'MOD 58 InspectionProcedureResx.str_Unit_mbar
                Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate) 'MOD 58 UnitOfMeasurement.ItemMbarMin
                    leakageUnit = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate)  'MOD 58 UnitOfMeasurement.ItemMbarMin
                    boundariesUnit = ModPlexorUnits.UnitChangeRate 'MOD 58 InspectionProcedureResx.str_Unit_mbar_min
                Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage) 'MOD 58 UnitOfMeasurement.ItemDm3h
                    'MOD 27
                    'If the leakage type in the scriptcommand is not leakage.dash then set unit to dm3/h
                    If leakageType <> Leakage.Dash And _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand51 Then leakageUnit = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage) 'MOD 58 leakageUnit = UnitOfMeasurement.ItemDm3h
                    boundariesUnit = ModPlexorUnits.UnitQvsLeakage  'MOD 58 InspectionProcedureResx.str_Unit_dm3h
                Case Else
                    'to do
            End Select

            lowHighCorrection = 1
            'MOD 58 If _manometermUnit.ToUpper = "bar".ToUpper And _scriptcommand5x.StationStepObject.Boundaries.UOV = UnitOfMeasurement.ItemMbar Then
            If _manometermUnit.ToUpper = ModPlexorUnits.UnitHighPressure.ToUpper And _scriptcommand5x.StationStepObject.Boundaries.UOV = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitLowPressure) Then
                lowHighCorrection = CastToDoubleOrNan(ModPlexorUnits.FactorLowHighPressure) 'MOD 58 0.001
                boundariesUnit = ModPlexorUnits.UnitHighPressure 'InspectionProcedureResx.str_Unit_bar
            End If
            If _manometermUnit.ToUpper = ModPlexorUnits.UnitLowPressure.ToUpper And _scriptcommand5x.StationStepObject.Boundaries.UOV = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitHighPressure) Then
                lowHighCorrection = 1 / CastToDoubleOrNan(ModPlexorUnits.FactorLowHighPressure) 'MOD 58 1000
                boundariesUnit = ModPlexorUnits.UnitLowPressure 'InspectionProcedureResx.str_Unit_mbar
            End If


            'Set the correct value in the GUI
            If Double.IsNaN(_scriptcommand5x.StationStepObject.Boundaries.ValueMin) Then
                SetRadlabelText(rlblMinimumBoundUnit, "")
                SetRadlabelText(rlblMinimumBound, "-")
            Else
                SetRadlabelText(rlblMinimumBoundUnit, boundariesUnit)
                SetRadlabelText(rlblMinimumBound, Round(_scriptcommand5x.StationStepObject.Boundaries.ValueMin * lowHighCorrection, 4))
            End If

            If Double.IsNaN(_scriptcommand5x.StationStepObject.Boundaries.ValueMax) Then
                SetRadlabelText(rlblMaximumBoundUnit, "")
                SetRadlabelText(rlblMaximumBound, "-")
            Else
                SetRadlabelText(rlblMaximumBoundUnit, boundariesUnit)
                SetRadlabelText(rlblMaximumBound, Round(_scriptcommand5x.StationStepObject.Boundaries.ValueMax * lowHighCorrection, 4))
            End If

        Else
            'MOD 25
            SetRadlabelText(rlblMinimumBoundUnit, "")
            SetRadlabelText(rlblMinimumBound, "-")
            SetRadlabelText(rlblMaximumBoundUnit, "")
            SetRadlabelText(rlblMaximumBound, "-")
        End If

        'MOD 82
        picStatusSensorIO.Visible = False
        rdTextSensorIO.Visible = False

        Select Case _scriptcommand5x.ScriptCommand
            Case ScriptCommand5XType.ScriptCommand51
                rdTextCalculatedValue.Text = InspectionProcedureResx.str_ValueLeakage
                'MOD 28
                flashTextrdTextCalculatedValue = rdTextCalculatedValue.Text
                Dim unitText As String
                'MOD 27 Determine range "mbar/min" or "dm3/h"
                'MOD 58 If leakageUnit = UnitOfMeasurement.ItemDm3h Then unitText = InspectionProcedureResx.str_Unit_dm3h Else unitText = InspectionProcedureResx.str_Unit_mbar_min
                If leakageUnit = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage) Then unitText = ModPlexorUnits.UnitQvsLeakage Else unitText = ModPlexorUnits.UnitChangeRate

                SetRadlabelText(rlblCalculatedValueUnit, unitText)
                msChart.ChartAreas("Default").AxisY2.Title = unitText
            Case ScriptCommand5XType.ScriptCommand52
                rdTextCalculatedValue.Text = InspectionProcedureResx.str_ValueAverage
                'MOD 28
                flashTextrdTextCalculatedValue = rdTextCalculatedValue.Text
                SetRadlabelText(rlblCalculatedValueUnit, _manometermUnit)
            Case ScriptCommand5XType.ScriptCommand53, ScriptCommand5XType.ScriptCommand56
                rdTextCalculatedValue.Text = InspectionProcedureResx.str_ValueMaximum
                'MOD 28
                flashTextrdTextCalculatedValue = rdTextCalculatedValue.Text
                SetRadlabelText(rlblCalculatedValueUnit, _manometermUnit)
                'MOD 82
                picStatusSensorIO.Visible = True
                rdTextSensorIO.Visible = True
            Case ScriptCommand5XType.ScriptCommand54, ScriptCommand5XType.ScriptCommand57
                rdTextCalculatedValue.Text = InspectionProcedureResx.str_ValueMimimum
                'MOD 28
                flashTextrdTextCalculatedValue = rdTextCalculatedValue.Text
                SetRadlabelText(rlblCalculatedValueUnit, _manometermUnit)
                'MOD 82
                picStatusSensorIO.Visible = True
                rdTextSensorIO.Visible = True
            Case ScriptCommand5XType.ScriptCommand55
                rdTextCalculatedValue.Text = InspectionProcedureResx.str_ValueActual
                'MOD 28
                flashTextrdTextCalculatedValue = rdTextCalculatedValue.Text
                SetRadlabelText(rlblCalculatedValueUnit, _manometermUnit)

        End Select

        'Setting the last result
        If _lastresult IsNot Nothing Then
            'Setting value and unit labels for result
            Dim resultUnit As String = "-"
            If _lastresult.MeasureValue IsNot Nothing Then
                Select Case _lastresult.MeasureValue.UOM
                    Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitHighPressure) 'Case UnitOfMeasurement.ItemBar
                        resultUnit = ModPlexorUnits.UnitHighPressure.ToString
                    Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitLowPressure) 'Case UnitOfMeasurement.ItemMbar
                        resultUnit = ModPlexorUnits.UnitLowPressure.ToString
                    Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate) ' UnitOfMeasurement.ItemMbarMin
                        resultUnit = ModPlexorUnits.UnitChangeRate.ToString
                    Case CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage) 'Case UnitOfMeasurement.ItemDm3h
                        resultUnit = ModPlexorUnits.UnitQvsLeakage.ToString
                End Select

                'MOD 36
                Dim lowHighResultCorrection As Double = 1
                'MOD 58If _manometermUnit.ToUpper = "bar".ToUpper And _lastresult.MeasureValue.UOM = UnitOfMeasurement.ItemMbar Then
                If _manometermUnit.ToUpper = ModPlexorUnits.UnitHighPressure.ToUpper And _lastresult.MeasureValue.UOM = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitLowPressure) Then
                    'mbarBarResultCorrection = 0.001
                    'resultUnit = InspectionProcedureResx.str_Unit_bar
                    lowHighResultCorrection = ModPlexorUnits.FactorLowHighPressure
                    resultUnit = ModPlexorUnits.UnitHighPressure
                End If
                'MOD 58 If _manometermUnit.ToUpper = "mbar".ToUpper And _lastresult.MeasureValue.UOM = UnitOfMeasurement.ItemBar Then
                If _manometermUnit.ToUpper = ModPlexorUnits.UnitLowPressure.ToUpper And _lastresult.MeasureValue.UOM = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitHighPressure) Then
                    'lmbarBarResultCorrection = 1000
                    'resultUnit = InspectionProcedureResx.str_Unit_mbar
                    lowHighResultCorrection = 1 / ModPlexorUnits.FactorLowHighPressure
                    resultUnit = ModPlexorUnits.UnitLowPressure
                End If

                SetRadlabelText(rlblPreviousValueUnit, resultUnit)
                SetRadlabelText(rlblPreviousValue, Round(_lastresult.MeasureValue.Value * lowHighResultCorrection, 4))

                'MOD 36 SetRadlabelText(rlblPreviousValue, _lastresult.MeasureValue.Value)
                'MOD 36 SetRadlabelText(rlblPreviousValueUnit, resultUnit)
                'TO DO ; Only the time is present do not display SetRadlabelToolTipText(rlblPreviousValue, _lastresult.Time)
            Else
                'MOD 57
                SetRadlabelText(rlblPreviousValue, "-")
                SetRadlabelText(rlblPreviousValueUnit, "")
            End If
        Else
            SetRadlabelText(rlblPreviousValue, "-")
            SetRadlabelText(rlblPreviousValueUnit, "")
        End If

        LoadChart()
        'Set the current time/ date stemp; This is used for the chart
        ldate = Now
        lmeasurementStartTime = ldate
    End Sub
    ''' <summary>
    ''' Reload the chart
    ''' Clear labels and lines; Set new start time (current time)
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ReloadChart()
        SetRadlabelText(rlblMeasuredValue, My.Resources.InspectionProcedureResx.str_DefaultNoValue)
        SetRadlabelText(rlblCalculatedValue, My.Resources.InspectionProcedureResx.str_DefaultNoValue)
        msChart.Series(0).Points.Clear()
        msChart.Series(1).Points.Clear()
        msChart.Series(2).Points.Clear()
        msChart.Series(3).Points.Clear()
        msChart.Update()
        'Set the current time/ date stemp; This is used for the chart
        ldate = Now
        lmeasurementStartTime = ldate

        'MOD 41
        ' Predefine the viewing area of the chart
        Dim minValue As Date
        Dim maxValue As Date
        minValue = ldate.ToString
        maxValue = minValue.AddSeconds(5)
        msChart.ChartAreas(0).AxisX.Minimum = minValue.ToOADate()
        msChart.ChartAreas(0).AxisX.Maximum = maxValue.ToOADate()
        msChart.ChartAreas(0).AxisX.Interval = 1
        msChart.ChartAreas(0).AxisX.IntervalType = DateTimeIntervalType.Seconds

    End Sub
    'MOD 26
    ''' <summary>
    ''' Initialize the start of the measurement
    ''' Setting the correct times 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub IntializeStartMeasurement()
        'Set the current time/ date stemp; This is used for the chart
        ldate = Now
        lmeasurementStartTime = ldate

        myTimer.Interval = 500
        myTimer.Start()

    End Sub

    ''' <summary>
    ''' Finilize
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        myTimer.Stop()
    End Sub

#End Region

#Region "Chart handling"
    ''' <summary>
    ''' Loading the chart settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadChart()

        'MOD 62         LoadChartTemplate(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WS-gas\CONNEXION V5.x\INSPECTORPC", "ChartSettings.xml"))
        LoadChartTemplate(ChartSettingsFile)

        StartNewChart()

        'Setting chart minimum and maximum X as value
        Dim tmpSetting As String = ""

        tmpSetting = ModuleSettings.SettingFile.GetSetting(GsSectionChart, GsSettingChartAxisXMinimum)
        If tmpSetting = ModuleSettings.SettingFile.NoValue Then chartAxisMinimum = 10000 / 1000 Else chartAxisMinimum = tmpSetting / 1000
        tmpSetting = ModuleSettings.SettingFile.GetSetting(GsSectionChart, GsSettingChartAxisXMaximum)
        If tmpSetting = ModuleSettings.SettingFile.NoValue Then chartAxisMaximum = 500 / 1000 Else chartAxisMaximum = tmpSetting / 1000
    End Sub
    ''' <summary>
    ''' Loading the chart template
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <remarks></remarks>
    Public Sub LoadChartTemplate(ByVal fileName As String)
        msChart.Serializer.IsResetWhenLoading = False
        msChart.Serializer.Load(fileName)
        msChart.Dock = Windows.Forms.DockStyle.Fill
    End Sub

    '/Creating new chart, with series and set the x-as values
    ''' <summary>
    ''' Creating new chart, with series and set the x-as values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartNewChart()
        Dim minValue As Date
        Dim maxValue As Date

        ' Predefine the viewing area of the chart
        minValue = DateTime.Now.ToString
        maxValue = minValue.AddSeconds(5)
        msChart.ChartAreas(0).AxisX.Minimum = minValue.ToOADate()
        msChart.ChartAreas(0).AxisX.Maximum = maxValue.ToOADate()
        msChart.ChartAreas(0).AxisX.Interval = 1
        msChart.ChartAreas(0).AxisX.IntervalType = DateTimeIntervalType.Seconds


        ChartSettings()
        ' Reset number of series in the chart.
        msChart.Series.Clear()

        'MOD 02
        Dim axisTypeBoundaries As AxisType

        If Not IsNothing(_scriptcommand5x.StationStepObject.Boundaries) Then
            'MOD 58 If _scriptcommand5x.StationStepObject.Boundaries.UOV = UnitOfMeasurement.ItemMbarMin Or _scriptcommand5x.StationStepObject.Boundaries.UOV = UnitOfMeasurement.ItemDm3h Then axisTypeBoundaries = AxisType.Secondary Else axisTypeBoundaries = AxisType.Primary
            If _scriptcommand5x.StationStepObject.Boundaries.UOV = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate) Or _scriptcommand5x.StationStepObject.Boundaries.UOV = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitQvsLeakage) Then axisTypeBoundaries = AxisType.Secondary Else axisTypeBoundaries = AxisType.Primary
        End If
        CreateNewChartSeries(My.Resources.InspectionProcedureResx.str_ValuebndMax, SeriesChartType.FastLine, Color.Red, axisTypeBoundaries)
        CreateNewChartSeries(My.Resources.InspectionProcedureResx.str_ValuebndMin, SeriesChartType.FastLine, Color.RosyBrown, axisTypeBoundaries)

        CreateNewChartSeries(My.Resources.InspectionProcedureResx.str_ValueCurrent, SeriesChartType.Line, Color.BlueViolet, AxisType.Primary)


        ' Dim newseries As New Series("0")
        'newseries.ChartType = SeriesChartType.Line
        'newseries.BorderWidth = 2
        'newseries.Color = Color.Green
        'newseries.XAxisType = AxisType.Primary
        'msChart.Series.Add(newseries)
        'CreateNewChartSeries("0", SeriesChartType.FastLine, Color.Green, AxisType.Primary)
        ' CreateNewChartSeries("Range", SeriesChartType.Range, Color.RosyBrown)

        Select Case _scriptcommand5x.ScriptCommand
            Case ScriptCommand5XType.ScriptCommand51
                CreateNewChartSeries(My.Resources.InspectionProcedureResx.str_ValueLeakage, SeriesChartType.Line, Color.ForestGreen, AxisType.Secondary)
            Case ScriptCommand5XType.ScriptCommand52
                CreateNewChartSeries(My.Resources.InspectionProcedureResx.str_ValueAverage, SeriesChartType.Line, Color.ForestGreen, AxisType.Primary)
            Case ScriptCommand5XType.ScriptCommand53, ScriptCommand5XType.ScriptCommand56
                CreateNewChartSeries(My.Resources.InspectionProcedureResx.str_ValueMaximum, SeriesChartType.Line, Color.ForestGreen, AxisType.Primary)
            Case ScriptCommand5XType.ScriptCommand54, ScriptCommand5XType.ScriptCommand57
                CreateNewChartSeries(My.Resources.InspectionProcedureResx.str_ValueMimimum, SeriesChartType.Line, Color.ForestGreen, AxisType.Primary)
        End Select

    End Sub

    ''' <summary>
    ''' Settings of the chart
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ChartSettings()
        'Initialize chart
        'Axis appearance
        msChart.ChartAreas("Default").AxisY.Minimum = [Double].NaN
        msChart.ChartAreas("Default").AxisY.Maximum = [Double].NaN
        msChart.ChartAreas("Default").AxisY.IsStartedFromZero = False

        msChart.ChartAreas("Default").AxisX.ScaleView.Zoomable = True
        msChart.ChartAreas("Default").AxisX.LabelStyle.Format = "T"
        msChart.ChartAreas("Default").AxisX.ScrollBar.Size = 10
        msChart.ChartAreas("Default").AxisX.ScrollBar.IsPositionedInside = True
        msChart.ChartAreas("Default").AxisX.ScrollBar.Enabled = True
        msChart.ChartAreas("Default").AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.ResetZoom

        msChart.ChartAreas("Default").CursorX.IsUserEnabled = True
        msChart.ChartAreas("Default").CursorX.IsUserSelectionEnabled = True

        msChart.ChartAreas("Default").AxisY2.Minimum = [Double].NaN
        msChart.ChartAreas("Default").AxisY2.Maximum = [Double].NaN
        msChart.ChartAreas("Default").AxisY2.IsStartedFromZero = False
        msChart.ChartAreas("Default").AxisY2.LineDashStyle = ChartDashStyle.Solid
        msChart.ChartAreas("Default").AxisY2.Enabled = AxisEnabled.True

        'Chart1.ChartAreas("Default").AlignmentStyle = AreaAlignmentStyles.None

        'Axis Titles
        msChart.ChartAreas("Default").AxisX.Title = InspectionProcedureResx.str_yAxisText
        msChart.ChartAreas("Default").AxisX.TitleFont = New Font(rdTextMeasuredValue.Font.ToString, 10, FontStyle.Bold)
        msChart.ChartAreas("Default").AxisY.TitleFont = New Font(rdTextMeasuredValue.Font.ToString, 10, FontStyle.Bold)
        msChart.ChartAreas("Default").AxisY2.TitleFont = New Font(rdTextMeasuredValue.Font.ToString, 10, FontStyle.Bold)



        'Legend appearance
        'msChart.Legends("Default").IsDockedInsideChartArea = False
        'Chart1.Legends("Default").Docking = Docking.Bottom
        'Chart1.Legends("Default").LegendStyle = LegendStyle.Row
        'Chart1.Legends("Default").Alignment = StringAlignment.Far
        'Chart1.Legends("Default").BorderDashStyle = ChartDashStyle.Solid
        'Chart1.Legends("Default").BorderColor = Color.Gray
        msChart.Legends("Default").ForeColor = Color.Black

        rdPanelChart.PanelElement.PanelBorder.Visibility = Telerik.WinControls.ElementVisibility.Hidden
        'Chart1.Legends("Default").BorderWidth = 2

        msChart.Update()
    End Sub
    ''' <summary>
    ''' Create a new chart series
    ''' </summary>
    ''' <param name="seriesName"></param>
    ''' <param name="lineStyls"></param>
    ''' <remarks></remarks>
    Private Sub CreateNewChartSeries(ByVal seriesName As String, ByVal lineStyls As SeriesChartType, ByVal color As Color, ByVal yAxisType As AxisType)
        ' create a line chart series
 
        Dim newSeries As New Series(seriesName)
        newSeries.ChartType = lineStyls

        newSeries.XValueType = ChartValueType.DateTime
        msChart.Series.Add(newSeries)

        msChart.Series(seriesName).IsVisibleInLegend = False
        'MOD 05
        msChart.Series(seriesName).BorderWidth = ModuleSettings.SettingFile.GetSetting(GsSectionChart, GsSettingChartSeriesLineSize)

        msChart.Series(seriesName).LegendText = seriesName
        msChart.Series(seriesName).Tag = seriesName
        msChart.Series(seriesName).YAxisType = yAxisType

        'Add the series to the legend
        msChart.Series(seriesName).Color = color
        msChart.Legends("Default").CustomItems.Add(msChart.Series(seriesName).Color, seriesName)
        msChart.Legends("Default").CustomItems(msChart.Legends("Default").CustomItems.Count - 1).Tag = msChart.Series(seriesName)
    End Sub
    ''' <summary>
    ''' Set the x value range of the chart
    ''' </summary>
    ''' <param name="controlChart">Chart control</param>
    ''' <param name="timeStamp">Current time stamp</param>
    ''' <remarks></remarks>
    Private Sub SetChartRangeXaxilValue(ByVal [controlChart] As System.Windows.Forms.DataVisualization.Charting.Chart, ByVal [timeStamp] As DateTime)
        If [controlChart].InvokeRequired Then
            Dim SetChartRangeX_Delegate As New SetChartRangeX_Delegate(AddressOf SetChartRangeXaxilValue)
            Me.Invoke(SetChartRangeX_Delegate, New Object() {[controlChart], [timeStamp]})
        Else
            Debug.Print("Times | '{0}' | '{1}' | '{2}'", [timeStamp].AddSeconds(-chartAxisMinimum).ToString("HH:mm:ss:fff"), [timeStamp].AddSeconds(chartAxisMaximum).ToString("HH:mm:ss:fff"), chartAxisMaximum)

            [controlChart].ChartAreas(0).AxisX.Minimum = [timeStamp].AddSeconds(-chartAxisMinimum).ToOADate()
            [controlChart].ChartAreas(0).AxisX.Maximum = [timeStamp].AddSeconds(chartAxisMaximum).ToOADate()

            'Force to redraw the chart
            [controlChart].Invalidate()
        End If
    End Sub

    ''' <summary>
    ''' Set the y value range of the chart
    ''' </summary>
    ''' <param name="controlChart">Chart control</param>
    ''' <param name="minValueY1"></param>
    ''' <param name="maxValueY1"></param>
    ''' <param name="minValueY2"></param>
    ''' <param name="maxValueY2"></param>
    ''' <remarks></remarks>
    Private Sub SetChartRangeYaxilValue(ByVal [controlChart] As System.Windows.Forms.DataVisualization.Charting.Chart, ByVal [minValueY1] As Double, ByVal [maxValueY1] As Double, ByVal [minValueY2] As Double, ByVal [maxValueY2] As Double)
        If [controlChart].InvokeRequired Then
            Dim SetChartRangeY_Delegate As New SetChartRangeY_Delegate(AddressOf SetChartRangeYaxilValue)
            Me.Invoke(SetChartRangeY_Delegate, New Object() {[controlChart], [minValueY1], [maxValueY1]})
        Else
            GetRangeYChart([minValueY1], [maxValueY1])
     
            [controlChart].ChartAreas(0).AxisY.Minimum = [minValueY1]
            [controlChart].ChartAreas(0).AxisY.Maximum = [maxValueY1]

            GetRangeYChart([minValueY2], [maxValueY2])
            [controlChart].ChartAreas(0).AxisY2.Minimum = [minValueY2]
            [controlChart].ChartAreas(0).AxisY2.Maximum = [maxValueY2]

            ' Force to redraw the chart
            [controlChart].Invalidate()
        End If
    End Sub
    ''' <summary>
    ''' Setting the the minimum and maximum range for a chart y axis
    ''' </summary>
    ''' <param name="minValue">Minimum value (Also returned)</param>
    ''' <param name="maxValue">Maximum value (Also returned)</param>
    ''' <remarks></remarks>
    Private Sub GetRangeYChart(ByRef minValue As Double, ByRef maxValue As Double)
        ' ''MOD 11
        Dim exponent As Integer
        Dim spanRange As Double
        Dim minValueFloor As Double
        Dim maxValueCeil As Double

        'In case max or min value is null or NAN. 
        If maxValue = 0 Or Double.IsNaN(maxValue) Or Double.IsNegativeInfinity(maxValue) Then maxValue = 0.1
        If minValue = 0 Or Double.IsNaN(minValue) Or Double.IsPositiveInfinity(minValue) Then minValue = -0.1

        'MOD 13
        'Determine interval/ span of values
        spanRange = (maxValue - minValue)

        If Double.IsInfinity(spanRange) Then spanRange = 1
        'Determine log10 of span
        If spanRange = 0 Then exponent = 0 Else exponent = Math.Floor(Math.Log10(Math.Abs(spanRange)))

        minValueFloor = Math.Floor(minValue)
        maxValueCeil = Math.Ceiling(maxValue)

        'Exponent from the span will determine the max- and minimum value
        Select Case exponent
            Case Is <= -1 '(<0)
                maxValueCeil = Math.Ceiling((maxValue * 10) + 0.3) / 10
                minValueFloor = Math.Floor((minValue * 10) - 0.3) / 10
            Case 0 '(0-9)
                maxValueCeil = Math.Ceiling(maxValue) + 0.2
                minValueFloor = Math.Floor(minValue) - 0.2
            Case 1 '(10- 99)
                maxValueCeil = Math.Ceiling(maxValue) + 5
                minValueFloor = Math.Floor(minValue) - 5
            Case 2 '(100 - 999)
                maxValueCeil = Math.Ceiling((maxValue) + 50)
                minValueFloor = Math.Floor((minValue) - 50)
            Case 3 '(1000 - 9999)
                maxValueCeil = Math.Ceiling((maxValue) + 100)
                minValueFloor = Math.Floor((minValue) - 100)
            Case 4 '(10000 - 99999)
                maxValueCeil = Math.Ceiling((maxValue) + 1000)
                minValueFloor = Math.Floor((minValue) - 1000)
            Case 5 '(100000 - 999999)
                maxValueCeil = Math.Ceiling((maxValue) + 10000)
                minValueFloor = Math.Floor((minValue) - 10000)
            Case 6 '(1000000 - 9999999)
                maxValueCeil = Math.Ceiling((maxValue) + 100000)
                minValueFloor = Math.Floor((minValue) - 100000)
        End Select

        Debug.Print("exponent: '{0}' |spanRange: '{1}' |maxValueCeil: '{2}' |minValueFloor: '{3}' |maxValue: '{4}' |minValue: '{5}'", exponent, spanRange, maxValueCeil, minValueFloor, maxValue, minValue)

        minValue = minValueFloor
        maxValue = maxValueCeil
    End Sub

    ''' <summary>
    ''' Set the chart value to the series
    ''' </summary>
    ''' <param name="controlChart">The chart control</param>
    ''' <param name="ChartSeries">Chart series to which the value is applied to</param>
    ''' <param name="value">The y value</param>
    ''' <param name="timeStamp">timestamp for x value</param>
    ''' <remarks></remarks>
    Private Sub SetChartValue(ByVal [controlChart] As System.Windows.Forms.DataVisualization.Charting.Chart, ByVal [ChartSeries] As String, ByVal [value] As Single, ByVal [timeStamp] As DateTime)
        If [controlChart].InvokeRequired Then
            Dim setChartDelegate As New SetChart_Delegate(AddressOf SetChartValue)
            Me.Invoke(setChartDelegate, New Object() {[controlChart], [ChartSeries], [value], [timeStamp]})
        Else

            ' remove all points from the source series older than 1.5 minutes.
            'Dim removeBefore As Double = timeStamp.AddSeconds((CDbl(90) * -1)).ToOADate()
            ''remove oldest values to maintain a constant number of data points
            'While ptSeries.Points(0).XValue < removeBefore
            '    ptSeries.Points.RemoveAt(0)
            'End While

            [controlChart].Series([ChartSeries]).Points.AddXY([timeStamp], [value])

        End If
    End Sub

    Private Sub SetAnnotion(ByVal [controlChart] As System.Windows.Forms.DataVisualization.Charting.Chart, ByVal [chartSeries] As String, ByVal [chartPoint] As Integer, ByVal [annotationText] As String)
        If [controlChart].InvokeRequired Then
            Dim setChartDelegate As New SetChart_Delegate(AddressOf SetChartValue)
            Me.Invoke(setChartDelegate, New Object() {[controlChart], [chartSeries]})
        Else
            '// create a callout annotation
            Dim annotation1 As New CalloutAnnotation()
            '// setup visual attributes
            annotation1.AnchorDataPoint = [controlChart].Series([chartSeries]).Points([chartPoint])
            annotation1.Text = [annotationText]
            annotation1.BackColor = Color.Yellow
            annotation1.ForeColor = Color.Black

            '// prevent moving or selecting
            annotation1.AllowMoving = False
            annotation1.AllowAnchorMoving = False
            annotation1.AllowSelecting = False

            ' // add the annotation to the collection
            [controlChart].Annotations.Add(annotation1)
        End If
    End Sub
    ''' <summary>
    ''' Apply the chart series colors to the chart legend
    ''' </summary>
    ''' <param name="lineChart"></param>
    ''' <remarks></remarks>
    Private Sub ApplyColoursAcrossLegend(ByRef lineChart As Chart)
        '
        Try
            For Each thisSeries As Series In lineChart.Series
                lineChart.ApplyPaletteColors()
                For Each thislegendItem As LegendItem In msChart.Legends("Default").CustomItems
                    If thislegendItem.Name = thisSeries.Name Then
                        thislegendItem.Color = thisSeries.Color
                        Exit For
                    End If
                Next
            Next
        Catch ex As Exception
            'ErrorHandler(ex)
        End Try
    End Sub
    ''' <summary>
    ''' Handling of mouse down in the legend
    ''' When an item in the legend is selected. Show or hide it in the chart
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ChartResults_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles msChart.MouseDown
        Dim result As HitTestResult = msChart.HitTest(e.X, e.Y)
        If Not (result Is Nothing) And Not (result.Object Is Nothing) Then
            ' When user hits the LegendItem
            If TypeOf result.Object Is LegendItem Then
                ' Legend item result
                Dim legendItem As LegendItem = CType(result.Object, LegendItem)
                ' series item selected
                Dim selectedSeries As Series = CType(legendItem.Tag, Series)

                If Not (selectedSeries Is Nothing) Then
                    If selectedSeries.Enabled Then selectedSeries.Enabled = False Else selectedSeries.Enabled = True
                    msChart.ChartAreas("Default").AxisY.Minimum = [Double].NaN
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' Display all data in the chart
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayAllChartdata()
 
        msChart.ChartAreas(0).AxisX.Minimum = lmeasurementStartTime.ToOADate()
        msChart.ChartAreas(0).AxisX.Maximum = ldate.ToOADate()

        Dim interval As Integer
        Dim interaltype As System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType

        Select Case DateDiff(DateInterval.Second, lmeasurementStartTime, ldate)
            Case Is < 30
                interval = 1
                interaltype = DateTimeIntervalType.Seconds
            Case Is < 100
                interval = 5
                interaltype = DateTimeIntervalType.Seconds
            Case Is < 300
                interval = 10
                interaltype = DateTimeIntervalType.Seconds
            Case Is < 600
                interval = 0.5
                interaltype = DateTimeIntervalType.Minutes
            Case Is >= 600
                interval = 1
                interaltype = DateTimeIntervalType.Minutes
        End Select

        'MOD 59 interval = 100
        'MOD 59 interaltype = DateTimeIntervalType.Milliseconds

        msChart.ChartAreas("Default").AxisX.Interval = interval
        msChart.ChartAreas("Default").AxisX.IntervalType = interaltype

        msChart.ChartAreas("Default").CursorX.IsUserEnabled = True
        msChart.ChartAreas("Default").CursorX.IsUserSelectionEnabled = True
        msChart.ChartAreas("Default").CursorX.IntervalType = DateTimeIntervalType.Milliseconds
        msChart.ChartAreas("Default").CursorX.Interval = 0.005D

        msChart.ChartAreas("Default").AxisX.ScaleView.SmallScrollSizeType = DateTimeIntervalType.Milliseconds
        msChart.ChartAreas("Default").AxisX.ScaleView.SmallScrollSize = 0.005D
        msChart.ChartAreas("Default").AxisX.ScaleView.Zoomable = True

        msChart.ChartAreas("Default").AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Milliseconds
        msChart.ChartAreas("Default").AxisX.ScaleView.MinSize = 0.005D

        msChart.ChartAreas("Default").AxisX.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Milliseconds
        msChart.ChartAreas("Default").AxisX.ScaleView.SmallScrollMinSize = 0.005D

        msChart.ChartAreas("Default").CursorY.IsUserEnabled = True
        msChart.ChartAreas("Default").CursorY.IsUserSelectionEnabled = True
        msChart.ChartAreas("Default").AxisY.ScaleView.Zoomable = True
        msChart.ChartAreas("Default").AxisY.ScrollBar.IsPositionedInside = True
    End Sub

    'Public Sub CreateYAxis(ByVal chart As Chart, ByVal area As ChartArea, ByVal series As Series, ByVal axisOffset As Double, ByVal labelsSize As Double)

    '    ' Create new chart area for original series
    '    Dim areaSeries As New ChartArea
    '    areaSeries = chart.ChartAreas.Add("ChartArea_" + series.Name)
    '    areaSeries.BackColor = Color.Transparent
    '    areaSeries.BorderColor = Color.Transparent
    '    areaSeries.Position.FromRectangleF(area.Position.ToRectangleF())
    '    areaSeries.InnerPlotPosition.FromRectangleF(area.InnerPlotPosition.ToRectangleF())
    '    areaSeries.AxisX.MajorGrid.Enabled = False
    '    areaSeries.AxisX.MajorTickMark.Enabled = False
    '    areaSeries.AxisX.LabelStyle.Enabled = False
    '    areaSeries.AxisY.MajorGrid.Enabled = False
    '    areaSeries.AxisY.MajorTickMark.Enabled = False
    '    areaSeries.AxisY.LabelStyle.Enabled = False
    '    areaSeries.AxisY.IsStartedFromZero = area.AxisY.IsStartedFromZero


    '    series.ChartArea = areaSeries.Name

    '    ' Create new chart area for axis
    '    Dim areaAxis As New ChartArea
    '    areaAxis = chart.ChartAreas.Add("AxisY_" + series.ChartArea)
    '    areaAxis.BackColor = Color.Transparent
    '    areaAxis.BorderColor = Color.Transparent
    '    areaAxis.Position.FromRectangleF(chart.ChartAreas(series.ChartArea).Position.ToRectangleF())
    '    areaAxis.InnerPlotPosition.FromRectangleF(chart.ChartAreas(series.ChartArea).InnerPlotPosition.ToRectangleF())

    '    ' Create a copy of specified series
    '    Dim seriesCopy As New Series
    '    seriesCopy = chart.Series.Add(series.Name + "_Copy")
    '    seriesCopy.ChartType = series.ChartType
    '    For Each point As DataPoint In series.Points
    '        seriesCopy.Points.AddXY(point.XValue, point.YValues(0))

    '    Next

    '    ' Hide copied series
    '    seriesCopy.IsVisibleInLegend = False
    '    seriesCopy.Color = Color.Transparent
    '    seriesCopy.BorderColor = Color.Transparent
    '    seriesCopy.ChartArea = areaAxis.Name

    '    ' Disable drid lines & tickmarks
    '    areaAxis.AxisX.LineWidth = 0
    '    areaAxis.AxisX.MajorGrid.Enabled = False
    '    areaAxis.AxisX.MajorTickMark.Enabled = False
    '    areaAxis.AxisX.LabelStyle.Enabled = False
    '    areaAxis.AxisY.MajorGrid.Enabled = False
    '    areaAxis.AxisY.IsStartedFromZero = area.AxisY.IsStartedFromZero
    '    areaAxis.AxisY.LabelStyle.Font = area.AxisY.LabelStyle.Font

    '    'Adjust area position
    '    areaAxis.Position.X -= axisOffset
    '    areaAxis.InnerPlotPosition.X += labelsSize
    'End Sub

#End Region
#Region "Update chart values"
    ''' <summary>
    ''' Update the chart and the values.
    ''' </summary>
    ''' <param name="measurementvalues"></param>
    ''' <remarks></remarks>
    Public Sub UpdateUI(ByVal measurementvalues As Global.Inspector.BusinessLogic.Interfaces.Events.MeasurementEventArgs)

        'Set the labels with the values
        SetRadlabelText(rlblMeasuredValue, measurementvalues.Measurements.Last.Measurement)
        If _measurementResultsCompleted = False Then
            Select Case _scriptcommand5x.ScriptCommand
                Case ScriptCommand5XType.ScriptCommand51
                    'MOD 01
                    If DateDiff(DateInterval.Second, lmeasurementStartTime, Now) > showLeakageIntervals Then
                        'MOD 27
                        Dim leakageValueText As Double
                        If leakageUnit = UnitOfMeasurement.ItemDm3h Then
                            'Now check the leakagetype in the scriptcommand
                            Select Case leakageType
                                Case Leakage.Dash : leakageValueText = measurementvalues.Measurements.Last.LeakageValue
                                Case Leakage.Membrane : leakageValueText = measurementvalues.Measurements.Last.LeakageMembrane
                                Case Leakage.V1 : leakageValueText = measurementvalues.Measurements.Last.LeakageV1
                                Case Leakage.V2 : leakageValueText = measurementvalues.Measurements.Last.LeakageV2
                            End Select
                        Else
                            'In case mbar/min
                            leakageValueText = measurementvalues.Measurements.Last.LeakageValue
                        End If

                        SetRadlabelText(rlblCalculatedValue, Round(leakageValueText, 3))
                    Else
                        If textLeakageWait = " - " Then textLeakageWait = "-  -" Else textLeakageWait = " - "
                        SetRadlabelText(rlblCalculatedValue, textLeakageWait)
                    End If

                Case ScriptCommand5XType.ScriptCommand52
                    SetRadlabelText(rlblCalculatedValue, Round(measurementvalues.Measurements.Last.Average, 4))
                Case ScriptCommand5XType.ScriptCommand53
                    SetRadlabelText(rlblCalculatedValue, measurementvalues.Measurements.Last.Maximum)
                Case ScriptCommand5XType.ScriptCommand56
                    SetRadlabelText(rlblCalculatedValue, measurementvalues.Measurements.Last.Maximum)
                Case ScriptCommand5XType.ScriptCommand54
                    SetRadlabelText(rlblCalculatedValue, measurementvalues.Measurements.Last.Minimum)
                Case ScriptCommand5XType.ScriptCommand57
                    SetRadlabelText(rlblCalculatedValue, measurementvalues.Measurements.Last.Minimum)
                Case ScriptCommand5XType.ScriptCommand55
                    SetRadlabelText(rlblCalculatedValue, measurementvalues.Measurements.Last.Measurement)
            End Select
            'MOD 42
        ElseIf _displayLeakageResult = True Then
            _displayLeakageResult = False
            'MOD 27
            'Display once
            'Display leakage in dm3/h at the end of the measurement
            'MOD 58 If leakageUnit = UnitOfMeasurement.ItemMbarMin And _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand51 And leakageType <> Leakage.Dash Then
            If leakageUnit = CastToTypeUnitsValueOrUnset(ModPlexorUnits.UnitChangeRate) And _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand51 And leakageType <> Leakage.Dash Then

                rdTextResultLeakageDm3h.Visible = True
                rlblResultLeakageDm3h.Visible = True
                rlblResultLeakageDm3hUnit.Visible = True
                'Now check the leakagetype in the scriptcommand
                Select Case leakageType
                    Case Leakage.Dash : SetRadlabelText(rlblResultLeakageDm3h, "-")
                    Case Leakage.Membrane : SetRadlabelText(rlblResultLeakageDm3h, _lastReceivedMeasurement.LeakageMembrane)
                    Case Leakage.V1
                        If Not (Double.IsNaN(_lastReceivedMeasurement.LeakageV1)) Then
                            SetRadlabelText(rlblResultLeakageDm3h, _lastReceivedMeasurement.LeakageV1)
                        Else
                            SetRadlabelText(rlblResultLeakageDm3h, "-")
                        End If
                    Case Leakage.V2
                        If Not (Double.IsNaN(_lastReceivedMeasurement.LeakageV2)) Then
                            SetRadlabelText(rlblResultLeakageDm3h, _lastReceivedMeasurement.LeakageV2)
                        Else
                            SetRadlabelText(rlblResultLeakageDm3h, "-")
                        End If
                End Select
                SetRadlabelText(rlblResultLeakageDm3hUnit, ModPlexorUnits.UnitQvsLeakage) 'MOD 58 InspectionProcedureResx.str_Unit_dm3h)
            End If
        End If

        'Set the charts with the values
        For Each m As InspectionMeasurement.ScriptCommand5XMeasurement In measurementvalues.Measurements
            'MOD 27 'Storing the last received measurement result
            _lastReceivedMeasurement = m

            'MOD 59
            Dim values As String = [String].Format("Values | '{0}' | '{1}' | '{2}' | '{3}' | '{4}' | '{5}' | '{6}' | '{7}' | '{8}' |", m.Measurement, m.Minimum, m.Maximum, m.Average, m.LeakageValue, m.LeakageMembrane, m.LeakageV1, m.LeakageV2, m.IoStatus)
            'Depending on measurement frequency set date
            ldate = ldate.AddMilliseconds(1000 / _scriptcommand5x.MeasurementFrequency)
            Debug.Print("NOW: " & Format(Now, "HH:mm:ss:fff") & " " & ldate.ToString("HH:mm:ss:fff") & ": " & values)

            'These values are always displayed
            SetChartValue(msChart, My.Resources.InspectionProcedureResx.str_ValueCurrent, m.Measurement, ldate)
            'msChart.Series(0).Points.AddXY(ldate, 4)
            minValueY1 = m.Minimum
            maxValueY1 = m.Maximum



            'Do not update these values if the measurement (result value) is finished
            'In case of an extra measurement period the current value is updated.
            If _measurementResultsCompleted = False Then
                Select Case _scriptcommand5x.ScriptCommand
                    Case ScriptCommand5XType.ScriptCommand51

                        'MOD 01
                        If DateDiff(DateInterval.Second, lmeasurementStartTime, ldate) > showLeakageIntervals Then
                            'MOD 27
                            Dim leakageValueChart As Double

                            If leakageUnit = UnitOfMeasurement.ItemDm3h Then
                                'Now check the leakagetype in the scriptcommand
                                Select Case leakageType
                                    Case Leakage.Dash : leakageValueChart = m.LeakageValue
                                    Case Leakage.Membrane : leakageValueChart = m.LeakageMembrane
                                    Case Leakage.V1 : leakageValueChart = m.LeakageV1
                                    Case Leakage.V2 : leakageValueChart = m.LeakageV2
                                End Select
                            Else
                                'In case mbar/min
                                leakageValueChart = m.LeakageValue
                            End If

                            SetChartValue(msChart, My.Resources.InspectionProcedureResx.str_ValueLeakage, leakageValueChart, ldate)
                            'MOD 13
                            minValueY2 = Math.Min(minValueY2, leakageValueChart)
                            maxValueY2 = Math.Max(maxValueY2, leakageValueChart)
                        End If
                    Case ScriptCommand5XType.ScriptCommand52
                        SetChartValue(msChart, My.Resources.InspectionProcedureResx.str_ValueAverage, m.Average, ldate)
                    Case ScriptCommand5XType.ScriptCommand53, ScriptCommand5XType.ScriptCommand56
                        'MOD 14
                        If Not (Double.IsNaN(m.Maximum)) Then
                            SetChartValue(msChart, My.Resources.InspectionProcedureResx.str_ValueMaximum, m.Maximum, ldate)
                            '-----------------------------------------------
                            'MOD 59
                            If m.Measurement >= m.Maximum Then annotionPointMaxIsCurrent = msChart.Series(My.Resources.InspectionProcedureResx.str_ValueMaximum).Points.Count - 1
                            '-----------------------------------------------
                        End If
                    Case ScriptCommand5XType.ScriptCommand54, ScriptCommand5XType.ScriptCommand57
                        'MOD 14
                        If Not (Double.IsNaN(m.Minimum)) Then
                            SetChartValue(msChart, My.Resources.InspectionProcedureResx.str_ValueMimimum, m.Minimum, ldate)
                            '-----------------------------------------------
                            'MOD 59
                            If m.Measurement <= m.Minimum Then annotionPointMaxIsCurrent = msChart.Series(My.Resources.InspectionProcedureResx.str_ValueMimimum).Points.Count - 1
                            '-----------------------------------------------
                        End If
                End Select
            End If

            '-----------------------------------------------
            'MOD 59
            '---------------------------------------------------------------------------------------------------------
            '                                       Poke                        Sensor
            '			                            Rechts	links	midden		detect	trapped	Geen sensor
            'rechts	sensor	input2	links			Groen	Geel	blue		Groen 	geel	Rood
            '1	    2	    4	    8									
            '                               0                       x                           x
            'x				                1		x						                    x
            '       x			            2			    x					                x
            'x	    x			            3		x						                    x
            '		        x		        4				        x		    x		
            'x		        x		        5		x				            x		
            '	    x	    x		        6				        x			        x	
            'x	    x	    x		        7		x					                x	
            '		        x	    x	    12  			x			        x		
            '	    x	    x	    x	    14  			x				            x	
            '			            x	    8	    		x					                x
            '---------------------------------------------------------------------------------------------------------

            'To prevent that the annotation information is set at the start of the measurement when IOstatus = 7 (no reset)
            If m.IoStatus = 7 And previousIoStatus <> 7 And annotationSet = False Then ShowAnnotation = False Else previousIoStatus = m.IoStatus
            If m.IoStatus <> -1 Then rdPanelPN1003.Visible = True

            Select Case m.IoStatus
                Case 0
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_blue
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_red
                Case 1
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_green
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_red
                Case 2
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_yellow
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_red
                Case 3
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_green
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_red
                Case 4
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_blue
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_green
                Case 5
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_green
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_green
                Case 6
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_blue
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_yellow
                Case 8
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_yellow
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_red

                Case 12
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_yellow
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_green
                Case 14
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_yellow
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_yellow
                Case 7
                    picStatusPokeIO.Image = Resources.bullet_ball_glass_green
                    picStatusSensorIO.Image = Resources.bullet_ball_glass_yellow
                    If annotationSet = False And ShowAnnotation = False Then
                        If _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand56 Or _
                            _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand53 Then
                            annotationChartSeries = My.Resources.InspectionProcedureResx.str_ValueMaximum
                            ShowAnnotation = True
                        End If
                        If _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand57 Or _
                            _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand54 Then
                            annotationChartSeries = My.Resources.InspectionProcedureResx.str_ValueMimimum
                            ShowAnnotation = True
                        End If
                        If ShowAnnotation = True Then
                            SetAnnotion(msChart, annotationChartSeries, annotionPointMaxIsCurrent, My.Resources.InspectionProcedureResx.str_SAV_Setting) 'Ist 
                            SetAnnotion(msChart, annotationChartSeries, msChart.Series(annotationChartSeries).Points.Count - 1, My.Resources.InspectionProcedureResx.str_SAV_trapValue) 'angesprochen
                            annotationSet = True
                            safetyValueTripped = True
                        End If
                    End If
            End Select
            '-----------------------------------------------

        Next
        'Set the range values
        SetChartRangeXaxilValue(msChart, ldate)

        'Set boudaries values
        If _scriptcommand5x.StationStepObject.Boundaries IsNot Nothing Then
            'Minimum value
            SetChartValue(msChart, My.Resources.InspectionProcedureResx.str_ValuebndMin, _scriptcommand5x.StationStepObject.Boundaries.ValueMin * lowHighCorrection, ldate)
            'Maximum value
            SetChartValue(msChart, My.Resources.InspectionProcedureResx.str_ValuebndMax, _scriptcommand5x.StationStepObject.Boundaries.ValueMax * lowHighCorrection, ldate)

            If _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand51 Then
                'MOD 58
                If Not Double.IsNaN(_scriptcommand5x.StationStepObject.Boundaries.ValueMin) Then minValueY2 = Math.Min(minValueY2, _scriptcommand5x.StationStepObject.Boundaries.ValueMin * lowHighCorrection)
                If Not Double.IsNaN(_scriptcommand5x.StationStepObject.Boundaries.ValueMax) Then maxValueY2 = Math.Max(maxValueY2, _scriptcommand5x.StationStepObject.Boundaries.ValueMax * lowHighCorrection)
            Else
                If Not Double.IsNaN(_scriptcommand5x.StationStepObject.Boundaries.ValueMin) Then minValueY1 = Math.Min(minValueY1, _scriptcommand5x.StationStepObject.Boundaries.ValueMin * lowHighCorrection)
                If Not Double.IsNaN(_scriptcommand5x.StationStepObject.Boundaries.ValueMax) Then maxValueY1 = Math.Max(maxValueY1, _scriptcommand5x.StationStepObject.Boundaries.ValueMax * lowHighCorrection)
            End If
        End If

        'MOD 13
        If _scriptcommand5x.ScriptCommand <> ScriptCommand5XType.ScriptCommand51 Then
            minValueY2 = minValueY1
            maxValueY2 = maxValueY1
        End If

        'MOD 13
        'Setting the range of the y-axil
        SetChartRangeYaxilValue(msChart, minValueY1, maxValueY1, minValueY2, maxValueY2)

    End Sub
#End Region
#Region "GUI handling"
    ''' <summary>
    ''' Set the label text
    ''' </summary>
    ''' <param name="controllabel"></param>
    ''' <param name="text"></param>
    ''' <remarks></remarks>
    Private Sub SetRadlabelText(ByVal [controllabel] As Telerik.WinControls.UI.RadLabel, ByVal [text] As String)
        If [controllabel].InvokeRequired Then
            Try
                Dim SetLabelText_Delegate As New SetLabelText_Delegate(AddressOf SetRadlabelText)
                Me.Invoke(SetLabelText_Delegate, New Object() {[controllabel], [text]})
            Catch ex As Exception
            End Try
        Else
            [controllabel].Text = [text]
        End If
    End Sub

    ''' <summary>
    ''' Set the label text
    ''' </summary>
    ''' <param name="controllabel"></param>
    ''' <param name="text"></param>
    ''' <remarks></remarks>
    Private Sub SetRadlabelToolTipText(ByVal [controllabel] As Telerik.WinControls.UI.RadLabel, ByVal [text] As String)
        If [controllabel].InvokeRequired Then
            Dim SetLabelText_Delegate As New SetLabelText_Delegate(AddressOf SetRadlabelText)
            Me.Invoke(SetLabelText_Delegate, New Object() {[controllabel], [text]})
        Else
            [controllabel].LabelElement.ToolTipText = [text]
        End If
    End Sub

#End Region


#Region "Properties"
    ''' <summary>
    ''' Set script command 43 information
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property Scriptcommand5x() As InspectionProcedure.ScriptCommand5X
        Set(ByVal value As InspectionProcedure.ScriptCommand5X)
            _scriptcommand5x = value
        End Set
    End Property
    ''' <summary>
    ''' Set the unit of manometer 1.
    ''' This is for settings the unit with the measurement data
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property ManometerUnit As String
        Set(value As String)
            _manometermUnit = value
        End Set
    End Property
    ''' <summary>
    ''' The last colleted result
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property lastResult As InspectionReportingResults.ReportResult
        Set(value As InspectionReportingResults.ReportResult)
            _lastresult = value
        End Set
    End Property


    ''' <summary>
    ''' Set if the measurement is completed and the information should not be updated during the extra measurement period
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property MeasurementResultComplete As Boolean
        Set(value As Boolean)
            _measurementResultsCompleted = value
            _displayLeakageResult = True
        End Set
    End Property

    ''' <summary>
    ''' MOD 82 Set if the measurement has a Value; Set false if measurement with SSD is stopped by user
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property MeasurementHasValue As Boolean
        Set(value As Boolean)
            _measurementHasValue = value
        End Set
    End Property
    ''' <summary>
    ''' Set if the measurement is completed
    ''' This will stops the timer to blink 
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property MeasurementComplete As Boolean
        Set(value As Boolean)

            ShowText = True
            myTimer.Stop()
            SetRadlabelText(rdTextMeasuredValue, flashTextrdTextMeasuredValue)
            '-----------------------------------------------

            ' '' ''MOD 59
            'If _scriptcommand5x.ScriptCommand = ScriptCommand5XType.ScriptCommand57 And safetyValueTripped = False Then
            ' updateValue = False
            'rlblCalculatedValue.Text = "-"
            'End If
            'MOD 82 Set if measurement is done with SSD sensor but user has manual stop measurement. No value is stored and displayed.
            If _measurementHasValue = False Then rlblCalculatedValue.Text = "-"

            '-----------------------------------------------
            SetRadlabelText(rdTextCalculatedValue, flashTextrdTextCalculatedValue)
            _measurementCompleted = value
        End Set
    End Property

#End Region


    ''' <summary>
    ''' Handling of timer elapsed
    ''' Used to flash the current value and the measurement value.
    ''' If Measurement is completed the flashing is stopped
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MyTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles myTimer.Elapsed

        If Me.ShowText = True Then
            SetRadlabelText(rdTextMeasuredValue, flashTextrdTextMeasuredValue)
            SetRadlabelText(rdTextCalculatedValue, flashTextrdTextCalculatedValue)
            Me.ShowText = False
        Else
            SetRadlabelText(rdTextMeasuredValue, String.Empty)
            If _measurementResultsCompleted = False Then SetRadlabelText(rdTextCalculatedValue, String.Empty)
            Me.ShowText = True
        End If
    End Sub


    Private Sub RadPanel1_Paint(sender As Object, e As Windows.Forms.PaintEventArgs)

    End Sub

    Private Sub rdTextMaximumBound_Click(sender As Object, e As EventArgs) Handles rdTextMaximumBound.Click

    End Sub
End Class


