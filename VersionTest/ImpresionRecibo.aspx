<%@ page title="" language="VB" masterpagefile="~/mpMenu.master" autoeventwireup="false" inherits="ImpresionRecibo, App_Web_awnzaboq" %>
<%@ MasterType VirtualPath="~/mpMenu.master"  %>
<%@ Register src="~/ImpresionReciboQTR.ascx" tagname="ctlImpresionRecibo" tagprefix="ucIR" %>
<%@ OutputCache Location="None" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <div id="menu" style="height: 20px; width: 970px;">
        <div id="lblUsr">
        <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label" Visible ="false"></asp:Label>
        </div>    
        <br />
        <ul class="tabs" id="button">
            <li ><a>
                <asp:LinkButton Visible="false" ID="LnkCambiaRFC" runat="server">&nbsp&nbsp Cambia Proveedor </asp:LinkButton>
                </a></li>
            <li><a >   
                <asp:LinkButton Visible="False" ID="LnkPreEntrega" runat="server" >&nbsp&nbsp Etiquetas &nbsp&nbsp</asp:LinkButton>
                </a></li>        
            <li><a>
                <asp:LinkButton visible="false" ID="LinkAcuse" runat="server">&nbsp&nbsp Acuses de Mercancía </asp:LinkButton>
                </a></li>
            <li><a>
                <asp:LinkButton ID="LnkSeguimientoCap" runat="server" OnClick="LnkSeguimiento_Click">&nbsp&nbsp Seguimiento </asp:LinkButton>
                </a></li>                        
            <li><a>
                <asp:LinkButton ID="LinkFactura" runat="server" OnClick="LinkFactura_Click">&nbsp&nbsp Subir Facturas </asp:LinkButton>
                </a></li>
           
        </ul>
        

        <br />
        <br />
<%--        
         <div id="button2" runat="server">
       <ul class="tabs" id="butt2" >
            <li ><a>
                <asp:LinkButton Visible="false" ID="LnkCambiaRFC" runat="server">Cambia Proveedor</asp:LinkButton></a></li>
        </ul>
         </div>--%>
    </div>
 <%--   <div id="principal" style="background-repeat: no-repeat; background-position: left; background-color: Transparent;">
        <div id="informacion" style="font-size: medium; border: 0">--%>
            <table style="width: 861px; height: 460px; margin-left: 20px" border="0">
                <tr>
                    <td  colspan="2" valign="top">
                  <br />
                  <br />
                  <asp:Label ID="LblProveedor" runat="server" Font-Bold="True" ForeColor="#336699" 
                      Width ="835px" ></asp:Label>
                  <br />
                  <br />
                        <table style="font-size: small; margin-left: 10px;">
                            <tr>
                                <td>
                                    <div>
                                        <ucIR:ctlImpresionRecibo ID="ctlImpresionRecibo" runat="server" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
      <%--  </div>
    </div>--%>
</asp:Content>
