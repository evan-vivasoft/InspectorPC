'
'//===============================================================================
'// Copyright Wigersma
'// All rights reserved.
'//===============================================================================
'
Imports System.Collections.Generic
Imports System.Xml.Serialization

Namespace Model.Station.Entities

    <XmlRoot(ElementName:="PRSData")> _
    Public Class PRSDataEntity
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")> _
        <XmlElement(ElementName:="PRS")> _
        Public Property PresureRegulatorStations() As List(Of PRSSyncEntity)
            Get
                Return m_PresureRegulatorStation
            End Get
            Set(value As List(Of PRSSyncEntity))
                m_PresureRegulatorStation = value
            End Set
        End Property
        Private m_PresureRegulatorStation As New List(Of PRSSyncEntity)

        <XmlElement(ElementName:="GasControlLine", IsNullable:=False)> _
        Public Property GasControlLine() As List(Of Model.Station.Entities.GclSyncEntity)
            Get
                Return m_GasControlLine
            End Get
            Set(value As List(Of Model.Station.Entities.GclSyncEntity))
                m_GasControlLine = value
            End Set
        End Property
        Private m_GasControlLine As New List(Of GclSyncEntity)
    End Class

 
End Namespace
