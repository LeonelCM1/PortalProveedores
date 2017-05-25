<%@ Control Language="VB" AutoEventWireup="true" CodeFile="CajaFacturasControl.ascx.vb" Inherits="CajaFacturasControl" %>

<table style="width: 100%;">
    <tr>
        <td>Cuenta Bancaria</td>
        <td>Cuenta Contable</td>
        <td>División</td>
        <td></td>
    </tr>
    <tr>
        <td>
            <asp:DropDownList ID="ddlCtaBan" class="styled-select" runat="server" Height="25px" Width="240px"/>
        </td>
        <td>
            <asp:DropDownList ID="ddlCtaCble" class="styled-select" runat="server" Height="25px" Width="240px"/>
        </td>
        <td>
            <asp:DropDownList ID="ddlDiv" class="styled-select" runat="server" Heigth="25px" Width="240px"/>
        </td>
        <td>
            <asp:Button runat="server" class="btnCommand" ID="BtnAdjuntarXml" Heigth="25px" Text="Subir facturas" OnClientClick="return validaFiles();"/>
        </td>
    </tr>
    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td>Ingrese los archivos de las facturas</td>
        <td></td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td colspan="4"> 
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            
          
            <ajaxToolkit:AjaxFileUpload ID="AjaxFileUpload1" runat="server" 
                    OnUploadComplete="UploadComplete" AllowedFileTypes="xml,pdf"/>
 

        </td>
         <td></td>
                <td></td>
                <td></td>
    </tr>
    <tr>
        <td>
             <asp:GridView ID="gvResumen" runat="server" AutoGenerateColumns="false" HeaderStyle-Font-Bold="true"
            HeaderStyle-ForeColor="white" HeaderStyle-Height="30px" Font-Size="XSmall" Width="100%"
            CssClass="mGrid3" HeaderStyle-HorizontalAlign="Left" RowHeaderColumn="0" 
           Visible="false">
          
            <Columns>
             
                <asp:TemplateField HeaderText="Factura" >
                    <ItemTemplate>
                         <asp:Label ID="lfact" runat="server" Text='<%# Bind("Factura") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Wrap="false" HorizontalAlign="Left" />
                    <ItemStyle Wrap="false"></ItemStyle>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Resultado" >
                    <ItemTemplate>
                         <asp:Label ID="resfact" runat="server" Text='<%# Bind("Resultado") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Wrap="false"  HorizontalAlign="Left" />
                    <ItemStyle Wrap="false" ></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Mensajes" >
                    <ItemTemplate>
                        <asp:Label ID="msMens" runat="server"  Text='<%# Bind("Mensajes") %>'></asp:Label>
                     
                    </ItemTemplate>
                    <%--<HeaderStyle Wrap="false" Width="30%"  HorizontalAlign="Left" />
                    <ItemStyle Wrap="false"  Width="30%" ></ItemStyle>--%>
                    <HeaderStyle Wrap="True" Width="500px"  HorizontalAlign="Left" />
                    <ItemStyle Wrap="True"  Width="500px" ></ItemStyle>

                </asp:TemplateField>
            </Columns>
            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
        </asp:GridView>
        </td>
         <td></td>
                <td></td>
                <td></td>
    </tr>
</table>