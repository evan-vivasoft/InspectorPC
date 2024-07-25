'
'//===============================================================================
'// Copyright Wigersma
'// All rights reserved.
'//===============================================================================
'
Imports System.Collections.Generic
Imports System.Xml.Serialization

Namespace Model.Station.Entities



    ''' <summary>
    ''' PRSEntitiy
    ''' </summary>
    <XmlRoot(ElementName:="PRS")> _
    Public Class PRSSyncEntity

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
            Set(value As DateTime)
                m_FileDate = value
            End Set
        End Property
        Private m_FileDate As DateTime
        <XmlIgnore()> _
        Public Property PRS_Id() As Integer
            Get
                Return m_PRS_Id
            End Get
            Set(value As Integer)
                m_PRS_Id = value
            End Set
        End Property
        Private m_PRS_Id As Integer
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
        Public Property GCL_OverallStatus() As Integer
            Get
                Return m_GCL_OverallStatus
            End Get
            Set(value As Integer)
                m_GCL_OverallStatus = value
            End Set
        End Property
        Private m_GCL_OverallStatus As String

        Public Property Route() As String
            Get
                Return m_Route
            End Get
            Set(value As String)
                m_Route = value
            End Set
        End Property
        Private m_Route As String
        Public Property PRSCode() As String
            Get
                Return m_PRSCode
            End Get
            Set(value As String)
                m_PRSCode = value
            End Set
        End Property
        Private m_PRSCode As String
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
        Public Property Information() As String
            Get
                Return m_Information
            End Get
            Set(value As String)
                m_Information = value
            End Set
        End Property
        Private m_Information As String
        Public Property InspectionProcedure() As String
            Get
                Return m_InspectionProcedure
            End Get
            Set(value As String)
                m_InspectionProcedure = value
            End Set
        End Property
        Private m_InspectionProcedure As String
        <XmlElement(IsNullable:=False)> _
        Public Property PRSObjects() As List(Of PRSSyncObject)
            Get
                Return m_PRSObjects
            End Get
            Set(value As List(Of PRSSyncObject))
                m_PRSObjects = value
            End Set
        End Property
        Private m_PRSObjects As List(Of PRSSyncObject)



        Public Sub New()
            Me.PRSObjects = New List(Of PRSSyncObject)()
        End Sub
    End Class

    ''' <summary>
    ''' PRSPRSObjects
    ''' </summary>
    Public Class PRSSyncObject
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
        'Public Property FieldNo() As System.Nullable(Of Integer)
        '    Get
        '        Return m_FieldNo
        '    End Get
        '    Set(value As System.Nullable(Of Integer))
        '        m_FieldNo = value
        '    End Set
        'End Property
        'Private m_FieldNo As System.Nullable(Of Integer)
    End Class
End Namespace
