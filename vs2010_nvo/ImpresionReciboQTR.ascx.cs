using System;
using System.Web;
using Skytex.FacturaElectronica;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

public partial class ImpresionReciboQTR : System.Web.UI.UserControl
{
    readonly Datos _dat = new Datos();
    string _msgs = null;
    string _numFol = null;
    string _tipdocCve = null;
    string _efCve = null;
    string DBdevelop = "";
    string DBproduc = "";
    string DBCat = "";
    string DBPro = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        _Server srv = new _Server();
        DBdevelop = srv.DBdevelop;
        DBproduc = srv.DBproduc;
        DBCat = srv.DBCat;
        DBPro = srv.DBpro;

        if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "0")
        { Response.Redirect("Login.aspx"); return; }

        if (HttpContext.Current.Session["RFC"] == null && HttpContext.Current.Session["USRswUsrInterno"].ToString() == "1")
        { Response.Redirect("UserLogin.aspx"); return; }


        //HttpContext.Current.Session["FolioContra"] = 30762;//31585;
        //HttpContext.Current.Session["DoctoContra"] = "QTCR";
        //HttpContext.Current.Session["ef_cve"] = "001";
        //LblProveedor.Text = "CHT DE MEXICO, S.A. DE C.V., RFC:CME040114HB5";
        //HttpContext.Current.Session["nomProveedor"] = "Bienvenido CHT DE MEXICO, S.A. DE C.V. - [P@001929]";

        _msgs = (String)HttpContext.Current.Session["msgs"]; 

         if (_msgs != "")
         { TxtMensajes.Text = _msgs; 
             //err = 1; 
            TxtMensajes.Visible = true;
             BtnImprimir.Visible = false; }
         else
         {
             if (_numFol == null)
             {
                 _numFol = HttpContext.Current.Session["FolioContra"].ToString();
                 _tipdocCve = HttpContext.Current.Session["DoctoContra"].ToString();
                 _efCve = HttpContext.Current.Session["ef_cve"].ToString();
             }

             this.TxtMensajeExitoso.Text = @"La factura se ha procesado exitosamente, consulte la sección Seguimiento para descargar el folio de contrarecibo: " + _numFol.Trim() + Environment.NewLine;
             this.TxtMensajeExitoso.Visible = true;
             TxtMensajes.Text = "";
             TxtMensajes.Visible = false;
         }
    }

    protected void BtnImprimir_Click(object sender, EventArgs e)
    {
        //if (_numFol == null)
        //{
        //    _numFol = HttpContext.Current.Session["FolioContra"].ToString();
        //    _tipdocCve = HttpContext.Current.Session["DoctoContra"].ToString();
        //    _efCve = HttpContext.Current.Session["ef_cve"].ToString(); 
        //}
        //ImprimeReporte(_efCve, _numFol, _tipdocCve);



        if (_numFol == null)
        {
            _numFol = HttpContext.Current.Session["FolioContra"].ToString();
            _tipdocCve = HttpContext.Current.Session["DoctoContra"].ToString();
            _efCve = HttpContext.Current.Session["ef_cve"].ToString();
        }


        HttpContext.Current.Session["ef_cve_imp"] = _efCve;
        HttpContext.Current.Session["num_fol_imp"]= _numFol;
        HttpContext.Current.Session["tipdoc_imp"] = _tipdocCve;
        Response.Redirect("Reportes.aspx");
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
        if (db == DBdevelop)//JPO: 27-06-16 valida el nombre de la instancia SQL a consultar
        //if (db == "DEVELOP")
        {
            //rpt.Load(Server.MapPath("Files/QRCRWEBdev.rpt"));
            rpt.Load(Server.MapPath("Files/QRCRWEB.rpt"));
            //rpt.DataSourceConnections[0].SetConnection("skyhdev", "develop", false);
           // rpt.DataSourceConnections[0].SetConnection("skyhdev3", "skytex", false);
            //rpt.DataSourceConnections[0].SetLogon("soludin", "pluma");
            //rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");

            rpt.DataSourceConnections[0].SetConnection(DBdevelop, DBCat, false);
            rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
        }

        if (db == DBproduc)//if (db == "SKYTEX")
        {
            rpt.Load(Server.MapPath("Files/QRCRWEB.rpt"));
            //rpt.DataSourceConnections[0].SetConnection("192.168.18.21", "skytex", false);
            ////rpt.DataSourceConnections[0].SetConnection("192.168.18.49", "skytex", false);
            //rpt.DataSourceConnections[0].SetLogon("soludin", "pluma");
            //rpt.DataSourceConnections[0].SetConnection("SQL", "skytex", false);
            //rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
            rpt.DataSourceConnections[0].SetConnection(DBproduc, DBPro, false);
            rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
        }
        if (db == "192.168.18.96")//if (db == "SKYTEX")
        {
            rpt.Load(Server.MapPath("Files/QRCRWEBskyhsql.rpt"));
            //rpt.DataSourceConnections[0].SetConnection("192.168.18.21", "skytex", false);
            ////rpt.DataSourceConnections[0].SetConnection("192.168.18.49", "skytex", false);
            //rpt.DataSourceConnections[0].SetLogon("soludin", "pluma");
            //rpt.DataSourceConnections[0].SetConnection("SQL", "skytex", false);
            //rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
            rpt.DataSourceConnections[0].SetConnection("192.168.18.96", "skytex", false);
            rpt.DataSourceConnections[0].SetLogon("soludin_develop", "dinamico20");
        }
        rpt.SetParameterValue("@ef_cve", efCve);
        rpt.SetParameterValue("@tipdoc_cve", doctoContra);
        rpt.SetParameterValue("@fol_ini", folioContra);
        rpt.SetParameterValue("@fol_fin", folioContra);
        rpt.SetParameterValue("@cve_rpt", 4);
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
            rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, exportPath); // aqui importando el espacio de nombres
            rpt.Close();
        }
        catch (Exception ex)
        {
            string exm = ex.Message;
            ex = null;
            if (rpt.IsLoaded == true)
            {
                if (!DBNull.Value.Equals(rpt.DataSourceConnections[0]))
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