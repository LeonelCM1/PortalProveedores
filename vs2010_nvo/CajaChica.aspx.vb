
Partial Class CajaChica
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        lblUsuarioActivo.Text = "Usuario Actual:" & Session("USRnomUsuario")
        lblUsuarioActivo.Visible = True
        LnkRetornaMenuOpc.Visible = True

        Page.Session("RFC") = ""

        If Session("USRswUsrInterno") = 1 And Session("USRCve") = "" Then
            'aqui el rfc debe ir vacio por que van a procesar varios archivos de diferentes proveedores'esta variable debe mantenerse vacia 
            Response.Redirect("UserLogin.aspx")
            Exit Sub
        End If

    End Sub

    Protected Sub LnkRetornaMenuOpc_Click(sender As Object, e As System.EventArgs) Handles LnkRetornaMenuOpc.Click
        Response.Redirect("AdministraProveedor.aspx")
    End Sub
    Protected Sub LnkSegCajaChica_Click(sender As Object, e As System.EventArgs) Handles LnkSegCajaChica.Click
        Response.Redirect("SeguimientoCaja.aspx")
    End Sub

End Class
