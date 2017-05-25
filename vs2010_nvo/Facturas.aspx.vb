Imports System.Data.SqlClient
Imports System.Data
Imports CLPortalAbstract

Partial Public Class Facturas
    Inherits Page
    Private _rfc As String
    Private _Conexion As New Skytex.FacturaElectronica.Datos
    Private _myConnection As SqlConnection = _Conexion.MiConexion

#Region "Metodos y Funciones"
  
    Private Function FNombreProveedor(ByVal RFC As String) As String

        Dim user = CType(Page.Session("user"), PerfilesUser)
        Dim consulta = From con In user.perfil _
                    Select nom_prov = con.proveedor _
                    Distinct
        Dim proveedor As String = ""

        For Each dato In consulta
            proveedor = dato
        Next

        Return proveedor

    End Function


    Private Function obtRFC() As String

        Dim user = CType(Page.Session("user"), PerfilesUser)
        Dim consulta = From con In user.perfil _
                    Select rfc = con.rfc _
                    Distinct


        For Each dato In consulta
            _rfc = dato
        Next

        Return _rfc

    End Function

#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'GCM 06012015 ver si se pinta el acuse o no
        Dim _user = CType(Page.Session("user"), PerfilesUser)
        Dim swfCom = 0
        Dim Fcomp = _user.Perfil.Where(Function(q As DatosUser) q.TipdocCve = "QTFAPN")
        Dim facComp As IEnumerable(Of DatosUser) = If(TryCast(Fcomp, List(Of DatosUser)), Fcomp.ToList())
        'GCM 14012015 para etiquetas
        Dim prov = _user.Perfil.Where(Function(q As DatosUser) q.CcTipo = "P")
        Dim exProv As IEnumerable(Of DatosUser) = If(TryCast(prov, List(Of DatosUser)), prov.ToList())

        If facComp.Any() Then
            LinkAcuse.Visible = True
            Page.Session("Acuse") = 1
        Else
            Page.Session("Acuse") = 0
        End If
        'GCM 06012015 ver si se pinta el acuse o no

        'GCM 06012015 para etiqueta proveedores
        If exProv.Any() Then
            LnkPreEntrega.Visible = True
            Page.Session("Prov") = 1
        Else
            Page.Session("Prov") = 0
        End If

        If Not IsPostBack And Not Page.PreviousPage Is Nothing Then
            Page.Session("RFC") = obtRFC()
            Page.Session("nomProveedor") = ""
        Else
            Page.Session("RFC") = obtRFC()
            If Session("USRswUsrInterno") = 1 Then
                Me.LnkCambiaRFC.Visible = True
                lblUsuarioActivo.Text = "Usuario Actual:" & Session("USRnomUsuario")
                lblUsuarioActivo.Visible = True
                'button2.Visible = True
                'Else
                'button2.Visible = False
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
                    LblProveedor.Text = "Bienvenido " + Trim(FNombreProveedor(Page.Session("RFC").ToString()))

                    If Page.Session("nomProveedor") = "" Then
                        Page.Session("nomProveedor") = ""
                        Page.Session("nomProveedor") = LblProveedor.Text
                    End If
                Else
                    LblProveedor.Text = Page.Session("nomProveedor").ToString()
                End If

                LblProveedor.Attributes("style") = "TEXT-ALIGN: right"
            End If
        End If
    End Sub
 


    Protected Sub LinkAcuse_Click(sender As Object, e As EventArgs) Handles LinkAcuse.Click
        Response.Redirect("AcuseMercancia.aspx")
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
    Protected Sub LnkEtiqueta_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkEtiqueta.Click
        Response.Redirect("Facturas.aspx")
    End Sub
End Class
