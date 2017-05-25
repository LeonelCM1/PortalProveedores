
Imports System.Data.SqlClient
Imports System.Data
Imports CLPortalAbstract


Partial Class CapturaEmb
    Inherits System.Web.UI.Page
    Private dtNomProv As DataTable = Nothing
    Private RFC As String
    Private _Conexion As New Skytex.FacturaElectronica.Datos
    Private MyConnection As SqlConnection = _Conexion.MiConexion
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
                        RFC = UserName1.UserName
                    End If
                End If
            End If
        End If
    End Sub
    Private Function FNombreProveedor(ByVal RFC As String) As String

        Dim user As PerfilesUser = Page.Session("user")
        Dim consulta = From con In user.perfil _
                    Select nom_prov = con.proveedor _
                    Distinct
        Dim proveedor As String = ""

        For Each dato In consulta
            proveedor = dato
        Next

        Return proveedor

    End Function
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack And Not Page.PreviousPage Is Nothing Then
            FObtieneRFC()
            Page.Session("RFC") = RFC
            Page.Session("nomProveedor") = ""
        Else

            If Session("USRswUsrInterno") = 1 Then
                Me.LnkCambiaRFC.Visible = True
                lblUsuarioActivo.Text = "Usuario Actual:" & Session("USRnomUsuario")
                lblUsuarioActivo.Visible = True
                'GCM 06012015 Comentado
                'button2.Visible = True
                'Else
                '  button2.Visible = False
            End If

            'GCM 06012015 activa acuse
            If Session("Acuse") = 1 Then
                LinkAcuse.Visible = True
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
                    LblProveedor.Text = "Bienvenido " + Trim(FNombreProveedor(Page.Session("RFC")))

                    If Page.Session("nomProveedor") = "" Then
                        Page.Session("nomProveedor") = ""
                        Page.Session("nomProveedor") = LblProveedor.Text
                    End If
                Else
                    LblProveedor.Text = Page.Session("nomProveedor")
                End If

                LblProveedor.Attributes("style") = "TEXT-ALIGN: right"
            End If
        End If
    End Sub
    Private Sub LnkFacturas_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LinkFactura.Click
        Response.Redirect("Facturas.aspx")
    End Sub
    Protected Sub LinkAcuse_Click(sender As Object, e As System.EventArgs) Handles LinkAcuse.Click
        Response.Redirect("AcuseMercancia.aspx")
    End Sub

    Protected Sub LnkSeguimiento_Click(sender As Object, e As System.EventArgs) Handles LnkSeguimientoCap.Click
        Response.Redirect("SeguimientoCap.aspx")
    End Sub
    Protected Sub LnkCambiaRFC_Click(sender As Object, e As EventArgs) Handles LnkCambiaRFC.Click
        Response.Redirect("AdministraProveedor.aspx")
    End Sub
    Protected Sub LnkPreEntrega_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LnkPreEntrega.Click
        Response.Redirect("PreEntregaMercancia.aspx")
    End Sub
End Class
