


Public Class SessionHandler
    Private Shared _UserName As String = "UserName"

    Public Shared Property UserName() As String
        Get
            If (HttpContext.Current.Session(_UserName) Is Nothing) Then
                Return String.Empty
            Else
                Return HttpContext.Current.Session(_UserName).ToString()
            End If
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session(_UserName) = value
        End Set
    End Property

    Private Shared _cc_cve As String = "cc_cve"

    Public Shared Property cc_cve() As String
        Get
            If (HttpContext.Current.Session(_cc_cve) Is Nothing) Then
                Return String.Empty
            Else
                Return HttpContext.Current.Session(_cc_cve).ToString()
            End If
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session(_cc_cve) = value
        End Set
    End Property


End Class

