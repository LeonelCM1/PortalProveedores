<%@ control language="VB" autoeventwireup="false" inherits="AdministraControl, App_Web_hxhuble3" %>

<asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
<table>
    <tr>
        <td>
            
            <asp:Label ID="lblUsuarioActivo" runat="server" Text="Label"></asp:Label>
        </td>
       
    </tr>
    <tr>
        <td style="height:100px;">
        </td>
       
    </tr>
    <tr>
        <td>
            <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional" Visible="true"
                RenderMode="Inline">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td style="width: 10px">
                            </td>
                            <td style="width: 200px">
                                <asp:Label ID="lblTipoProv" runat="server" Text="Tipo de Proveedor: " Visible="True" />
                            </td>
                            <td style="width: 420px">
                                <asp:Label ID="lblNombreProv" runat="server" Text="Nombre de Proveedor: " Visible="True" />
                            </td>
                        </tr>
                        <tr>
                            <td >
                            </td>
                            <td >
                                <asp:DropDownList ID="ddlTipoProv" runat="server" AutoPostBack="True" Visible="True"
                                    Height="25px" Width="180px" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlNombreProv" runat="server" Width="350px" 
                                    Visible="True" Height="25px" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 50px">
                            </td>
                            <td style="width: 140px; height: 30px;" colspan="2">
                                <asp:Label ID="lblError" runat="server" class="icon-bar" Text=""></asp:Label>
                            </td>
                           
                        </tr>
                        <tr>
                            <td style="width: 50px">
                            </td>
                            <td style="width: 140px">
                                &nbsp;
                            </td>
                            <td>
                                <asp:Button ID="btnCambiar" class="btnCommand" runat="server" Text="Ingresar" Width="191px" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlTipoProv" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <div>
            </div>
        </td>
        
    </tr>
</table>
