<%@ control language="VB" autoeventwireup="false" inherits="CajaFacturas, App_Web_3g02hau3" %>



<style type="text/css">
    


    .overlay
    {
        position: fixed;
        z-index: 98;
        top: 0px;
        left: 0px;
        right: 0px;
        bottom: 0px;
        background-color: #aaa;
        filter: alpha(opacity=80);
        opacity: 0.8;
    }
    .overlayContent
    {
        text-align:center;
        z-index: 99;
        margin: 250px auto;
        width: 400px;
        height: 80px;
    }
    .overlayContent h2
    {
        font-size: 18px;
        font-weight: bold;
        color: #000;
    }
    .overlayContent img
    {
        width: 80px;
        height: 80px;
    }
</style>

<style type="text/css">

    .button
    {
        border-style: none;
        border-color: inherit;
        border-width: medium;
        background: url('Layouts/images/botones/add.png') no-repeat;
        /*background: url('Layouts/images/botones/add.gif') no-repeat;*/
        cursor:pointer;
        width: 16px;
    }
    
    .btnRemove 
    {
        border-style: none;
        border-color: inherit;
        border-width: medium;
        background: url('Layouts/images/botones/gtk-remove.png') no-repeat;
        /*background: url('Layouts/images/botones/gtk-remove.gif') no-repeat;*/
        cursor:pointer;
        width: 16px;
        
    }
   
   
</style>
 <style type="text/CSS">
     .hidden {
         display:none;
     }
     
     .buttonUpl 
     {
         display: inline-block;
         margin-bottom: 0;
         line-height: 1;
         text-align: center;
         vertical-align: middle;
         cursor: pointer;
         background:#336699;
         border: none;
         color: #ffffff;
         padding: 10px 30px;
         /*margin-right:5px*/
     }

     .buttonUpl:hover, .buttonUpl:focus {
         background: #2586d7;
         color: #ffffff;
     }

              
    
 </style>



<script src="Layouts/scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
<%-- <script type="text/javascript">
     var resized = false;

     function resizeSelect(selected) {
         if (resized) return;
         var sel = document.getElementById('select');
         for (var i = 0; i < sel.options.length; i++) {
             sel.options[i].title = sel.options[i].innerHTML;
             if (i != sel.options.selectedIndex) sel.options[i].innerHTML = '';
         }
         resized = true;
         sel.blur();
     }

     function restoreSelect() {
         if (!resized) return;
         var sel = document.getElementById('select');
         for (var i = 0; i < sel.options.length; i++) {
             sel.options[i].innerHTML = sel.options[i].title;
         }
         resized = false;
     }
