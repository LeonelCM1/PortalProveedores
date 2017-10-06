<%@ control language="VB" autoeventwireup="false" inherits="GeneraEtiqueta, App_Web_xg4me3vo" %>

<style type="text/css">
    .style1
    {
        width: 101px;
    }
</style>

<script type="text/javascript">
    //solo numeros
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
    
    //Solo numeros y punto
    function onlynumcp(e) {

        var val = (document.all);
        var key = val ? e.keyCode : e.which;

        if (key > 31 && (key < 46 || key > 57 || key == 47)) {
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
        <td valign="top" font-size="XX-Small">Empresa:</td>
        <td  valign="top">
            <asp:DropDownList ID="ddlEmpresa" runat="server" Height="22px" Width="230px">
            </asp:DropDownList>
        </td>
        <td  valign="top" font-size="XX-Small">Documento:</td>
        <td  valign="top">
            <asp:DropDownList ID="ddlRecibo" runat="server" Height="22px" Width="200px">
            </asp:DropDownList>
        </td>
        <td  valign="top" font-size="XX-Small">Folio:</td>
        <td  valign="top">
            <asp:TextBox runat="server" Height="22px" Width="80px" 
                ValidationGroup="Numeros" ID="txtFolio" onkeypress="onlyNumbersF(event);" ></asp:TextBox>
        </td>
        

        <td valign="top">            
                        <asp:Button class="btnCommand" ID="btnGen" runat="server" Text="Generar" Width="107px"
                OnClick="btnGen_Click" Style="margin-top: 0px"/>
        </td>
    </tr>
<br />
<br />
    <table id="tblActu">
    <tr>
        <td valign="top" font-size="XX-Small" Width="55px" >
            <asp:Label ID="lblReng" runat="server" Visible="False" Text="Renglón:"></asp:Label>        
        </td>
        <td  valign="top">
            <asp:TextBox ID="txtReng" runat="server" Height="22px" Width="50px" 
            onkeypress="onlyNumbersF(event);" visible="false" Enabled="false">
            </asp:TextBox>
        </td>
        <td valign="top" font-size="XX-Small" align="right">
            <asp:Label ID="lblDesc" runat="server" Visible="False" Text="Desc:"></asp:Label>        
        </td>
        <td  valign="top">
            <asp:TextBox ID="txtDesc" runat="server" Height="40px" Width="170px" 
             Visible="false" TextMode="MultiLine" Enabled="false">
            </asp:TextBox>
        </td>
        <td valign="top" font-size="XX-Small" align="right">
            <asp:Label ID="lblCant" runat="server" Visible="False" Text="Cantidad:"></asp:Label>        
        </td>
        <td  valign="top">
            <asp:TextBox ID="txtCant" runat="server" Height="22px" Width="60px" 
            onkeypress="onlynumcp(event);" Visible="false">
            </asp:TextBox>
        </td>
        <td  valign="top" font-size="XX-Small" align="right">
            <asp:Label ID="lblCaja" runat="server" Visible="False" Text="Caja:"></asp:Label>
        </td>
        <td  valign="top">
            <asp:TextBox ID="txtCaja" runat="server" Height="22px" Width="60px" 
            onkeypress="onlyNumbersF(event);" Visible="false">
            </asp:TextBox>
        </td>
        <td  valign="top" font-size="XX-Small" align="right">
            <asp:Label ID="lblLote" runat="server" Visible="False" Text="Lote:"></asp:Label>        
        </td>
        <td  valign="top" class="style1">
            <asp:TextBox runat="server" Height="22px" Width="100px" 
                ValidationGroup="Numeros" ID="txtLote" Visible="False"></asp:TextBox>
        </td>
        

        <td valign="top">            
                        <asp:Button class="btnCommand" ID="btnAct" runat="server" Text="Actualizar" Width="107px"
                OnClick="btnAct_Click" Style="margin-top: 0px" Visible="False"/>
        </td>
    </tr>
    </table>

    <tr>
        <td colspan="7" align="right" valign="top">
            <asp:Label ID="lblRFC" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="lblErr" runat="server" Font-Bold="True"
                Font-Size="10pt" Visible="False" ForeColor="Maroon"></asp:Label>
        </td>
    </tr>
        <td colspan="7" align="right" valign="top">
            <asp:Label ID="lblFol" runat="server" Visible="false"></asp:Label>
            <asp:Label ID="lblDoc" runat="server" Visible="false"></asp:Label> 
        </td>
       
         

    <tr>
        <td colspan="7" align="justify" valign="top">                 

            <asp:GridView ID="GridView1" runat="server" 
    AutoGenerateColumns="False" OnRowCommand="GridView1_RowCommand"
                HeaderStyle-Font-Bold="true" HeaderStyle-ForeColor="white" CssClass="mGrid"
                HeaderStyle-BackColor="#336699" Font-Size="XX-Small" 
    Width="350px" Height="250px"
                HeaderStyle-HorizontalAlign="Center" RowHeaderColumn="0"
                HorizontalAlign="Center" AllowPaging="True" PageSize="8" 
                OnPageIndexChanging="GridView1_PageIndexChanging"
                Visible="False" AllowSorting="True"> 
                <Columns>
                        <asp:BoundField HeaderText="Folio" DataField="Folio" ItemStyle-Width="70px" ItemStyle-Height="30px" Visible="false" >
                            <%-- <ItemStyle Height="30px" Width="50px"></ItemStyle>--%>
<ItemStyle Height="30px" Width="70px"></ItemStyle>
                        </asp:BoundField>    
                        <asp:BoundField HeaderText="Documento" DataField="tipo_doc" ItemStyle-Width="70px" ItemStyle-Height="30px" Visible="false" >
                            <%-- <ItemStyle Height="30px" Width="50px"></ItemStyle>--%>
<ItemStyle Height="30px" Width="70px"></ItemStyle>
                        </asp:BoundField>                                             
                        <asp:BoundField HeaderText="Renglon" DataField="reng" ReadOnly="true"></asp:BoundField>
                        <asp:BoundField HeaderText="Articulo" DataField="Articulo" ReadOnly="true"></asp:BoundField>
                        <asp:BoundField HeaderText="Descripcion" DataField="Descripcion" ReadOnly="true" ></asp:BoundField>
                        <asp:BoundField HeaderText="Cantidad" DataField="Cantidad" ReadOnly="true"></asp:BoundField>
                        <asp:BoundField HeaderText="Caja" DataField="Caja" ReadOnly="true"></asp:BoundField>
                        <asp:BoundField HeaderText="Lote" DataField="Lote" ReadOnly="true"></asp:BoundField>

                        
                         <%--<asp:TemplateField HeaderText="Cantidad">
                            <ItemTemplate>
                                <%# Eval("cantidad")%>
                                <asp:Label ID="lblCant" Width="60px" runat="server" 
                                    Text='<%# Bind("cantidad") %>' onkeypress="onlyNumbersF(event);"></asp:Label>
                            
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCant" runat ="server" Width="100px" Text='<%# Eval("cantidad")%>' onkeypress="onlyNumbersF(event);"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCant" Width="60px" runat="server" Text='<%# Bind("cantidad") %>' onkeypress="onlyNumbersF(event);"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Caja">
                            <ItemTemplate>
                                <%# Eval("caja")%>
                                <asp:Label ID="lblCaj" Width="60px" runat="server" Text='<%# Bind("caja") %>'></asp:Label>
                            
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCaj" runat ="server" Width="100px" Text='<%# Eval("caja")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCaj" Width="60px" runat="server" Text='<%# Bind("caja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Lote">
                            <ItemTemplate>
                                <%# Eval("Lote")%>
                                <asp:Label ID="lblLote" Width="60px" runat="server" Text='<%# Bind("Lote") %>'></asp:Label>
                            
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLote" runat ="server" Width="100px" Text='<%# Eval("Lote")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLote" Width="60px" runat="server" Text='<%# Bind("Lote") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                  <%--   <asp:ButtonField ButtonType="Link" CommandName="Imprimir" DataTextField="cc_tipo"
                        HeaderText="Folio" Text="Imprimir" />--%>                        
                        <asp:ButtonField CommandName="Select" Text="Seleccionar" />
                </Columns>
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
            </asp:GridView>
                <br />
            <asp:Button class="btnCommand" ID="btnEtiq" runat="server" Text="Etiqueta" OnClick="btnEtiq_Click" Visible="false" Enabled="false"/>
                            <%--   <asp:Button class="btnCommand" ID="btnEtiq" runat="server" Text="Etiqueta" Width="107px"
                OnClick="btnEtiq_Click" Style="margin-top: 0px" />--%>
        
    </tr>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
            </tr>
            <br />
            <asp:Panel ID="Panel1" runat="server" Height="200px" Visible="false">
            </asp:Panel>
        </td>
    </tr>
</table>

<p>
    &nbsp;
</p>
