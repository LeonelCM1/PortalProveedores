<%@ WebHandler Language="C#" Class="ArchivoDownload" %>

using System.Web;
using System.Web.SessionState;

public class ArchivoDownload : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {



        var sfilename = (string)HttpContext.Current.Session["archivo"];
        var filepath = (string)HttpContext.Current.Session["ruta"];

        HttpResponse response = HttpContext.Current.Response;
        response.ClearContent();
        response.Clear();
        response.ContentType = "text/plain";
        response.AddHeader("Content-Disposition", "attachment; filename=" + sfilename + ";");
        //response.TransmitFile(Server.MapPath("FileDownload.csv"));
        //response.TransmitFile(@"Z:\documentos\upload\" + sfilename.Trim());
        //response.TransmitFile(@"\\websrv1\ArchivosLibreriasWeb\documentos\upload\" + sfilename);
        response.TransmitFile(@filepath + sfilename);
        response.Flush();
        response.End();

    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}