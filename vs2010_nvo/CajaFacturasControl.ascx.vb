Imports System.Data
Imports CLPortalAbstract
Imports Skytex.FacturaElectronica

Partial Class CajaFacturasControl
    Inherits System.Web.UI.UserControl
    ReadOnly _facturaBis As New Skytex.FacturaElectronica.FacturaBis

    Dim _ruta As String = ""
    Dim _rutaTmp As String = ""
    Dim _rutaExis As String = ""
    Dim _rutaErr As String = ""
    Dim _eMails As String = ""
    Dim _decimalesTrucados As Integer = 0
    Dim _decimales As Integer = 0
    Dim _ccCve, _ccTipo As String
    Dim _user As PerfilesUser = Nothing

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Page.Session("sw_cbos") = Nothing Then ' 0 Then
                Page.Session("sw_captura") = 0
                CargarCombos()
            End If

        End If
    End Sub

    Private Sub CargarCombos()
        Page.Session("sw_cbos") = 1
        LlenaCboCtaCBan() ' Cuenta bancaria
        LlenaCboCtaCble() ' Cuenta Contable
        LlenaCboDiv()
    End Sub


    Private Sub LlenaCboCtaCBan()
        Dim datosCombo As New combos
        datosCombo.combo = "CtaBanc"
        datosCombo.rfc = "?"
        datosCombo.parametro1 = "?"
        datosCombo.parametro2 = "?"

        Dim dt = _facturaBis.LlenaCbo(datosCombo)

        ddlCtaBan.DataSource = Nothing
        ddlCtaBan.DataSource = dt
        ddlCtaBan.DataTextField = "descripcion"
        ddlCtaBan.DataValueField = "valor"
        ddlCtaBan.DataBind()
        ddlCtaBan.Items(0).Selected = True
    End Sub

    Private Sub LlenaCboCtaCble()
        Dim datosCombo As New combos
        datosCombo.combo = "Clasif"
        datosCombo.rfc = "?"
        datosCombo.parametro1 = "?"
        datosCombo.parametro2 = "?"

        Dim dt = _facturaBis.LlenaCbo(datosCombo)

        ddlCtaCble.DataSource = Nothing
        ddlCtaCble.DataSource = dt
        ddlCtaCble.DataTextField = "descripcion"
        ddlCtaCble.DataValueField = "valor"
        ddlCtaCble.DataBind()
        ddlCtaCble.Items(0).Selected = True
    End Sub

    Private Sub LlenaCboDiv()
        Dim datosCombo As New combos
        datosCombo.combo = "Division"
        datosCombo.rfc = "?"
        datosCombo.parametro1 = "?"
        datosCombo.parametro2 = "?"

        Dim dt = _facturaBis.LlenaCbo(datosCombo)

        ddlDiv.DataSource = Nothing
        ddlDiv.DataSource = dt
        ddlDiv.DataTextField = "descripcion"
        ddlDiv.DataValueField = "valor"
        ddlDiv.DataBind()
        ddlDiv.Items(0).Selected = True
    End Sub


    Private Sub LoadConfig()
        Dim config = CType(Page.Session("config"), List(Of ConfigGral))
        _ruta = _facturaBis.GetConfigure(config, "Ruta").ToString()
        _rutaTmp = _facturaBis.GetConfigure(config, "RutaTMP").ToString()
        _rutaExis = _facturaBis.GetConfigure(config, "RutaExis").ToString()
        _eMails = _facturaBis.GetConfigure(config, "Email_Admin").ToString()
        _rutaErr = _facturaBis.GetConfigure(config, "RutaErr").ToString()
        _decimalesTrucados = CType(_facturaBis.GetConfigure(config, "decimalTrun"), Integer)
        _decimales = CType(_facturaBis.GetConfigure(config, "decimales"), Integer)

    End Sub



    Protected Sub UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs)
        Dim path As String = Server.MapPath("~/Uploads/") + IO.Path.GetFileName(e.FileName)
        AjaxFileUpload1.SaveAs(path)
    End Sub



    'Protected Sub BtnAdjuntarXML_Click(sender As Object, e As System.EventArgs) Handles BtnAdjuntarXml.Click

    '    Dim tipdocCve As String = ""
    '    Dim errores As New List(Of Errores)
    '    Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
    '    Dim iErrorVal As Integer = 0
    '    Dim rfcSession As String = ""


    '    LoadConfig()
    '    gvResumen.Visible = False

    '    If Session("USRswUsrInterno") <> 1 Then
    '        Response.Redirect("UserLogin.aspx")
    '        Exit Sub
    '    End If
    '    tipdocCve = "BTCOM" '	Comisiones Bancarias 
    '    Dim comprobantes As New List(Of GrupoComprobantes)

    '    ValidarArchivosXml(_rutaTmp, comprobantes)


    '    Dim iCantCmp = comprobantes.Count - 1

    '    For i As Integer = 0 To iCantCmp
    '        ' ya tengo los archivos que se van a procesar solo, 
    '        'mandare los que al momento no tienen ningun error
    '        _facturaBis.IErrorG = 0
    '        _facturaBis.EfCveG = ""
    '        Dim CmpPaso = comprobantes(i)

    '        Dim archivo As String
    '        archivo = Trim(Replace(CmpPaso.ArchivosCmp.ArchivoXml.ToUpper, ".XML", ""))
    '        Dim llaveCfd = New llave_cfd
    '        CmpPaso.DocumentoCfd = New Comprobante()
    '        CmpPaso.DocumentoCfd.nom_arch = archivo
    '        llaveCfd.nom_arch = archivo
    '        llaveCfd.IdSoludin = Page.Session("USRCve").ToString()
    '        'llaveCfd.Proveedor = Page.Session("nomProveedor").ToString()
    '        llaveCfd.sw_sin_addenda = 1  ' la caja chica se manejara sin addenda 

    '        'CmpPaso.DocumentoCfd.cc_tipo = _ccTipo
    '        'CmpPaso.DocumentoCfd.cc_cve = _ccCve
    '        CmpPaso.DocumentoCfd.tipodoc_cve = tipdocCve
    '        CmpPaso.Llave = llaveCfd
    '        PLeeCfd(_rutaTmp + archivo + ".xml", CmpPaso.ErroresList, CmpPaso.DocumentoCfd, CmpPaso.Llave, rfcSession, _decimalesTrucados, _decimales)

    '        llaveCfd.sw_tmp = 1

    '        If CmpPaso.ErroresList.Count = 0 Then
    '            Dim sdoc = New Document
    '            Dim addenda = New addenda
    '            Dim requesForPayment = New requestforpayment
    '            Dim provider = New provider
    '            Dim aditionalData = New aditionaldata
    '            Dim paymentTimePeriod = New paymenttimeperiod
    '            Dim facturas As New List(Of nuevas_facturas)
    '            ' se manda el del combo seleccionado
    '            sdoc.referenceIdentification = "BTCOM@" + ddlCtaBan.SelectedValue.ToString().Trim()
    '            requesForPayment.document = sdoc
    '            addenda.requestforpayment = requesForPayment
    '            CmpPaso.DocumentoCfd.Addenda = addenda
    '            provider.providerid = CmpPaso.DocumentoCfd.cc_tipo + "@" + CmpPaso.DocumentoCfd.cc_cve + "@" + llaveCfd.ef_cve
    '            CmpPaso.DocumentoCfd.Addenda.requestforpayment.provider = provider
    '            aditionalData.text_data = ddlDiv.SelectedValue '"z"
    '            aditionalData.metododepago = "TRANSFERENCIA ELECTRONICA"
    '            If CmpPaso.DocumentoCfd.moneda <> "" Then
    '                aditionalData.moneda = CmpPaso.DocumentoCfd.moneda
    '            Else
    '                aditionalData.moneda = "MXN"
    '            End If
    '            CmpPaso.DocumentoCfd.Addenda.requestforpayment.aditional_data = aditionalData
    '            paymentTimePeriod.timeperiod = 0
    '            CmpPaso.DocumentoCfd.Addenda.requestforpayment.paymenttimeperiod = paymentTimePeriod

    '            Dim items = New List(Of lineitem)
    '            Dim item = New lineitem
    '            item.reference_identification = sdoc.referenceIdentification
    '            item.type = 1
    '            item.number = 0
    '            item.monto_decuento = Decimal.Parse("0.0000")
    '            item.pct_decuento = Decimal.Parse("0.0000")
    '            item.uns = 0
    '            item.precio = Decimal.Parse("0.0000")
    '            item.sku = ddlCtaCble.SelectedValue
    '            item.partida = 0
    '            item.art_tip = "Y"
    '            item.uni_med = "zzzzzz"
    '            items.Add(item)
    '            CmpPaso.DocumentoCfd.Addenda.requestforpayment.line_items = items
    '            GenFacturaBis(_rutaTmp + archivo + ".xml", _rutaTmp + archivo + ".pdf", CmpPaso.ErroresList, New List(Of lineitem)(), CmpPaso.DocumentoCfd, llaveCfd, facturas)
    '            CmpPaso.FacturaGenerada = facturas
    '        End If

    '    Next

    '    iCantCmp = comprobantes.Count - 1


    '    Dim dt As New DataTable("Resultado")
    '    dt.Columns.Add("Factura")
    '    dt.Columns.Add("Resultado")
    '    dt.Columns.Add("Mensajes")

    '    For i As Integer = 0 To iCantCmp
    '        ' ya tengo los archivos que se van a procesar solo, 
    '        'mandare los que al momento no tienen ningun error
    '        gvResumen.Visible = True
    '        Dim CmpPaso = comprobantes(i)
    '        errores = CmpPaso.ErroresList
    '        Dim archivo As String
    '        archivo = Trim(Replace(CmpPaso.ArchivosCmp.ArchivoXml.ToUpper, ".XML", ""))

    '        Dim swFact = Not (CmpPaso.Llave.timbre_fiscal Is Nothing)


    '        Dim folio As String = ""

    '        If swFact = True Then
    '            folio = "Emisor: " & CmpPaso.DocumentoCfd.NombreProveedor & " <br />" & _
    '            "Folio: " & CmpPaso.Llave.folio_factura.ToString() & " <br />" & _
    '            "Serie: " & CmpPaso.Llave.serie & " <br />" & _
    '            "UUID: " & CmpPaso.Llave.timbre_fiscal.uuid & " <br />" & _
    '            "Archivo: " & CmpPaso.ArchivosCmp.ArchivoXml & " <br />"
    '        Else
    '            folio = "Archivo: " & CmpPaso.ArchivosCmp.ArchivoXml & " <br />"
    '        End If


    '        If errores.Count = 0 Then
    '            For Each fact In CmpPaso.FacturaGenerada
    '                dt.Rows.Add(New Object() {folio, "Se proceso con exito", " "})
    '            Next
    '        End If

    '        If errores.Count > 0 Then

    '            Dim cadena As String = ""

    '            Dim msgUsr = From msg In errores _
    '                         Where (msg.Interror = 1 And msg.Message.Trim() <> "") _
    '                        Select msg
    '            Dim msgAdmin = From msg In errores _
    '                    Where (msg.Interror = 3 And msg.Message.Trim() <> "") Or (msg.Interror = 2 And msg.Message.Trim() <> "")
    '                    Select msg
    '            For Each msgs In msgUsr
    '                cadena = cadena + msgs.Message + "<br />"
    '            Next
    '            Dim msgMails = From msg In errores _
    '                   Where (msg.Interror = 1 And msg.Message.Trim() <> "") _
    '                   Select msg

    '            Dim swMail As Boolean = msgUsr.Any()
    '            Dim swMailAd As Boolean = msgAdmin.Any()


    '            If swMail = True And msgMails.Count() <> 0 And swFact = True Then
    '                _facturaBis.GenMailErrHtml(CmpPaso.Llave, "Error al recibir Factura Electrónica", msgMails.ToList(), _eMails, CmpPaso.DocumentoCfd.NombreProveedor)
    '            End If

    '            If swMailAd = True And msgAdmin.Count() <> 0 And swFact = True Then
    '                _facturaBis.GenMailErrHtml(CmpPaso.Llave, "Error al recibir Factura Electrónica", msgAdmin.ToList(), _eMails, CmpPaso.DocumentoCfd.NombreProveedor)
    '            End If

    '            If swFact = False Then
    '                Dim lAttach As List(Of String)

    '                If swMailAd = True And msgAdmin.Count() <> 0 And swFact = True Then
    '                    _facturaBis.MailGeneric("Error al intentar leer archivo", CmpPaso.ArchivosCmp.ArchivoXml, _eMails, lAttach)
    '                End If
    '                If swMail = True And msgMails.Count() <> 0 And swFact = True Then
    '                    _facturaBis.MailGeneric("Error al intentar leer archivo", CmpPaso.ArchivosCmp.ArchivoXml, _eMails, lAttach)
    '                End If
    '            End If


    '            If cadena = String.Empty Then
    '                cadena = msgAdmin.Aggregate("", Function(current, msgs) current & msgs.Message + "<br />")
    '            End If

    '            dt.Rows.Add(New Object() {folio, "Error al procesar", cadena})

    '            PInicializaVariables()
    '        End If

    '        gvResumen.DataSource = dt
    '        gvResumen.DataBind()

    '        If errores.Count > 0 Then
    '            FileSystem.FileCopy(_rutaTmp + archivo + ".xml", _rutaErr + archivo + ".xml")

    '            If FileIO.FileSystem.FileExists(_rutaTmp + archivo + ".pdf") = True Then
    '                FileSystem.FileCopy(_rutaTmp + archivo + ".pdf", _rutaErr + archivo + ".pdf")
    '            End If
    '        End If

    '    Next


    'End Sub





End Class
