
Imports CLPortalAbstract
Imports System.Data
Imports System.ServiceModel.Security
Imports System.Activities.Expressions
Imports System.Web.UI.WebControls.Expressions
Imports Skytex.FacturaElectronica
Imports System.Text.RegularExpressions


Partial Class CajaFacturas
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
    'Dim _rutaTmpPas As String = ""



    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not (Page.IsPostBack) Then 'And Not Page.PreviousPage Is Nothing Then
            Page.Session("sw_captura") = 0
            CargarCombos()
        End If

    End Sub

    Private Sub CargarCombos()
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
        '_rutaTmpPas = _facturaBis.GetConfigure(config, "TmpPas").ToString()

    End Sub


    Protected Sub BtnAdjuntarXML_Click(sender As Object, e As System.EventArgs) Handles BtnAdjuntarXML.Click

        Dim tipdocCve As String = ""
        Dim errores As New List(Of Errores)
        Dim contra As New List(Of Skytex.FacturaElectronica.nuevas_facturas)
        Dim iErrorVal As Integer = 0
        Dim rfcSession As String = ""


        LoadConfig()
        gvResumen.Visible = False

        If Session("USRswUsrInterno") <> 1 Then
            Response.Redirect("UserLogin.aspx")
            Exit Sub
        End If
        tipdocCve = "BTCOM" '	Comisiones Bancarias 
        Dim comprobantes As New List(Of GrupoComprobantes)

        ValidarArchivosXml(_rutaTmp, comprobantes)


        Dim iCantCmp = comprobantes.Count - 1

        For i As Integer = 0 To iCantCmp
            ' ya tengo los archivos que se van a procesar solo, 
            'mandare los que al momento no tienen ningun error
            _facturaBis.IErrorG = 0
            _facturaBis.EfCveG = ""
            Dim CmpPaso = comprobantes(i)

            Dim archivo As String
            archivo = Trim(Replace(CmpPaso.ArchivosCmp.ArchivoXml.ToUpper, ".XML", ""))
            Dim llaveCfd = New llave_cfd
            CmpPaso.DocumentoCfd = New Comprobante()
            CmpPaso.DocumentoCfd.nom_arch = archivo
            llaveCfd.nom_arch = archivo
           

            If CmpPaso.ErroresList.Count = 0 Then 'GCM 16102014 se agrego el IF para la lista de errores
                llaveCfd.nom_arch = archivo
                llaveCfd.IdSoludin = Page.Session("USRCve").ToString()
                'llaveCfd.Proveedor = Page.Session("nomProveedor").ToString()
                llaveCfd.sw_sin_addenda = 1  ' la caja chica se manejara sin addenda 

                'CmpPaso.DocumentoCfd.cc_tipo = _ccTipo
                'CmpPaso.DocumentoCfd.cc_cve = _ccCve
                CmpPaso.DocumentoCfd.tipodoc_cve = tipdocCve
                CmpPaso.Llave = llaveCfd
                PLeeCfd(_rutaTmp + archivo + ".xml", CmpPaso.ErroresList, CmpPaso.DocumentoCfd, CmpPaso.Llave, rfcSession, _decimalesTrucados, _decimales)

                llaveCfd.sw_tmp = 1
            End If


            If CmpPaso.ErroresList.Count = 0 Then
                Dim sdoc = New Document
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim provider = New provider
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim facturas As New List(Of nuevas_facturas)
                ' se manda el del combo seleccionado
                sdoc.referenceIdentification = "BTCOM@" + ddlCtaBan.SelectedValue.ToString().Trim()
                requesForPayment.document = sdoc
                addenda.requestforpayment = requesForPayment
                CmpPaso.DocumentoCfd.Addenda = addenda
                provider.providerid = CmpPaso.DocumentoCfd.cc_tipo + "@" + CmpPaso.DocumentoCfd.cc_cve + "@" + llaveCfd.ef_cve
                CmpPaso.DocumentoCfd.Addenda.requestforpayment.provider = provider
                aditionalData.text_data = ddlDiv.SelectedValue '"z"
                aditionalData.metododepago = "TRANSFERENCIA ELECTRONICA"
                If CmpPaso.DocumentoCfd.moneda <> "" Then
                    'GCM 05112014 se agrego contemplar tm ninguno
                    'GCM 14112014 se contemplo moneda nacional
                    If (CmpPaso.DocumentoCfd.moneda = "Ninguno" And (CmpPaso.DocumentoCfd.tc = "1" Or CmpPaso.DocumentoCfd.tc = "1.00")) Or
                        CmpPaso.DocumentoCfd.moneda = "MONEDA NACIONAL" Then
                        aditionalData.moneda = "MXN"
                    Else
                        aditionalData.moneda = CmpPaso.DocumentoCfd.moneda
                    End If
                Else
                    aditionalData.moneda = "MXN"
                End If
                CmpPaso.DocumentoCfd.Addenda.requestforpayment.aditional_data = aditionalData
                paymentTimePeriod.timeperiod = 0
                CmpPaso.DocumentoCfd.Addenda.requestforpayment.paymenttimeperiod = paymentTimePeriod

                Dim items = New List(Of lineitem)
                Dim item = New lineitem
                item.reference_identification = sdoc.referenceIdentification
                item.type = 1
                item.number = 0
                item.monto_decuento = Decimal.Parse("0.0000")
                item.pct_decuento = Decimal.Parse("0.0000")
                item.uns = 0
                item.precio = Decimal.Parse("0.0000")
                item.sku = ddlCtaCble.SelectedValue
                item.partida = 0
                item.art_tip = "Y"
                item.uni_med = "zzzzzz"
                items.Add(item)
                CmpPaso.DocumentoCfd.Addenda.requestforpayment.line_items = items
                GenFacturaBis(_rutaTmp + archivo + ".xml", _rutaTmp + archivo + ".pdf", CmpPaso.ErroresList, New List(Of lineitem)(), CmpPaso.DocumentoCfd, llaveCfd, facturas)
                CmpPaso.FacturaGenerada = facturas
            End If

        Next
        
        iCantCmp = comprobantes.Count - 1


        Dim dt As New DataTable("Resultado")
        dt.Columns.Add("Factura")
        dt.Columns.Add("Resultado")
        dt.Columns.Add("Mensajes")

        For i As Integer = 0 To iCantCmp
            ' ya tengo los archivos que se van a procesar solo, 
            'mandare los que al momento no tienen ningun error
            gvResumen.Visible = True
            Dim CmpPaso = comprobantes(i)
            errores = CmpPaso.ErroresList
            Dim archivo As String
            archivo = Trim(Replace(CmpPaso.ArchivosCmp.ArchivoXml.ToUpper, ".XML", ""))
            Dim swFact As Boolean


            'GCM 16102014 agregado para mandar error correcto original Dim swFact = Not (CmpPaso.Llave.timbre_fiscal Is Nothing)
            If (CmpPaso.Llave.timbre_fiscal Is Nothing) Then
                swFact = False
            End If

            Dim folio As String = ""

            If swFact = True Then
                folio = "Emisor: " & CmpPaso.DocumentoCfd.NombreProveedor & " <br />" & _
                "Folio: " & CmpPaso.Llave.folio_factura.ToString() & " <br />" & _
                "Serie: " & CmpPaso.Llave.serie & " <br />" & _
                "UUID: " & CmpPaso.Llave.timbre_fiscal.uuid & " <br />" & _
                "Archivo: " & CmpPaso.ArchivosCmp.ArchivoXml & " <br />"
            Else
                folio = "Archivo: " & CmpPaso.ArchivosCmp.ArchivoXml & " <br />"
            End If


            If errores.Count = 0 Then
                For Each fact In CmpPaso.FacturaGenerada.GroupBy(Function(m) m.tipo_doc)
                    dt.Rows.Add(New Object() {folio, "Se proceso con exito", " "})
                Next
            End If

            If errores.Count > 0 Then

                Dim cadena As String = ""

                Dim msgUsr = From msg In errores _
                             Where (msg.Interror = 1 And msg.Message.Trim() <> "") _
                            Select msg
                Dim msgAdmin = From msg In errores _
                        Where (msg.Interror = 3 And msg.Message.Trim() <> "") Or (msg.Interror = 2 And msg.Message.Trim() <> "")
                        Select msg
                For Each msgs In msgUsr
                    cadena = cadena + msgs.Message + "<br />"
                Next
                Dim msgMails = From msg In errores _
                       Where (msg.Interror = 1 And msg.Message.Trim() <> "") _
                       Select msg

                Dim swMail As Boolean = msgUsr.Any()
                Dim swMailAd As Boolean = msgAdmin.Any()


                If swMail = True And msgMails.Count() <> 0 And swFact = True Then
                    _facturaBis.GenMailErrHtml(CmpPaso.Llave, "Error al recibir Factura Electrónica", msgMails.ToList(), _eMails, CmpPaso.DocumentoCfd.NombreProveedor)
                End If

                If swMailAd = True And msgAdmin.Count() <> 0 And swFact = True Then
                    _facturaBis.GenMailErrHtml(CmpPaso.Llave, "Error al recibir Factura Electrónica", msgAdmin.ToList(), _eMails, CmpPaso.DocumentoCfd.NombreProveedor)
                End If

                If swFact = False Then
                    Dim lAttach As List(Of String)

                    If swMailAd = True And msgAdmin.Count() <> 0 And swFact = True Then
                        _facturaBis.MailGeneric("Error al intentar leer archivo", CmpPaso.ArchivosCmp.ArchivoXml, _eMails, lAttach)
                    End If
                    If swMail = True And msgMails.Count() <> 0 And swFact = True Then
                        _facturaBis.MailGeneric("Error al intentar leer archivo", CmpPaso.ArchivosCmp.ArchivoXml, _eMails, lAttach)
                    End If
                End If



                If cadena = String.Empty Then
                    cadena = msgAdmin.Aggregate("", Function(current, msgs) current & msgs.Message + "<br />")
                End If

                dt.Rows.Add(New Object() {folio, "Error al procesar", cadena})

                PInicializaVariables()
            End If

            gvResumen.DataSource = dt
            gvResumen.DataBind()

            If errores.Count > 0 Then
                'GCM comentar para que no me de error
                FileSystem.FileCopy(_rutaTmp + archivo + ".xml", _rutaErr + archivo + ".xml")

                If FileIO.FileSystem.FileExists(_rutaTmp + archivo + ".pdf") = True Then
                    FileSystem.FileCopy(_rutaTmp + archivo + ".pdf", _rutaErr + archivo + ".pdf")
                End If
            End If

        Next


    End Sub


    Private Sub GenFacturaBis(ByVal nombreArchivoXml As String, _
                              ByVal nombreArchivoPdf As String, _
                              ByVal errores As List(Of Errores), _
                              ByVal items As List(Of lineitem), _
                              ByVal comprobante As Comprobante, _
                              ByVal llaveCfd As llave_cfd,
                              ByVal facturaGenerada As List(Of nuevas_facturas))


        _facturaBis.GrabaTmp(errores, comprobante, llaveCfd)
        'Cambio para CDFI 3.3 FGV (08/08/2017)
        If errores.Count = 0 And _facturaBis.IErrorG = 0 Then
            'If llaveCfd.version = "3.3" Then
            '_facturaBis.ValidaDatosEncabezadoCap3_3(errores, comprobante, llaveCfd)
            'Else
            _facturaBis.ValidaDatosEncabezadoCap(errores, comprobante, llaveCfd)
            'End If
            '_facturaBis.ValidaDatosEncabezadoCap(errores, comprobante, llaveCfd)
        End If

        'Cambio para CDFI 3.3 FGV (08/08/2017)
        If errores.Count = 0 And _facturaBis.IErrorG = 0 Then
            'If llaveCfd.version = "3.3" Then
            '_facturaBis.ValidaDatosDetalleCap3_3(errores, items, llaveCfd)
            'Else
            _facturaBis.ValidaDatosDetalleCap(errores, items, llaveCfd)
            'End If
        End If

        'Cambio para CDFI 3.3 FGV (08/08/2017)
        If errores.Count = 0 And _facturaBis.IErrorG = 0 Then
            'If llaveCfd.version = "3.3" Then
            '_facturaBis.ValidaDatosPapa3_3(errores, llaveCfd)
            'Else
            _facturaBis.ValidaDatosPapa(errores, llaveCfd)
            'End If
        End If

        'Cambio para CDFI 3.3 FGV (08/08/2017)
        If errores.Count = 0 And _facturaBis.IErrorG = 0 Then
            'If llaveCfd.version = "3.3" Then
            '_facturaBis.GeneraEncabezadoFactura3_3(errores, llaveCfd, facturaGenerada)
            'Else
            _facturaBis.GeneraEncabezadoFactura(errores, llaveCfd, facturaGenerada)
            'End If

            For Each fact In facturaGenerada
                Try
                    If FileIO.FileSystem.FileExists(_ruta + fact.ef_cve + fact.tipo_doc + fact.num_fol.ToString() + ".pdf") = True Then
                        FileSystem.FileCopy(nombreArchivoXml, _rutaExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")
                        If FileIO.FileSystem.FileExists(nombreArchivoPdf) = True Then
                            FileSystem.FileCopy(nombreArchivoPdf, _rutaExis + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                        End If
                    Else
                        FileSystem.FileCopy(nombreArchivoXml, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".xml")

                        If FileIO.FileSystem.FileExists(nombreArchivoPdf) = True Then
                            FileSystem.FileCopy(nombreArchivoPdf, _ruta + fact.ef_cve + fact.tipo_doc.Trim() + fact.num_fol.ToString() + ".pdf")
                        End If
                    End If
                Catch ex As Exception
                    _facturaBis.agrega_err(3, ex.Message, errores)
                End Try
            Next
        End If



        If errores.Count = 0 And _facturaBis.IErrorG = 0 Then

            'For Each fact In facturaGenerada
            '    TxtMensajeExitoso.Text = TxtMensajeExitoso.Text & "La factura " + nombreArchivoXml + "se ha procesado exitosamente, consulte la sección seguimiento folio de comision bancaria: " + fact.num_fol.ToString.Trim + vbNewLine
            '    TxtMensajeExitoso.Text = TxtMensajeExitoso.Text + "rfc: " + llaveCfd.rfc_emisor + _
            '    "serie: " + llaveCfd.serie + " folio: " + llaveCfd.folio_factura.ToString() + _
            '    "uuid: " + llaveCfd.timbre_fiscal.uuid + "<br />"
            'Next

            'TxtMensajes.Text = ""
            '
            Page.Session("ef_cve") = _facturaBis.EfCveG
            'PInicializaVariables()
        End If

        If _facturaBis.IErrorG > 0 And errores.Count = 0 Then
            _facturaBis.agrega_err(1, "", errores)
        End If

        If errores.Count > 0 And _facturaBis.IErrorG > 0 Then
            _facturaBis.LeerErroresSql(errores, llaveCfd)
        End If

    End Sub

    Private Sub PInicializaVariables()
        Page.Session("ContPDF") = 1
        Page.Session("NomPDF") = ""
        Page.Session("ListaPDF") = ""
        Page.Session("ContXML") = 1
        Page.Session("NomXML") = ""
        Page.Session("ListaXML") = ""
        'Page.Session("sw_Prov_Add") = 1 ' con addenda 0 sin addenda
        Page.Session("sw_captura") = 0
    End Sub

    Private Sub ConsultaCreaProveedor()

    End Sub

    'Private Function ValidarArchivosXml(ByRef rutaTmp As String, ByRef comprobantes As List(Of GrupoComprobantes)) As Integer
    Private Sub ValidarArchivosXml(ByRef rutaTmp As String, ByRef comprobantes As List(Of GrupoComprobantes))
        Dim iError = 0
        Dim j = 1
        Dim uploadFilCol As HttpFileCollection = Request.Files
        Dim controles = uploadFilCol.AllKeys.ToArray()
        Dim iLop = ((controles.Count() / 2) - 1)

        Dim iXml = 0
        Dim iPdf = 1
        Dim msg = ""
        Dim CadenaLimpia = 0
        Dim CadenaNormal = 0

        For i As Integer = 0 To iLop

            Dim cmp As New GrupoComprobantes
            Dim archivos As New Archivos
            Dim err = New List(Of Errores)
            Dim fileXml As HttpPostedFile = uploadFilCol(iXml)
            Dim filePdf As HttpPostedFile = uploadFilCol(iPdf)


            cmp.InumFactura = i

            Dim fileNameXml = System.IO.Path.GetFileName(fileXml.FileName)
            If fileNameXml = "" Then
                msg = msg + "Debe agregar el archivo XML"
                iError = 1
            End If

            archivos.ArchivoXml = fileNameXml


            Dim fileNamePdf = System.IO.Path.GetFileName(filePdf.FileName)


            If fileNamePdf = "" Then ' se agrega la validaicon del pdf forzoso
                msg = msg + "Debe agregar el archivo PDF"
                iError = 1
            End If

            If fileNamePdf <> "" Then

                archivos.ArchivoPdf = fileNamePdf

                If fileNameXml.Substring(0, fileNameXml.IndexOf(".")) <> fileNamePdf.Substring(0, fileNamePdf.IndexOf(".")) Then
                    msg = msg + "el nombre de archivos diferentes XML:" + fileNameXml + " PDF:" + fileNamePdf
                    iError = 1
                End If
            End If


            'GCM 16102014 No permite nombres con caracteres especiales

            If iError = 0 Then
                CadenaLimpia = Len(Regex.Replace(fileNameXml, "[^A-Za-z0-9_]", ""))
                CadenaNormal = Len(fileNameXml) - 1
                If CadenaLimpia <> CadenaNormal Then
                    msg = msg + "Nombre del archivo incorrecto, verificar:" + fileNameXml
                    iError = 1
                End If

            End If

            'Dim consecutivo As Integer = CType(Request.Form("AdjXML" + j.ToString()), Integer)

            Dim extXml = fileNameXml.Substring(fileNameXml.LastIndexOf(".", System.StringComparison.Ordinal) + 1).ToLower()
            Dim extPdf = fileNamePdf.Substring(fileNamePdf.LastIndexOf(".", System.StringComparison.Ordinal) + 1).ToLower()

            If extXml.ToUpper().Trim() <> "XML" Then
                msg = msg + "Extencion de archivo incorrecta, verificar:" + fileNameXml
                iError = 1
            End If

            If fileNamePdf <> "" Then
                If extPdf.ToUpper().Trim() <> "PDF" Then
                    msg = msg + "Extencion de archivo incorrecta, verificar:" + fileNamePdf
                    iError = 1
                End If
            End If

            If fileXml.ContentLength > 0 Then
                fileXml.SaveAs(rutaTmp & fileNameXml)
            End If

            If filePdf.ContentLength > 0 Then
                filePdf.SaveAs(rutaTmp & fileNamePdf)
            End If

            'If iError <> 0 Then
            '    msg = msg + msg + vbCrLf
            '    iError = 0
            'End If

            iXml += 2
            iPdf += 2

            j += 1

            'GCM Comentado 16102014
            'If iError > 0 Then
            '    Dim e As Errores = Nothing
            '    'e.CveError = 1
            '    e.Interror = 1
            '    e.Message = msg
            '    err.Add(e)
            'End If

            'GCM  16102014
            If iError > 0 Then
                Dim e = New Errores 'GCM
                'e.CveError = 1
                e.Interror = 1
                e.Message = msg
                err.Add(e)
            End If


            cmp.ErroresList = err
            cmp.ArchivosCmp = archivos
            comprobantes.Add(cmp)
        Next

        'If iError <> 0 Then
        '    TxtMensajes.Text = msg
        '    TxtMensajes.Visible = True
        'End If

        'Return iError
    End Sub

    Private Sub PLeeCfd(ByVal nombreArchivoXml As String, ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd, ByVal rfcSession As String, ByVal decTrun As Integer, ByVal decimales As Integer)
        Dim er As New Errores
        Dim swErrins As Integer = 0
        Dim receptor = New receptor
        Dim emisor = New emisor
        Dim regimen = New RegimenFiscal
        Dim proveedor = New ClaveProveedor

        Try
            If errores.Count = 0 And _facturaBis.IErrorG = 0 Then
                _facturaBis.LeeDatosLlaveLinq(errores, llaveCfd, nombreArchivoXml, receptor, emisor)
            End If
        Catch ex As Exception
            _facturaBis.agrega_err(3, ex.Message, errores)
        End Try

        Try
            If errores.Count = 0 And _facturaBis.IErrorG = 0 Then
                proveedor = _facturaBis.ConsultaCreaProveedor(errores, emisor, llaveCfd)
                comprobante.cc_cve = proveedor.CcCve
                comprobante.cc_tipo = proveedor.CcTipo
                comprobante.NombreProveedor = proveedor.NombreProveedor
            End If
        Catch ex As Exception
            _facturaBis.agrega_err(3, ex.Message, errores)
        End Try

        llaveCfd.sw_sin_addenda = 1 ' la caja chica se manejara sin addenda 

        Try
            If errores.Count = 0 And _facturaBis.IErrorG = 0 Then
                _facturaBis.ConsInsertaXml(errores, llaveCfd, proveedor.CcTipo, proveedor.CcCve, receptor.rfc, comprobante.tipodoc_cve)
                If _facturaBis.IErrorG = 1 Then
                    swErrins = 1
                End If
            End If
        Catch ex As Exception
            _facturaBis.agrega_err(3, ex.Message, errores)
        End Try

        Try
            If errores.Count = 0 And _facturaBis.IErrorG = 0 And llaveCfd.sw_sin_addenda = 1 Then
                _facturaBis.ValidaXSDLinq_SNAdd(errores, nombreArchivoXml, llaveCfd)
            End If
        Catch ex As Exception
            _facturaBis.agrega_err(3, ex.Message, errores)
        End Try

        Try
            If errores.Count = 0 And _facturaBis.IErrorG = 0 And llaveCfd.sw_sin_addenda = 1 Then
                '_facturaBis.LeeDatosFacturaLINQ_SNAdd(errores, comprobante, nombreArchivoXml, llaveCfd)
            End If
        Catch ex As Exception
            _facturaBis.agrega_err(1, "Ocurrio un error al leer el XML por favor contacte con el administrador de sitio ", errores)
        End Try

        Try
            If llaveCfd.sw_sin_addenda = 1 And errores.Count = 0 And _facturaBis.IErrorG = 0 Then
                Dim dicEmp As Dictionary(Of String, String) = _facturaBis.lee_ef_cve(receptor.rfc)
                Dim pairK As KeyValuePair(Of String, String)
                For Each pairK In dicEmp
                    llaveCfd.ef_cve = pairK.Key
                    comprobante.Receptor.nombre = pairK.Value
                Next
            End If
        Catch ex As Exception
            _facturaBis.agrega_err(3, ex.Message, errores)
        End Try

        Dim swValidarTotales = True

        'GCM 16102014 ya no se saltara toda la validación de totales, solo la del IVA
        'GCM 23102014 se hizo una excepcion para este RFC 
        'If llaveCfd.rfc_emisor.Trim() = "GGE001005Q89" Then 'se mete exepcion 26092014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "ITA891227GZ4" And llaveCfd.timbre_fiscal.uuid = "bb8ffe50-cba9-4fbc-955d-f669245223b3" Then 'se mete exepcion 26092014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "TAB020824HB8" And llaveCfd.timbre_fiscal.uuid.ToUpper() = "508A7C9E-AF1D-4A2C-831D-F9BA79E267B3" Then 'se mete exepcion 24102014 solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica 
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "TCH850701RM1" And (llaveCfd.timbre_fiscal.uuid.ToUpper() = "48B78548-29B4-4764-BBA5-2F9AFDDD9CA1" Or
        '                                                    llaveCfd.timbre_fiscal.uuid.ToUpper() = "19AF34FA-C4CD-41EA-B29A-8A7F83FACF6D") Then 'se mete exepcion 29102014 solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica 
        '    swValidarTotales = False
        'End If
        'If llaveCfd.rfc_emisor.Trim() = "SCR860403SP4" Then 'se mete exepcion 29102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "TCH850701RM1" Then 'se mete exepcion 31102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "CFE370814QI0" Then 'GCM se mete exepcion 05112014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "SCV0508246M6" Then 'se mete exepcion 01102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "SAR990323RG1" Then 'se mete exepcion 01102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "ESS070709EQ8" Then 'se mete exepcion 01102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "PAR900605CAA" Then 'GCM se mete exepcion 14102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "TCM951030A17" Then 'GCM se mete exepcion 14102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "NWM9709244W4" Then 'GCM se mete exepcion 14102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "IABC820706GM6" Then 'GCM se mete exepcion 15102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If

        'If llaveCfd.rfc_emisor.Trim() = "PAR900605CAA" Then 'GCM se mete exepcion 16102014 por que este marca error con IVA y solicito zor que se brincara mientras investigaran. para que pudieran generarse solo en caja chica
        '    swValidarTotales = False
        'End If


        If swValidarTotales = True Then
            Try
                If errores.Count = 0 And llaveCfd.sw_sin_addenda = 1 And _facturaBis.IErrorG = 0 Then
                    'Cambio para CDFI 3.3 FGV (08/08/2017)
                    If llaveCfd.version = "3.3" Then
                        _facturaBis.ValidaTotales_SNAdd3_3(errores, comprobante, llaveCfd, decTrun, decimales)
                    Else
                        _facturaBis.ValidaTotales_SNAdd(errores, comprobante, llaveCfd, decTrun, decimales)
                    End If
                    '_facturaBis.ValidaTotales_SNAdd(errores, comprobante, llaveCfd, decTrun, decimales)
                End If
            Catch ex As Exception
                _facturaBis.agrega_err(3, ex.Message + "ValidaTotales_SNAdd sw addenda:" + llaveCfd.sw_sin_addenda.ToString(), errores)
            End Try
        End If


        Dim msg60000 = From msg In errores _
            Select interror = msg.Interror, message = msg.Message _
            Where (interror = 60000) Or (interror = 1 And message.Trim = "")
        If msg60000.Count <> 0 Then
            If swErrins = 1 And _facturaBis.IErrorG > 0 And llaveCfd.folio_factura <> 0 And llaveCfd.rfc_emisor <> "" And llaveCfd.serie <> "" Then
                _facturaBis.LeerErroresSql(errores, llaveCfd)
            End If
        End If

    End Sub



End Class
