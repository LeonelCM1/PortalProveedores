Imports System.Data
Imports System.Data.SqlClient
Imports CLPortalAbstract
Imports System.Activities.Expressions
Imports System.Xml.Schema
Imports System.Math
Imports System.Net.Mail

Namespace Skytex.FacturaElectronica
    Public Class FacturaBis

#Region "Variables"

        Private _iErrorG As Integer = 0
        Private _Conexion As New Datos
        Private Conexion As SqlConnection = _Conexion.MiConexion
        Private ReadOnly _timeout As Integer = _Conexion.time_out
        Private _efCveG As String
        Private _dtValidacion As DataTable
        Private _transaccion As SqlTransaction
        Private DataBase As String = "SKYHDEV3"
        Dim _server As New _Server
        Private DBdevelop As String = _Server.DBdevelop
        Private _DBproduc As String = _Server.DBproduc

#End Region

#Region "Propiedades"

        Public Property IErrorG() As Integer
            Get
                Return _iErrorG
            End Get
            Set(ByVal value As Integer)
                _iErrorG = value
            End Set
        End Property
        Public Property EfCveG() As String
            Get
                Return _efCveG
            End Get
            Set(ByVal value As String)
                _efCveG = value
            End Set
        End Property
        Public Property ValidacionEncabezado() As DataTable
            Get
                Return _dtValidacion
            End Get

            Set(ByVal value As DataTable)
                _dtValidacion = value
            End Set
        End Property
#End Region

