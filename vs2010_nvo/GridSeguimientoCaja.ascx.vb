
Imports CLPortalAbstract
Imports System.Data
Imports Skytex.FacturaElectronica

Partial Class GridSeguimientoCaja
    Inherits System.Web.UI.UserControl


#Region "Variables"
    ReadOnly _facturaBis As New Skytex.FacturaElectronica.FacturaBis
    'Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    'Private ReadOnly _myConnection As SqlConnection = _conexion.MiConexion
    Dim _rutaTmp As String = ""
    Dim _rutaOrig As String = ""
    Dim _decimalesTruncados As Integer = 0
    Dim _decimales As Integer = 0
    Dim _ccCveLoc As String = ""
    Dim _ccTipoLoc As String = ""
    Dim _user As PerfilesUser

#End Region

    Private Sub LoadConfig()
        Dim config = CType(Page.Session("config"), List(Of ConfigGral))
        _rutaOrig = _facturaBis.GetConfigure(config, "Ruta").ToString()
        _rutaTmp = _facturaBis.GetConfigure(config, "RutaTMP").ToString()
        _decimales = CType(_facturaBis.GetConfigure(config, "decimales"), Integer)
        _decimalesTruncados = CType(_facturaBis.GetConfigure(config, "decimalTrun"), Integer)
    End Sub
    Protected Sub GridView1_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles GridView1.SelectedIndexChanged

    End Sub
    Protected Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        ConsultaFolios()
    End Sub
    Protected Sub GridView1_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim gridView = DirectCast(sender, GridView)
            Dim header = DirectCast(gridView.Controls(0).Controls(0), GridViewRow)

            header.Cells(10).Visible = False
            header.Cells(11).ColumnSpan = 2
            header.Cells(11).Text = "Archivos"
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub PresionaBoton(ByVal sender As Object, ByVal e As EventArgs) Handles btnConsulta.Click
        ConsultaFolios()
    End Sub
    'Protected Sub ddlFacturas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFacturas.SelectedIndexChanged
    '    ConsultaFolios()
    'End Sub
    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Dim errores As New List(Of Errores)
        Dim swOpc As Int16 = 1
        LoadConfig()

        Dim opcionGrid As String = e.CommandName

        If opcionGrid = "ImpXml" Or opcionGrid = "ImpPdf" Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim folioFac As TableCell = selectedRow.Cells(2)
            Dim serie As TableCell = selectedRow.Cells(3)
            Dim uuid As TableCell = selectedRow.Cells(4)
            Dim folioCfd As String = folioFac.Text
            Dim serieCfd As String = serie.Text
            Dim uuidCfd As String = uuid.Text
            If uuid.Text = "&nbsp;" Then
                uuidCfd = " "
            End If
            Dim dt As DataTable = CType(Page.Session("tmp"), DataTable)
            Dim rfc = dt.Rows(index).Item("rfc_fin").ToString()
            'Page.Session("tmp") = Nothing
            Dim docto As String
            Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
            _facturaBis.ObtArchivo(rfc, serieCfd.Trim(), folioCfd, uuidCfd, contra, 5, errores)
            Dim qry = From datos In contra _
            Select datos.nom_arch, datos.num_fol
            For Each dat In qry
                Dim folioContrarecibo As String = dat.num_fol.ToString
                docto = dat.nom_arch
                Dim rutaTmpPaso As String
                Dim ext As String = ""
                If folioContrarecibo = 0 Then
                    rutaTmpPaso = _rutaTmp
                Else
                    rutaTmpPaso = _rutaOrig
                End If

                If e.CommandName = "ImpXml" Then
                    ext = ".xml"
                End If
                If e.CommandName = "ImpPdf" Then
                    ext = ".pdf"
                End If
                Page.Session("archivo") = docto.Trim() + ext.Trim()
                Page.Session("ruta") = rutaTmpPaso
                Response.Redirect("ArchivoDownload.ashx")
            Next
        End If
        'opcion sw 2
        If opcionGrid = "Imprimir" Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim opcion As TableCell = selectedRow.Cells(6)
            Dim linkbtn As LinkButton = DirectCast(opcion.Controls(0), LinkButton)
            linkbtn.Enabled = False

        End If


        If swOpc = 1 And opcionGrid = "Imprimir" Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim folioFac As TableCell = selectedRow.Cells(2)
            Dim serie As TableCell = selectedRow.Cells(3)
            Dim uuid As TableCell = selectedRow.Cells(4)
            Dim folioCfd As String = folioFac.Text
            Dim serieCfd As String = serie.Text
            Dim uuidCfd As String = uuid.Text
            If uuid.Text = "&nbsp;" Then
                uuidCfd = " "
            End If

            Dim folioContrarecibo As String
            Dim docto As String
            Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
            _facturaBis.ObtieneDatos(Page.Session("RFC").ToString(), serieCfd, folioCfd, uuidCfd, contra, 1, errores)
            Dim qry = From datos In contra _
            Select datos.num_fol, datos.tipo_doc, datos.ef_cve
            For Each dat In qry
                folioContrarecibo = dat.num_fol.ToString
                docto = dat.tipo_doc
                Dim efCveReceptor As String = dat.ef_cve
                Page.Session("ef_cve_imp") = efCveReceptor
                Page.Session("num_fol_imp") = folioContrarecibo
                Page.Session("tipdoc_imp") = docto
                Response.Redirect("Reportes.aspx")
            Next
        End If


    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LlenaCboRfc()
            LlenaCboCtaCBan()
            LlenaGrid("", "", 0, "", "")
        End If
    End Sub

    Private Sub ConsultaFolios()

        Dim rfc As String = ""
        Dim serie As String = ""
        Dim folio As Integer = 0
        Dim uuid As String = ""
        Dim cadenaCons As String = ""
        Dim ctaCble As String = ""

        If ddlCtaCble.SelectedValue <> "NA" Then 'And ddlFacturas.SelectedIndex > -1 Then
            ctaCble = ddlCtaCble.SelectedValue
            cadenaCons = cadenaCons + ctaCble
        End If

       
        If ddlFacturas.SelectedValue <> "NA" Then 'And ddlFacturas.SelectedIndex > -1 Then
            Dim index = ddlFacturas.SelectedValue.IndexOf("@", System.StringComparison.Ordinal)
            rfc = ddlFacturas.SelectedValue.Substring(0, index)
            cadenaCons = cadenaCons + rfc
        End If

        If txtSerie.Text <> "" Then
            serie = txtSerie.Text.Trim()
            cadenaCons = cadenaCons + serie
        End If

        If txtFolio.Text <> "" Then
            folio = Integer.Parse(txtFolio.Text.Trim())
            cadenaCons = cadenaCons + txtFolio.Text.Trim()
        End If

        If txtuuid.Text <> "" Then
            uuid = txtuuid.Text.Trim()
            cadenaCons = cadenaCons + uuid
        End If

        If Page.Session("consulta") <> cadenaCons Then
            Page.Session("consulta") = cadenaCons
            LlenaGrid(rfc, serie, folio, uuid, ctaCble)
        End If




    End Sub

    Private Sub LlenaGrid(ByVal rfc As String,
                          ByVal serie As String,
                          ByVal folio As Integer,
                          ByVal uuid As String,
                          ByVal ctaCble As String)
        Dim swPantalla As Int16 = 0
        Dim tipoDoc As String = "BTCOM"
        Dim idSoludin As String = Page.Session("USRCve").ToString()

        Dim datos = _facturaBis.ConsultaFoliosSeguimientoCaja(rfc, serie, folio, uuid, swPantalla, tipoDoc, idSoludin, ctaCble)
        Page.Session("tmp") = datos
        GridView1.DataSource = datos
        GridView1.DataBind()
    End Sub

    Private Sub LlenaCboRfc()
        Dim datosCombo As New combos
        datosCombo.combo = "RfcProvXTipo"
        datosCombo.rfc = "?"
        datosCombo.parametro1 = "?"
        datosCombo.parametro2 = "G"

        Dim dt = _facturaBis.LlenaCbo(datosCombo)

        ddlFacturas.DataSource = Nothing
        ddlFacturas.DataSource = dt
        ddlFacturas.DataTextField = "descripcion"
        ddlFacturas.DataValueField = "valor"
        ddlFacturas.DataBind()
        ddlFacturas.Items.Insert(0, New ListItem("Seleccione Opcion", "NA"))
        'ddlFacturas.Items(0).Selected = True
    End Sub

    Private Sub LlenaCboCtaCBan()
        Dim datosCombo As New combos
        datosCombo.combo = "CtaBanc"
        datosCombo.rfc = "?"
        datosCombo.parametro1 = "?"
        datosCombo.parametro2 = "?"

        Dim dt = _facturaBis.LlenaCbo(datosCombo)

        ddlCtaCble.DataSource = Nothing
        ddlCtaCble.DataSource = dt
        ddlCtaCble.DataTextField = "descripcion"
        ddlCtaCble.DataValueField = "valor"
        ddlCtaCble.DataBind()
        ddlCtaCble.Items.Insert(0, New ListItem("Seleccione Opcion", "NA"))
        'ddlCtaCble.Items(0).Selected = True
    End Sub

End Class
