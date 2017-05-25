Imports System
Imports System.Data.SqlClient
Imports CLPortalAbstract

Public Class Factura
    Private _Conexion As New Factura.Datos
    Private Conexion As SqlConnection = _Conexion.MiConexion
    Private _transaccion As SqlTransaction

    Public Function GetPortalUser(ByVal pUser As String, ByVal pPassword As String) As PerfilesUser
        Dim sqlAdapter = New SqlDataAdapter("sp_accesoweb", Conexion)
        'Dim sqlAdapter = New SqlDataAdapter("sp_accesoweb_zfj", Conexion)
        Dim ds As New DataSet
        Dim pf = New PerfilesUser
        Dim usr As New List(Of DatosUser)

        sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        sqlAdapter.SelectCommand.CommandTimeout = 50
        sqlAdapter.SelectCommand.Parameters.AddWithValue("@RFC", pUser)
        sqlAdapter.SelectCommand.Parameters.AddWithValue("@password", pPassword)
        Try
            Conexion.Open()
            _transaccion = Conexion.BeginTransaction
            sqlAdapter.SelectCommand.Transaction = _transaccion
            sqlAdapter.Fill(ds, "sp_accesoweb")
            Dim acceso As DataTable = ds.Tables("sp_accesoweb")
            Dim consulta = From datos In acceso.AsEnumerable _
                Select New With { _
                    .rfc = datos.Field(Of String)("rfc"), _
                    .proveedor = datos.Field(Of String)("proveedor"), _
                    .cc_tipo = datos.Field(Of String)("cc_tipo"), _
                    .cc_cve = datos.Field(Of String)("cc_cve"), _
                    .tipdoc_cve = datos.Field(Of String)("tipdoc_cve"), _
                    .nom_doc = datos.Field(Of String)("nom_doc")
                    }
            For Each con In consulta
                Dim dta As New DatosUser
                dta.Rfc = Trim(con.rfc)
                dta.Proveedor = con.proveedor
                dta.CcTipo = con.cc_tipo
                dta.CcCve = con.cc_cve
                dta.TipdocCve = con.tipdoc_cve
                dta.NomDoc = con.nom_doc
                dta.Seleccionado = False
                usr.Add(dta)
            Next
            pf.Perfil = usr
            sqlAdapter.Dispose()
            _transaccion.Commit()
            Conexion.Close()
            _transaccion.Dispose()
        Catch ex As Exception
            Dim msg As String
            msg = "Error al ejecutar sp. sp_accesoweb:" + pUser + ", " + pPassword
        Finally
            If Conexion.State = ConnectionState.Open Then
                Conexion.Close()
            End If
        End Try
        Return pf
    End Function
    Public Function GetConfig(ByVal noInf As Integer) As List(Of ConfigGral)
        Dim myDataAdapter = New SqlDataAdapter("sp_consInfXML", Conexion)
        Dim ds As New DataSet()
        Dim configuraciones = New List(Of ConfigGral)
        myDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure
        Conexion.Open()
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@rfc_emisor", "?")
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@folio_factura", 0)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@serie", "?")
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@num_info", 10)
        myDataAdapter.SelectCommand.Parameters.AddWithValue("@uuid", "?")
        Try
            myDataAdapter.Fill(ds, "sp_consInfXML")
            myDataAdapter.Dispose()
            Conexion.Close()
            For Each itm As DataRow In ds.Tables(0).Rows
                Dim config As String = itm(0)
                Dim tipo As String = itm(1).ToString.Trim
                Dim valors As String = itm(2).ToString.Trim
                Dim valorn As Decimal = itm(3)
                If tipo = "s" Then
                    configuraciones.Add(New ConfigGral() With {.config = config, .tipo = tipo, .valor = valors})
                End If
                If tipo = "n" Then
                    configuraciones.Add(New ConfigGral() With {.config = config, .tipo = tipo, .valor = valorn})
                End If
            Next
        Catch ex As Exception
            Dim msg As String = "Error al consultar sp_consInfXML"
        Finally
            If Conexion.State = ConnectionState.Open Then
                Conexion.Close()
            End If
        End Try
        Return configuraciones
    End Function

    Public Class Datos
        Public Function MiConexion() As SqlConnection
            'Dim Conexion = Global.CLPortalUsersDataLink.My.MySettings.Default.skytexConnectionString
            'Dim Conexion = Global.CLPortalUsersDataLink.My.MySettings.Default.developConnectionString
            Dim Conexion = Global.CLPortalUsersDataLink.My.MySettings.Default.AppServerConnectionString
            Dim MyConnection As New SqlConnection(Conexion)
            Return MyConnection
        End Function
    End Class
End Class
