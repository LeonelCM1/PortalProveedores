<%@ page masterpagefile="~/mpMenu.master" language="VB" autoeventwireup="false" inherits="Logon, App_Web_cswz4jso" title="Cambio de Contraseña" %>
<%@ MasterType VirtualPath="~/mpMenu.master" %>
<%@ OutputCache Location="None" %>


<asp:content id="MainContent" contentplaceholderid="MainContentPlaceHolder" runat="server">  

      
		<div id="informacion" style="font-size:medium; border:0">
		<table style="width: 1024px; height:503px" border="0">
		
	        <tr>
	        <td colspan="2" style="width: 1102px" ></td>
	          <td colspan="4" valign="top">
	          <div>
	          </div>
	           <div>
                  <asp:Label ID="LblProveedor" runat="server" Font-Bold="True" ForeColor="#336699" Font-Size="X-Large"
                      Width ="929px" Text= "Proveedores"   ></asp:Label>
                  <br />
               </div>
               <div>
                   <hr style="border-width:medium; width: 779px; height: -12px; margin-left:0;" />
               </div>
	         
	         
                 <table style="font-size:small;  width: 505px;">
                       <tr>
                       <td colspan ="2">
                        <asp:Label ID="Label1" runat="server" Width="723px" ForeColor="#336699" Font-Bold="False" Font-Size="11pt">        La nueva contraseña le será enviada al correo electrónico que tenga registrado con Skytex México.</asp:Label>
                       </td>
                       </tr>
                       <tr><td colspan ="2">&nbsp;</td>
                       </tr>
                       <tr>
                       <td>
                      <div style="width: 329px">
                       <asp:Label ID="LblRFC" runat="server" Text="Ingrese su RFC:      " Font-Bold="True"  ForeColor="#336699"
                               Font-Size="11pt"></asp:Label>
                       <asp:TextBox ID="TxtRFC" runat="server" MaxLength="12"></asp:TextBox>
                       </div>    
                       </td>
                       </tr>
                       <tr><td style="height: 20px"></td></tr>
                       <tr>
                       <td>
                           <asp:Label ID="LblMensaje" runat="server" Font-Bold="True" 
                               Font-Size="10pt" Visible="False" ForeColor="Maroon"></asp:Label>
                           </td>
                       </tr>
                       <tr>
                       <td colspan ="1" align ="left" style="width: 67px"  >
                           <asp:Button class="btnCommand" ID="BtnEnviar" runat="server" Text="Enviar" Width="119px" />
                       </td>
                       </tr>
                 </table>
	            
	          </td>
	          
	        </tr>
	        
        </table> 
        </div>
        
        <div></div>
               
        
</asp:Content>