</script>
--%>

 <script type="text/javascript">
     function ShowProgress() {
         document.getElementById('<% Response.Write(UpdateProgress1.ClientID) %>').style.display = "inline";
     }
 </script>

     <script lang="javascript" type="text/javascript">
         var currentIndex = 1;
         function addUploadField() {
             currentIndex++;

             if (currentIndex > 1) {
                 var iAnt = currentIndex - 1;

                 var sPaso = document.getElementById("AdjXML" + iAnt).value;

                 if (sPaso.trim() == "") {
                     sPaso = document.getElementById("AdjXML" + iAnt).textContent;
                 }

                 if (sPaso.trim() == "") {
                     currentIndex--;
                     return;
                 }

             }
             var varPdf = "PDF";
             var varXml = "XML";
             var newRow = '<tr>';
             newRow += '<td><input type="file" class="hidden" name="AdjXML' + currentIndex + '" id="AdjXML' + currentIndex + '" accept=".xml" onchange="nombre(this.value, this.id);load_image(this.id,this.value);" /><div class="buttonUpl" id="upXml' + currentIndex + '"  onClick=" var varXml = ' + "\'XML\'" + '; ejecutaUp(varXml, id)">XML</div></td><td><span id="lblXML' + currentIndex + '"></span></td>';
             newRow += '<td><input type="file" class="hidden" name="AdjPDF' + currentIndex + '" id="AdjPDF' + currentIndex + '" accept=".pdf" onchange="nombre(this.value, this.id);load_image(this.id,this.value);" /><div class="buttonUpl" id="upPdf' + currentIndex + '"  onClick=" var varPdf = ' + "\'PDF\'" + '; ejecutaUp(varPdf, id)">PDF</div></td><td><span id="lblPDF' + currentIndex + '"></span></td>';
             newRow += '<td><input type="button" class="btnRemove" onclick="return removeUploadField(this, this.id)" Id="btnRemove' + currentIndex + '" /></td>';
             newRow += '</tr>';
             newRow = $(newRow);
             newRow.insertBefore($('#trUploadRow'));


         }
         function removeUploadField(e, id) {
             

             var index = 0;
             index = parseInt(id.replace("btnRemove", ""));
            
             document.getElementById("AdjXML" + index).value = null;
             document.getElementById("AdjPDF" + index).value = null;
             currentIndex--;
             $(e.parentNode.parentNode).remove();
         }
     </script>

     <%--GCM 13/10/2014 valida que solo se capturen letras y números
            Función: contar e isAlphaNumeric
            --%>
      <script type="text/javascript">
          function contar(letra, texto) {
              cont = 0;
              for (i = 0; i < texto.length; i++) {
                  let = texto.substring(i, (i + 1));
                  if (letra == let) {
                      cont = cont + 1;
                  }
              }
              return (cont);
          }   
        </script>

        <script type="text/javascript">


            function isAlphaNumeric(texto) {
                if (texto.match(/^[a-zA-Z0-9_]+$/)) {
                    return (1);
                }
                else {
                    return (2);
                }
            } 
        </script>     
  
    <script type="text/javascript">
        function nombre(fic, name) {
            if (name == "" || fic == "")
                return;

            fic = fic.split('\\');
            var nombreEtiqueta = "";
            var iExis = 0;
            var swNomDif = 0;
            var nombreArchivo = fic[fic.length - 1].toString();
            //GCM 16102014 revisa que el nombre no tenga caracteres especiales
            var punto = 0;
            var formato = 0;
            punto = contar('.', nombreArchivo);
            //GCM 16102014


            iExis = name.indexOf("AdjXML");
            if (iExis >= 0) {
                var index1 = parseInt(name.replace("AdjXML", ""));
                nombreEtiqueta = "lblXML" + index1.toString(); //+ currentIndex.toString();    

                var ficPdf = document.getElementById("AdjPDF" + index1.toString()).value;
                ficPdf = ficPdf.split('\\');

                if (ficPdf !="") {
                    var nomPdf = ficPdf[ficPdf.length - 1].toString();

                    if (nombreArchivo.substring(0,nombreArchivo.indexOf(".")) != nomPdf.substring(0,nomPdf.indexOf("."))) {
                        alert('Los archivos son diferentes XML:' + nombreArchivo + ' PDF:' + nomPdf);
                        document.getElementById(name).value = "";
                        document.getElementById("lblXML" + index1).innerText = "";
                        return; //false;
                    }
                }

                //GCM 16102014 revisa que no lleve caracteres especiales
                if (punto > 1) {
                    alert('El Archivo tiene nombre incorrecto: ' + nombreArchivo);
                    document.getElementById(name).value = "";
                    document.getElementById("lblXML" + index1).innerText = "";
                    return;
                }

                formato = isAlphaNumeric(nombreArchivo.substring(0, nombreArchivo.indexOf(".")));
                if (formato > 1) {
                    alert('El Archivo tiene nombre incorrecto: ' + nombreArchivo);
                    document.getElementById(name).value = "";
                    document.getElementById("lblXML" + index1).innerText = "";
                    return;
                }
               
                
            }
            iExis = 0;
            iExis = name.indexOf("AdjPDF");
            if (iExis >= 0) {
                var index2 = parseInt(name.replace("AdjPDF", ""));
                nombreEtiqueta = "lblPDF" + index2.toString();

                var ficXml = document.getElementById("AdjXML" + index2.toString()).value;
                ficXml = ficXml.split('\\');
                if (ficXml != "") {
                    var nomXml = ficXml[ficXml.length - 1].toString();

                    if (nombreArchivo.substring(0, nombreArchivo.indexOf(".")) != nomXml.substring(0, nomXml.indexOf("."))) {
                        alert('Los archivos son diferentes XML:' + nomXml + ' PDF:' + nombreArchivo);
                        document.getElementById(name).value = "";
                        document.getElementById("lblPDF" + index2).innerText = "";
                        return; //false;
                    }
                }

                //GCM 16102014 revisa que no lleve caracteres especiales
                if (punto > 1) {
                    alert('El Archivo tiene nombre incorrecto: ' + nombreArchivo);
                    document.getElementById(name).value = "";
                    document.getElementById("lblXML" + index1).innerText = "";
                    return;
                }

                formato = isAlphaNumeric(nombreArchivo.substring(0, nombreArchivo.indexOf(".")));
                if (formato > 1) {
                    alert('El Archivo tiene nombre incorrecto: ' + nombreArchivo);
                    document.getElementById(name).value = "";
                    document.getElementById("lblXML" + index1).innerText = "";
                    return;
                }
                
            }

            document.getElementById(nombreEtiqueta).innerText = nombreArchivo;
        }
        function validaFiles() {
            var listaInputs = document.querySelectorAll("#tblArchivos input");
            var sw = true;
            for (var i = 0; i < listaInputs.length; i++) {
                var inputItem = listaInputs[i];
                if (inputItem.type == "file") {
                    
                    var iExis = inputItem.id.indexOf("AdjXML");
                    if (iExis >= 0) {
                        var valorInput = inputItem.value;
                        if (valorInput == "") {
                            alert('Faltan adjuntar archivos XML');
                            sw = false;    
                        }
                    }


                    var iExisP = inputItem.id.indexOf("AdjPDF");
                    if (iExisP >= 0) {
                        var valorInput = inputItem.value;
                        if (valorInput == "") {
                            alert('Faltan adjuntar archivos PDF');
                            sw = false;
                        }
                    }
                    
                }
            }

            if (sw == false) {
                return false;
            }
            else {
                //HabiliarCombos();
                ShowProgress();
            }



        }
    </script>

     <script type="text/javascript">
         function load_image(id, ext) {

             if (id == "" || ext == "")
                 return;

             var iExis = 0;



             var arreglo = $("span[id^='lblXML']");

             if (arreglo.length > 1) {


                 var ficPdf = document.getElementById(id).value;
                 ficPdf = ficPdf.split('\\');
                 var valorCompara = ficPdf[ficPdf.length - 1].toString();

                 
                 
                 var sw = false;
                 for (i = 0; i < arreglo.length; i++) {
                     var a = arreglo[i]
                     var index = parseInt(a.id.replace("lblXML", ""));

                     if ((a.innerText == valorCompara) && ("AdjXML" + index != id)) {
                         //alert('valor repetido en ' + i);
                         sw = true;
                     }
                     
                     if (sw == true)
                         break;
                 }
                 if (sw == true) {
                     alert("El archivo que selecciono ya existe en la lista de archivos");
                     var index = parseInt(id.replace("AdjXML", ""));
                     document.getElementById("lblXML" + index).innerText = "";
                     document.getElementById(id).value = "";
                     document.getElementById(id).focus();
                     return;
                 }
             }
             
             
             iExis = id.indexOf("XML");
             if (iExis >= 0) {
                 if (validateExtension(ext) == false) {
                     alert("Solo archivos XML");
                     document.getElementById(id).value = "";
                     document.getElementById(id).focus();
                     return;
                 }
             } else {
                 if (validateExtensionPdf(ext) == false) {
                     alert("Solo archivos PDF");
                     document.getElementById(id).value = "";
                     document.getElementById(id).focus();
                     return;
                 }
             }

             

         }

         function validateExtension(v) {
             var allowedExtensions = new Array("XML", "xml");
             for (var ct = 0; ct < allowedExtensions.length; ct++) {
                 sample = v.lastIndexOf(allowedExtensions[ct]);
                 if (sample != -1) { return true; }
             }
             return false;
         }

         function validateExtensionPdf(v) {
             var allowedExtensions = new Array("PDF", "pdf");
             for (var ct = 0; ct < allowedExtensions.length; ct++) {
                 sample = v.lastIndexOf(allowedExtensions[ct]);
                 if (sample != -1) { return true; }
             }
             return false;
         }
     </script>


