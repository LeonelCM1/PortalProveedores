<%@ page masterpagefile="~/mpLogin.master" language="VB" autoeventwireup="false" inherits="Login, App_Web_uagyq1ll" title="Proveedores" %>

<%@ MasterType VirtualPath="~/mpLogin.master" %>
<%@ Register Src="~/Login.ascx" TagName="ctlLogin" TagPrefix="uc1" %>
<%@ OutputCache Location="None" %>


<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table>
        <tr align="right">
            <td></td>
            <td></td>
            <td></td>
            <td style=" width: 900px;" ><asp:HyperLink ID="Doc" runat="server" NavigateUrl="http://www.skytex.com.mx/Documentacion/">Documentación Recepción Factura Electrónica</asp:HyperLink></td> </tr>
    </table>
    <table>
        <tr>
            <td style="height: 66px; width: 650px;" colspan="2"></td>
            <td style="width: 900px">
            </td>
            <td >
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
            </td>

        </tr>
        <tr>
            <td style=" height: 20px; width: 800px;"></td>
            <td colspan="2" style="height: 20px ">
                <div>
                    <uc1:ctlLogin ID="ctlLogin" runat="server" />
                </div>
            </td>
            <td style="height: 40px;"></td>
        </tr>
        <tr>
            <td style="width: 800px">               
            </td>
        </tr>         
       
            <td colspan="1" style="width: 800px"></td>
           
            <td></td>
        </tr>
    </table>
    <br/>
    
        <table>
        <tr align="left">         
        <td style=" width: 900px; font-size: medium; ">&nbsp;&nbsp;&nbsp;&nbsp;AVISO <br /><br /></td>        
        </tr>
        <tr align="left"> 
      
            <td style=" width: 1800px; font-size: small; " >&nbsp;&nbsp;&nbsp;&nbsp;"ES RESPONSABILIDAD DEL PROVEEDOR EL CERCIORARSE QUE SU FACTURA TENGA FOLIO <br />
            &nbsp;&nbsp;&nbsp;&nbsp;FISCAL VIGENTE, YA QUE NUESTRO SISTEMA VALIDA TAL SITUACIÓN.<br />
            &nbsp;&nbsp;&nbsp;&nbsp;EN CASO QUE NO ESTE VIGENTE EL FOLIO FISCAL ENTREGADO, DEBERAN ENTREGAR OTRO<br />
            &nbsp;&nbsp;&nbsp;&nbsp;FOLIO Y EL PLAZO DE CREDITO VOLVERA A EMPEZAR A PARTIR DE ESA FECHA PROCEDIENDO<br />
            &nbsp;&nbsp;&nbsp;&nbsp;A FACTURAR CON UN 3% SOBRE VALOR FACTURA + IVA COMO GASTOS ADMINISTRATIVO ADICIONAL"<br />            
            </td> 
            <td style=" width: 200px;"></td> 
            <%--<td style=" width: 900px; font-size: smaller;" >FISCAL VIGENTE, YA QUE NUESTRO SISTEMA VALIDA TAL SITUACIÓN</td>        
            <td style=" width: 900px; font-size: smaller;" >ES RESPONSABILIDAD DEL PROVEEDOR EL CERCIORARSE QUE SU FACTURA TENGA FOLIO FISCAL</td>        
            <td style=" width: 900px; font-size: smaller;" >ES RESPONSABILIDAD DEL PROVEEDOR EL CERCIORARSE QUE SU FACTURA TENGA FOLIO FISCAL</td>        
            --%>            
        </tr>
        <tr align="left">
            <td>
                <!--<h3><a href="Files/FechaLimiteFacturas2016.pdf" target="_blank" style=" padding: 20px; ">Fechas límite para recibir facturas electrónicas 2016 </a></h3>-->
            </td>
        </tr>
    </table>
    <br />
    
<%--        <table>
        <tr>
            <td style=" width: 20px;" ></td>
            <td style=" width: 900px;" >
             <asp:LinkButton ID="LnkCartaAviso" runat="server" ForeColor="Blue" ToolTip="Ultimas fechas para recibir facturas 2014">
                Ultimas fechas para recibir facturas 2014
                    </asp:LinkButton>
            </td>
        </tr>
        </table>--%>

    
    <%--<div class="Aviso">
    <strong>
        <asp:Label ID="Label3" ForeColor="#000000" runat="server" Text="ATENCION PROVEEDORES DE BIENES Y SERVICIOS"></asp:Label>
    </strong>
    <br />
    <br />
    <asp:Label ID="Label4" ForeColor="#000000" runat="server" Text="Se les informa de las fecha limite en que se recibirán a revisión facturas de año 2013 a través de nuestro portal de internet: "></asp:Label>
    <br />
    <asp:Label ID="Label5" ForeColor="#000000" runat="server" Text="Pago en Diciembre 2013&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- Facturas subidas hasta el 09 de Diciembre de 2013"></asp:Label>
    <br />
    <asp:Label ID="Label8" ForeColor="#000000" runat="server" Text="Pago en Enero 2014&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- Facturas subidas hasta el 06 de Enero de 2014"></asp:Label>
    <br />
    <br />
    <asp:HyperLink ID="HyperLink1" runat="server" Font-Size="8" NavigateUrl="http://www.skytex.com.mx/Documentacion/PROVEEDORES_DE_BIENES_Y_SERVICIOS_2013.zip">Continúa...</asp:HyperLink>
    <br />
    </div>--%>
    <%--<br />
    <br />
    <br />--%>
    <br />     
    <br />
    <br />
 


</asp:Content>


