Imports CLPortalAbstract
Partial Class Login12
    Inherits UserControl
    Public ObjSecurity As CLPortalUsersBussiness.BLSecurity = New CLPortalUsersBussiness.BLSecurity

    Protected Sub Login1_Authenticate(ByVal sender As Object, ByVal e As AuthenticateEventArgs) Handles Login1.Authenticate
        Dim objLogin As DevSecurityProvider = New DevSecurityProvider()
        Dim user As PerfilesUser = Nothing
        Dim config As List(Of ConfigGral)
        user = objLogin.ValidateUser(Login1.UserName.Trim(), Login1.Password.Trim(), user)

        config = ObjSecurity.ObtConfig()
        Page.Session("config") = config
        Page.Session("password") = Login1.Password.Trim() 'GCM 14012015 para etiquetas
        If user.perfil.Any() Then
            e.Authenticated = True
        End If

        If e.Authenticated Then
            Web.Security.FormsAuthentication.RedirectFromLoginPage(Login1.UserName, False)
            Session("USRswUsrInterno") = 0 'sw apagado cuando es proveedor
            Page.Session("user") = user
            Response.Redirect("Facturas.aspx", False)
            Server.Transfer("Facturas.aspx", False)

        Else
            lblError.Visible = True
            lblError.Text = "Contraseña incorrecta"
            Exit Sub
        End If
    End Sub



End Class


