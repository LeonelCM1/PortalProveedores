<%@ page title="" language="VB" masterpagefile="~/mpOptionsUser.master" autoeventwireup="false" inherits="CajaFacChica, App_Web_rhgnm1es" %>
<%@ MasterType VirtualPath="~/mpOptionsUser.master" %>
<%@ Register src="~/CajaFacturasControl.ascx" tagName="ctlCajaFactCtrl" tagPrefix="ucCF" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    
    
    <div id="menu" style="height:20px;width:980px;">
        <div>
            <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label" Visible="False" ></asp:Label>
        </div>
    </div>
    <br/>
  
    <ul class="tabs3" id="button3">
        <li>
            <a >
            <asp:LinkButton class="Ligth" ID="lnkEtiqueta" runat="server">Subir facturas caja chica</asp:LinkButton>    
            </a>
        </li>
        <li>
            <a>
                <asp:LinkButton ID="LnkSegCajaChica" runat="server">Seguimiento caja chica</asp:LinkButton>
            </a>
        </li>
    </ul>
    <br/>
    <br/>
<%--  <div id="button2" runat="server">
       <ul class="tabs3" >
           <li>
               <a>
                   <asp:LinkButton runat="server" ID="LnkRetornaMenuOpc">Regresar opciones de usuario</asp:LinkButton>
               </a>
           </li>
       </ul>
    </div>--%>
    
    <table style="width:910px; height: 389px;margin-left: 20px" border="0">
        <tr>
            <td colspan="2" valign="top">
                <br/>
                <table style="font-size:small; margin-left:10px;width: 100px;">
                    <tr>
                        <td>
                            <div>
                                <ucCF:ctlCajaFactCtrl ID="ctlCajaFact" runat="server" />
                            </div>
                        </td>
                    </tr>    
                </table>
            </td>
        </tr>
    </table>

</asp:Content>

