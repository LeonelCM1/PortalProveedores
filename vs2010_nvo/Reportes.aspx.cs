using System;
using System.Web;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using Skytex.FacturaElectronica;

public partial class Reportes : System.Web.UI.Page
{
    readonly Datos _dat = new Datos();
    string DBdevelop;
    string DBproduc;
    string DBCat;
    string DBPro;
    protected void Page_Load(object sender, EventArgs e)
    {
        _Server _srv = new _Server();

        var _DBdevelop = (string)HttpContext.Current.Session["DBdevelop"];
        var _DBproduc = (string)HttpContext.Current.Session["DBproduc"];
        var _DBCat = (string)HttpContext.Current.Session["DBCat"];
        var _DBPro = (string)HttpContext.Current.Session["DBPro"];

        DBdevelop = _srv.DBdevelop;
        DBproduc = _srv.DBproduc;
        DBCat = _srv.DBCat;
        DBPro = _srv.DBpro;

        var efCve = (string)HttpContext.Current.Session["ef_cve_imp"];
        var folio = (string)HttpContext.Current.Session["num_fol_imp"];
        var doc = (string)HttpContext.Current.Session["tipdoc_imp"];

        if (efCve != "" && folio != string.Empty && doc != string.Empty)
            ImprimeReporte(efCve, folio, doc);  
    }

    private void ImprimeReporte(string efCve, string folioContra, string doctoContra)
    {
        

        ReportDocument rpt;
        try
        {
            rpt = new ReportDocument();
        }
        catch (Exception)
        {

            rpt = new ReportDocument();
        }
        const string comodin = "?";
        const string fechacom = "1960-01-01 00:00:00.000";
        string exportPath = "";
        exportPath = folioContra + "_" + efCve;



        string db = _dat.ObtieneDb();
        string reporte = "";
        int cveRpt = 0;


        if (doctoContra == "QTCR" || doctoContra == "BTCRE" || doctoContra == "BTCR")
        {
            cveRpt = 4;
            if (db == DBdevelop) //if (db == "DEVELOP")//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
            {
                reporte = "Files/QRCRWEBdev.rpt";
                //reporte = "Files/QRCRWEB.rpt";
            }
            if (db == "SKYTEX") //if (db == "SKYTEX")
            {
                reporte = "Files/QRCRWEB.rpt";
            }
            if (db == "192.168.18.96") //if (db == "DEVELOP")//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
            {
                reporte = "Files/QRCRWEBskyhsql.rpt";
                //reporte = "Files/QRCRWEB.rpt";
            }
            if (db == "SKYHJ") //if (db == "DEVELOP")//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
            {
                reporte = "Files/QRCRWEBskyhj.rpt";
                //reporte = "Files/QRCRWEB.rpt";
            }
        }
        //if (DoctoContra == "ITABXP")
        //{
        //    cve_rpt=2;
        //    reporte = "Files/Irmov_itabxp.rpt";
        //}
        //if (DoctoContra == "ITABXP") 
        //{
        //    cve_rpt = 2;
        //    reporte = "Files/Irmov.rpt";
        //}

        rpt.Load(Server.MapPath(reporte));



        if (db == DBdevelop) //if (db == "DEVELOP")//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
        {
            //rpt.DataSourceConnections[0].SetConnection("skyhdev", "develop", false);
            rpt.DataSourceConnections[0].SetConnection("SKYHDEV3", "skytex", false);
            rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
        }

        if (db == "SKYTEX") //if (db == "SKYTEX")
        {
            //rpt.DataSourceConnections[0].SetConnection("192.168.18.21", "skytex", false);
            ////rpt.DataSourceConnections[0].SetConnection("192.168.18.49", "skytex", false);
            //rpt.DataSourceConnections[0].SetLogon("soludin", "pluma");
            rpt.DataSourceConnections[0].SetConnection("SQL", "skytex", false);
            rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
        }
        if (db == "192.168.18.96") //if (db == "DEVELOP")//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
        {
            //rpt.DataSourceConnections[0].SetConnection("skyhdev", "develop", false);
            //rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "192.168.18.96", "skytex", false);
            rpt.DataSourceConnections[0].SetConnection("192.168.18.96", "skytex", false);
            rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
        }
        if (db == "SKYHJ") //if (db == "DEVELOP")//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
        {
            //rpt.DataSourceConnections[0].SetConnection("skyhdev", "develop", false);
            //rpt.SetDatabaseLogon("soludin_develop", "dinamico20", "192.168.18.96", "skytex", false);
            rpt.DataSourceConnections[0].SetConnection("skyhj", "skytex", false);
            rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
        }

        rpt.SetParameterValue("@ef_cve", efCve);
        rpt.SetParameterValue("@tipdoc_cve", doctoContra);
        rpt.SetParameterValue("@fol_ini", folioContra);
        rpt.SetParameterValue("@fol_fin", folioContra);
        rpt.SetParameterValue("@cve_rpt", cveRpt);
        rpt.SetParameterValue("@r1_clstip", comodin);
        rpt.SetParameterValue("@r1_cls", 0);
        rpt.SetParameterValue("@r1_ini", comodin);
        rpt.SetParameterValue("@r1_fin", comodin);
        rpt.SetParameterValue("@r2_clstip", comodin);
        rpt.SetParameterValue("@r2_cls", 0);
        rpt.SetParameterValue("@r2_ini", comodin);
        rpt.SetParameterValue("@r2_fin", comodin);
        rpt.SetParameterValue("@r3_clstip", comodin);
        rpt.SetParameterValue("@r3_cls", 0);
        rpt.SetParameterValue("@r3_ini", comodin);
        rpt.SetParameterValue("@r3_fin", comodin);
        rpt.SetParameterValue("@ra_ini", comodin);
        rpt.SetParameterValue("@ra_fin", comodin);
        rpt.SetParameterValue("@fmov_ini", fechacom);
        rpt.SetParameterValue("@fmov_fin", fechacom);
        rpt.SetParameterValue("@st1_doc", comodin);
        rpt.SetParameterValue("@st1_cve", 0);
        rpt.SetParameterValue("@st1_fini", fechacom);
        rpt.SetParameterValue("@st1_ffin", fechacom);
        rpt.SetParameterValue("@st1_term", 0);
        rpt.SetParameterValue("@st2_doc", comodin);
        rpt.SetParameterValue("@st2_cve", 0);
        rpt.SetParameterValue("@st2_fini", fechacom);
        rpt.SetParameterValue("@st2_ffin", fechacom);
        rpt.SetParameterValue("@st2_term", 0);
        rpt.SetParameterValue("@st3_doc", comodin);
        rpt.SetParameterValue("@st3_cve", 0);
        rpt.SetParameterValue("@st3_fini", fechacom);
        rpt.SetParameterValue("@st3_ffin", fechacom);
        rpt.SetParameterValue("@st3_term", 0);
        rpt.SetParameterValue("@id_ultact", "WEB");



       

         try
    {
            Response.Buffer = false;
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, exportPath + ".pdf"); // aqui importando el espacio de nombres
            rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, exportPath); // aqui importando el espacio de nombres
            rpt.Close();
    }
	catch (Exception ex)
	{
        string exm = ex.Message;
        ex = null;
        if  (rpt.IsLoaded == true) 
        {   
           if(!DBNull.Value.Equals(rpt.DataSourceConnections[0]))
                {
                    rpt.Dispose();
                    rpt.Close();
                    rpt = null;
                }
        }

            GC.Collect();
    }

    }

}