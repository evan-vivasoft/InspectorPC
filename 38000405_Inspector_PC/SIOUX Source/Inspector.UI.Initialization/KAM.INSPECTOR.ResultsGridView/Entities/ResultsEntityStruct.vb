Imports Inspector.Model
Namespace Model.ResultView.Entities
    Public Class ResultsEntityStruct

        Inherits InspectionReportingResults.ReportInspectionResult

        Public Shadows Property Results

        Public Sub New()
            Me.m_Values = New List(Of Value_entity)()

        End Sub

        Public Property Values() As List(Of Value_entity)
            Get
                Return m_Values
            End Get
            Set(ByVal value As List(Of Value_entity))

                m_Values = value
            End Set
        End Property
        Private m_Values As List(Of Value_entity)


        Public Class Value_entity
            Public Sub New()
                Me.ListSC4 = New List(Of List_Value)()

            End Sub


            Public Property Section() As String
                Get
                    Return m_Section
                End Get
                Set(ByVal value As String)
                    m_Section = value
                End Set
            End Property
            Private m_Section As String

            Public Property SubSection() As String
                Get
                    Return m_SubSection
                End Get
                Set(ByVal value As String)
                    m_SubSection = value
                End Set
            End Property
            Private m_SubSection As String

            Public Property ScriptCommand() As String
                Get
                    Return m_ScriptCommand
                End Get
                Set(ByVal value As String)
                    m_ScriptCommand = value
                End Set
            End Property
            Private m_ScriptCommand As String

            Public Property InstructionText() As String
                Get
                    Return m_InstructionText
                End Get
                Set(ByVal value As String)
                    m_InstructionText = value
                End Set
            End Property
            Private m_InstructionText As String

            Public Property FieldNo() As String
                Get
                    Return m_FieldNo
                End Get
                Set(ByVal value As String)
                    m_FieldNo = value
                End Set
            End Property
            Private m_FieldNo As String

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

            Public Property ObjectDescription As String
                Get
                    Return m_ObjectDescription
                End Get
                Set(ByVal value As String)
                    m_ObjectDescription = value
                End Set
            End Property
            Private m_ObjectDescription As String

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

            Public Property MeasurePointDescription As String
                Get
                    Return m_MeasurePointDescription
                End Get
                Set(ByVal value As String)
                    m_MeasurePointDescription = value
                End Set
            End Property
            Private m_MeasurePointDescription As String

            Public Property ValueMin() As String
                Get
                    Return m_ValueMin
                End Get
                Set(ByVal value As String)
                    m_ValueMin = value
                End Set
            End Property
            Private m_ValueMin As String

            Public Property ValueMax() As String
                Get
                    Return m_ValueMax
                End Get
                Set(ByVal value As String)
                    m_ValueMax = value
                End Set
            End Property
            Private m_ValueMax As String

            Public Property ValueActuel() As String
                Get
                    Return m_ValueActuel
                End Get
                Set(ByVal value As String)
                    m_ValueActuel = value
                End Set
            End Property
            Private m_ValueActuel As String


            Public Property ValueText() As String
                Get
                    Return m_ValueText
                End Get
                Set(ByVal value As String)
                    m_ValueText = value
                End Set
            End Property
            Private m_ValueText As String

            Public Property DateTime() As String
                Get
                    Return m_DateTime
                End Get
                Set(ByVal value As String)
                    m_DateTime = value
                End Set
            End Property
            Private m_DateTime As String


            Public Property ListSC4() As List(Of List_Value)
                Get
                    Return m_ListSC4
                End Get
                Set(ByVal value As List(Of List_Value))
                    m_ListSC4 = value
                End Set
            End Property
            Private m_ListSC4 As List(Of List_Value)


        End Class

        Public Class List_Value

            Public Property ListText() As String
                Get
                    Return m_ListText
                End Get
                Set(ByVal value As String)
                    m_ListText = value
                End Set
            End Property
            Private m_ListText As String
            Public Property ListCode() As String
                Get
                    Return m_ListCode
                End Get
                Set(ByVal value As String)
                    m_ListCode = value
                End Set
            End Property
            Private m_ListCode As String
            Public Property ListValueChecked() As String
                Get
                    Return m_ListValueChecked
                End Get
                Set(ByVal value As String)
                    m_ListValueChecked = value
                End Set
            End Property
            Private m_ListValueChecked As String
            Public Property ListQuestion() As String
                Get
                    Return m_ListQuestion
                End Get
                Set(ByVal value As String)
                    m_ListQuestion = value
                End Set
            End Property
            Private m_ListQuestion As String

        End Class

    End Class
End Namespace