<script type="text/JavaScript">
    function ejecutaUp(tipo, nombre) {

        if (nombre == "" || tipo == "")
            return;
        
        var index = 0;
        if (tipo == "XML") {
            index = parseInt(nombre.replace("upXml", ""));
        }

        if (tipo == "PDF") {
            index = parseInt(nombre.replace("upPdf", ""));
        }

        
        if (index != 0) {
            if (tipo == "XML") {
                $("#AdjXML" + index.toString()).click();
            }

            if (tipo == "PDF") {
                $("#AdjPDF" + index.toString()).click();
            }


        }

    }
		 
</script>

<script type="text/JavaScript">

    $.fn.getSelectWidth = function () {
        var width = Math.round($(this).wrap('<span></span>').width());
        $(this).unwrap();
        return width;
    }
</script>
    
    
  <table style="width: 100%;">
            <tr>
                 <td>
                    Cuenta Bancaria</td>
                <td>
                    Cuenta Contable</td>
                <td> &nbsp;
                    División</td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlCtaBan" runat="server"  class="styled-select"
                    Height="25px" Width="240px">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCtaCble" runat="server" class="styled-select"
                    Height="25px" Width="240px">
                    </asp:DropDownList>
                </td>
                <td>
                  
                     <asp:DropDownList ID="ddlDiv" runat="server" class="styled-select"
                    Height="25px" Width="240px">
                    </asp:DropDownList>
                </td>
                <td> <asp:Button class="btnCommand" ID="BtnAdjuntarXML" Text="Subir facturas" runat="server" 
           OnClientClick="return validaFiles();" /></td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                 &nbsp;
                </td>
                <td>
                 &nbsp;
                </td>
                  <td>
                 &nbsp;
                </td>
            </tr>
             <tr>
                <td>
                   Ingrese los archivos de las facturas<br />
                </td>
                <td>
                 &nbsp;
                </td>
                <td>
                 &nbsp;
                </td>
                  <td>
                 &nbsp;
                </td>
            </tr>
       
        <tr>
