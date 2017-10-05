<%@ control language="VB" autoeventwireup="false" inherits="GridSeguimientoCaja, App_Web_ld2l1fol" %>
    
<script type="text/javascript">
    function onlyNumbersF(e) {
        var val = (document.all);
        var key = val ? e.keyCode : e.which;
        if (key > 31 && (key < 48 || key > 57)) {
            if (val)
                window.event.keyCode = 0;
            else {
                e.stopPropagation();
                e.preventDefault();
            }
        }
    }
</script>
            


        <table>
            <tr>
                <td>
                    Cuenta Bancaria</td>
                <td>
                    RFC Emisor:
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
                <td>
                </td>
            </tr>
            <tr>
                <td>
                      <asp:DropDownList ID="ddlCtaCble" runat="server" 
                     Width="240px">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddlFacturas" runat="server" Width="220px" >
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox ID="txtSerie" runat="server" MaxLength="12" Width="50px"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txtFolio" runat="server" MaxLength="12" Width="75px" onkeypress="onlyNumbersF(event);"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txtuuid" runat="server" MaxLength="32" Width="175px"></asp:TextBox>
                </td>
                <td>
                    
                    <asp:Button class="btnCommand"
                    ID="btnConsulta" 
                    runat="server" 
                    Text="consultar" 
                    Width="107px" 
                    OnClick="PresionaBoton"
                    Style="margin-top: 0px" />
                      

                  
                </td>
            </tr>
            <tr>
                <td colspan="6" align="right" valign="top">
                   <%-- <asp:Label ID="lblRFC" runat="server" Visible="False"></asp:Label>
                    <asp:Label ID="lblErr" runat="server" Font-Bold="True" Font-Size="10pt" Visible="False"
                        ForeColor="Maroon"></asp:Label>--%>
                </td>
            </tr>
            <tr>
                <td colspan="6" align="left" valign="top">
                    <asp:Panel ID="Panel2" runat="server" Width="900px">
                        
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" 
                    runat="server" 
                    ChildrenAsTriggers="True" 
                    UpdateMode="Conditional">
                        <ContentTemplate>
                        

                        <asp:GridView ID="GridView1" runat="server" 
                            AutoGenerateColumns="false" OnRowCommand="GridView1_RowCommand"
                            HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white" 
                            HeaderStyle-Height="30px"
                            Font-Size="XX-Small" CssClass="mGrid" Width="100%"
                            OnPreRender="GridView1_PreRender" HeaderStyle-HorizontalAlign="Center"
                            RowHeaderColumn="0" HorizontalAlign="Center" AllowPaging="True" PageSize="6"
                            OnPageIndexChanging="GridView1_PageIndexChanging">
                            <Columns>
                                <asp:BoundField HeaderText="Cuenta Bancaria" DataField="cta_fondo" ItemStyle-Width="110px">
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Emisor" DataField="Proveedor" ItemStyle-Width="200px"
                                    ItemStyle-Height="30px"></asp:BoundField>
                                <asp:BoundField HeaderText="Folio de Factura" DataField="FolioFactura" ItemStyle-Width="70px"
                                    ItemStyle-Height="30px"></asp:BoundField>
                                <asp:BoundField HeaderText="Serie" DataField="Serie"></asp:BoundField>
                                <asp:BoundField HeaderText="UUID" DataField="UUID"></asp:BoundField>
                                <asp:BoundField HeaderText="Total" DataField="Total"  ItemStyle-Width="70px"></asp:BoundField>
                                <asp:BoundField HeaderText="Fecha Recibida" DataField="FechaRecibida" ItemStyle-Width="77px">
                                </asp:BoundField>
                                
                                <asp:BoundField HeaderText="Estado" DataField="Status"></asp:BoundField>
                                <asp:BoundField HeaderText="Documento Soludin" DataField="Documento Soludin"></asp:BoundField>
                                <asp:BoundField HeaderText="Mensaje de Error" DataField="MsgError" ItemStyle-Width="165px"
                                    ItemStyle-HorizontalAlign="Left"></asp:BoundField>
                                <asp:ButtonField ButtonType="Link" CommandName="ImpXml" ItemStyle-Width="20px" Text="XML" />
                                <asp:ButtonField ButtonType="Link" CommandName="ImpPdf" ItemStyle-Width="20px" Text="PDF" />
                            </Columns>
                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                        </asp:GridView>
                        
                        
                          </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnConsulta" />
                            <%--<asp:AsyncPostBackTrigger ControlID="btnConsulta" EventName="Click" />--%>
                            <%--<asp:AsyncPostBackTrigger ControlID="GridView1" EventName="PageIndexChanging" />--%>
                        </Triggers>
                    </asp:UpdatePanel>

                    </asp:Panel>
                </td>
            </tr>
        </table>
   


  

