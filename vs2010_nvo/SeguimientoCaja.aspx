<%@ Page Title="" Language="VB" MasterPageFile="~/mpOptionsUser.master" AutoEventWireup="false" CodeFile="SeguimientoCaja.aspx.vb" Inherits="SeguimientoCaja" %>
<%@ MasterType VirtualPath="~/mpOptionsUser.master"  %>
<%@ Register src="~/GridSeguimientoCaja.ascx" tagname="ctlGridCH" tagprefix="ucCH" %>
<%@ OutputCache Location="None" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="menu" style="height: 20px; width: 980px;">
        <div id="lblUsr">
            <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label" Visible="false"></asp:Label>
        </div>
        <br />
        
      
            <ul class="tabs" id="button">
                <li><a>
                    <asp:LinkButton Visible="false" ID="LnkRetornaMenuOpc" runat="server">&nbsp&nbsp Regresar opciones de usuario &nbsp&nbsp</asp:LinkButton>
                    </a></li>
                <li><a>
                <%--<asp:Label  ID="LnkSegCajaChica" runat="server"  >Seguimiento Caja Chica</asp:Label></a></li>--%>
                  <asp:LinkButton  class="Ligth" ID="lnkEtiqueta" runat="server"> &nbsp&nbsp Seguimiento caja chica</asp:LinkButton>
                    </a></li>    
                <li><a>
                <asp:LinkButton ID="lblSubir" runat="server"> &nbsp&nbsp Subir facturas caja chica</asp:LinkButton>
                    </a></li>                
            <%--<li><a>
                    <asp:LinkButton  ID="LnkRetornaMenuOpc" runat="server">Regresar Opciones de Usuario</asp:LinkButton></a></li>    --%>
            </ul>
        
        <br />
        <br />
<%--        <div id="button2" runat="server">
            <ul class="tabs3" id="butt2">
                <li><a>
                    <asp:LinkButton Visible="false" ID="LnkRetornaMenuOpc" runat="server">Regresar opciones de usuario</asp:LinkButton></a></li>
            </ul>
        </div>--%>
    </div>



    <table style="width: 861px; height: 389px; margin-left: 20px" border="0">
        <tr>
            <td colspan="2" valign="top">
                <br />
                <table style="font-size: small; margin-left: 10px;">
                    <tr>
                        <td>
                            <div>
                                <ucCH:ctlGridCH ID="ctlGridCH" runat="server" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div>
    </div>
</asp:Content>

