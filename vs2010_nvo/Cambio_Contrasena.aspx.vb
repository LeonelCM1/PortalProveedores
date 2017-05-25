Imports System.Data.SqlClient
Imports System.Net.Mail
Imports System.Net
Imports System.Data
Partial Class Logon
    Inherits Page
    Private ReadOnly _conexion As New Skytex.FacturaElectronica.Datos
    Private ReadOnly _myConnection As SqlConnection = _Conexion.MiConexion
    Private ReadOnly _ds As New DataSet()
    Dim _dtResultados As DataTable

    Private Sub PObtieneMail(ByVal rfc As String)
        Dim myDataAdapter = New SqlDataAdapter("sp_recuperacion_contra", _myConnection)
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@RFC", rfc)
        Try
            myDataAdapter.Fill(_ds, "sp_recuperacion_contra")
            myDataAdapter.Dispose()
            _myConnection.Close()
            _dtResultados = _ds.Tables.Item(0)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub PActualizaContraseña(ByVal rfc As String, ByVal passwordNuevo As String)
        Dim myDataAdapter = New SqlDataAdapter("sp_cambio_contraseña", _myConnection)
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        _myConnection.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@RFC", rfc)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@password_nuevo", passwordNuevo)
        Try
            myDataAdapter.Fill(_ds, "sp_cambio_contraseña")
            myDataAdapter.Dispose()
            _myConnection.Close()
            _dtResultados = _ds.Tables.Item(0)
        Catch ex As Exception
        End Try
    End Sub
    Private Function FGeneraNuevaContrasena(ByVal rfc As String) As String 'As Long
        Dim contra As String 'Long
        Randomize()
        contra = (CLng(555555 - 999999999) * Rnd() + 999999999).ToString()
        PActualizaContraseña(RFC, contra)
        Return contra
    End Function

    Private Sub BtnEnviar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnEnviar.Click
        Dim objMail As New MailMessage
        Dim smtp As New SmtpClient
        Dim emailFrom As String = ConfigurationManager.AppSettings("Email_From")
        Dim emailSubject As String = ConfigurationManager.AppSettings("Email_Subject")
        Dim emailCco As String = ConfigurationManager.AppSettings("Email_CCO")
        Dim emailSmtp As String = ConfigurationManager.AppSettings("Email_SMTP")
        Dim emailUser As String = ConfigurationManager.AppSettings("Email_User")
        Dim emailPsswd As String = ConfigurationManager.AppSettings("Email_Psswd")
        Try
            If TxtRFC.Text <> "" Then
                PObtieneMail(TxtRFC.Text.Trim)
                If _dtResultados.Rows.Count > 0 Then
                    objMail.To.Add(_dtResultados.Rows(0).Item(0).ToString)
                    objMail.From = New MailAddress(emailFrom)
                    objMail.Bcc.Add(emailCco)
                    objMail.Subject = emailSubject
                    objMail.Body = "Este correo es enviado a petición de un cambio de contraseña realizado mediante la pagina de www.skytex.com.mx en la sección de proveedores." + vbNewLine + vbNewLine + _
                                   "Los datos con los que deberá ingresar en adelante son: " + vbNewLine + _
                                   "RFC: " + TxtRFC.Text.Trim + vbNewLine + _
                                   "Password: " + FGeneraNuevaContrasena(TxtRFC.Text.Trim) + vbNewLine
                    objMail.IsBodyHtml = False
                    objMail.Priority = MailPriority.Normal
                    smtp.Host = emailSmtp
                    smtp.Credentials = New NetworkCredential(emailUser, emailPsswd)
                    smtp.Send(objMail)
                    LblMensaje.Visible = True
                    LblMensaje.Text = "Mensaje Enviado. Revise su correo electrónico"
                    TxtRFC.Text = ""
                Else
                    LblMensaje.Visible = True
                    LblMensaje.Text = "No se encontraron coincidencias con el RFC capturado."
                End If
            Else
                LblMensaje.Visible = True
                LblMensaje.Text = "Capture un RFC válido."
            End If
        Catch ex As SmtpException
            LblMensaje.Text = ex.Message
        End Try
    End Sub

End Class
