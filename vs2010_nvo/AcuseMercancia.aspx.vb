Imports System.Data.SqlClient
Imports System.Data
Imports CLPortalAbstract


Partial Class AcuseMercancia
    Inherits Page
    Private _dtNomProv As DataTable = Nothing
    Private _rfc As String
    Private _Conexion As New Skytex.FacturaElectronica.Datos
    Private _myConnection As SqlConnection = _Conexion.MiConexion
    Private DS As New DataSet()


#Region "Metodos y Funciones"
    Private Sub FObtieneRFC()
        If Not Page.PreviousPage Is Nothing Then
            Dim CP As ContentPlaceHolder = CType(PreviousPage.Master.FindControl("_mainContent"), ContentPlaceHolder)
            If Not CP Is Nothing Then
                Dim LoginControl As UserControl = CType(CP.FindControl("ctlLogin1"), UserControl)
                If Not LoginControl Is Nothing Then
                    Dim UserName1 As System.Web.UI.WebControls.Login
                    UserName1 = LoginControl.FindControl("Login1")
                    If Not UserName1 Is Nothing Then
                        _rfc = UserName1.UserName
                    End If
                End If
            End If
        End If
    End Sub

    Private Function FNombreProveedor(ByVal rfc As String) As String

        Dim user As PerfilesUser = CType(Page.Session("user"), PerfilesUser)
        Dim consulta = From con In user.Perfil _
                    Select nomProv = con.Proveedor _
                    Distinct
        Dim proveedor As String = ""

        For Each dato In consulta
            proveedor = dato
        Next

        Return proveedor

    End Function
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'Response.Headers("Cache-Control") = "no-cache, no-store, must-revalidate" '// HTTP 1.1.
        'Response.Headers("Pragma") = "no-cache" '// HTTP 1.0.
        'Response.Headers("Expires") = "0" '// Proxies.
        If Not IsPostBack And Not Page.PreviousPage Is Nothing Then
            FObtieneRfc()
            Page.Session("RFC") = _rfc
            Page.Session("nomProveedor") = ""
        Else
            If Session("USRswUsrInterno") = 1 Then
                Me.LnkCambiaRFC.Visible = True
                lblUsuarioActivo.Text = "Usuario Actual:" & Session("USRnomUsuario")
                lblUsuarioActivo.Visible = True
                'GCM 072012015 Comentado
                'button2.Visible = True
                'Else
                'button2.Visible = False
            End If

            'GCM 19012015 muestra acuse
            If Session("Prov") = 1 Then
                LnkPreEntrega.Visible = True
            End If

            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 0 Then
                Response.Redirect("Login.aspx")
                Exit Sub
            End If

            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 1 Then
                Response.Redirect("UserLogin.aspx")
                Exit Sub
            End If

            If Not IsPostBack Then
                If Page.Session("nomProveedor") = "" Then
                    LblProveedor.Text = Trim(FNombreProveedor(Page.Session("RFC").ToString()))

                    If Page.Session("nomProveedor") = "" Then
                        Page.Session("nomProveedor") = LblProveedor.Text
                    End If
                Else
                    LblProveedor.Text = Page.Session("nomProveedor").ToString()
                End If

                LblProveedor.Attributes("style") = "TEXT-ALIGN: right"
            End If
        End If
    End Sub

   

    Protected Sub LinkFacturas_Click(sender As Object, e As EventArgs) Handles LinkFacturas.Click
        Response.Redirect("Facturas.aspx")
    End Sub

    Protected Sub LnkSeguimiento_Click(sender As Object, e As EventArgs) Handles LnkSeguimiento.Click
        Response.Redirect("SeguimientoCap.aspx")
    End Sub

    Protected Sub LnkCambiaRFC_Click(sender As Object, e As EventArgs) Handles LnkCambiaRFC.Click
        Response.Redirect("AdministraProveedor.aspx")
    End Sub
    Protected Sub LnkPreEntrega_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LnkPreEntrega.Click
        Response.Redirect("PreEntregaMercancia.aspx")
    End Sub
End Class
