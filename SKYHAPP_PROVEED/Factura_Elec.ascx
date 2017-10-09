<%@ control language="VB" autoeventwireup="false" inherits="UserControlsRecepcion, App_Web_ugro5ftg" %>

<script type="text/javascript">
    function ShowProgress() {
        document.getElementById('<% Response.Write(UpdateProgress1.ClientID) %>').style.display = "inline";
    }
</script>
<style type="text/css">
    .overlay
    {
        position: fixed;
        z-index: 98;
        top: 0px;
        left: 0px;
        right: 0px;
        bottom: 0px;
        background-color: #aaa;
        filter: alpha(opacity=80);
        opacity: 0.8;
    }
    .overlayContent
    {
        text-align:center;
        z-index: 99;
        margin: 250px auto;
        width: 400px;
        height: 80px;
    }
    .overlayContent h2
    {
        font-size: 18px;
        font-weight: bold;
        color: #000;
    }
    .overlayContent img
    {
        width: 80px;
        height: 80px;
    }
    
    /*GCM 05012015 estilo*/
    .style1
    {
        height: 23px;
    }
    
    .styloMenu
    {
	    border: 0px solid #999;
        float: left;
	    padding: 0px;
	    height: 31px; 
	    line-height: 31px; 
	    overflow: hidden;
	    position: relative;
        margin-left: 0px;
        margin-right: 0px;
        margin-top: 0px;
        text-decoration:none;
        color:#336699; 
        font-weight: bold;      
        
    }
  
    .fondo
    {
	   background-color: #D3D3D3;
       width: 866px;              
    }  
    /*GCM 05012015 estilo*/
      
</style>


<asp:ScriptManager ID="ScriptManager1" runat="server" />

 <%--GCM 05012015 linkbuttons--%>
<asp:Panel ID="pnlOpciones" runat="server" >
    <asp:LinkButton ID="LnkFComp" runat="server" Visible="false" CssClass ="styloMenu">&nbsp&nbsp Factura de compra &nbsp&nbsp</asp:LinkButton>
    <asp:LinkButton ID="LnkFEmb" runat="server" Visible="false" CssClass ="styloMenu">&nbsp&nbsp Factura de embarque &nbsp&nbsp</asp:LinkButton>
    <asp:LinkButton ID="LnkFGto" runat="server" Visible="false" CssClass ="styloMenu">&nbsp&nbsp Factura de gastos &nbsp&nbsp</asp:LinkButton>
</asp:Panel> 
 <%--GCM 05012015 linkbuttons--%>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="rdlFactura" EventName="SelectedIndexChanged" />
        <%--<asp:PostBackTrigger ControlID="rdlFactura" />--%>
    </Triggers>
    <ContentTemplate>
    <br />
    <br />
    <asp:Panel ID="pnlRFac" runat="server">
        <table>
         <table >           
        <tr>
         <td class="style1">
         <asp:Label ID="lblMensaje" runat="server" Text="No hay documentos por facturar" Visible="False" /></td>
         </td>
         <td class="style1">
         </td>
            <tr>
                <td class="style1" style="padding-left: 20px">
                <asp:Label ID="lblrdlFac" runat="server" Text="Tipo de factura a subir: " Visible="False" /></td>
                </td>
                <td class="style1"></td>
            </tr>
            <tr>
                <td style="padding-left: 50px">
                    <%--CssClass="Buttons"--%>                    
                    <asp:RadioButtonList  ID="rdlFactura" runat="server" OnSelectedIndexChanged="rdlFactura_SelectedIndexChanged" AutoPostBack="True">
                    </asp:RadioButtonList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td style="width: 50px"></td>
                            <td style="width: 140px" >
                                <asp:Label ID="lblContrato" runat="server" Text="No. Contrato: " Visible="False" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlContrato" runat="server" Visible="False" AutoPostBack="False" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 50px"></td>
                            <td style="width: 140px">
                                <%--<asp:Label ID="lblMetodo" runat="server" Text="Metodo de Pago: " Visible="False" />--%>
                            </td>
                            <td>
                                <%--<asp:DropDownList ID="ddlMetodo" runat="server" Width="235px" DataSourceID="xmldsMetodo"
                                    DataTextField="nombre" DataValueField="ID" Visible="False" AutoPostBack="False" /> --%>
                            </td>
                        </tr>
                    </table>
                    <div>
                        
                    </div>
                </td>
                <td></td>
            </tr>
        </table>
        
    </asp:Panel>
    </ContentTemplate>

</asp:UpdatePanel>


<asp:UpdatePanel runat="server" ID="upUfiles"  UpdateMode="Conditional">
     <Triggers>
        <asp:PostBackTrigger ControlID="BtnAdjuntarXML" />
    </Triggers>
    <ContentTemplate>
        
     <asp:Panel ID="pnlupfile" runat="server">       
        <table>
            <tr>
                <td valign="top" style="padding-left: 20px">                             
                    <asp:Label ID="lblXML" runat="server" Text="Ingrese la ruta del archivo XML a enviar: " Visible="True" />     
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="padding-left: 20px">                    
                    <asp:FileUpload ID="AdjXML" runat="server" ViewStateMode="Disabled" Width="830px"  accept=".xml" Visible="True" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td valign="top" style="padding-left: 20px">
                    <asp:Label ID="lblPDF" runat="server" Text="Ingrese la ruta del archivo PDF a enviar: " Visible="True" />                         
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="padding-left: 20px">
                    <asp:FileUpload ID="AdjPDF" runat="server" ViewStateMode="Disabled" Width="830px"   accept=".pdf" visible = "True"/>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <br />
        
        
        <table >
        <tr>
            <td>
            <asp:Label runat="server" ID="TxtMensajes" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
            ForeColor="Maroon"  TextMode="MultiLine"
            Font-Names="Calibri" Font-Size="Small" />
            </td>
            <td>
            <asp:Label runat="server" ID="TxtMensajeExitoso" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
            ForeColor="Green" TextMode="MultiLine"
            Font-Names="Calibri" Font-Size="Small" />
            </td>
        </tr>
        </table >
    </asp:Panel>
                <br />
                <asp:Button class="btnCommand" ID="BtnAdjuntarXML" Text="Adjuntar" runat="server" 
                   OnClientClick="ShowProgress();" visible="false"/>
            </ContentTemplate>
   
</asp:UpdatePanel>
<br />



<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="upUfiles">
    <ProgressTemplate>
        <div class="overlay" />
        <div class="overlayContent">
            <h2>
                Procesando archivos espere un momento...</h2>
            <img src="Layouts/images/img/loading.gif" alt="Loading" border="0" />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
<asp:Button ID="BtnImprimir" runat="server" Text="Imprimir Contra Recibo" Visible="False" />
<p>
    &nbsp;
</p>

<asp:XmlDataSource ID="xmldsMetodo" runat="server" DataFile="~/App_Data/Catalogos.xml"
            XPath="Catalogos/metodosDePago/metodoDePago"></asp:XmlDataSource>