<asp:ScriptManager ID="ScriptManager1" runat="server" />
<asp:UpdatePanel runat="server" ID="upUfiles"  UpdateMode="Conditional">
     <Triggers>
        <asp:PostBackTrigger ControlID="BtnAdjuntarXML" />
    </Triggers>
    <ContentTemplate>
        
            
        <table id="tblArchivos" style="width: 100%; "  class="mGrid3" >
           
            <tr>
                <td style="width:9%;">
                    <input type="file" class="hidden" name="AdjXML1" id="AdjXML1" accept=".xml"  onchange="nombre(this.value, this.id);load_image(this.id,this.value);"/>
                    <div class="buttonUpl" id="upXml1" onClick="ejecutaUp('XML', id)">XML</div>
                     
                </td>
                <td style="width:40%;"><span id="lblXML1"></span></td>
                <td style="width:9%;">
                    <input type="file" class="hidden" name="AdjPDF1" id="AdjPDF1"  accept=".pdf" onchange="nombre(this.value, this.id);load_image(this.id,this.value);"/>
                    <div class="buttonUpl" id="upPdf1" onClick="ejecutaUp('PDF', id)">PDF</div>
                   
                </td>
                <td style="width:40%;"> <span id="lblPDF1"></span></td>
                <td style="width:2%;">
                    <input type="button" onclick="return addUploadField()" class="button"  />
                </td>
            </tr>
            <tr id="trUploadRow" style="display:none;">
                <td colspan="5" />
            </tr>

         
        </table>
        <br />
        
        
<table style="width: 100%; " >
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
</tr>
<tr>
    <td>
        <br/>
       <%-- <asp:Button class="btnCommand" ID="BtnAdjuntarXML" Text="Adjuntar archivos" runat="server" 
           OnClientClick="return validaFiles();" />--%>
    </td>
</tr>
</table>

    </ContentTemplate>
   
</asp:UpdatePanel>
    </tr>
     </table>


<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="upUfiles">
    <ProgressTemplate>
        <div class="overlay" />
        <div class="overlayContent">
            <h2>
                Procesando archivos espere un momento...</h2>
            <img src="Layouts/images/img/loading.gif" alt="Loading" border="0" />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>

