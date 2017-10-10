Imports System.Activities.Expressions
Imports Skytex.FacturaElectronica
Imports System.Data.SqlClient
Imports System.Data
Imports CLPortalAbstract

Partial Class UserControlsGridSeguimientoCap
    Inherits UserControl
    ReadOnly _factura As New Skytex.FacturaElectronica.Factura
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Private ReadOnly _myConnection As SqlConnection = _conexion.MiConexion
    Dim _rutaTmp As String = ""
    Dim _rutaOrig As String = ""
    Dim _decimalesTruncados As Integer = 0
    Dim _decimales As Integer = 0
    Dim _dtResultados As DataTable
    Dim _ccCveLoc As String = ""
    Dim _ccTipoLoc As String = ""
    Dim _user As PerfilesUser = Nothing
    Dim _rfc As String = ""

#Region "Metodos y Funciones"
    Private Sub ConsultaFolios()
        'If Not IsPostBack Then
        Dim myDataAdapter = New SqlDataAdapter("sp_consCFDxml", _myConnection)
        Dim ds As New DataSet()

        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()

        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", lblRFC.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", txtSerie.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", txtFolio.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", txtuuid.Text)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@tipo_doc", ddlFacturas.SelectedValue)
        Try
            myDataAdapter.Fill(ds, "sp_consCFDxml")
            myDataAdapter.Dispose()
            _myConnection.Close()

            _dtResultados = ds.Tables.Item(0)


            GridView1.DataSource = _dtResultados
            GridView1.DataBind()

            'ScriptManager.RegisterStartupScript(Page, GetType(Page), "Key", "<script>MakeStaticHeader('" + GridView1.ClientID + "', 250, 850 , 40 ,true); </script>", False)

        Catch ex As Exception
            lblErr.Text = "sp_consCFDxml"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
        'End If
    End Sub
    Private Sub ObtArchivo(ByVal rfc As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, _
                     ByVal contra As List(Of Skytex.FacturaElectronica.nuevas_facturas), ByVal numInfo As Integer, ByVal errores As List(Of Errores))
        Dim recibo As New Skytex.FacturaElectronica.nuevas_facturas
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim ds As New DataSet()
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)

        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tblInfo As DataTable
            tblInfo = ds.Tables.Item(ds.Tables.Count - 1)
            Dim iError As Integer = Int16.Parse(tblInfo.Rows(0).Item("error").ToString())
            Dim msg = tblInfo.Rows(0).Item("msg").ToString.Trim
            If iError = 0 Then
                If (Boolean.Parse(tblInfo.Rows(0).Item("sw_acept").ToString())) = True Then
                    recibo.num_fol = 1
                Else
                    recibo.num_fol = 0
                End If
                recibo.nom_arch = tblInfo.Rows(0).Item("nom_arch").ToString.Trim
                contra.Add(recibo)
            End If
            If iError <> 0 Then
                _factura.agrega_err(iError, msg, errores, "")
            End If
        Catch ex As Exception
            lblErr.Text = "sp_consInfXML"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
    End Sub
    Private Sub ObtieneDatos(ByVal rfc As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, _
                     ByVal contra As List(Of Skytex.FacturaElectronica.nuevas_facturas), ByVal numInfo As Integer, ByVal errores As List(Of Errores))
        Dim recibo As New Skytex.FacturaElectronica.nuevas_facturas
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim ds As New DataSet()
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)
        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tablaInfo As DataTable
            tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)
            Dim iError As Integer = CType(tablaInfo.Rows(0).Item("error").ToString.Trim, Integer) 'Int(row("error"))
            Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim 'CStr(row("msg"))
            If iError = 0 Then
                recibo.num_fol = CType(tablaInfo.Rows(0).Item("num_fol_contra"), Integer) 'CStr(row("num_fol_contra"))
                recibo.ef_cve = tablaInfo.Rows(0).Item("ef_cve").ToString.Trim 'CStr(row("ef_cve"))
                recibo.tipo_doc = tablaInfo.Rows(0).Item("tipo_doc_contra").ToString.Trim 'CStr(row("tipo_doc_contra"))
                recibo.nom_arch = tablaInfo.Rows(0).Item("nom_arch").ToString.Trim
                contra.Add(recibo)
            End If
            If iError <> 0 Then
                _factura.agrega_err(iError, msg, errores)
            End If
        Catch ex As Exception
            lblErr.Text = "sp_consInfXML"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
    End Sub
    Private Sub ObtieneDatos(ByVal rfc As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, _
                     ByVal user As List(Of DatosUser), ByVal numInfo As Integer, ByVal errores As List(Of Errores))
        Dim usr As New DatosUser
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim ds As New DataSet()
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)
        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tablaInfo As DataTable
            tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)
            Dim iError As Integer = CType(tablaInfo.Rows(0).Item("error").ToString.Trim, Integer) 'Int(row("error"))
            Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim 'CStr(row("msg"))
            If iError = 0 Then
                usr.NomArch = tablaInfo.Rows(0).Item("nom_arch").ToString.Trim
                usr.TipdocCve = tablaInfo.Rows(0).Item("tipo_doc").ToString.Trim
                usr.CcTipo = tablaInfo.Rows(0).Item("cc_tipo").ToString.Trim
                usr.CcCve = tablaInfo.Rows(0).Item("cc_cve").ToString.Trim
                usr.NomDoc = tablaInfo.Rows(0).Item("ef_cve").ToString.Trim
                user.Add(usr)
            End If
            If iError <> 0 Then
                _factura.agrega_err(iError, msg, errores)
            End If
        Catch ex As Exception
            lblErr.Text = "sp_consInfXML"
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
    End Sub
    Private Sub ObtMails(ByVal rfc As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, _
                    ByVal contra As List(Of Skytex.FacturaElectronica.nuevas_facturas), ByVal numInfo As Integer, ByVal errores As List(Of Errores))
        Dim recibo As New Skytex.FacturaElectronica.nuevas_facturas
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim ds As New DataSet()
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)
        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tablaInfo As DataTable
            tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)
            Dim iError As Integer = Int16.Parse(tablaInfo.Rows(0).Item("error").ToString())
            Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim
            If iError = 0 Then
                recibo.tipo_doc = tablaInfo.Rows(0).Item("emails").ToString.Trim
                recibo.nom_arch = tablaInfo.Rows(0).Item("nom_arch").ToString.Trim
                contra.Add(recibo)
            End If
            If iError <> 0 Then
                _factura.agrega_err(iError, msg, errores)
            End If
        Catch ex As Exception
            lblErr.Text = "sp_consInfXML " + " num_info;" + numInfo.ToString()
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
    End Sub
    Private Sub ObtenDetalle(ByVal rfcPaso As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, _
                  ByVal items As List(Of lineitem), ByVal numInfo As Integer, ByVal errores As List(Of Errores), ByVal adicional As List(Of String))
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim ds As New DataSet()
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        'aqui mandamos la clave numInfo 7
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfcPaso)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)
        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tablaInfo As DataTable
            tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)
            Dim iError As Integer = Int16.Parse(tablaInfo.Rows(0).Item("error").ToString())
            Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim
            If iError = 0 Then
                For i As Integer = 0 To tablaInfo.Rows().Count - 1
                    Dim itmPaso As New lineitem
                    adicional.Add(tablaInfo.Rows(i).Item("rfc_receptor").ToString().Trim())
                    adicional.Add(tablaInfo.Rows(i).Item("nom2").ToString().Trim())
                    itmPaso.partida = Int16.Parse(tablaInfo.Rows(i).Item("num_rengp").ToString().Trim())
                    itmPaso.reference_identification = tablaInfo.Rows(i).Item("recepcion").ToString().Trim()
                    itmPaso.OrdCompra = tablaInfo.Rows(i).Item("ordcompra").ToString().Trim()
                    itmPaso.sku = tablaInfo.Rows(i).Item("sku_cve").ToString.Trim
                    itmPaso.art_tip = tablaInfo.Rows(i).Item("art_tip").ToString.Trim
                    itmPaso.uns = Decimal.Parse(tablaInfo.Rows(i).Item("uns").ToString.Trim())
                    itmPaso.precio = Decimal.Parse(tablaInfo.Rows(i).Item("precio").ToString.Trim())
                    itmPaso.pct_decuento = Decimal.Parse(tablaInfo.Rows(i).Item("pct_descto").ToString.Trim())
                    itmPaso.type = Int16.Parse(tablaInfo.Rows(i).Item("tipo_reng").ToString.Trim())
                    items.Add(itmPaso)
                Next
            End If
            If iError <> 0 Then
                _factura.agrega_err(iError, msg, errores)
            End If
        Catch ex As Exception
            lblErr.Text = "sp_consInfXML " + " num_info:" + numInfo.ToString()
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
    End Sub
    Private Function ObtErrBuzon(ByVal rfcEmisor As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, ByVal errores As List(Of Errores)) As String
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim ds As New DataSet()
        Dim errCadena As String = ""
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        'aqui mandamos la clave numInfo 8 para obeter el error del renglonque fallo
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfcEmisor.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", folio)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", 9) '9
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid.Trim())
        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tablaInfo As DataTable
            tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)
            Dim iError As Integer = Int16.Parse(tablaInfo.Rows(0).Item("error").ToString())
            Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim
            If iError = 0 Then
                For i As Integer = 0 To tablaInfo.Rows().Count
                    errCadena = tablaInfo.Rows(i).Item("mensaje").ToString().Trim()
                Next
            End If
            If iError <> 0 Then
                _factura.agrega_err(iError, msg, errores)
            End If
        Catch ex As Exception
            lblErr.Text = "sp_consInfXML num_info: 9 "
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try

        Return errCadena
    End Function
    Private Function LlamaConcepto(ByVal skuCve As String, ByVal artTip As String, ByVal numInfo As Integer, ByVal errores As List(Of Errores)) As String
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", _myConnection)
        Dim ds As New DataSet()
        Dim concepto As String = ""
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        'aqui mandamos la clave numInfo 8 para saber el nombre del cargo 
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", skuCve.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", 0)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", artTip.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo) ' 8
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", "?")
        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            _myConnection.Close()
            Dim tablaInfo As DataTable
            tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)
            Dim iError As Integer = Int16.Parse(tablaInfo.Rows(0).Item("error").ToString())
            Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim
            If iError = 0 Then
                For i As Integer = 0 To tablaInfo.Rows().Count
                    concepto = tablaInfo.Rows(i).Item("concepto").ToString().Trim()
                Next
            End If
            If iError <> 0 Then
                _factura.agrega_err(iError, msg, errores)
            End If
        Catch ex As Exception
            lblErr.Text = "sp_consInfXML " + " num_info:" + numInfo.ToString()
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
        Return concepto
    End Function
    Private Sub MovStatusReclamacion(ByVal rfc As String, ByVal serie As String, ByVal folio As String, ByVal uuid As String, _
                   ByVal status As Integer, ByVal errores As List(Of Errores))
        Dim myDataAdapter = New SqlDataAdapter("sp_gnews_xml", _myConnection)
        Dim ds As New DataSet()
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@cve_status", status)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@sw_si_no", 1)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@sw_terminado", 0)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@id", "WEB")
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)
        Try
            myDataAdapter.Fill(ds, "sp_gnews_xml")
            myDataAdapter.Dispose()
            _myConnection.Close()
        Catch ex As Exception
            Dim er = New Errores()
            er.Interror = 3
            er.Message = ex.Message.Trim()
            errores.Add(er)
            lblErr.Text = "Error al consultar sp_gnews_xml " + rfc + "@" + folio + "@" + serie + "@" + uuid
            _factura.agrega_err(3, ex.Message.Trim(), errores)
        Finally
            If _myConnection.State = ConnectionState.Open Then
                _myConnection.Close()
            End If
        End Try
    End Sub
    Private Sub LlenaCboFacturas()
        ddlFacturas.Items.AddRange((From a In _user.Perfil Select New ListItem(a.NomDoc, a.TipdocCve)).ToArray())
        ddlFacturas.Items(0).Selected = True
        ConsultaFolios()
    End Sub
