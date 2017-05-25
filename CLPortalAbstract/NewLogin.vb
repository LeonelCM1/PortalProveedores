Public Class NewLogin

End Class

Public Class DatosUser

    Private _rfc As String
    Private _proveedor As String
    Private _ccTipo As String
    Private _ccCve As String
    Private _tipdocCve As String
    Private _nomDoc As String
    Private _seleccionado As Boolean
    Private _nomArch As String
    Private _rutaDocs As String


    Public Property Rfc() As String
        Get
            Return _rfc
        End Get
        Set(ByVal value As String)
            _rfc = value
        End Set
    End Property
    Public Property Proveedor() As String
        Get
            Return _proveedor
        End Get
        Set(ByVal value As String)
            _proveedor = value
        End Set
    End Property
    Public Property CcTipo() As String
        Get
            Return _ccTipo
        End Get
        Set(ByVal value As String)
            _ccTipo = value
        End Set
    End Property
    Public Property CcCve() As String
        Get
            Return _ccCve
        End Get
        Set(ByVal value As String)
            _ccCve = value
        End Set
    End Property
    Public Property TipdocCve() As String
        Get
            Return _tipdocCve
        End Get
        Set(ByVal value As String)
            _tipdocCve = value
        End Set
    End Property
    Public Property NomDoc() As String
        Get
            Return _nomDoc
        End Get
        Set(ByVal value As String)
            _nomDoc = value
        End Set
    End Property
    Public Property Seleccionado() As Boolean
        Get
            Return _seleccionado
        End Get
        Set(ByVal value As Boolean)
            _seleccionado = value
        End Set
    End Property
    Public Property NomArch() As String
        Get
            Return _nomArch
        End Get
        Set(ByVal value As String)
            _nomArch = value
        End Set
    End Property
    Public Property RutaDocs() As String
        Get
            Return _rutaDocs
        End Get
        Set(ByVal value As String)
            _rutaDocs = value
        End Set
    End Property


    Public Sub DatosUser()
    End Sub

    Public Sub DatosUser(ByVal Rfc As String, ByVal Proveedor As String, ByVal CcTipo As String, _
                          ByVal CcCve As String, ByVal TipdocCve As String, _
                          ByVal NomDoc As String, ByVal Seleccionado As Boolean, _
                          ByVal NomArch As String, ByVal RutaDocs As String)
        Me.Rfc = _rfc
        Me.Proveedor = _proveedor
        Me.CcTipo = _ccTipo
        Me.CcCve = _ccCve
        Me.TipdocCve = _tipdocCve
        Me.NomDoc = _nomDoc
        Me.Seleccionado = _seleccionado
        Me.NomArch = _nomArch
        Me.RutaDocs = _rutaDocs
    End Sub
End Class
Public Class PerfilesUser

    Private _perfil As List(Of DatosUser)

    Public Property Perfil() As List(Of DatosUser)
        Get
            Return _perfil
        End Get
        Set(ByVal value As List(Of DatosUser))
            _perfil = value
        End Set
    End Property

    Public Sub PerfilesUser()
    End Sub

    Public Sub PerfilesUser(ByVal Perfil As List(Of DatosUser))
        Me.Perfil = _perfil
    End Sub
End Class
Public Class ConfigGral

    Private _config As String
    Public Property config() As String
        Get
            Return _config
        End Get
        Set(ByVal value As String)
            _config = value
        End Set
    End Property
    Private _tipo As String
    Public Property tipo() As String
        Get
            Return _tipo
        End Get
        Set(ByVal value As String)
            _tipo = value
        End Set
    End Property
    Private _valor As Object
    Public Property valor() As Object
        Get
            Return _valor
        End Get
        Set(ByVal value As Object)
            _valor = value
        End Set
    End Property



End Class

