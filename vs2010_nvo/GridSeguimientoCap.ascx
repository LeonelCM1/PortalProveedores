<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GridSeguimientoCap.ascx.vb" Inherits="UserControlsGridSeguimientoCap" %>
<asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
<asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional" Visible="true"
    RenderMode="Inline">
    <ContentTemplate>

<script src="Layouts/scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
<script type="text/javascript">
//    $(".disableLink").click(function () {
//        return confirm('Are you sure you wish to delete this record?');

//    });

    $(document).ready(function () {
        $(".disableLink").click(function () {
            return confirm('Are you sure you wish to delete this record?');
            alert("Prueba");

        });

//        console.log("ready!");
    });
</script>

<table>
    <tr>
    <td style=" width:110px;"></td>
        <td>
            Tipo de factura:
        </td>
        <td>
            Serie:
        </td>
        <td>
            Folio:
        </td>
        <td>
            UUID:
        </td>
        <td></td>
    </tr>
    <tr>
     <td></td>
        <td >
            <asp:DropDownList ID="ddlFacturas" runat="server" Width="220px" 
                OnSelectedIndexChanged="ddlFacturas_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList>
        </td>

       
        <td >
            <asp:TextBox ID="txtSerie" runat="server" MaxLength="12" Width="94px"></asp:TextBox>
        </td>
       
        <td >
            <asp:TextBox ID="txtFolio" runat="server" MaxLength="12" Width="75px"></asp:TextBox>
        </td>
        
        <td >
            <asp:TextBox ID="txtuuid" runat="server" MaxLength="32" Width="175px"></asp:TextBox>
        </td>
        <td >
            <asp:Button class="btnCommand" ID="btnCons" runat="server" Text="consultar" Width="107px"
                OnClick="btnCons_Click" Style="margin-top: 0px" />
        </td>
    </tr>
<%--</table>

<table>--%>
    <tr>
        <td colspan="6" align="right" valign="top">
            <asp:Label ID="lblRFC" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lblErr" runat="server" Font-Bold="True"
                Font-Size="10pt" Visible="False" ForeColor="Maroon"></asp:Label>
        </td>
    </tr>
    <tr><%--HeaderStyle-BackColor="#336699"--%>
        <td colspan="6" align="right" valign="top">
            <%--<asp:Panel ID="Panel1" runat="server" Height="260px" Width="865px">--%>
                <asp:Panel ID="Panel2" runat="server"  Width="865px">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" OnRowCommand="GridView1_RowCommand"
                    HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white" HeaderStyle-Height="30px"
                     Font-Size="XX-Small"  CssClass="mGrid"  OnPreRender="GridView1_PreRender"
                    HeaderStyle-HorizontalAlign="Center" RowHeaderColumn="0"
                    HorizontalAlign="Center" AllowPaging="True" PageSize="6" OnPageIndexChanging="GridView1_PageIndexChanging">
                    <Columns>
                        <asp:BoundField HeaderText="Folio de Factura" DataField="FolioFactura" ItemStyle-Width="70px" ItemStyle-Height="30px"></asp:BoundField>
                        <asp:BoundField HeaderText="Serie" DataField="Serie"></asp:BoundField>
                        <asp:BoundField HeaderText="UUID" DataField="UUID"></asp:BoundField>
                        <asp:BoundField HeaderText="Fecha Recibida" DataField="FechaRecibida" ItemStyle-Width="150px"></asp:BoundField>
                        <asp:BoundField HeaderText="Estado" DataField="Status"></asp:BoundField>

                      <%--  <asp:ButtonField ButtonType="Link" CommandName="Imprimir" DataTextField="ContraRecibo" ItemStyle-CssClass="disableLink"
                            HeaderText="Imprime Contra Recibo" Text="Imprimir" />
--%>
                      <asp:ButtonField ButtonType="Link" CommandName="Imprimir" DataTextField="ContraRecibo"
                            HeaderText="Imprime Contra Recibo" Text="Imprimir" />
                      <%--  <asp:TemplateField HeaderText="Imprime Contra Recibo"   SortExpression="Imprimir" >
                            <ItemTemplate>
                                    <asp:LinkButton  id="lnkImprimir" runat="server" 
                                       CommandArgument="<%# CType(Container,GridViewRow).RowIndex %>"
                                       CommandName="Imprimir"
                                       text='<%# Bind("ContraRecibo") %>'
                                       Font-Underline="true" Font-Bold="true"
                                       CssClass="dummyPhysicalNoteIdentifier">
                                    </asp:LinkButton >
                            </ItemTemplate>
                        </asp:TemplateField>--%>


                        <asp:ButtonField ButtonType="Link" CommandName="Captura" DataTextField="Captura"  ItemStyle-Width="150px"
                            HeaderText="Captura" Text="Captura" />
                        <asp:BoundField HeaderText="Mensaje de Error" DataField="MsgError" ItemStyle-Width="165px" ItemStyle-HorizontalAlign="Left"></asp:BoundField>
                        <asp:ButtonField ButtonType="Link" CommandName="ImpXml" ItemStyle-Width="20px" Text="XML" />
                        <asp:ButtonField ButtonType="Link" CommandName="ImpPdf" ItemStyle-Width="20px" Text="PDF"  />
                    </Columns>
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>

</table>
 </ContentTemplate>
 <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ddlFacturas" EventName="SelectedIndexChanged" />
    </Triggers>
    
</asp:UpdatePanel>
<%--<asp:XmlDataSource ID="XmlDataSource1" runat="server" 
    DataFile="~/App_Data/Catalogos.xml"  XPath="Catalogos/Empresas/Empresa"></asp:XmlDataSource>--%>
<p>
    &nbsp;
</p>
