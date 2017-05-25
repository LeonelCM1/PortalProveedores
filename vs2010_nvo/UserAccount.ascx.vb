
Partial Class UserAccount
    Inherits System.Web.UI.UserControl
    ReadOnly _factura As New Skytex.FacturaElectronica.Factura
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (Page.IsPostBack) Then
            'LlenaEntidad()
            LlenaUsuarios()
        End If
        ddlUser.Focus()
    End Sub
    
    Private Sub LlenaUsuarios()
        Dim dtUsr = _factura.ConsultaUsuarios("001")
        'ddlUser.DataSource = Nothing
        ddlUser.DataSource = dtUsr
        ddlUser.DataTextField = "nombre"
        ddlUser.DataValueField = "user_cve"
        ddlUser.DataBind()
        ddlUser.Items(1).Selected = True

    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As System.EventArgs) Handles btnLogin.Click
        Dim objLogin As DevSecurityProvider = New DevSecurityProvider()
        UserName.Text = ddlUser.SelectedValue

        Dim valida As Int16 = _factura.ValidaUsuario("001", ddlUser.SelectedValue, txtPassword.Text)
        If valida = 1 Then
            objLogin.ValidateUserInternal(UserName.Text)
            Web.Security.FormsAuthentication.RedirectFromLoginPage(UserName.Text, False)
            Session("USRnomUsuario") = ddlUser.SelectedItem.Text.ToString()
            Session("USRCve") = ddlUser.SelectedValue.ToString()
            Session("USRswUsrInterno") = 1
            Response.Redirect("AdministraProveedor.aspx", False)
            'Server.Transfer("AdministraProveedor.aspx", False)
        Else
            lblError.Visible = True
            lblError.Text = "Contraseña incorrecta"
        End If
    End Sub


End Class
