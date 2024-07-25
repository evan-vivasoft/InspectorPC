Namespace Model.ResultView.Entities
    Public Class ListInspectionResultsEntityStruct

        Public Property InspectionResults() As List(Of InspectionResult_entity)
            Get
                Return m_ResultsEntitie
            End Get
            Set(ByVal value As List(Of InspectionResult_entity))

                m_ResultsEntitie = value
            End Set
        End Property
        Private m_ResultsEntitie As List(Of InspectionResult_entity)


        Public Sub New()
            Me.InspectionResults = New List(Of InspectionResult_entity)()

        End Sub

        Public Class InspectionResult_entity


            Public Property PRSName() As String
                Get
                    Return m_PRSName
                End Get
                Set(ByVal value As String)
                    m_PRSName = value
                End Set
            End Property
            Private m_PRSName As String

            Public Property GCLName() As String
                Get
                    Return m_GCLName
                End Get
                Set(ByVal value As String)
                    m_GCLName = value
                End Set
            End Property
            Private m_GCLName As String = ""

            Public Property InspectionProcedure() As String
                Get
                    Return m_InspectionProcedure
                End Get
                Set(ByVal value As String)
                    m_InspectionProcedure = value
                End Set
            End Property
            Private m_InspectionProcedure As String


            Public Property DateTime() As DateTime
                Get
                    Return m_DateTime
                End Get
                Set(ByVal value As DateTime)
                    m_DateTime = value
                End Set
            End Property
            Private m_DateTime As DateTime

        End Class


    End Class
End Namespace
