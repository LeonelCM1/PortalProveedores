Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.IO

Public Class _Server

    Public DBdevelop As String = "SKYHDEV3" 'nombre del servidor de develop
    'Public DBdevelop As String = "develop" 'nombre del servidor de develop
    Public DBproduc As String = "skyhj" 'nombre del servidor de produccion
    Public DBCat As String = "skytex" 'nombre de la base de datos en servidor develop
    Public DBpro As String = "skytex" 'nombre de la base de datos en servidor de produccion

    Private _session As String



    Public Property _DBdevelop As String
        Get
            Return DBdevelop

            System.Web.HttpContext.Current.Session("DBdevelop") = DBdevelop


        End Get
        Set(value As String)

        End Set
    End Property
    Public Property _DBproduc As String
        Get
            Return DBproduc
            System.Web.HttpContext.Current.Session("DBproduc") = DBproduc
        End Get
        Set(value As String)

        End Set
    End Property

    Public Property _DBCat As String
        Get
            Return DBCat
            System.Web.HttpContext.Current.Session("DBCat") = DBCat
        End Get
        Set(value As String)

        End Set
    End Property

    Public Property _DBPro As String
        Get
            Return DBpro
            System.Web.HttpContext.Current.Session("DBPro") = DBpro
        End Get
        Set(value As String)

        End Set
    End Property
End Class