#Region "Metodos"


        Public Sub GenMailErrHtml(ByVal llaveCfd As llave_cfd, ByVal titulo As String,
                                 ByVal errores As List(Of Errores),
                                 ByVal eMail As String, ByVal proveedor As String)

            Dim nomProveedor As String = proveedor 'proveedor.Substring(10, proveedor.Length - 10)

            Dim sBody As String
            Dim sAttach As New List(Of String)
            Const sEnc As String = "<!DOCTYPE HTML><meta http-equiv='Content-Type' content='text/html; charset=ISO-8859-1'> <style type='text/css'> body { font-family:Calibri, sans-serif;font-size:14px;} </style> <head></head><body> <table  border='0' cellpadding='0' cellspacing='0' ><tr><td style:'width:800px;' >"
            sBody = sEnc + "Datos del comprobante:" + "</br>"
            sBody = sBody + "</br>"
            sBody = sBody + " <table border='0' cellpadding='0' cellspacing='0' >"
            'sBody = sBody + "<tr>"
            'sBody = sBody + "<th style='width:30px;text-align:left;'></th>"
            'sBody = sBody + "<th style='width:90px;text-align:left;'></th>"
            'sBody = sBody + "<th style='width:700px;text-align:left;'></th>"
            'sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px;width:30px;text-align:left;'></td>"
            sBody = sBody + "<td style='height:5px;width:90px;text-align:left;'>Proveedor:</td>"
            sBody = sBody + "<td style='height:5px;width:700px;text-align:left;'>" + nomProveedor.Trim() + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px'></td>"
            sBody = sBody + "<td style='height:5px'>Folio:</td>"
            sBody = sBody + "<td style='height:5px'>" + llaveCfd.folio_factura.ToString() + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px'></td>"
            sBody = sBody + "<td style='height:5px'>Serie:</td>"
            sBody = sBody + "<td style='height:5px'>" + llaveCfd.serie + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            sBody = sBody + "<td style='height:5px'></td>"
            sBody = sBody + "<td style='height:5px'>UUID:</td>"
            sBody = sBody + "<td style='height:5px'>" + llaveCfd.timbre_fiscal.uuid + "</td>"
            sBody = sBody + "</tr>"
            sBody = sBody + "<tr>"
            'sBody = sBody + "<td></br></td>"
            sBody = sBody + "<td></td>"
            sBody = sBody + "<td></td>"
            sBody = sBody + "<td></td>"
            sBody = sBody + "</tr>"
            For Each o As Errores In errores
                Dim tipoError As String
                Dim claveMsg As String
                Dim descripcionError As String
                tipoError = o.Interror.ToString()
                claveMsg = o.CveError
                descripcionError = o.Message.Trim()
                If claveMsg <> "" Then
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></br></td>"
                    sBody = sBody + "<td style='height:5px'>Tipo de error:</td>"
                    sBody = sBody + "<td style='height:5px'>" + tipoError + "</td>"
                    sBody = sBody + "</tr>"
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></td>"
                    sBody = sBody + "<td style='height:5px'>Clave del error:</td>"
                    sBody = sBody + "<td style='height:5px'>" + claveMsg + "</td>"
                    sBody = sBody + "</tr>"
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></td>"
                    sBody = sBody + "<td style='height:5px'>Mensaje:</td>"
                    sBody = sBody + "<td style='height:5px'>" + descripcionError + "</td>"
                    sBody = sBody + "</tr>"
                    'sBody = sBody + "</tr></br>"
                Else
                    sBody = sBody + "<tr>"
                    sBody = sBody + "<td style='height:5px'></td>"
                    If tipoError = 1 Then
                        sBody = sBody + "<td style='height:5px'>Mensaje:</td>"
                    Else
                        sBody = sBody + "<td style='height:5px'>Rutina:</td>"
                    End If

                    sBody = sBody + "<td style='height:5px'>" + descripcionError + "</td>"
                    sBody = sBody + "</tr>"
                    'sBody = sBody + "</tr></br>"
                End If
            Next
            sBody = sBody + "</table> "
            sBody = sBody + "</td></tr></table>"
            sBody = sBody + "</br>"
            sBody = sBody + "</body></HTML> "
            MailGeneric(titulo, sBody, eMail, sAttach)
        End Sub

        Public Sub MailGeneric(ByVal sSubject As String, _
                               ByVal sBody As String, _
                               ByVal sEmails As String,
                               ByVal lAttach As List(Of String))

            Dim objMail As New MailMessage
            Dim smtp As New SmtpClient
            Dim dat As New Datos
            Dim emailFrom As String = ConfigurationManager.AppSettings("Email_From")
            Dim emailSmtp As String = ConfigurationManager.AppSettings("Email_SMTP")
            Dim emailUser As String = ConfigurationManager.AppSettings("Email_User")
            Dim emailPsswd As String = ConfigurationManager.AppSettings("Email_Psswd")
            Dim oAttch As Attachment
            Dim base As String = ""
            base = dat.ObtieneDb()
            Try
                objMail.To.Add(sEmails)

                For Each item As String In lAttach
                    oAttch = New Attachment(item.Trim())
                    objMail.Attachments.Add(oAttch)
                Next
                objMail.From = New MailAddress(emailFrom)
                'If base.ToLower() = "develop" Then
                If base.ToLower() = DBdevelop Then '//JPO: 27-06-16 valida el nombre de la instancia SQL a consultarl
                    objMail.Subject = sSubject + " Desarrollo DEVELOP"
                Else
                    objMail.Subject = sSubject
                End If
                objMail.Body = sBody.Trim()
                objMail.IsBodyHtml = True
                objMail.BodyEncoding = Encoding.UTF8
                objMail.Priority = MailPriority.Normal
                smtp.Host = emailSmtp
                smtp.Credentials = New Net.NetworkCredential(emailUser, emailPsswd)
                smtp.Send(objMail)
            Catch ex As SmtpException
                'MensajeError = "Error al enviar mail en la aplicación. " + sBody + " ::: " + ex.Message
                iErrorG = 1
            End Try

        End Sub


        Public Sub LeeDatosLlaveLinq(ByVal errores As List(Of Errores), _
                                     ByVal llaveCfd As llave_cfd, _
                                     ByVal xmlDocFilePath As String, _
                                     ByVal receptor As receptor,
                                     ByVal emisorCFDI As emisor
                                     )
            Dim xmlElm As XElement
            Try
                xmlElm = XElement.Load(xmlDocFilePath)
                Dim root As XElement = XElement.Load(xmlDocFilePath)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr
                Dim timbre = New timbre_fiscal_digital
                Dim contador = 0

                If IsNothing(root.Attribute("version")) Then
                    agrega_err(1, "No puede leerse el dato version", errores)
                Else
                    llaveCfd.version = root.Attribute("version").Value.ToString()
                End If
                Select Case llaveCfd.version
                    Case "2.2"
                        llaveCfd.version_nom = "CFD"
                    Case "3.2"
                        llaveCfd.version_nom = "CFDI"
                    Case Else
                        agrega_err(1, "Version de comprobante incorrecta", errores)
                        llaveCfd.version_nom = ""
                End Select
                Dim sello As String = ""
                If IsNothing(root.Attribute("sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    sello = root.Attribute("sello").Value.ToString()
                End If
                If llaveCfd.version_nom = "CFDI" Then
                    Dim qList = From xe In xmlElm.Descendants _
                                Select xe
                    For Each xe In qList
                        If xe.Name.LocalName = "TimbreFiscalDigital" Then
                            timbre.version = xe.Attribute("version").Value
                            timbre.uuid = xe.Attribute("UUID").Value
                            timbre.fecha_timbrado = xe.Attribute("FechaTimbrado").Value
                            timbre.sello_cfd = xe.Attribute("selloCFD").Value
                            timbre.no_certificado_sat = xe.Attribute("noCertificadoSAT").Value
                            timbre.sello_sat = xe.Attribute("selloSAT").Value
                            contador = 1
                            Exit For
                        End If
                    Next
                    llaveCfd.timbre_fiscal = timbre

                    'GCM 22102014 se agrego el error sin timbre fiscal
                    If contador = 0 Then
                        agrega_err(1, "sin timbre fiscal", errores)
                    End If

                    If sello <> llaveCfd.timbre_fiscal.sello_cfd And contador = 1 Then
                        agrega_err(1, "el sello es diferente al del timbre fiscal", errores)
                    End If
                Else
                    'Cambio para CDFI 3.3 FGV (07/08/2017)
                    If llaveCfd.version_nom = "CFDI_3" Then
                        Dim qList = From xe In xmlElm.Descendants _
                                    Select xe
                        For Each xe In qList
                            If xe.Name.LocalName = "TimbreFiscalDigital" Then
                                timbre.version = xe.Attribute("version").Value
                                timbre.uuid = xe.Attribute("UUID").Value
                                timbre.fecha_timbrado = xe.Attribute("FechaTimbrado").Value
                                timbre.sello_cfd = xe.Attribute("selloCFD").Value
                                timbre.no_certificado_sat = xe.Attribute("noCertificadoSAT").Value
                                timbre.sello_sat = xe.Attribute("selloSAT").Value
                                contador = 1
                                Exit For
                            End If
                        Next
                        llaveCfd.timbre_fiscal = timbre

                        'GCM 22102014 se agrego el error sin timbre fiscal
                        If contador = 0 Then
                            agrega_err(1, "sin timbre fiscal", errores)
                        End If

                        If sello <> llaveCfd.timbre_fiscal.sello_cfd And contador = 1 Then
                            agrega_err(1, "el sello es diferente al del timbre fiscal", errores)
                        End If
                    Else
                        timbre.uuid = ""
                        llaveCfd.timbre_fiscal = timbre
                    End If
                End If


                If IsNothing(root.Attribute("serie")) Then
                    llaveCfd.serie = ""
                Else
                    llaveCfd.serie = root.Attribute("serie").Value.ToString()
                End If
                If IsNothing(root.Attribute("folio")) Then
                    llaveCfd.folio_factura = 0
                Else
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", ""))  'Int32.Parse(root.Attribute("folio").Value)
                    'Soporte 74669 FGV se valida el maximo tamaño del folio de la factura para truncarlo si excede el numero 2147483647 para que no genere problemas al momento de insertar la info
                    If folPaso >= 2147483647 Then
                        llaveCfd.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        llaveCfd.folio_factura = folPaso
                    End If

                End If
                Dim qList1 = From xe In xmlElm.Descendants _
                Select xe

                For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Emisor"
                    LeeEmisorCFDI(xe, emisorCFDI, errores)
                    llaveCfd.rfc_emisor = emisorCFDI.rfc
                    Exit For
                Next



                For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "Receptor"
                    receptor.rfc = CType(xe.Attribute("rfc"), String)
                    Exit For
                Next


                If llaveCfd.rfc_emisor = "" Then
                    agrega_err(1, "No puede leerse el dato rfc del Emisor", errores)
                End If
                If receptor.rfc = "" Then
                    agrega_err(1, "No puede leerse el dato rfc del Receptor", errores)
                End If


            Catch ex As Exception
                agrega_err(1, ex.Message, errores)
            End Try

        End Sub


        Private Sub LeeEmisorCFDI(ByVal elemento As XElement, ByVal emisorTmp As emisor, ByVal errores As List(Of Errores))
            'Dim emisorTmp = New emisor
            Dim regimen = New RegimenFiscal
            Dim domicilio As New domiciliofiscal
            Dim swDomiciolioFiscal As Boolean = False
            Dim swRegimen As Boolean = False
            Dim qList1 = From xe In elemento.Descendants _
             Select xe

            'Cambio para CDFI 3.3 FGV (07/08/2017)
            Dim versionCFDI = elemento.Attribute("version").Value.ToString()

            If Not IsNothing(elemento.Attribute("rfc")) Then
                emisorTmp.rfc = elemento.Attribute("rfc").Value.ToString()
            Else
                emisorTmp.rfc = ""
            End If


            If Not IsNothing(elemento.Attribute("nombre")) Then
                emisorTmp.nombre = elemento.Attribute("nombre").Value.ToString()
            Else
                emisorTmp.nombre = ""
            End If

            'Cambio para CDFI 3.3 FGV (07/08/2017)
            If versionCFDI = "3.3" Then
                swDomiciolioFiscal = False
            Else
                For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "DomicilioFiscal"
                    swDomiciolioFiscal = True
                    If Not IsNothing(xe.Attribute("calle")) Then
                        domicilio.calle = xe.Attribute("calle").Value.ToString()
                    Else
                        domicilio.calle = ""
                    End If

                    If Not IsNothing(xe.Attribute("noExterior")) Then
                        domicilio.no_exterior = xe.Attribute("noExterior").Value.ToString()
                    Else
                        domicilio.no_exterior = ""
                    End If

                    If Not IsNothing(xe.Attribute("noInterior")) Then
                        domicilio.no_interior = xe.Attribute("noInterior").Value.ToString()
                    Else
                        domicilio.no_interior = ""
                    End If

                    If Not IsNothing(xe.Attribute("colonia")) Then
                        domicilio.colonia = xe.Attribute("colonia").Value.ToString()
                    Else
                        domicilio.colonia = ""
                    End If

                    If Not IsNothing(xe.Attribute("localidad")) Then
                        domicilio.localidad = xe.Attribute("localidad").Value.ToString()
                    Else
                        domicilio.localidad = ""
                    End If
                    If Not IsNothing(xe.Attribute("municipio")) Then
                        domicilio.municipio = xe.Attribute("municipio").Value.ToString()
                    Else
                        domicilio.municipio = ""
                    End If

                    If Not IsNothing(xe.Attribute("estado")) Then
                        domicilio.estado = xe.Attribute("estado").Value.ToString()
                    Else
                        domicilio.estado = ""
                    End If

                    If Not IsNothing(xe.Attribute("pais")) Then
                        domicilio.pais = xe.Attribute("pais").Value.ToString()
                    Else
                        domicilio.pais = ""
                    End If

                    If Not IsNothing(xe.Attribute("codigoPostal")) Then
                        domicilio.codigo_postal = xe.Attribute("codigoPostal").Value.ToString()
                    Else
                        domicilio.codigo_postal = ""
                    End If
                Next
            End If

            If swDomiciolioFiscal = False Then
                domicilio.calle = ""
                domicilio.no_exterior = ""
                domicilio.no_interior = ""
                domicilio.colonia = ""
                domicilio.localidad = ""
                domicilio.localidad = ""
                domicilio.municipio = ""
                domicilio.estado = ""
                domicilio.pais = ""
                domicilio.codigo_postal = ""
            End If

            'Cambio para CDFI 3.3 FGV (07/08/2017)
            If versionCFDI = "3.3" Then
                If Not IsNothing(elemento.Attribute("nombre")) Then
                    swRegimen = True
                    regimen.RegimenFisc = elemento.Attribute("RegimenFiscal").Value.ToString()
                Else
                    swRegimen = False
                    regimen.RegimenFisc = ""
                End If

                If swRegimen = False Then
                    agrega_err(1, "El regimen del emisor es un dato requerido", errores)
                End If

                If emisorTmp.rfc = "" Then
                    agrega_err(1, "El rfc del emisor es un dato requerido", errores)
                End If
            Else
                For Each xe In From xe1 In qList1 Where xe1.Name.LocalName = "RegimenFiscal"
                    If Not IsNothing(xe.Attribute("Regimen")) Then
                        swRegimen = True
                        regimen.RegimenFisc = CType(xe.Attribute("Regimen"), String)
                    End If
                    Exit For
                Next

                If swRegimen = False Then
                    agrega_err(1, "El regimen del emisor es un dato requerido", errores)
                End If

                If emisorTmp.rfc = "" Then
                    agrega_err(1, "El rfc del emisor es un dato requerido", errores)
                End If
            End If

            emisorTmp.DomicilioFiscal = domicilio
            emisorTmp.Regimen = regimen
            'Return emisorTmp
        End Sub
        Public Sub agrega_err(ByVal errorXml As Integer, ByVal smensaje As String, ByVal errores As List(Of Errores))
            Dim er As New Errores
            IErrorG = 1
            er.Interror = errorXml
            er.Message = smensaje
            er.CveError = ""
            errores.Add(er)
        End Sub
        Public Sub agrega_err(ByVal errorXml As Integer, ByVal smensaje As String, ByVal errores As List(Of Errores), ByVal cveError As String)
            Dim er As New Errores
            IErrorG = 1
            er.Interror = errorXml
            er.Message = smensaje
            er.CveError = cveError
            errores.Add(er)
        End Sub
        Public Function GetConfigure(ByVal listaConfig As List(Of ConfigGral), ByVal sConf As String) As Object
            Dim value As Object = Nothing
            Dim rutaEnum = From con In listaConfig
                                Where con.config = sConf
                                Select con.valor
            For Each itm As String In rutaEnum
                value = itm
            Next
            Return value
        End Function
        Public Function LlenaCbo(ByVal combo As combos) As DataTable

            Dim sqlAdapter = New SqlDataAdapter("sp_cboWebXML", Conexion)
            Dim ds As New DataSet()
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            Conexion.Open()
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", combo.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cbo", combo.combo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@prm1", combo.parametro1.Trim())
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@prm2", combo.parametro2.Trim())

            Try
                sqlAdapter.Fill(ds, "sp_cboWebXML")
                sqlAdapter.Dispose()
                Conexion.Close()
                If ds.Tables(0).Rows.Count >= 1 Then
                    Return ds.Tables(ds.Tables.Count - 1)
                End If
            Catch ex As Exception

            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return ds.Tables(ds.Tables.Count - 1)

        End Function
        Public Function ConsultaCreaProveedor(ByVal errores As List(Of Errores), ByVal emisorCFDI As emisor, ByVal llaveCfd As llave_cfd) As ClaveProveedor

            Dim proveedor = New ClaveProveedor
            Dim ds As New DataSet()
            Dim sqlAdapter = New SqlDataAdapter("sp_consProCreaWeb", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout

            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc", emisorCFDI.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nombre", emisorCFDI.nombre)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@calle", emisorCFDI.DomicilioFiscal.calle)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_exterior", emisorCFDI.DomicilioFiscal.no_exterior)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_interior", emisorCFDI.DomicilioFiscal.no_interior)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@colonia", emisorCFDI.DomicilioFiscal.colonia)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@localidad", emisorCFDI.DomicilioFiscal.localidad)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@municipio", emisorCFDI.DomicilioFiscal.municipio)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@estado", emisorCFDI.DomicilioFiscal.estado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pais", emisorCFDI.DomicilioFiscal.pais)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@codigo_postal", emisorCFDI.DomicilioFiscal.codigo_postal)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@regimen", emisorCFDI.Regimen.RegimenFisc)

            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_consProCreaWeb")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)

                IErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If IErrorG = 0 Then
                    proveedor.CcCve = ValidacionEncabezado.Rows(0).Item("cc_cve").ToString().Trim()
                    proveedor.CcTipo = ValidacionEncabezado.Rows(0).Item("cc_tipo").ToString().Trim()
                    proveedor.NombreProveedor = ValidacionEncabezado.Rows(0).Item("proveedor").ToString().Trim()
                End If

                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()

            Catch ex As Exception
                Dim msg As String = ""
                Dim er = New Errores

                msg = "sp_consProCreaWeb"

                er.Interror = 3
                er.Message = ex.Message

                agrega_err(3, msg, errores)
                IErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_consProCreaWeb")

            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

            Return proveedor

        End Function
        Public Sub graba_error(ByVal errores As List(Of Errores), ByVal err As Errores, ByVal llaveCfd As llave_cfd, ByVal msgCve As String, ByVal procedimiento As String)
            Dim sqlAdapter = New SqlDataAdapter("sp_gerror_xml", Conexion)
            Dim ds As New DataSet
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            Dim errMsg As String = ""
            If err.Message.Length > 50 Then
                errMsg = err.Message.Substring(0, 50)
            End If
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_error", err.Interror)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@msg_cve", msgCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@msg_adicional", errMsg)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@procedimiento", procedimiento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                If Conexion.State = ConnectionState.Closed Then
                    Conexion.Open()
                End If
                sqlAdapter.Fill(ds, "sp_gerror_xml")
                sqlAdapter.Dispose()
                Conexion.Close()
                ds.Reset()
            Catch ex As Exception


                Dim msg As String
                msg = "sp_cons_errores_xml"
                agrega_err(IErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub
        Public Sub ConsInsertaXml(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal ccTipo As String, ByVal ccCve As String, ByVal rfcReceptor As String, ByVal tipdocCve As String)
            Dim sqlAdapter = New SqlDataAdapter("sp_inserta_xml", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            Dim er As New Errores
            Dim ds As New DataSet
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id", "WEB")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_cf", llaveCfd.version_nom)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", llaveCfd.nom_arch)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", ccCve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", ccTipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", rfcReceptor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_doc", tipdocCve)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_inserta_xml")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                IErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                Dim sw As Integer
                If IErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                    llaveCfd.ef_cve = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                    sw = CType(ValidacionEncabezado.Rows(0).Item("sw_cfd"), Integer)
                End If
                If sw = 3 Then
                    llaveCfd.sw_sin_addenda = 1
                End If

                If tipdocCve = "BTFSER" And tipdocCve = "BTCOM" Then 'siempre sera como captura para hacer efecto en tablas temporales
                    llaveCfd.sw_sin_addenda = 1
                End If

                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
                If IErrorG = 60000 Then
                    agrega_err(1, "Usted subió un archivo con este folio y serie, consulte seguimiento", errores)
                End If
                If IErrorG = 60057 Then
                    agrega_err(1, "Factura incorrecta, ya existen folios posteriores", errores)
                End If
                If IErrorG <> 0 Then
                    agrega_err(IErrorG, "", errores, IErrorG.ToString())
                End If
            Catch ex As Exception
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
                Dim msg As String
                msg = "sp_inserta_xml"
                IErrorG = 3
                er.Interror = IErrorG
                er.Message = ex.Message
                graba_error(errores, er, llaveCfd, "0", "sp_inserta_xml")
                agrega_err(IErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub
        Public Sub ValidaXSDLinq_SNAdd(ByVal errores As List(Of Errores), ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            Dim xmlElm As XDocument
            xmlElm = XDocument.Load(xmlDocFilePath)
            Dim schemas As XmlSchemaSet = New XmlSchemaSet()

            Dim leyendasFiscales As Integer = 0
            Dim detallista As Integer = 0
            Dim registrofiscal As Integer = 0
            Dim impuestosLocales As Integer = 0
            Dim donatarias As Integer = 0
            Dim valesdedespensa As Integer = 0
            Dim aerolineas As Integer = 0
            'GCM 27102014 agregue venta vehiculos
            Dim VentaVehiculos As Integer = 0
            'GCM 21012015 agregue terceros
            Dim Terceros As Integer = 0
            'JPO 29-06-2016 AGREGAMOS IEP
            Dim iep As Integer = 0

            Dim qList1 = From xe In xmlElm.Descendants _
               Select xe

            Dim xElements As IEnumerable(Of XElement) = If(TryCast(qList1, List(Of XElement)), qList1.ToList())
            If xElements.Any(Function(xe) xe.Name.LocalName = "LeyendasFiscales") Then
                leyendasFiscales = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "detallista") Then
                detallista = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "CFDIRegistroFiscal") Then
                registrofiscal = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "ImpuestosLocales") Then
                impuestosLocales = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "Donatarias") Then
                donatarias = 1
            End If

            If xElements.Any(Function(xe) xe.Name.LocalName = "ValesDeDespensa") Then
                valesdedespensa = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "Aerolineas") Then
                aerolineas = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "VentaVehiculos") Then
                VentaVehiculos = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "PorCuentadeTerceros") Then
                Terceros = 1
            End If
            If xElements.Any(Function(xe) xe.Name.LocalName = "acreditamientoIEPS") Then
                iep = 1
            End If

            'esto es para remover la addenda
            For Each a In From a1 In xmlElm.Root.Elements Where a1.Name.LocalName = "Addenda"
                a.Remove()
            Next

            Dim xsd22 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/CFD_Skytex-2.2.xsd"))
            Dim xsd32 As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdv32.xsd"))
            Dim xsdTdf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/TimbreFiscalDigital.xsd"))
            Dim xsdLf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/leyendasFisc.xsd"))
            Dim xsdDet As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/detallista.xsd"))
            Dim xsdRegf As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/cfdiregistrofiscal.xsd"))
            Dim xsdImp As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/implocal.xsd"))
            Dim xsdDon As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/donat11.xsd"))
            Dim xsdVal As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/valesdedespensa.xsd"))
            Dim xsdAer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/aerolineas.xsd"))
            Dim xsdVtaV As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/ventavehiculos11.xsd"))
            Dim xsdTer As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/terceros11.xsd"))
            Dim xsdIep As XElement = XElement.Load(HttpContext.Current.Server.MapPath("~/App_Data/AcreditamientoIEPS10.xsd"))

            schemas.Add("http://www.sat.gob.mx/cfd/2", xsd22.CreateReader)
            schemas.Add("http://www.sat.gob.mx/cfd/3", xsd32.CreateReader)
            schemas.Add("http://www.sat.gob.mx/TimbreFiscalDigital", xsdTdf.CreateReader)
            If leyendasFiscales = 1 Then
                schemas.Add("http://www.sat.gob.mx/leyendasFiscales", xsdLf.CreateReader)
            End If
            If detallista = 1 Then
                schemas.Add("http://www.sat.gob.mx/detallista", xsdDet.CreateReader)
            End If
            If registrofiscal = 1 Then
                schemas.Add("http://www.sat.gob.mx/registrofiscal", xsdRegf.CreateReader)
            End If
            If impuestosLocales = 1 Then
                schemas.Add("http://www.sat.gob.mx/implocal", xsdImp.CreateReader)
            End If
            If donatarias = 1 Then
                schemas.Add("http://www.sat.gob.mx/donat", xsdDon.CreateReader)
            End If

            If valesdedespensa = 1 Then
                schemas.Add("http://www.sat.gob.mx/valesdedespensa", xsdVal.CreateReader)
            End If
            If aerolineas = 1 Then
                schemas.Add("http://www.sat.gob.mx/aerolineas", xsdAer.CreateReader)
            End If
            If VentaVehiculos = 1 Then
                schemas.Add("http://www.sat.gob.mx/ventavehiculos", xsdVtaV.CreateReader)
            End If
            If Terceros = 1 Then
                schemas.Add("http://www.sat.gob.mx/terceros", xsdTer.CreateReader)
            End If
            If iep = 1 Then
                schemas.Add("http://www.sat.gob.mx/acreditamiento", xsdIep.CreateReader)
            End If

            xmlElm.Validate(schemas, Function(s As Object, e As ValidationEventArgs) XsdErr(s, e, errores, llaveCfd), True)

            If errores.Count > 0 Then
                Dim er As New Errores
                er.Interror = 1
                er.Message = ""
                graba_error(errores, er, llaveCfd, "60068", "XSDErr")
            End If
        End Sub
        Private Function XsdErr(ByVal sender As Object, ByVal e As ValidationEventArgs, ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd) As Errores
            Dim er As New Errores

            iErrorG = 1
            er.Interror = 1
            er.Message = e.Message
            errores.Add(er)

            Return er
        End Function
        Public Function ExecuteScalar() As Object

        End Function

        Public Sub LeeDatosFacturaLINQ_SNAdd(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal xmlDocFilePath As String, ByVal llaveCfd As llave_cfd)

            'GCM 12112014 paraobtener el IVA
            Dim cmd As New SqlCommand
            Dim pctIva As Object
            cmd.CommandText = "select pct_iva from iciva where iva_cve = 1"
            cmd.CommandType = CommandType.Text
            cmd.Connection = Conexion
            Conexion.Open()
            pctIva = cmd.ExecuteScalar()
            Conexion.Close()
            comprobante.pctIva = pctIva

            Dim xmlElm As XElement
            Try
                xmlElm = XElement.Load(xmlDocFilePath)
                Dim root As XElement = XElement.Load(xmlDocFilePath)
                Dim qAtrib As IEnumerable(Of XAttribute) = _
                    From atr In root.Attributes() _
                    Select atr
                'GCM 23102014 
                Dim qList1 = From xe In xmlElm.Descendants _
                Select xe
                ''GCM 23102014 

                'esto es para remover la addenda
                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Addenda" Then
                        a.Remove()
                    End If
                Next

                'GCM 23102014 obtenemos el valor de total de traslados
                For Each xe In qList1
                    If xe.Name.LocalName = "ImpuestosLocales" Then
                        comprobante.TotaldeTraslados = Decimal.Parse(xe.Attribute("TotaldeTraslados").Value)
                        comprobante.TotaldeRetenciones = Decimal.Parse(xe.Attribute("TotaldeRetenciones").Value) 'GCM 11112014 obtenemos el valor de total de retenciones
                    End If
                Next
                'GCM 23102014 

                ''GCM 11112014 obtenemos el valor de total de retenciones
                'For Each xe In qList1
                '    If xe.Name.LocalName = "ImpuestosLocales" Then
                '        comprobante.TotaldeRetenciones = Decimal.Parse(xe.Attribute("TotaldeRetenciones").Value)
                '    End If
                'Next
                ''GCM 11112014 

                For Each a In xmlElm.Elements
                    If a.Name.LocalName = "Complemento" Then
                        a.Remove()
                    End If
                Next

                If llaveCfd.version_nom <> "" Then
                    comprobante.version = llaveCfd.version
                End If
                If IsNothing(root.Attribute("serie")) Then
                    comprobante.serie = ""
                Else
                    comprobante.serie = root.Attribute("serie").Value.ToString()
                End If
                'folio 
                If IsNothing(root.Attribute("folio")) Then
                    comprobante.folio_factura = 0
                Else
                    'comprobante.folio_factura = Int64.Parse(root.Attribute("folio").Value)
                    Dim folPaso = Int64.Parse(Regex.Replace(root.Attribute("folio").Value, "[^0-9]", "")) 'Int32.Parse(root.Attribute("folio").Value)
                    If folPaso >= 2147483647 Then
                        'comprobante.folio_factura = Int64.Parse(folPaso.ToString())
                        comprobante.folio_factura = Int64.Parse(folPaso.ToString().Substring(0, 8))
                    Else
                        comprobante.folio_factura = folPaso
                    End If
                End If
                'fecha
                If IsNothing(root.Attribute("fecha")) Then
                    agrega_err(1, "No puede leerse el dato fecha", errores)
                Else
                    comprobante.fecha_factura = root.Attribute("fecha").Value.ToString()
                End If
                'fecha
                If IsNothing(root.Attribute("sello")) Then
                    agrega_err(1, "No puede leerse el dato sello", errores)
                Else
                    comprobante.sello = root.Attribute("sello").Value.ToString()
                End If
                If llaveCfd.version_nom = "CFD" Then
                    ' numero de aprobacion
                    If IsNothing(root.Attribute("noAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato noAprobacion", errores)
                    Else
                        comprobante.no_aprobacion = Integer.Parse(root.Attribute("noAprobacion").Value)
                    End If
                    ' año de aprobacion
                    If IsNothing(root.Attribute("anoAprobacion")) Then
                        agrega_err(1, "No puede leerse el dato anoAprobacion", errores)
                    Else
                        comprobante.ano_aprobacion = Integer.Parse(root.Attribute("anoAprobacion").Value)
                    End If
                End If
                If IsNothing(root.Attribute("noCertificado")) Then
                    agrega_err(1, "No puede leerse el dato noCertificado", errores)
                Else
                    comprobante.no_certificado = root.Attribute("noCertificado").Value.ToString()
                End If
                If Not IsNothing(root.Attribute("certificado")) Then
                    comprobante.certificado = root.Attribute("certificado").Value.ToString()
                Else
                    comprobante.certificado = ""
                End If
                ' subtotal
                If IsNothing(root.Attribute("subTotal")) Then
                    agrega_err(1, "No puede leerse el dato subTotal", errores)
                Else
                    comprobante.sub_total = Decimal.Parse(root.Attribute("subTotal").Value)
                End If
                ' descuento
                If IsNothing(root.Attribute("descuento")) Then
                    'agrega_err(1, "No puede leerse el dato descuento", errores)
                    comprobante.descuento = 0 'el descuento se manda en 0 en caso de no haber atributo 14/08/2013
                Else
                    comprobante.descuento = Decimal.Parse(root.Attribute("descuento").Value)
                End If
                ' total
                If IsNothing(root.Attribute("total")) Then
                    agrega_err(1, "No puede leerse el dato total", errores)
                Else
                    comprobante.total = Decimal.Parse(root.Attribute("total").Value)
                End If
                ' tipo de comprobante
                If IsNothing(root.Attribute("tipoDeComprobante")) Then
                    agrega_err(1, "No puede leerse el dato tipoDeComprobante", errores)
                Else
                    Dim tipoDeComprobante = root.Attribute("tipoDeComprobante").Value.ToString()
                End If
                'GCM 05112014 Se agrego el TC
                If IsNothing(root.Attribute("TipoCambio")) Then
                    comprobante.tc = "0"
                Else
                    comprobante.tc = root.Attribute("TipoCambio").Value.ToString
                End If

                'Cambio para CDFI 3.3 FGV (07/08/2017)
                If comprobante.tipodoc_cve = "BTFSER" Or comprobante.tipodoc_cve = "BTCOM" Then
                    If IsNothing(root.Attribute("Moneda")) Then
                        comprobante.moneda = ""
                    Else
                        'enviar el numero del tipo de moneda al sp
                        comprobante.moneda = root.Attribute("Moneda").Value.ToString()
                    End If
                End If



                Dim qList = From xe In xmlElm.Descendants _
                Select xe
                Dim emisor = New emisor
                Dim receptor = New receptor
                Dim conce As New List(Of concepto)
                Dim impuestos = New impuestos
                Dim traslados = New List(Of traslado)
                Dim retenciones = New List(Of retencion)
                Dim addenda = New addenda
                Dim requesForPayment = New requestforpayment
                Dim currency = New currency
                Dim aditionalData = New aditionaldata
                Dim paymentTimePeriod = New paymenttimeperiod
                Dim provider = New provider
                Dim lineItems As New List(Of lineitem)
                Const swCargosConcep As Boolean = False
                Dim errorConceptos As Boolean = False
                impuestos.total_imp_trasl = 0
                impuestos.sw_retencion = 0
                impuestos.total_imp_reten = 0
                Dim swTotalCapT As Boolean = False
                Dim swTotalCapR As Boolean = False
                Dim swTImporte As Boolean = False
                Dim Tot_ImpuestosTras As Decimal = 0
                comprobante.swImpTras = True
                comprobante.swTImporte = False

                For Each xe In qList
                    If xe.Name.LocalName = "Emisor" Then
                        emisor.rfc = xe.Attribute("rfc").Value.ToString()
                    End If
                    If xe.Name.LocalName = "Receptor" Then
                        receptor.rfc = xe.Attribute("rfc").Value.ToString()
                    End If

                    If xe.Name.LocalName = "Impuestos" Then
                        If IsNothing(xe.Attribute("totalImpuestosTrasladados")) Then

                            comprobante.totalImpuestosTrasladados = 0
                            comprobante.swImpTras = False
                        Else

                            comprobante.totalImpuestosTrasladados = xe.Attribute("totalImpuestosTrasladados").Value.ToString()
                        End If
                        If IsNothing(xe.Attribute("totalImpuestosRetenidos")) Then 'GCM 19112015 si trae totalimpuestosretenidos marcamos error
                            If errorConceptos = False Then
                                errorConceptos = False
                            End If
                        Else
                            If CType(xe.Attribute("totalImpuestosRetenidos"), Decimal) > 0 Then
                                agrega_err(1, "Factura contiene Impuestos Retenidos", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If

                    If xe.Name.LocalName = "Concepto" And errorConceptos = False Then
                        Dim itemComceptos = New concepto
                        itemComceptos.cantidad = CType(xe.Attribute("cantidad"), Decimal)
                        itemComceptos.valor_unitario = CType(xe.Attribute("valorUnitario"), Decimal)
                        itemComceptos.importe = CType(xe.Attribute("importe"), Decimal)
                        conce.Add(itemComceptos)
                        If itemComceptos.cantidad = 0 And itemComceptos.valor_unitario > 0 Then
                            If comprobante.tipodoc_cve = "BTFSER" Then 'GCM 05082015 Se agrego la excepcion a btfser 
                                errorConceptos = False 'no aplica ya que hay facturas como esta que no maneja cantidad.
                            Else
                                agrega_err(1, "Si la cantidad es 0, no debe agregar valor unitario", errores)
                                errorConceptos = True
                            End If
                        End If

                        If swCargosConcep = True And errorConceptos = False Then
                            If itemComceptos.cantidad <> 1 Then
                                agrega_err(1, "La cantidad siempre debe ser 1 en el cargo extra", errores)
                                errorConceptos = True
                            End If
                        End If
                    End If
                    If xe.Name.LocalName = "Retencion" Then ' And swTotalCapR = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            impuestos.total_imp_reten = impuestos.total_imp_reten + CType(xe.Attribute("importe"), Decimal)
                            Dim itemRet = New retencion
                            itemRet.impuesto = CType(xe.Attribute("impuesto"), String)
                            itemRet.importe = CType(xe.Attribute("importe"), Decimal)
                            retenciones.Add(itemRet)
                            If impuestos.total_imp_reten > 0 Then
                                impuestos.sw_retencion = 1
                            End If

                        Else
                            impuestos.total_imp_trasl = 0
                        End If
                    End If

                    If xe.Name.LocalName = "Traslado" Then 'And  swTotalCapT = False Then
                        If Not IsNothing(xe.Attribute("importe")) Then
                            Dim itemTras = New traslado
                            If xe.ToString().Contains("terceros") Then 'JPO / 09-06-2016 / : se agrego esta condicion para evitar que tome en cuenta el impuesto del concepto llamado Traslado Terceros
                                ' Dim val As String

                                'impuestos.total_imp_trasl = CType(xe.Attribute("importe"), Decimal)

                            Else
                                ' impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("importe"), Decimal)
                                impuestos.total_imp_trasl = CType(xe.Attribute("importe"), Decimal)
                                itemTras.impuesto = CType(xe.Attribute("impuesto"), String)
                            End If
                            'If IsNothing(xe.Attribute("terceros")) Then 'JPOH 09/06/2016 se anexo esta condicion para evitar que duplique el impuesto que contiene el concepto Terceros

                            '    impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("importe"), Decimal)
                            '    comprobante.swTImporte = True
                            '    If CType(xe.Attribute("tasa"), Decimal) > 0 And CType(xe.Attribute("importe"), Decimal) > 0 Then
                            '        itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                            '        itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            '    Else
                            '        itemTras.tasa = 0
                            '        itemTras.importe = 0
                            '    End If
                            'Else
                            'impuestos.total_imp_trasl = impuestos.total_imp_trasl + CType(xe.Attribute("importe"), Decimal)
                            comprobante.swTImporte = True


                            'itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                            'itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            If CType(xe.Attribute("tasa"), Decimal) > 0 And CType(xe.Attribute("importe"), Decimal) > 0 Then
                                itemTras.tasa = CType(xe.Attribute("tasa"), Decimal)
                                itemTras.importe = CType(xe.Attribute("importe"), Decimal)
                            Else
                                itemTras.tasa = 0
                                itemTras.importe = 0
                            End If
                            'End If
                            traslados.Add(itemTras)
                        End If
                    End If



                Next
                If conce.Count = 0 Then
                    agrega_err(1, "El comprobante no tiene conceptos", errores)
                    errorConceptos = True
                End If
               
                aditionalData.text_data = "Z"
                provider.providerid = ""
                comprobante.Conceptos = conce
                requesForPayment.provider = provider
                requesForPayment.line_items = lineItems
                requesForPayment.currency = currency
                requesForPayment.aditional_data = aditionalData
                requesForPayment.paymenttimeperiod = paymentTimePeriod
                addenda.requestforpayment = requesForPayment
                comprobante.Emisor = emisor
                comprobante.Receptor = receptor
                impuestos.Traslados = traslados
                impuestos.Retenciones = retenciones
                comprobante.Impuestos = impuestos
                comprobante.Addenda = addenda
                Dim msgUsr = From msg In errores _
                    Select interror = msg.Interror, message = msg.Message _
                    Where (interror = 1)
                Dim er As New Errores
                Dim cadena As String = msgUsr.Aggregate("", Function(current, msgs) current & msgs.message.Trim + ", ")
                If cadena.Trim <> "" Then
                    er.Interror = 1
                    er.Message = cadena.Trim
                    graba_error(errores, er, llaveCfd, "60069", "LeeDatosFacturaLINQ_SinAddenda")
                End If
            Catch ex As Exception
                'agrega_err(1, ex.Message, errores)
                agrega_err(1, "Ocurrio un erro al leer el XML por favor contacte con el administrador de sitio ", errores, "60069")
            End Try

        End Sub
        Public Function lee_ef_cve(ByVal rfcReceptor As String) As Dictionary(Of String, String)
            Dim er As New Errores
            Dim ds As New DataSet
            Dim dicEmpresa = New Dictionary(Of String, String)
            Dim sqlAdapter = New SqlDataAdapter("sp_qefcve", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", rfcReceptor)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_qefcve")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                Dim iCount = ValidacionEncabezado.Rows.Count()
                If iCount > 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString()
                    Dim empresa As String = ValidacionEncabezado.Rows(0).Item("empresa").ToString().Trim()
                    dicEmpresa.Add(EfCveG, empresa)
                End If
                Conexion.Close()
                ds.Reset()
            Catch ex As Exception
                Dim msg = "sp_qefcve:" + rfcReceptor
                iErrorG = 3
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            Return dicEmpresa
        End Function
        Public Function RevisaIntTasa(ByVal tasa As String) As Boolean
            Dim sw As Boolean = True

            If IsNumeric(tasa) Then
                If CInt(tasa) Then
                    sw = True
                Else
                    sw = False
                End If
            End If

            Return sw
        End Function
        Public Sub ValidaTotales_SNAdd(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal LlaveCfd As llave_cfd, ByVal decimales_truncados As Integer, ByVal decimales As Integer)

            Dim mtoDescGlobal As Decimal = 0
            Dim minimoTotal As Decimal
            Dim maximoTotal As Decimal
            Dim tasaVarIva As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIva As Decimal = 0
            Dim pctIva As Decimal = 0

            '24022015 GCM se contempla el rango max y min del query del servidor
            'Dim cmax As New SqlCommand
            'Dim Excepmax As Object
            'cmax.CommandText = "select convert(decimal(19,4),prm15) from xcdconapl_cl where tipdoc_cve = 'xmlcfd' and sp_cve = 'webconfig' and spd_cve = 'rangoMinMax'"
            'cmax.CommandType = CommandType.Text
            'cmax.Connection = Conexion
            'Conexion.Open()
            'Excepmax = cmax.ExecuteScalar()
            'Conexion.Close()

            Dim iva = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "IVA")
            'GCM 12112014 Agrego para contemplar caso totalimpuestos
            Dim noiva = Aggregate cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "IVA")
                          Into Count()
            'GCM 12112014 Agrego para contemplar caso totalimpuestos

            If noiva > 0 Then

                For Each i In iva
                    Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                    If swtasa = True Then
                        tasaVarIva = i.tasa / 100 'FormatNumber(i.tasa / 100, decimales)
                    Else
                        tasaVarIva = i.tasa
                    End If
                    importeIva = importeIva + i.importe  'FormatNumber(i.importe, decimales)
                Next
            Else
                importeIva = comprobante.totalImpuestosTrasladados
                pctIva = comprobante.pctIva / 100
            End If
            
            Dim ieps = From cons In comprobante.Impuestos.Traslados
                      Select cons.tasa, cons.importe, cons.impuesto
                      Where (impuesto = "IEPS")

            Dim noieps = Aggregate cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "IEPS")
                          Into Count()
            Dim tasaIeps As Decimal = 0 '= comprobante.Impuestos.Traslados.tasa / 100
            Dim importeIeps As Decimal = 0
            For Each i In ieps
                Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                If swtasa = True Then
                    tasaIeps = i.tasa / 100 ' FormatNumber(i.tasa / 100, decimales)
                Else
                    tasaIeps = i.tasa
                    End If
                importeIeps = importeIeps + i.importe ' FormatNumber(i.importe, decimales)
            Next

            Dim retenciones = From cons In comprobante.Impuestos.Retenciones
                    Select cons.importe, cons.impuesto
                    Where (impuesto = "IVA")

            Dim retenImporteIva As Decimal = 0
            For Each i In retenciones
                retenImporteIva = retenImporteIva + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim retisr = From cons In comprobante.Impuestos.Retenciones
                  Select cons.importe, cons.impuesto
                  Where (impuesto = "ISR")
            Dim retenImporteIsr As Decimal = 0
            'verificar impuestos 
            For Each i In retisr
                retenImporteIsr = retenImporteIsr + i.importe 'FormatNumber(i.importe, decimales)
            Next
            Dim errorCfd As Integer = 0
            Dim qryResultConceptos = From com In comprobante.Conceptos _
                       Select comprobante.Conceptos
            Dim concep As List(Of concepto) = Nothing
            For Each concep In qryResultConceptos
            Next
            Dim qryResLine = From com2 In concep _
                Let sub_total = com2.cantidad * com2.valor_unitario _
                Select sub_total, com2.importe, com2.no_identificacion, com2.cantidad, com2.valor_unitario
            Dim qry_tot_con = From com2 In concep _
               Let tot_con = com2.importe _
               Select com2.importe, com2.no_identificacion
            Dim cant_concep = Aggregate com2 In qryResLine _
                              Into Sum(com2.cantidad)
            Dim val_unit = From com2 In qryResLine _
                            Select com2.valor_unitario _
                            Distinct
            If errorCfd = 0 Then
                    '------------------------------------------ Validar los descuentos
                mtoDescGlobal = FormatNumber(comprobante.descuento, decimales) ' Monto global de descuento
                End If
            Dim subtotalComprobante = FormatNumber(comprobante.sub_total, decimales_truncados)
            Dim subtotalConceptos = From com4 In concep _
                                     Let sub_total = com4.cantidad * com4.valor_unitario _
                                     Select sub_total, com4.importe, com4.no_identificacion
            Dim Subt_c = subtotalComprobante
                'GCM 11112014 Comentado para no validar sub-total
            Dim info = Aggregate com4 In subtotalConceptos _
                          Into Sum(com4.sub_total)
                'Subt_c = FormatNumber(Round(Subt_c, decimales_truncados, MidpointRounding.AwayFromZero), decimales)
                'If errorCfd = 0 Then 
                '    minimoTotal = subtotalComprobante - 0.5
                '    maximoTotal = subtotalComprobante + 0.5
                '    If Subt_c < minimoTotal Or Subt_c > maximoTotal Then
                '        errorCfd = 1
                '    End If
                'End If
            Dim totalComprobante = FormatNumber(comprobante.total, decimales_truncados)
            Dim traslComprobanteTotImp = FormatNumber(Round(((importeIva + importeIeps)), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            Dim traslConceptos = traslComprobanteTotImp
            importeIva = Math.Round(importeIva, 2)
            importeIeps = Math.Round(importeIeps, 2)
            Dim TotaldeTraslados = comprobante.TotaldeTraslados 'GCM 23102014 agrego total de traslado
            Dim TotaldeRetenciones = comprobante.TotaldeRetenciones
            'GCM 16102014 Dim traslConceptos = FormatNumber(Round(((Subt_c - mtoDescGlobal) * tasaVarIva), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            Dim totImpRet = (retenImporteIva + retenImporteIsr)
            'GCM 16102014 comentado para ya no validar iva
            'If errorCfd = 0 Then


            '    If errorCfd = 0 Then
            '        minimoTotal = CType((traslConceptos - 0.5), Decimal)
            '        maximoTotal = CType((traslConceptos + 0.5), Decimal)

            '        If traslComprobanteTotImp < minimoTotal Or traslComprobanteTotImp > maximoTotal Then
            '            errorCfd = 4
            '        End If

            '    End If

            'End If
            'GCM 13112014 Agrego la separacion de ieps e iva
            Dim totalConceptos = FormatNumber(Round((Subt_c - mtoDescGlobal + traslConceptos - totImpRet + TotaldeTraslados - TotaldeRetenciones), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
            If errorCfd = 0 Then
                minimoTotal = totalConceptos - 0.8
                maximoTotal = totalConceptos + 0.8

                If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
                    totalConceptos = FormatNumber(Round((Subt_c - mtoDescGlobal + importeIva - totImpRet + TotaldeTraslados - TotaldeRetenciones), decimales_truncados, MidpointRounding.AwayFromZero), decimales)

                    minimoTotal = totalConceptos - 0.8
                    maximoTotal = totalConceptos + 0.8

                    If totalComprobante < minimoTotal Or totalComprobante > maximoTotal Then
                        errorCfd = 5
                    End If
                End If
                If errorCfd = 0 Then
                    comprobante.total_Valido = totalConceptos
                End If
            End If


            'en caso de que no exista IVA e IEPS
            If errorCfd = 0 Then
                If noiva = 0 And noieps = 0 Then
                    'Dim subTotalCalculado = FormatNumber(Round((Subt_c - mtoDescGlobal + traslConceptos - totImpRet + TotaldeRetenciones), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
                    Dim subTotalCalculado = FormatNumber(Round((Subt_c - mtoDescGlobal - totImpRet + TotaldeRetenciones), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
                    Dim totalImpuestos As Decimal
                    If importeIva = 0 Then
                        totalImpuestos = 0
                    Else
                        totalImpuestos = Math.Round((subTotalCalculado - comprobante.descuento) * (comprobante.pctIva / 100), 2)
                    End If

                    'minimoTotal = totalImpuestos - 0.5
                    'maximoTotal = totalImpuestos + 0.5

                    If totalImpuestos = importeIva Or importeIva = 0 Then
                        comprobante.total_Valido = subTotalCalculado + totalImpuestos
                        comprobante.totalIEPS = totalImpuestos
                    Else
                        errorCfd = 7
                    End If
                End If
            End If

            'en caso de que no exista IVA
            If errorCfd = 0 Then
                If noiva = 0 And noieps >= 1 Then
                    Dim subTotalCalculado = totalConceptos - importeIva
                    Dim totalImpuestos = Math.Round((subTotalCalculado - comprobante.descuento) * tasaIeps, 2)
                    'minimoTotal = totalImpuestos - 0.5
                    maximoTotal = Decimal.Parse(totalImpuestos.ToString().Substring(0, totalImpuestos.ToString().Length - 1))
                    
                    If totalImpuestos = importeIeps Then
                        'comprobante.total_Valido = subTotalCalculado + totalImpuestos
                        comprobante.total_Valido = subTotalCalculado + maximoTotal
                    Else
                        errorCfd = 7
                    End If
                End If
            End If

            'en caso de que EXISTA IVA
            If errorCfd = 0 Then
                If noiva >= 1 And noieps = 0 Then
                    If importeIva > 0 Then
                        Dim subTotalCalculado = totalConceptos - importeIva '246.55
                        Dim totalImpuestos = Math.Round((Subt_c) * (comprobante.pctIva / 100), 2)
                        minimoTotal = totalImpuestos - 0.05
                        maximoTotal = totalImpuestos + 0.05

                        'If importeIva > minimoTotal And importeIva < maximoTotal Then
                        comprobante.total_Valido = subTotalCalculado + importeIva
                        'comprobante.total_Valido = subTotalCalculado
                        'Else
                        'errorCfd = 7

                        'End If
                    Else
                        Dim subTotalCalculado = totalConceptos - importeIva
                        comprobante.total_Valido = subTotalCalculado + importeIva
                    End If
                End If
                End If

                'en caso de que EXISTA IVA e IEPS
                If errorCfd = 0 Then
                    If noiva >= 1 And noieps >= 1 Then
                        Dim subTotalCalculado2 = FormatNumber(Round((Subt_c - mtoDescGlobal - totImpRet + TotaldeRetenciones), decimales_truncados, MidpointRounding.AwayFromZero), decimales)
                        Dim totalFactura As Decimal
                        totalFactura = CStr(totalComprobante)
                        totalFactura = FormatNumber(Round(totalFactura), 2)
                        Dim subTotalIVA = comprobante.sub_total + importeIva
                        minimoTotal = subTotalIVA - 0.05
                        maximoTotal = subTotalIVA + 0.05

                        If totalFactura > minimoTotal And totalFactura < maximoTotal Then
                            comprobante.total_Valido = subTotalIVA
                        Else
                            'Dim subTotalCalculado = totalConceptos - importeIva  '1590.56
                            Dim totalImpuestosIEPS = comprobante.sub_total + importeIeps + importeIva '1590.56
                            'minimoTotal = totalImpuestos - 0.5
                            'maximoTotal = totalImpuestos + 0.5

                            'If totalImpuestosIEPS = totalConceptos Then
                            comprobante.total_Valido = totalImpuestosIEPS
                            'Else
                            'errorCfd = 7
                            'End If
                        End If
                    End If
                End If


                'GCM 14112014 para calcular el iva cuando no hay 
                'If errorCfd = 0 Then
                '    If noiva = 0 Then
                '        Dim totalImpuestos = Math.Round((comprobante.sub_total - comprobante.descuento) * tasaIeps, 2)
                '        minimoTotal = totalImpuestos - 0.5
                '        maximoTotal = totalImpuestos + 0.5

                '        If importeIeps < minimoTotal Or importeIeps > maximoTotal Then
                '            errorCfd = 7
                '        Else
                '            comprobante.porcentajeIVA = (tasaIeps * 100)
                '            comprobante.totalIEPS = importeIeps
                '        End If
                '    End If
                'End If

                'If errorCfd = 0 Then
                '    ''If comprobante.swTImporte = False And comprobante.swImpTras = True Then
                '    If comprobante.swTImporte = True And comprobante.swImpTras = True Then
                '        If comprobante.total <> comprobante.sub_total Then
                '            Dim TotalImpuestosTras = Math.Round((comprobante.sub_total - comprobante.descuento) * (comprobante.pctIva / 100), 2)

                '            minimoTotal = TotalImpuestosTras - 0.5
                '            maximoTotal = TotalImpuestosTras + 0.5

                '            'minimoTotal = TotalImpuestosTras - 0.8
                '            'maximoTotal = TotalImpuestosTras + 0.8

                '            If comprobante.totalImpuestosTrasladados < minimoTotal Or comprobante.totalImpuestosTrasladados > maximoTotal Then
                '                errorCfd = 7
                '            Else
                '                comprobante.porcentajeIVA = TotalImpuestosTras
                '            End If
                '        End If
                '    End If
                'End If

                Dim msg As String = ""
                Dim er As New Errores
                If errorCfd > 0 Then
                    er.Interror = 1

                End If
                Select Case errorCfd
                    'GCM 11112014 comentado para ya no validar subtotal
                    'Case 1
                    '    msg = "El subtotal de la factura es incorrecto"
                    '    er.Message = msg
                    '    graba_error(errores, er, LlaveCfd, "60063", "ValidaTotales_SNAdd")
                    Case 2
                        msg = "El porcentaje de descuento en LineItems y monto de descuento del CFD no coinciden"
                        er.Message = msg
                        graba_error(errores, er, LlaveCfd, "60064", "ValidaTotales_SNAdd")
                    Case 3, 5
                        msg = "El total de la factura es incorrecto"
                        er.Message = msg
                        graba_error(errores, er, LlaveCfd, "60065", "ValidaTotales_SNAdd")
                        'GCM 16102014 comentado para ya no validar iva
                        'Case 4
                        '    msg = "No coincide el iva"
                        '    er.Message = msg
                        '    graba_error(errores, er, LlaveCfd, "60066", "ValidaTotales_SNAdd")
                    Case 6
                        msg = "No coincide cantidad, valor unitario de Concepto con uns y precio de lineItem  "
                        er.Message = msg
                        graba_error(errores, er, LlaveCfd, "60067", "ValidaTotales_SNAdd")
                    Case 7
                        msg = "Error en total impuesto trasladado"
                        er.Message = msg
                        graba_error(errores, er, LlaveCfd, "60125", "ValidaTotales_SNAdd")
                    Case Else
                        msg = ""
                End Select
                If errorCfd > 0 Then
                    agrega_err(1, msg, errores)
                End If
        End Sub
        Public Sub LeerErroresSql(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd)
            Dim ds As New DataSet
            Dim sqlAdapter = New SqlDataAdapter("sp_cons_errores_xml", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_cons_errores_xml")
                sqlAdapter.Dispose()
                Dim errorTable = ds.Tables.Item(0)

                For Each row As DataRow In errorTable.Rows
                    Dim rfcEmisor As String = CStr(row("rfc_emisor"))
                    Dim serie = CStr(row("serie"))
                    Dim folioFactura = CStr(row("folio_factura"))
                    Dim tipoError As Integer = CType(row("tipo_error"), Integer)
                    Dim msgCve As String = CStr(row("msg_cve"))
                    Dim mensajeErr As String = CStr(row("mensaje"))
                    Dim msgAdicional As String = CStr(row("msg_adicional"))
                    If tipoError = 1 Then
                        agrega_err(1, mensajeErr + " " + msgAdicional, errores)
                    End If

                    If tipoError = 2 Or tipoError = 3 Then
                        'Dim sCadena As String = mensajeErr + " " + msgAdicional + " ," + msgCve + " Tipo:" + tipoError.ToString _
                        '                       + vbNewLine + "RFC Emisor:" + rfcEmisor _
                        '                       + vbNewLine + "Serie:" + serie _
                        '                       + vbNewLine + "Folio:" + folioFactura

                        Dim sCadena As String = mensajeErr + " " + msgAdicional + vbNewLine
                        agrega_err(tipoError, sCadena, errores, msgCve)
                    End If
                    If msgCve = 60000 Then
                        agrega_err(1, "Su comprobante CFD no fue recibido, por favor consulte con Soporte CFD", errores)
                    End If
                    If msgCve = 60057 Then
                        agrega_err(1, "Factura incorrecta, ya existen folios posteriores", errores)
                    End If
                    If msgCve = 60051 Then
                        agrega_err(1, "", errores)
                    End If
                Next
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim msg = "sp_cons_errores_xml"
                iErrorG = 3
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

        End Sub

        Public Sub GrabaTmp(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            iErrorG = 0
            If errores.Count = 0 And iErrorG = 0 Then
                If comprobante.tipodoc_cve = "BTFACS" Or comprobante.tipodoc_cve = "BTFSER" Then
                    ValidaDatosDetalleTmpSrv(errores, llaveCfd, comprobante.Addenda.requestforpayment.document)
                Else
                    ValidaDatosDetalleTmp(errores, comprobante.Addenda.requestforpayment.line_items, llaveCfd)
                End If

            End If
            If errores.Count = 0 And iErrorG = 0 Then
                ValidaDatosEncabezadoTmp(errores, comprobante, llaveCfd)
            End If
            If iErrorG = 60089 Then
                agrega_err(1, "Error, la captura no fue procesada, el folio del comprobante ya había sido aceptado", errores, "60089")
            End If
            If iErrorG = 60090 Then
                agrega_err(1, "Error en la aplicacion, consulte con su administrador del sitio ", errores, "60090")
            End If
            If errores.Count > 0 Or iErrorG > 0 Then
                LeerErroresSql(errores, llaveCfd)
            End If
        End Sub
        Public Sub ValidaDatosEncabezadoTmp(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            Dim er As New Errores
            Dim ds As New DataSet
            Dim varIva As Decimal '= comprobante.Impuestos.Traslados.tasa / 100
            Dim varIvaImporte As Decimal
            Dim sqlAdapter = New SqlDataAdapter("sp_datos_xml_temp", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_datos_xml_temp_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            If Not (llaveCfd.ef_cve Is Nothing) And EfCveG Is Nothing Then
                EfCveG = llaveCfd.ef_cve
            End If
            If EfCveG Is Nothing Then
                EfCveG = "zzz"
            End If


            Dim iva = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.impuesto, cons.importe
                       Where (impuesto = "IVA")

            'GCM 12112014 Agrego para contemplar caso totalimpuestos
            Dim noiva = Aggregate cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "IVA")
                          Into Count()

            Dim ieps = From cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.impuesto, cons.importe
                       Where (impuesto = "IEPS")

            'GCM 12112014 Agrego para contemplar caso totalimpuestos
            Dim noieps = Aggregate cons In comprobante.Impuestos.Traslados
                       Select cons.tasa, cons.importe, cons.impuesto
                       Where (impuesto = "IEPS")
                          Into Count()

            If noiva = 0 And noieps = 0 Then
                varIva = comprobante.pctIva
                varIvaImporte = comprobante.totalIEPS
            End If

            'GCM 13112014 Agrego para contemplar caso totalimpuestos
            If noiva = 0 And noieps >= 1 Then
                'For Each i In iva
                '    Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                '    If swtasa = True Then
                '        varIva = i.tasa
                '    Else
                '        varIva = i.tasa * 100
                '    End If
                '    varIvaImporte = varIvaImporte + i.importe
                'Next
                varIva = 0
                varIvaImporte = 0
                'Else
                'varIva = comprobante.porcentajeIVA
                'varIvaImporte = comprobante.totalIEPS  'porcentaje de iva

                'varIva = comprobante.pctIva
                'varIvaImporte = comprobante.totalImpuestosTrasladados
            End If

            If noiva >= 1 And noieps = 0 Then
                For Each i In iva
                    Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                    If swtasa = True Then
                        varIva = i.tasa
                        'varIvaImporte = i.importe
                    Else
                        varIva = i.tasa * 100
                    End If
                    varIvaImporte = varIvaImporte + i.importe
                Next
            End If

            If noiva >= 1 And noieps >= 1 Then
                For Each i In iva
                    Dim swtasa = RevisaIntTasa(i.tasa.ToString())
                    If swtasa = True Then
                        varIva = i.tasa
                        'varIvaImporte = i.importe
                    Else
                        varIva = i.tasa * 100
                    End If
                    varIvaImporte = varIvaImporte + i.importe
                Next

            End If

            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", comprobante.nom_arch)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@version", comprobante.version)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", comprobante.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", comprobante.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@fecha_factura", comprobante.fecha_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sello", comprobante.sello)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_aprobacion", comprobante.no_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ano_aprobacion", comprobante.ano_aprobacion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado", comprobante.no_certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@certificado", comprobante.certificado)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@condiciones_pago", comprobante.Addenda.requestforpayment.paymenttimeperiod.timeperiod.ToString())
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@metodo_pago", comprobante.Addenda.requestforpayment.aditional_data.metododepago) 'comprobante.metodo_pago)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", comprobante.total_Valido - varIvaImporte) '- comprobante.TotaldeTraslados + comprobante.TotaldeRetenciones) 'GCM 11112014 contempla trasladados y retenciones 13112014 se contempla total original
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@descuento", 0) 'comprobante.descuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@total", comprobante.total)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", comprobante.Emisor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", comprobante.Receptor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tm", comprobante.Addenda.requestforpayment.aditional_data.moneda) 'comprobante.moneda)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@d_adicionales", comprobante.Addenda.requestforpayment.aditional_data.text_data)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_iva", varIva) 'Decimal.Parse(comprobante.Impuestos.Traslados.tasa))
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", comprobante.cc_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", comprobante.cc_tipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_Cve", llaveCfd.ef_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado_sat", llaveCfd.timbre_fiscal.no_certificado_sat)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id", "WEB")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_cf", llaveCfd.version_nom)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sw_reten_iva", comprobante.Impuestos.sw_retencion)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@iva", varIvaImporte)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@id_soludin", llaveCfd.IdSoludin)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_datos_xml_temp")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                IErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)

                If IErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                If IErrorG > 0 Then
                    agrega_err(1, "", errores)
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim msg As String
                msg = "sp_datos_xml_temp"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                IErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_datos_xml_temp")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub
        Public Sub ValidaDatosDetalleTmp(ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal llaveCfd As llave_cfd)
            Dim item = New lineitem
            For Each item In items
                ValidaDatosItemTmp(errores, item, llaveCfd)
                If errores.Count > 0 Or iErrorG > 0 Then
                    Exit For
                End If
            Next
        End Sub
        Public Sub ValidaDatosDetalleTmpSrv(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal documento As Document)
            Dim item = New lineitem
            item.reference_identification = documento.referenceIdentification
            item.type = 1
            item.number = 0
            item.monto_decuento = Decimal.Parse("0.0000")
            item.pct_decuento = Decimal.Parse("0.0000")
            item.uns = 0
            item.precio = Decimal.Parse("0.0000")
            item.sku = "zzzzzz"
            item.partida = 0
            item.art_tip = "zzz"
            item.uni_med = "zzzzzz"
            ValidaDatosItemTmp(errores, item, llaveCfd)
        End Sub
        Public Sub ValidaDatosItemTmp(ByVal errores As List(Of Errores), ByVal item As lineitem, ByVal llaveCfd As llave_cfd)

            Dim er As New Errores
            Dim ds As New DataSet
            Dim sqlAdapter = New SqlDataAdapter("sp_detalle_xml_temp", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_detalle_xml_temp_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sku_cve", item.sku)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@art_tip", item.art_tip)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_reng", item.number)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uns", item.uns)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uni_med", item.uni_med)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_descto", item.pct_decuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@mto_descto", item.monto_decuento)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@precio", item.precio)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", llaveCfd.ef_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@recepcion", item.reference_identification)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@num_rengp", item.partida)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tipo_reng", item.type)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_detalle_xml_temp")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                If iErrorG > 0 Then
                    agrega_err(1, "", errores)
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception

                Dim msg As String
                msg = "sp_detalle_xml_temp"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_detalle_xml_temp")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try

        End Sub
        Public Sub ValidaDatosEncabezadoCap(ByVal errores As List(Of Errores), ByVal comprobante As Comprobante, ByVal llaveCfd As llave_cfd)
            'Dim sqlAdapter = New SqlDataAdapter("sp_Valida_XML_zfj", Conexion)
            Dim sqlAdapter = New SqlDataAdapter("sp_Valida_XML", Conexion)
            Dim ds As New DataSet
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            If Not (llaveCfd.ef_cve Is Nothing) And EfCveG Is Nothing Then
                EfCveG = llaveCfd.ef_cve
            End If
            If EfCveG Is Nothing Then
                EfCveG = "zzz"
            End If
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@nom_arch", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@version", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", comprobante.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", comprobante.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@fecha_factura", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sello", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_aprobacion", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ano_aprobacion", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@certificado", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@condiciones_pago", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@metodo_pago", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sub_total", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@descuento", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@total", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", comprobante.Emisor.rfc)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_receptor", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@tm", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@d_adicionales", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@pct_iva", 0)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", comprobante.cc_cve)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@cc_tipo", comprobante.cc_tipo)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_Cve", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@no_certificado_sat", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@sw_reten_iva", comprobante.Impuestos.sw_retencion)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_Valida_XML")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
            Catch ex As Exception
                Dim er As Errores = Nothing
                Dim msg As String
                msg = "sp_Valida_XML"

                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_Valida_XML")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub
        Public Sub ValidaDatosDetalleCap(ByVal errores As List(Of Errores), ByVal items As List(Of lineitem), ByVal llaveCfd As llave_cfd)
            Dim item = New lineitem
            If llaveCfd.sw_tmp = 1 Then
                ValidaDatosItemCap(errores, item, llaveCfd)
            Else
                For Each item In items
                    ValidaDatosItemCap(errores, item, llaveCfd)
                    Exit For
                Next
            End If
        End Sub
        Public Sub ValidaDatosItemCap(ByVal errores As List(Of Errores), ByVal item As lineitem, ByVal llaveCfd As llave_cfd)

            Dim er As New Errores
            Dim ds As New DataSet
            Dim sqlAdapter = New SqlDataAdapter("sp_Inserta_Detalle_XML", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_Inserta_Detalle_XML_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", "")
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_Inserta_Detalle_XML")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
                If iErrorG > 0 Then
                    agrega_err(1, "", errores)
                End If
            Catch ex As Exception
                Dim msg As String
                msg = "sp_Inserta_Detalle_XML"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_Inserta_Detalle_XML")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub
        Public Sub ValidaDatosPapa(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd)
            Dim er As New Errores
            Dim ds As New DataSet
            Dim sqlAdapter = New SqlDataAdapter("sp_valida_XML_papa", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_valida_XML_papa_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)
            Try
                Conexion.Open()
                sqlAdapter.Fill(ds, "sp_valida_XML_papa")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(0)
                iErrorG = CType(ValidacionEncabezado.Rows(0).Item("error"), Integer)
                If iErrorG = 0 Then
                    EfCveG = ValidacionEncabezado.Rows(0).Item("ef_cve").ToString.Trim
                End If
                Conexion.Close()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()
                If iErrorG > 0 Then
                    'agrega_err(IErrorG, MensajeError, errores)
                    agrega_err(IErrorG, "", errores)
                End If
            Catch ex As Exception
                Dim msg As String
                msg = "sp_valida_XML_papa"
                er.Interror = 3
                er.Message = ex.Message
                agrega_err(3, msg, errores)
                iErrorG = 60090
                graba_error(errores, er, llaveCfd, "60090", "sp_valida_XML_papa")
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub
        Public Sub GeneraEncabezadoFactura(ByVal errores As List(Of Errores), ByVal llaveCfd As llave_cfd, ByVal factura As List(Of nuevas_facturas))

            Dim er As New Errores
            Dim ds As New DataSet
            Dim sqlAdapter = New SqlDataAdapter("sp_genera_factura", Conexion)
            'Dim sqlAdapter = New SqlDataAdapter("sp_genera_factura_zfj", Conexion)
            sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlAdapter.SelectCommand.CommandTimeout = _timeout

            sqlAdapter.SelectCommand.Parameters.AddWithValue("@ef_cve", EfCveG)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", llaveCfd.rfc_emisor)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@serie", llaveCfd.serie)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", llaveCfd.folio_factura)
            sqlAdapter.SelectCommand.Parameters.AddWithValue("@uuid", llaveCfd.timbre_fiscal.uuid)

            Try
                Conexion.Open()
                _transaccion = Conexion.BeginTransaction()
                sqlAdapter.SelectCommand.Transaction = _transaccion
                sqlAdapter.Fill(ds, "sp_genera_factura")
                sqlAdapter.Dispose()
                ValidacionEncabezado = ds.Tables.Item(ds.Tables.Count - 1)
                For Each row As DataRow In ValidacionEncabezado.Rows
                    IErrorG = CType(row("error"), Integer)
                    If IErrorG = 0 Then
                        EfCveG = CStr(row("ef_cve"))
                    End If
                    agrega_folio(factura, CType(row("num_fol"), Integer), CStr(row("tipo_doc")), CStr(row("ef_cve")))
                Next

                If iErrorG > 0 Then
                    agrega_err(iErrorG, "", errores)
                    _transaccion.Rollback()
                Else
                    _transaccion.Commit()
                End If
                _transaccion.Dispose()
                Conexion.Close()
                SqlConnection.ClearPool(Conexion)
                SqlConnection.ClearAllPools()
                ds.Reset()
                ValidacionEncabezado.Rows.Clear()

            Catch ex As Exception
                If _transaccion IsNot Nothing Then
                    _transaccion.Rollback()
                End If
                Dim msg As String
                msg = "sp_genera_factura"
                iErrorG = 3
                er.Interror = iErrorG
                er.Message = ex.Message + " GeneraEncabezadoFactura"
                Dim msgCve As String = DirectCast(ex, SqlException).Number.ToString()

                Const procedimiento As String = "sp_genera_factura"
                graba_error(errores, er, llaveCfd, msgCve.ToString(), procedimiento)
                agrega_err(iErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                    SqlConnection.ClearPool(Conexion)
                    SqlConnection.ClearAllPools()
                End If
            End Try

        End Sub

        Public Sub agrega_folio(ByVal factura As List(Of nuevas_facturas), ByVal numFol As Integer, ByVal tipoDoc As String, ByVal ef_cve As String)
            Dim fact As New nuevas_facturas
            fact.num_fol = numFol
            fact.tipo_doc = tipoDoc
            fact.ef_cve = ef_cve
            factura.Add(fact)
        End Sub
        Sub ObtieneDatos(ByVal rfc As String, _
                         ByVal serie As String, _
                         ByVal folio As String, _
                         ByVal uuid As String, _
                    ByVal contra As List(Of nuevas_facturas), _
                    ByVal numInfo As Integer, _
                    ByVal errores As List(Of Errores))
            Dim recibo As New nuevas_facturas
            Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", Conexion)
            Dim ds As New DataSet()

            myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            Conexion.Open()

            myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)

            Try
                myDataAdapter.Fill(ds, "sp_consInfXML")
                myDataAdapter.Dispose()
                Conexion.Close()

                Dim tablaInfo As DataTable
                tablaInfo = ds.Tables.Item(ds.Tables.Count - 1)

                Dim iError As Integer = Int32.Parse(tablaInfo.Rows(0).Item("error").ToString.Trim)
                Dim msg = tablaInfo.Rows(0).Item("msg").ToString.Trim

                If iError = 0 Then
                    recibo.num_fol = Int32.Parse(CType(tablaInfo.Rows(0).Item("num_fol_contra"), String))
                    recibo.ef_cve = tablaInfo.Rows(0).Item("ef_cve").ToString.Trim
                    recibo.tipo_doc = tablaInfo.Rows(0).Item("tipo_doc_contra").ToString.Trim
                    recibo.nom_arch = tablaInfo.Rows(0).Item("nom_arch").ToString.Trim
                    contra.Add(recibo)
                End If

                If iError <> 0 Then
                    agrega_err(iError, msg, errores)
                End If

            Catch ex As Exception

                Dim msg As String
                msg = "sp_consInfXML:"
                IErrorG = 3

                agrega_err(IErrorG, msg, errores)
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub



        Public Function ConsultaFoliosSeguimientoCaja(ByVal rfc As String, _
                                                       ByVal serie As String, _
                                                       ByVal folio As Integer, _
                                                       ByVal uuid As String, _
                                                       ByVal swPantalla As Int16, _
                                                       ByVal tipoDoc As String, _
                                                       ByVal idSoludin As String, _
                                                       ByVal ctaCble As String
                            ) As DataTable
            'Dim myDataAdapter = New SqlDataAdapter("sp_consCFDxml_tmpgastos", Conexion)
            Dim myDataAdapter = New SqlDataAdapter("sp_consCFDxml_gastos", Conexion)

            Dim ds As New DataSet()
            Dim dt As New DataTable
            myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            Conexion.Open()

            myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", folio)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@sw_pantalla", swPantalla)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@tipo_doc", tipoDoc)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@id_soludin", idSoludin)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@cc_cve", ctaCble)
            Try
                myDataAdapter.Fill(ds, "sp_consCFDxml_gastos")
                myDataAdapter.Dispose()
                Conexion.Close()


                dt = ds.Tables.Item(0)



            Catch ex As Exception
                'lblErr.Text = "sp_consCFDxml"
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
            'End If
            Return dt
        End Function


        Public Sub ObtArchivo(ByVal rfc As String, _
                               ByVal serie As String, _
                               ByVal folio As String, _
                               ByVal uuid As String, _
                               ByVal contra As List(Of Skytex.FacturaElectronica.nuevas_facturas), _
                               ByVal numInfo As Integer, _
                               ByVal errores As List(Of Errores))
            Dim recibo As New Skytex.FacturaElectronica.nuevas_facturas
            Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", Conexion)
            Dim ds As New DataSet()
            myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
            Conexion.Open()
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", rfc)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", Int64.Parse(folio))
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", serie.Trim())
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", numInfo)
            myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", uuid)

            Try
                myDataAdapter.Fill(ds, "sp_consInfXML")
                myDataAdapter.Dispose()
                Conexion.Close()
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
                    agrega_err(iError, msg, errores, "")
                End If
            Catch ex As Exception
                'lblErr.Text = "sp_consInfXML"
            Finally
                If Conexion.State = ConnectionState.Open Then
                    Conexion.Close()
                End If
            End Try
        End Sub


       

#End Region

    End Class

End Namespace

