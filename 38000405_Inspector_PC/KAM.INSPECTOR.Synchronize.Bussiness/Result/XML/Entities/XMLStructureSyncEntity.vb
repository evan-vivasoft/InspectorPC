'
'//===============================================================================
'// Copyright Wigersma
'// All rights reserved.
'//===============================================================================
'
Imports System.Collections.Generic
Imports System.Xml.Serialization

Namespace Model.Result.Entities
    <XmlRoot(ElementName:="InspectionResultsData")> _
        Public Class InspectionResultsDataEntity
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")> _
        <XmlElement(ElementName:="InspectionResult")> _
        Public Property InspectionResults() As List(Of InspectionResultsEntity)
            Get
                Return m_InspectionResults
            End Get
            Set(ByVal value As List(Of InspectionResultsEntity))
                m_InspectionResults = value
            End Set
        End Property
        Private m_InspectionResults As New List(Of InspectionResultsEntity)
    End Class

    ''' <summary>
    ''' InspectionResult
    ''' </summary>
    <XmlRoot(ElementName:="InspectionResult")> _
    Public Class InspectionResultsEntity
        ''' <summary>
        ''' Used for filtering only
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        Public Property FileDate() As DateTime
            Get
                Return m_FileDate
            End Get
            Set(ByVal value As DateTime)
                m_FileDate = value
            End Set
        End Property
        Private m_FileDate As DateTime

        <XmlIgnore()> _
        Public Property InspectionResult_Id() As Integer
            Get
                Return m_InspectionResult_Id
            End Get
            Set(ByVal value As Integer)
                m_InspectionResult_Id = value
            End Set
        End Property
        Private m_InspectionResult_Id As Integer

        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property
        Private m_Status As String

        Public Property PRSIdentification() As String
            Get
                Return m_PRSIdentification
            End Get
            Set(ByVal value As String)
                m_PRSIdentification = value
            End Set
        End Property
        Private m_PRSIdentification As String
        Public Property PRSName() As String
            Get
                Return m_PRSName
            End Get
            Set(ByVal value As String)
                m_PRSName = value
            End Set
        End Property
        Private m_PRSName As String
        Public Property PRSCode() As String
            Get
                Return m_PRSCode
            End Get
            Set(ByVal value As String)
                m_PRSCode = value
            End Set
        End Property
        Private m_PRSCode As String
        Public Property GasControlLineName() As String
            Get
                Return m_GasControlLineName
            End Get
            Set(ByVal value As String)
                m_GasControlLineName = value
            End Set
        End Property
        Private m_GasControlLineName As String
        Public Property GCLIdentification() As String
            Get
                Return m_GCLIdentification
            End Get
            Set(ByVal value As String)
                m_GCLIdentification = value
            End Set
        End Property
        Private m_GCLIdentification As String
        <XmlElement(IsNullable:=False)> _
        Public Property GCLCode() As String
            Get
                Return m_GCLCode
            End Get
            Set(ByVal value As String)
                m_GCLCode = value
            End Set
        End Property
        Private m_GCLCode As String
        Public Property CRC() As String
            Get
                Return m_CRC
            End Get
            Set(ByVal value As String)
                m_CRC = value
            End Set
        End Property
        Private m_CRC As String

        <XmlIgnore()> _
        Public Property LastResult() As String
            Get
                Return m_LastResult
            End Get
            Set(ByVal value As String)
                m_LastResult = value
            End Set
        End Property
        Private m_LastResult As String

        ' En een entiteit waarin de gegevens van de Result tabel terecht komen
        <XmlElement(ElementName:="Measurement_Equipment")> _
        Public Property Measurement_Equipment() As List(Of Model.Result.Entities.Measurement_EquipmentEntity)
            Get
                Return m_Measurement_Equipment
            End Get
            Set(ByVal value As List(Of Model.Result.Entities.Measurement_EquipmentEntity))

                m_Measurement_Equipment = value
            End Set
        End Property
        Private m_Measurement_Equipment As List(Of Model.Result.Entities.Measurement_EquipmentEntity)

        ' En een entiteit waarin de gegevens van de InspectionProcedure tabel terecht komen
        <XmlElement(ElementName:="InspectionProcedure")> _
        Public Property InspectionProcedure() As List(Of Model.Result.Entities.InspectionProcedureEntity)
            Get
                Return m_InspectionProcedure
            End Get
            Set(ByVal value As List(Of Model.Result.Entities.InspectionProcedureEntity))

                m_InspectionProcedure = value
            End Set
        End Property
        Private m_InspectionProcedure As List(Of Model.Result.Entities.InspectionProcedureEntity)

        ' En een entiteit waarin de gegevens van de InspectionProcedure tabel terecht komen
        <XmlElement(ElementName:="DateTimeStamp")> _
        Public Property DateTimeStamp() As List(Of Model.Result.Entities.DateTimeStampEntity)
            Get
                Return m_DateTimeStamp
            End Get
            Set(ByVal value As List(Of Model.Result.Entities.DateTimeStampEntity))

                m_DateTimeStamp = value
            End Set
        End Property
        Private m_DateTimeStamp As List(Of Model.Result.Entities.DateTimeStampEntity)


        ' En een entiteit waarin de gegevens van de Result tabel terecht komen
        <XmlElement(ElementName:="Result")> _
        Public Property Result() As List(Of Model.Result.Entities.ResultsEntity)
            Get
                Return m_Result
            End Get
            Set(ByVal value As List(Of Model.Result.Entities.ResultsEntity))

                m_Result = value
            End Set
        End Property
        Private m_Result As List(Of Model.Result.Entities.ResultsEntity)

        Public Sub New()
            Me.Result = New List(Of Model.Result.Entities.ResultsEntity)()
            Me.Measurement_Equipment = New List(Of Model.Result.Entities.Measurement_EquipmentEntity)()
            Me.InspectionProcedure = New List(Of Model.Result.Entities.InspectionProcedureEntity)()
            Me.DateTimeStamp = New List(Of Model.Result.Entities.DateTimeStampEntity)()
        End Sub


    End Class

    ''' <summary>
    ''' Measurement_EquipmentEntitiy
    ''' </summary>
    <XmlRoot(ElementName:="Measurement_Equipment")> _
    Public Class Measurement_EquipmentEntity
        ''' <summary>
        ''' Used for filtering only
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore()> _
        Public Property Measurement_Equipment_Id() As Integer
            Get
                Return m_Measurement_Equipment_Id
            End Get
            Set(ByVal value As Integer)
                m_Measurement_Equipment_Id = value
            End Set
        End Property
        Private m_Measurement_Equipment_Id As Integer

        Public Property ID_DM1() As String
            Get
                Return m_ID_DM1
            End Get
            Set(ByVal value As String)
                m_ID_DM1 = value
            End Set
        End Property
        Private m_ID_DM1 As String
        Public Property ID_DM2() As String
            Get
                Return m_ID_DM2
            End Get
            Set(ByVal value As String)
                m_ID_DM2 = value
            End Set
        End Property
        Private m_ID_DM2 As String
        Public Property BT_Address() As String
            Get
                Return m_BT_Address
            End Get
            Set(ByVal value As String)
                m_BT_Address = value
            End Set
        End Property
        Private m_BT_Address As String

    End Class


    ''' <summary>
    ''' InspectionProcedure
    ''' </summary>
    <XmlRoot(ElementName:="InspectionProcedure")> _
    Public Class InspectionProcedureEntity

        <XmlElement(ElementName:="Name")> _
        Public Property InspectionProcedureName() As String
            Get
                Return m_InspectionProcedureName
            End Get
            Set(ByVal value As String)
                m_InspectionProcedureName = value
            End Set
        End Property
        Private m_InspectionProcedureName As String

        <XmlElement(ElementName:="Version")> _
        Public Property InspectionProcedureVersion() As String
            Get
                Return m_InspectionProcedureVersion
            End Get
            Set(ByVal value As String)
                m_InspectionProcedureVersion = value
            End Set
        End Property
        Private m_InspectionProcedureVersion As String

    End Class

    ''' <summary>
    ''' DateTimeStamp
    ''' </summary>
    <XmlRoot(ElementName:="DateTimeStamp")> _
    Public Class DateTimeStampEntity
        ''' <summary>
        ''' 
        ''' </summary>
        Public Const DATE_FORMAT As String = "yyyy-MM-dd"
        ''' <summary>
        ''' 
        ''' </summary>
        Public Const TIME_FORMAT As String = "HH:mm:ss"

        Public Property StartDate() As String
            Get
                Return m_StartDate
            End Get
            Set(ByVal value As String)
                m_StartDate = value
            End Set
        End Property
        Private m_StartDate As String
        Public Property StartTime() As String
            Get
                Return m_StartTime
            End Get
            Set(ByVal value As String)
                m_StartTime = value
            End Set
        End Property
        Private m_StartTime As String
        Public Property EndTime() As String
            Get
                Return m_EndTime
            End Get
            Set(ByVal value As String)
                m_EndTime = value
            End Set
        End Property
        Private m_EndTime As String

        ' En een entiteit waarin de gegevens van de Timesettings tabel terecht komen
        <XmlElement(ElementName:="TimeSettings")> _
        Public Property TimeSettings() As List(Of Model.Result.Entities.TimeSettingsEntity)
            Get
                Return m_TimeSettings
            End Get
            Set(ByVal value As List(Of Model.Result.Entities.TimeSettingsEntity))

                m_TimeSettings = value
            End Set
        End Property
        Private m_TimeSettings As List(Of Model.Result.Entities.TimeSettingsEntity)

        Public Sub New()

            Me.TimeSettings = New List(Of Model.Result.Entities.TimeSettingsEntity)
        End Sub

    End Class

    ''' <summary>
    ''' TimeSettings
    ''' </summary>
    <XmlRoot(ElementName:="TimeSettings")> _
    Public Class TimeSettingsEntity
        Public Property TimeZone() As String
            Get
                Return m_TimeZone
            End Get
            Set(ByVal value As String)
                m_TimeZone = value
            End Set
        End Property
        Private m_TimeZone As String
        Public Property DST() As String
            Get
                Return m_DST
            End Get
            Set(ByVal value As String)
                m_DST = value
            End Set
        End Property
        Private m_DST As String

    End Class

    ''' <summary>
    ''' ResultEntitiy
    ''' </summary>
    <XmlRoot(ElementName:="Result")> _
    Public Class ResultsEntity

        Public Property ObjectName() As String
            Get
                Return m_ObjectName
            End Get
            Set(ByVal value As String)
                m_ObjectName = value
            End Set
        End Property
        Private m_ObjectName As String

        Public Property ObjectID() As String
            Get
                Return m_ObjectID
            End Get
            Set(ByVal value As String)
                m_ObjectID = value
            End Set
        End Property
        Private m_ObjectID As String
        Public Property MeasurePoint() As String
            Get
                Return m_MeasurePoint
            End Get
            Set(ByVal value As String)
                m_MeasurePoint = value
            End Set
        End Property
        Private m_MeasurePoint As String
        Public Property MeasurePointID() As String
            Get
                Return m_MeasurePointID
            End Get
            Set(ByVal value As String)
                m_MeasurePointID = value
            End Set
        End Property
        Private m_MeasurePointID As String
        <XmlElement(IsNullable:=False)> _
        Public Property FieldNo() As String
            Get
                Return m_FieldNo
            End Get
            Set(ByVal value As String)
                m_FieldNo = value
            End Set
        End Property
        Private m_FieldNo As String
        Public Property Time() As String
            Get
                Return m_Time
            End Get
            Set(ByVal value As String)
                m_Time = value
            End Set
        End Property
        Private m_Time As String
        <XmlElement(ElementName:="MeasureValue")> _
        Public Property MeasureValue() As List(Of MeasureValue)
            Get
                Return m_MeasureValue
            End Get
            Set(ByVal value As List(Of MeasureValue))

                m_MeasureValue = value
            End Set
        End Property
        Private m_MeasureValue As List(Of MeasureValue)

        Public Property Text() As String
            Get
                Return m_Text
            End Get
            Set(ByVal value As String)
                m_Text = value
            End Set
        End Property
        Private m_Text As String

        ' En een entiteit waarin de gegevens van de List terecht komen
        <XmlElement(ElementName:="List")> _
        Public Property List() As List(Of String)
            Get
                Return m_List
            End Get
            Set(ByVal value As List(Of String))
                m_List = value
            End Set
        End Property
        Private m_List As List(Of String)

        Public Sub New()
            Me.MeasureValue = New List(Of Model.Result.Entities.MeasureValue)
            Me.List = New List(Of String)()
        End Sub
    End Class

    Public Class MeasureValue
        ''' <summary>
        ''' Gets or sets the value.
        ''' </summary>
        ''' <value>
        ''' The value.
        ''' </value>
        Public Property Value() As Double
            Get
                Return m_Value
            End Get
            Set(value As Double)
                m_Value = Value
            End Set
        End Property
        Private m_Value As Double

        ''' <summary>
        ''' Gets or sets the UOM.
        ''' </summary>
        ''' <value>
        ''' The UOM.
        ''' </value>
        Public Property UOM() As String
            Get
                Return m_UOM
            End Get
            Set(value As String)
                m_UOM = value
            End Set
        End Property
        Private m_UOM As String

    End Class




End Namespace
