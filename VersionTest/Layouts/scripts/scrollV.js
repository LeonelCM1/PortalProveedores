// JScript File

iens6=document.all||document.getElementById
ns4=document.layers

//specify speed of scroll (greater=faster)
var speed=1

if (iens6){
document.write('<div id="container" style="position:relative;width:209px;height:158px;border:0px;overflow:hidden">')
document.write('<div id="content" style="position:absolute;width:209px;left:0;top:0">')
}

	
if (iens6){
var crossobj=document.getElementById? document.getElementById("content") : document.all.content
var contentheight=crossobj.offsetHeight
}
else if (ns4){
var crossobj=document.nscontainer.document.nscontent
var contentheight=crossobj.clip.height
}

function movedown(){
if (iens6&&parseInt(crossobj.style.top)>=(contentheight*(-1)+158))
crossobj.style.top=parseInt(crossobj.style.top)-speed+"px"
else if (ns4&&crossobj.top>=(contentheight*(-1)+158))
crossobj.top-=speed
movedownvar=setTimeout("movedown()",20)
}

function moveup(){
if (iens6&&parseInt(crossobj.style.top)<=0)
crossobj.style.top=parseInt(crossobj.style.top)+speed+"px"
else if (ns4&&crossobj.top<=0)
crossobj.top+=speed
moveupvar=setTimeout("moveup()",20)

}

function getcontent_height(){
if (iens6)
contentheight=crossobj.offsetHeight
else if (ns4)
document.nscontainer.document.nscontent.visibility="show"
}
window.onload=getcontent_height