#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblRFC.Text = Page.Session("RFC").ToString()
            ConsultaFolios()
        Else
            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 0 Then
                Response.Redirect("Login.aspx")
                Exit Sub
            End If

            If Page.Session("RFC") = "" And Session("USRswUsrInterno") = 1 Then
                Response.Redirect("UserLogin.aspx")
                Exit Sub
            End If
        End If
        If Not (Page.IsPostBack) Then
            If IsNothing(_user) Then
                _user = DirectCast(Page.Session("user"), PerfilesUser)
            End If
            LlenaCboFacturas()
            Const indice As Integer = 10000
            Page.Session("indice") = indice
        End If
    End Sub
    Private Sub LoadConfig()
        Dim config = CType(Page.Session("config"), List(Of ConfigGral))
        _rutaOrig = _factura.GetConfigure(config, "Ruta").ToString()
        _rutaTmp = _factura.GetConfigure(config, "RutaTMP").ToString()
        _decimales = CType(_factura.GetConfigure(config, "decimales"), Integer)
        _decimalesTruncados = CType(_factura.GetConfigure(config, "decimalTrun"), Integer)
    End Sub
    Protected Sub ddlFacturas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFacturas.SelectedIndexChanged
        ConsultaFolios()
    End Sub
    Protected Sub btnCons_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCons.Click
        ConsultaFolios()
    End Sub
    Protected Sub GridView1_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim gridView = DirectCast(sender, GridView)
            Dim header = DirectCast(gridView.Controls(0).Controls(0), GridViewRow)

            GridView1.UseAccessibleHeader = True
            GridView1.HeaderRow.TableSection = TableRowSection.TableHeader

            header.Cells(8).Visible = False
            header.Cells(9).ColumnSpan = 2
            header.Cells(9).Text = "Archivos"
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Dim errores As New List(Of Errores)
        Dim swOpc As Int16 = 1
        LoadConfig()

        Dim opcionGrid As String = e.CommandName
        'opcionGrid = "Captura" ''Para depurar quitar cuando este en produccicon

        If opcionGrid = "ImpXml" Or opcionGrid = "ImpPdf" Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim folioFac As TableCell = selectedRow.Cells(0)
            Dim serie As TableCell = selectedRow.Cells(1)
            Dim uuid As TableCell = selectedRow.Cells(2)
            Dim folioCfd As String = folioFac.Text
            Dim serieCfd As String = serie.Text
            Dim uuidCfd As String = uuid.Text
            If uuid.Text = "&nbsp;" Then
                uuidCfd = " "
            End If

            Dim docto As String
            Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
            ObtArchivo(Page.Session("RFC").ToString(), serieCfd, folioCfd, uuidCfd, contra, 5, errores)
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
            Dim opcion As TableCell = selectedRow.Cells(5)
            Dim linkbtn As LinkButton = DirectCast(opcion.Controls(0), LinkButton)
            linkbtn.Enabled = False
            If linkbtn.Text.ToString().Trim() = "Buzón de Reclamación" Or _
               linkbtn.Text.ToString().Trim() = "Buz&oacute;n de Reclamaci&oacute;n" Or _
                linkbtn.Text.ToString().Trim() = "Buz&#243;n de Reclamaci&#243;n" Then
                swOpc = 2
            End If
        End If

        If swOpc = 2 Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim folioFac As TableCell = selectedRow.Cells(0)
            Dim serie As TableCell = selectedRow.Cells(1)
            Dim uuid As TableCell = selectedRow.Cells(2)
            Dim folioCfd As String = folioFac.Text
            Dim serieCfd As String = serie.Text
            Dim uuidCfd As String = uuid.Text
            If uuid.Text = "&nbsp;" Then
                uuidCfd = " "
            End If
            Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
            Dim indice As Integer = CType(Page.Session("indice"), Integer)

            If indice = e.CommandArgument Then
                Exit Sub
            Else
                Page.Session("indice") = Integer.Parse(e.CommandArgument.ToString())
            End If

            ObtMails(Page.Session("RFC").ToString(), serieCfd, folioCfd, uuidCfd, contra, 6, errores)

            If errores.Count = 0 Then

                Dim qry = From datos In contra _
                    Select datos

                Dim sEmail As String = ""
                Dim sNomArch As String = ""
                For Each dat In qry
                    sNomArch = dat.nom_arch
                    sEmail = dat.tipo_doc
                Next
                Const sSubject As String = "Comprobante recibido en proceso de reclamación"
                Dim nombreProv = Page.Session("nomProveedor").ToString()
                Const sEnc As String = "<!DOCTYPE HTML><meta http-equiv='Content-Type' content='text/html; charset=ISO-8859-1'> <style type='text/css'> body { font-family:Calibri, sans-serif;font-size:14px;} </style> <head></head><body> <table><tr><td style:'width:800px;' >"
                Dim sBody As String = sEnc + "El comprobante adjunto entró a proceso de reclamación por solicitud del proveedor " + "</br>" + _
                    "Proveedor: " + nombreProv.Substring(10, nombreProv.IndexOf("-", StringComparison.Ordinal) - 11).Trim() + "</br>" + _
                    "Folio: " + folioCfd.Trim() + "</br>" + _
                    "Serie: " + serieCfd.Trim() + "</br></br>" + _
                    "Conserve la siguiente llave CFD para poder confirmar la factura en soludin: " + Page.Session("RFC").ToString().Trim() + "@" + serieCfd.Trim() + "@" + folioCfd.Trim() + "@" + uuidCfd.Trim() + "</br></br>" '+ vbNewLine + vbNewLine
                ' Manda mail de aviso
                Dim sAttach As New List(Of String)
                sAttach.Add(_rutaTmp + sNomArch + ".pdf")
                sAttach.Add(_rutaTmp + sNomArch + ".xml")
                Dim adicional As New List(Of String)
                Dim items As New List(Of lineitem)
                'errores = LeeDetalles(_rutaTmp + sNomArch + ".xml", adicional, items)
                'If errores.Count > 0 Or items.Count = 0 Then
                ' antes se leia desde el comprobante cuando era con addenda ahora siempre se consulta de la base de datos por la oc
                ObtenDetalle(Page.Session("RFC").ToString(), serieCfd, folioCfd, uuidCfd, items, 7, errores, adicional)
                If items.Count > 0 Then
                    errores.Clear()
                End If
                'End If

                If items.Count = 0 Then
                    Exit Sub
                End If

                Dim artTmp1 = From con In items
                             Select con
                             Where con.type = 1
                Dim artTmp2 = From con In items
                            Select con
                            Where con.type = 2
                Dim nombreReceptor As String = ""

                Dim arreglo = adicional.ToArray()
                If arreglo.Length() > 0 Then
                    nombreReceptor = arreglo(1)
                End If
                If errores.Count = 0 Then
                    Dim mensaje = ObtErrBuzon(Page.Session("RFC").ToString(), serieCfd, folioCfd, uuidCfd, errores)
                    sBody = sBody + "Motivo del error: " + "</br>"
                    sBody = sBody + mensaje.Trim() + "</br></br>"
                End If

                sBody = sBody + "Referencia:" + "</br>" ' + vbNewLine
                sBody = sBody + "Entidad Financiera: (" + nombreReceptor + ")" + "</br></br>" ' + vbNewLine + vbNewLine
                Dim constTabla As String = ""
                Dim swOrden As Boolean = False
                If errores.Count <> 0 Then
                    For Each item As Errores In errores
                        constTabla = item.Message
                    Next
                Else
                    constTabla = "</td></br> "

                    If artTmp1.Any() = True Then
                        constTabla = constTabla + " <table><tr> "
                        constTabla = constTabla + "<th style='width:20px; text-align:left;'>Reng</th>"
                        constTabla = constTabla + "<th style='width:150px;text-align:left;'>Referencia</th>"

                        Dim vTiOc = From tmp In artTmp1.ToArray()
                                    Where tmp.OrdCompra <> Nothing
                                    Select tmp.OrdCompra
                        If vTiOc.Cast(Of Object)().Any() Then
                            constTabla = constTabla + "<th style='width:150px;text-align:left;'>Orden de Compra</th>"
                            swOrden = True
                        End If
                        'Exit Sub
                        constTabla = constTabla + "<th style='width:100px;text-align:left;'>SKU</th>"
                        constTabla = constTabla + "<th style='width:20px;text-align:left;'>Art_tip</th>"
                        constTabla = constTabla + "<th style='width:100px;text-align:left;'>Cantidad</th>"
                        constTabla = constTabla + "<th style='width:100px;text-align:left;'>Precio</th>"
                        constTabla = constTabla + "<th style='width:100px;text-align:left;'>%Desc</th>"
                        constTabla = constTabla + "</tr>"
                        'Da formato al correo
                        For Each itm As lineitem In From itm1 In artTmp1 Where itm1.type = 1
                            constTabla = constTabla + "<tr>"
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.partida.ToString() + "</td>"
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.reference_identification.Trim() + "</td>"
                            If swOrden = True Then
                                constTabla = constTabla + "<td style='width:200px;'>" + itm.OrdCompra.Trim() + "</td>"
                            End If
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.sku.Trim() + "</td>"
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.art_tip.Trim() + "</td>"
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.uns.ToString().Trim() + "</td>"
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.precio.ToString().Trim() + "</td>"
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.pct_decuento.ToString().Trim() + "</td>"
                            constTabla = constTabla + "</tr>"
                        Next
                        constTabla = constTabla + "</table> "
                    End If
                    If artTmp2.Any() = True Then
                        constTabla = constTabla + " </br></br>Cargo Extra: </br>"
                        constTabla = constTabla + " <table><tr> "
                        constTabla = constTabla + "<th style='width:20px; text-align:left;'>Clave</th>"
                        constTabla = constTabla + "<th style='width:150px;text-align:left;'>Concepto</th>"
                        constTabla = constTabla + "<th style='width:100px;text-align:left;'>Importe</th>"
                        For Each itm As lineitem In From itm1 In artTmp2 Where itm1.type = 2
                            constTabla = constTabla + "<tr> "
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.sku.Trim() + "</td>"
                            constTabla = constTabla + "<td style='width:200px;'>" + LlamaConcepto(itm.sku.Trim(), itm.art_tip.Trim(), 8, errores) + "</td>"
                            constTabla = constTabla + "<td style='width:200px;'>" + itm.precio.ToString().Trim() + "</td>"
                            constTabla = constTabla + "</tr> "
                        Next
                        constTabla = constTabla + "</table>"
                    End If
                    constTabla = constTabla + " </tr></table>"
                    constTabla = constTabla + "</br>Nota. Verifique que la cantidad y/o precio de todos los renglones sea correcto</br></br>"
                    constTabla = constTabla + "</body></HTML> "
                End If

                sBody = sBody + constTabla
                'GCM Comentar para pruebas de envio de mail
                MovStatusReclamacion(Page.Session("RFC").ToString(), serieCfd, folioCfd, uuidCfd, 9, errores)
                If errores.Count = 0 Then
                    _factura.MailGeneric(sSubject, sBody, sEmail, sAttach)
                    'GCM envio solo a mi correo
                    '_factura.MailGeneric(sSubject, sBody, "geovana.contreras@skytex.com.mx", sAttach)
                End If
                ConsultaFolios()
            End If
        End If

        If swOpc = 1 And opcionGrid = "Imprimir" Then
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim folioFac As TableCell = selectedRow.Cells(0)
            Dim serie As TableCell = selectedRow.Cells(1)
            Dim uuid As TableCell = selectedRow.Cells(2)
            Dim folioCfd As String = folioFac.Text
            Dim serieCfd As String = serie.Text
            Dim uuidCfd As String = uuid.Text
            If uuid.Text = "&nbsp;" Then
                uuidCfd = " "
            End If

            Dim folioContrarecibo As String
            Dim docto As String
            Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
            ObtieneDatos(Page.Session("RFC").ToString(), serieCfd, folioCfd, uuidCfd, contra, 1, errores)
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

        If opcionGrid = "Captura" Then
            _rfc = Page.Session("RFC").ToString()
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView1.Rows(index)
            Dim folioFac As TableCell = selectedRow.Cells(0)
            Dim serie As TableCell = selectedRow.Cells(1)
            Dim uuid As TableCell = selectedRow.Cells(2)
            Dim folioCfd As String = folioFac.Text
            Dim serieCfd As String = serie.Text
            Dim uuidCfd As String = uuid.Text
            If uuid.Text = "&nbsp;" Then
                uuidCfd = " "
            End If

            Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
            Dim llaveCfd = New llave_cfd
            Dim timbre = New timbre_fiscal_digital
            Dim user As New List(Of DatosUser)
            ObtieneDatos(_rfc, serieCfd, folioCfd, uuidCfd, contra, 2, errores)
            ' aqui verifica que no  exista informacion en tmp si regresa 
            ' error = 1, mensaje=	No hay informacion en tmp con estos datos
            ' dejaria capturar solo limpiamos los errores para que continue el flujo de la captura

            llaveCfd.rfc_emisor = _rfc
            llaveCfd.serie = serieCfd
            llaveCfd.folio_factura = Int64.Parse(folioCfd)
            llaveCfd.timbre_fiscal = timbre
            llaveCfd.timbre_fiscal.uuid = uuidCfd

            If errores.Count = 1 Then
                errores.Clear()
                ObtieneDatos(_rfc, serieCfd, folioCfd, uuidCfd, user, 1, errores)
                errores.Clear()
                Dim comprobante = New Comprobante
                Dim archivo As String = ""
                For Each dat In user
                    archivo = Trim(Replace(dat.NomArch.ToUpper, ".XML", ""))
                    archivo = Trim(Replace(archivo, ".PDF", ""))
                    comprobante.tipodoc_cve = dat.TipdocCve
                    comprobante.cc_tipo = dat.CcTipo
                    comprobante.cc_cve = dat.CcCve
                Next
                Page.Session("cc_tipo") = comprobante.cc_tipo
                Page.Session("cc_cve") = comprobante.cc_cve
                Page.Session("tipdoc_cve") = comprobante.tipodoc_cve
                comprobante.nom_arch = archivo
                llaveCfd.nom_arch = archivo
                If IsDBNull(comprobante.cc_tipo) Or IsDBNull(comprobante.cc_cve) Then
                    If Session("USRswUsrInterno") = 0 Then
                        Response.Redirect("Login.aspx")
                        Exit Sub
                    End If

                    If Session("USRswUsrInterno") = 1 Then
                        Response.Redirect("UserLogin.aspx")
                        Exit Sub
                    End If

                End If
                llaveCfd.sw_sin_addenda = 1
                _factura.PLeeCfd(_rutaTmp + llaveCfd.nom_arch + ".xml", errores, comprobante, llaveCfd, _ccCveLoc, _ccTipoLoc, _decimalesTruncados, _decimales)
                If errores.Count = 0 Then
                    Page.Session("comprobante") = comprobante
                    Page.Session("LlaveCFD") = llaveCfd
                    llaveCfd.sw_sin_addenda = 1
                    comprobante.Addenda.requestforpayment.provider.providerid = comprobante.cc_tipo + "@" + comprobante.cc_cve + "@" + llaveCfd.ef_cve
                    llena_datos_Captura(errores, comprobante, llaveCfd)
                    If comprobante.tipodoc_cve = "BTFACS" Then
                        Response.Redirect("CapturaSrv.aspx")
                    ElseIf comprobante.tipodoc_cve = "VTFE" Then
                        LlenaDtEmb()
                        Response.Redirect("CapturaEmb.aspx")
                    ElseIf comprobante.tipodoc_cve = "QTFAPN" Then
                        LlenaTablasSession()
                        Response.Redirect("Captura.aspx")
                    Else
                        lblErr.Text = "Captura no disponible para esta opcion"
                    End If
                End If

            Else
                ' en caso de que exista tmp se manda a ejecutar el 
                ' sp de sp_consInfXML para que veulva a procesarce la 
                ' captura y se genere la factura con los datos ya capturados en temporales
                errores.Clear()
                _factura.ExecTmpFact(errores, llaveCfd, 3)
                If errores.Count > 0 Then
                    lblErr.Text = "Ocurrio un error al generar rpt"
                Else
                    ConsultaFolios()
                End If
            End If
        End If

    End Sub
    Private Function LeeDetalles(ByVal rutaComprobante As String, ByVal adicional As List(Of String), ByVal items As List(Of lineitem)) As List(Of Errores)
        Dim xmldoc As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/Catalogos.xml"))
        Dim errores As New List(Of Errores)
        Dim xmlElm As XElement
        Try
            xmlElm = XElement.Load(rutaComprobante)
            Dim qList = From xe In xmlElm.Descendants _
                 Select xe
            For Each xe In qList
                If xe.Name.LocalName = "Receptor" Then
                    Dim rfcTemp As String = CType(xe.Attribute("rfc"), String)
                    Dim nombreProv As String = ""
                    adicional.Add(rfcTemp)
                    Dim emp As IEnumerable(Of XElement) = _
                            From el In xmldoc.<Empresas>.<Empresa> _
                            Where el.@ID = rfcTemp _
                            Select el
                    For Each el As XElement In emp
                        nombreProv = CType(el.Attribute("Nombre"), String)
                    Next
                    adicional.Add(nombreProv)
                End If
                If xe.Name.LocalName = "lineItem" Then
                    Dim item = New lineitem
                    item.type = CType(xe.Attribute("type"), Integer)
                    item.number = CType(xe.Attribute("number"), Integer)
                    item.monto_decuento = CType(xe.Attribute("montoDescuento"), Decimal)
                    item.pct_decuento = CType(xe.Attribute("pctDescuento"), Decimal)
                    item.uns = CType(xe.Attribute("uns"), Decimal)
                    item.precio = CType(xe.Attribute("precio"), Decimal)
                    item.sku = CType(xe.Attribute("sku"), String)
                    item.partida = CType(xe.Attribute("partida"), Integer)
                    item.reference_identification = CType(xe.Attribute("referenceIdentification"), String)
                    item.OrdCompra = CType(xe.Attribute("ordcompra"), String)
                    item.art_tip = CType(xe.Attribute("art_tip"), String)
                    item.uni_med = CType(xe.Attribute("uni_med"), String)
                    items.Add(item)
                End If
            Next
        Catch ex As Exception
            Dim err = New Errores
            err.Interror = 1
            err.Message = "Ocurrio un error al obtener la informacion del comprobante"
            errores.Add(err)
        End Try
        Return errores
    End Function
    Private Sub LlenaTablasSession()
        Dim dt = New DataTable()
        dt.Columns.Add(New DataColumn("Referencia", Type.GetType("System.String")))
        dt.Columns.Add(New DataColumn("Partida", Type.GetType("System.Int64")))
        dt.Columns.Add(New DataColumn("NoIdentificacion", Type.GetType("System.String")))
        dt.Columns.Add(New DataColumn("Uns", Type.GetType("System.Decimal")))
        dt.Columns.Add(New DataColumn("UM", Type.GetType("System.String")))
        dt.Columns.Add(New DataColumn("Precio", Type.GetType("System.Decimal")))
        dt.Columns.Add(New DataColumn("PctDesc", Type.GetType("System.Decimal")))
        dt.Columns.Add(New DataColumn("Importe", Type.GetType("System.Decimal")))
        dt.AcceptChanges()
        Page.Session("dtArt") = dt
        Dim dt2 = New DataTable()
        dt2.Columns.Add(New DataColumn("NoIdentificacion", Type.GetType("System.String")))
        dt2.Columns.Add(New DataColumn("Description", Type.GetType("System.String")))
        dt2.Columns.Add(New DataColumn("Importe", Type.GetType("System.Decimal")))
        dt2.AcceptChanges()
        Page.Session("dtCargo") = dt2
    End Sub
    Private Sub LlenaDtEmb()
        Dim dte = New DataTable()
        dte.Columns.Add(New DataColumn("Referencia", Type.GetType("System.String")))
        dte.Columns.Add(New DataColumn("NoIdentificacion", Type.GetType("System.String")))
        dte.Columns.Add(New DataColumn("Description", Type.GetType("System.String")))
        dte.Columns.Add(New DataColumn("Importe", Type.GetType("System.Decimal")))
        dte.AcceptChanges()
        Page.Session("dtTransportes") = dte
        Const swRetProv As Boolean = False
        Page.Session("sw_ret_prov") = swRetProv
    End Sub
    Protected Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        ConsultaFolios()
    End Sub
    Private Sub llena_datos_Captura(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
        borraInfoCfd_session()
        Dim moneda As String = ""
        Dim metodoDePago As String = ""
        If comprobante.moneda <> "" Then
            moneda = comprobante.moneda
        End If

        If comprobante.metodo_pago <> "" Then
            metodoDePago = comprobante.metodo_pago
        End If

        Dim iva = From cons In comprobante.Impuestos.Traslados
                   Select cons.tasa, cons.importe, cons.impuesto
                   Where (impuesto = "IVA")

        Dim tasaVarIva As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
        Dim importeIva As Decimal = 0
        'FGV10.10.2017 se agrego el if para que solo entre a validar la tasa de iva en aquellos registros donde el importe sea > 0
        For Each i In iva
            If i.importe > 0 Then
                Dim swtasa = _factura.RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    tasaVarIva = i.tasa / 100 'FormatNumber(i.tasa / 100, decimales)
                Else
                    tasaVarIva = i.tasa
                End If
            End If
            
            importeIva = importeIva + i.importe  'FormatNumber(i.importe, decimales)
        Next

        Dim ieps = From cons In comprobante.Impuestos.Traslados
                  Select cons.tasa, cons.importe, cons.impuesto
                  Where (impuesto = "IEPS")

        Dim tasaIeps As Decimal = 0  '= comprobante.Impuestos.Traslados.tasa / 100
        Dim importeIeps As Decimal = 0
        'FGV10.10.2017 se agrego el if para que solo entre a validar la tasa de iva en aquellos registros donde el importe sea > 0
        For Each i In ieps
            Dim swtasa = _factura.RevisaIntTasa(i.tasa.ToString())
            If i.importe > 0 Then
                If swtasa = True Then
                    tasaIeps = i.tasa / 100 ' FormatNumber(i.tasa / 100, decimales)
                Else
                    tasaIeps = i.tasa
                End If
            End If

            importeIeps = importeIeps + i.importe ' FormatNumber(i.importe, decimales)
        Next

        Dim retenciones = From cons In comprobante.Impuestos.Retenciones
                Select cons.importe, cons.impuesto
                Where (impuesto = "IVA")

        Dim retenImporteIva As Decimal = 0

        For Each i In retenciones
            retenImporteIva = i.importe 'FormatNumber(i.importe, decimales)
        Next

        Dim retisr = From cons In comprobante.Impuestos.Retenciones
              Select cons.importe, cons.impuesto
              Where (impuesto = "ISR")

        Dim retenImporteIsr As Decimal = 0

        For Each i In retisr
            retenImporteIsr = i.importe 'FormatNumber(i.importe, decimales)
        Next

        Dim totalConceptos As Decimal
        Dim importeConceptos As Decimal

        Dim qryResLine = From com2 In comprobante.Conceptos _
               Let sub_total = com2.cantidad * com2.valor_unitario _
               Select sub_total, com2.importe, com2.no_identificacion, com2.cantidad, com2.valor_unitario '_

        totalConceptos = Aggregate com2 In qryResLine _
                                Into Sum(com2.sub_total)

        importeConceptos = Aggregate com2 In qryResLine _
                           Into Sum(com2.importe)
        Dim totalImpTrasl As Decimal


        If comprobante.Impuestos.total_imp_trasl = 0 Then
            totalImpTrasl = comprobante.Impuestos.Traslados.Aggregate(Of Decimal)(0, Function(current, item) current + item.importe)
        Else
            totalImpTrasl = comprobante.Impuestos.total_imp_trasl
        End If

        Page.Session("nombre_receptor") = comprobante.Receptor.nombre
        Page.Session("EmpresaTitle") = System.String.Empty
        Page.Session("subtotal") = comprobante.sub_total
        Page.Session("descuento") = comprobante.descuento
        Page.Session("total") = comprobante.total
        Page.Session("tot_imp_trasl") = totalImpTrasl
        Page.Session("tot_imp_ret") = comprobante.Impuestos.total_imp_reten
        Page.Session("tasa") = tasaVarIva
        Page.Session("importe") = importeIva
        Page.Session("total_conceptos") = totalConceptos
        Page.Session("importe_conceptos") = importeConceptos

    End Sub



    Private Sub borraInfoCfd_session()

        Page.Session("nombre_receptor") = System.String.Empty
        Page.Session("EmpresaTitle") = System.String.Empty
        Page.Session("subtotal") = System.Decimal.Zero
        Page.Session("descuento") = System.Decimal.Zero
        Page.Session("total") = System.Decimal.Zero
        Page.Session("tot_imp_trasl") = System.Decimal.Zero
        Page.Session("tot_imp_ret") = System.Decimal.Zero
        Page.Session("tasa") = System.Decimal.Zero
        Page.Session("importe") = System.Decimal.Zero
        Page.Session("total_conceptos") = System.Decimal.Zero
        Page.Session("importe_conceptos") = System.Decimal.Zero

    End Sub
    Private Sub GridView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridView1.SelectedIndexChanged
    End Sub
End Class

