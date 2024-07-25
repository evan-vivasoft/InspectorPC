Imports System.Collections.Generic
Imports Inspector.Model
Imports System.Xml.Serialization

Namespace Model.Station.Entities 'Inspector.BusinessLogic.Data.Configuration.InspectionManager.Model.Station
    ''' <summary>
    ''' GclSyncEntity
    ''' </summary>
    <XmlRoot(ElementName:="GasControlLines")> _
    Public Class GclSyncEntity

        'Used for filtering
        <XmlIgnore()> _
        Public Property PRS_Status() As Integer
            Get
                Return m_PRS_Status
            End Get
            Set(value As Integer)
                m_PRS_Status = value
            End Set
        End Property
        Private m_PRS_Status As String
        <XmlIgnore()> _
        Public Property GCL_Status() As Integer
            Get
                Return m_GCL_Status
            End Get
            Set(value As Integer)
                m_GCL_Status = value
            End Set
        End Property
        Private m_GCL_Status As String

        Public Property PRSName() As String
            Get
                Return m_PRSName
            End Get
            Set(value As String)
                m_PRSName = value
            End Set
        End Property
        Private m_PRSName As String
        Public Property PRSIdentification() As String
            Get
                Return m_PRSIdentification
            End Get
            Set(value As String)
                m_PRSIdentification = value
            End Set
        End Property
        Private m_PRSIdentification As String
        Public Property GasControlLineName() As String
            Get
                Return m_GasControlLineName
            End Get
            Set(value As String)
                m_GasControlLineName = value
            End Set
        End Property
        Private m_GasControlLineName As String
        Public Property PeMin() As String
            Get
                Return m_PeMin
            End Get
            Set(value As String)
                m_PeMin = value
            End Set
        End Property
        Private m_PeMin As String
        Public Property PeMax() As String
            Get
                Return m_PeMax
            End Get
            Set(value As String)
                m_PeMax = value
            End Set
        End Property
        Private m_PeMax As String
        Public Property VolumeVA() As String
            Get
                Return m_VolumeVA
            End Get
            Set(value As String)
                m_VolumeVA = value
            End Set
        End Property
        Private m_VolumeVA As String
        Public Property VolumeVAK() As String
            Get
                Return m_VolumeVAK
            End Get
            Set(value As String)
                m_VolumeVAK = value
            End Set
        End Property
        Private m_VolumeVAK As String
        Public Property PaRangeDM() As TypeRangeDM
            Get
                Return m_PaRangeDM
            End Get
            Set(value As TypeRangeDM)
                m_PaRangeDM = value
            End Set
        End Property
        Private m_PaRangeDM As TypeRangeDM
        Public Property PeRangeDM() As TypeRangeDM
            Get
                Return m_PeRangeDM
            End Get
            Set(value As TypeRangeDM)
                m_PeRangeDM = value
            End Set
        End Property
        Private m_PeRangeDM As TypeRangeDM
        Public Property GCLIdentification() As String
            Get
                Return m_GCLIdentification
            End Get
            Set(value As String)
                m_GCLIdentification = value
            End Set
        End Property
        Private m_GCLIdentification As String
        <XmlElement(IsNullable:=False)> _
        Public Property GCLCode() As String
            Get
                Return m_GCLCode
            End Get
            Set(value As String)
                m_GCLCode = value
            End Set
        End Property
        Private m_GCLCode As String
        Public Property InspectionProcedure() As String
            Get
                Return m_InspectionProcedure
            End Get
            Set(value As String)
                m_InspectionProcedure = value
            End Set
        End Property
        Private m_InspectionProcedure As String
        Public Property FSDStart() As Integer
            Get
                Return m_FSDStart
            End Get
            Set(value As Integer)
                m_FSDStart = value
            End Set
        End Property
        Private m_FSDStart As Integer
        <XmlElement(ElementName:="GCLObjects", IsNullable:=False)> _
        Public Property GCLObjects() As List(Of GCLObject)
            Get
                Return m_GCLObjects
            End Get
            Set(value As List(Of GCLObject))
                m_GCLObjects = value
            End Set
        End Property
        Private m_GCLObjects As List(Of GCLObject)

        Public Sub New()

        End Sub
    End Class

    ''' <summary>
    ''' TypeObjectID
    ''' </summary>
    Public Class GCLObject
        Public Property ObjectName() As String
            Get
                Return m_ObjectName
            End Get
            Set(value As String)
                m_ObjectName = value
            End Set
        End Property
        Private m_ObjectName As String
        Public Property ObjectID() As String
            Get
                Return m_ObjectID
            End Get
            Set(value As String)
                m_ObjectID = value
            End Set
        End Property
        Private m_ObjectID As String
        Public Property MeasurePoint() As String
            Get
                Return m_MeasurePoint
            End Get
            Set(value As String)
                m_MeasurePoint = value
            End Set
        End Property
        Private m_MeasurePoint As String
        Public Property MeasurePointID() As String
            Get
                Return m_MeasurePointID
            End Get
            Set(value As String)
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
        <XmlElement(IsNullable:=False)> _
        Public Property Boundaries() As TypeSyncObjectIDBoundaries
            Get
                Return m_Boundaries
            End Get
            Set(value As TypeSyncObjectIDBoundaries)
                m_Boundaries = value
            End Set
        End Property
        Private m_Boundaries As TypeSyncObjectIDBoundaries

        Public Sub New()
        End Sub
    End Class

    ''' <summary>
    ''' TypeObjectIDBoundaries
    ''' </summary>
    Public Class TypeSyncObjectIDBoundaries
        Public Property ValueMax() As Double
            Get
                Return m_ValueMax
            End Get
            Set(value As Double)
                m_ValueMax = value
            End Set
        End Property
        Private m_ValueMax As Double
        Public Property ValueMin() As Double
            Get
                Return m_ValueMin
            End Get
            Set(value As Double)
                m_ValueMin = value
            End Set
        End Property
        Private m_ValueMin As Double
        Public Property UOV() As UnitOfMeasurement
            Get
                Return m_UOV
            End Get
            Set(value As UnitOfMeasurement)
                m_UOV = value
            End Set
        End Property
        Private m_UOV As UnitOfMeasurement
    End Class
End Namespace


