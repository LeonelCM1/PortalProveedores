Namespace AbstractLayer
    Public MustInherit Class baseUserData
        Private _FirstName As String
        Public Property FirstName() As String
            Get
                Return _FirstName
            End Get
            Set(ByVal value As String)
                _FirstName = value
            End Set
        End Property
    End Class
    Public MustInherit Class baseReceptionData
        Public MustInherit Class baseRecepcionData
            Private _tipo_docp As String
            Public Property FolioOC() As String
                Get
                    Return _tipo_docp
                End Get
                Set(ByVal value As String)
                    _tipo_docp = value
                End Set
            End Property
        End Class
    End Class
End Namespace
