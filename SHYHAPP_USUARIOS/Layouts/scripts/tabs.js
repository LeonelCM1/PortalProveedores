function show(what)
{
	var prefix = what.split('_')[0];
	var classNames;
	var obj;
	var maxTabs = 15;
	
	//Hide the other items	
	for (i=1;i<=maxTabs;i++)
	{
		obj=document.getElementById(prefix+"_"+i+"_tab");
		if (obj)
		{
			classNames = obj.className;
			classNames = classNames.split("active")[0];
			classNames = classNames.split("inactive")[0];
			obj.className = classNames + " inactive";
		}
		
		obj=document.getElementById(prefix+"_"+i);
		if (obj)
		{
			classNames = obj.className;
			classNames = classNames.split(" fold")[0];
			classNames = classNames.split(" unfold")[0];
			obj.className = classNames + " fold";
		}
		else
		{
			//break;		
		}
	}
	
	//Show the item
	obj=document.getElementById(what);
	if (obj)
	{
		classNames = obj.className;
		classNames = classNames.split(" fold")[0];
		classNames = classNames.split(" unfold")[0];
		obj.className = classNames + " unfold";
	}
	
	obj=document.getElementById(what+"_tab");
	if (obj)
	{	
		classNames = obj.className;
		classNames = classNames.split("active")[0];
		classNames = classNames.split("inactive")[0];
		obj.className = classNames + " active";
	}
}

function hide(what)
{
	obj=document.getElementById(what);
	if (obj)
	{
		classNames = obj.className;
		classNames = classNames.split(" fold")[0];
		classNames = classNames.split(" unfold")[0];
		obj.className = classNames + " fold";
	}
}

function deactivate(what, currentImage)
{
	var prefix = what
	var classNames;
	var obj;
	var maxItems = 30;

	//Hide the other items	
	for (i=1;i<=maxItems;i++)
	{
		obj=document.getElementById(prefix+"_"+i);
		if (obj)
		{
			//Leave the first few visible
			offsetFor=3-currentImage;
			if (offsetFor<0) {offsetFor=0;}
			if (i>currentImage-3 && i<currentImage+3+offsetFor)
			{
				obj.className="inactive";
			}
			else
			{
				obj.className="hidden";
			}
		}
		else
		{
			//Backtrack at the far end
			if (currentImage>i-3 && i>5)
			{
				for (j=i-1;j>=i-5;j--)
				{
					obj=document.getElementById(prefix+"_"+j);
					obj.className="inactive";
				}
			}
			break;	
		}
	}
}

function rigImage(smallUrl, bigUrl, alt, caption, copyright, passedItem)
{
	var prefix = passedItem.split('_')[0];
	var imageHolder = document.getElementById(prefix + "_" + "imageHolder");
	var imageCaption = document.getElementById(prefix + "_" + "imageCaption");
	var imageCopyright = document.getElementById(prefix + "_" + "imageCopyright");
	
	//Sort out the navigation items
	var theLink = document.getElementById(passedItem);
	if (theLink)
	{
		
		var currentImage = eval(passedItem.split('_')[1]);
		
		deactivate(prefix, currentImage);
		theLink.className = "active";
	
		//Next button
		nextLink = document.getElementById(prefix + "_" + (currentImage + 1));
		nextButton = document.getElementById(prefix + "_" +"nextImage");
		if (nextLink)
		{
			nextButton.href = nextLink.href;
			nextButton.className = "ends";
		}
		else
		{
			nextButton.href = "javascript:deadEnd();";
			nextButton.className = "ends dead";
		}
		
		//Prev button
		prevLink = document.getElementById(prefix + "_" + (eval(currentImage) - 1));
		prevButton = document.getElementById(prefix + "_" +"prevImage");
		if (prevLink)
		{
			prevButton.href = prevLink.href;
			prevButton.className = "ends";
		}
		else
		{
			prevButton.href = "javascript:deadEnd();";
			prevButton.className = "ends dead";
		}
	}
	
	var htmlOutput = '<img src="' + smallUrl + '" alt="' + alt + '"/>';
	if (imageHolder)
	{
	   if(imageHolder.getElementsByTagName('div')[0]) {
		  imageHolder.getElementsByTagName('div')[0].innerHTML = htmlOutput;
		} else {
		  imageHolder.innerHTML = htmlOutput;
		}
		imageCaption.innerHTML = caption;
		imageCopyright.innerHTML = copyright;
	}
	
	/*Update the link on the zoom button*/
	var zoomButton = document.getElementById(prefix + "_" + "zoomButton");
	
	if (zoomButton)
	{
		if(bigUrl) {
			zoomButton.onclick = function(){zoom(bigUrl);}
		} else {
			zoomButton.style.display = 'none';
		}
	}
	
	/*Check for number of images and ahow pagination if required*/
	var imagePaginationLinks = document.getElementById(prefix + "_" + "imagePaginationLinks");
	var imagePagination = document.getElementById(prefix + "_" + "imagePagination");
	if (imagePaginationLinks)
	{
		if (imagePaginationLinks.innerHTML == "" || imagePaginationLinks.innerHTML == "{d069Repeater}")
		{
			/*Do nothing - this is disabled by default in CSS*/
		}
		else
		{
			imagePagination.style.display = "block";
		}
	}
}

var zoomed;
function zoom(url)
{
	newWindow = window.open(url,'TechRadar','width=748,height=760,scrollbars=1');
	newWindow.focus();
}

function deadEnd()
{
	/*Do nothing here - added to avoid IE displaying null*/
}

var xmlHttp = GetXmlHttpObject();
var returnedData = "waitState";

function GetXmlHttpObject()
{
  var xmlHttp=null;
  try
    {
    // Firefox, Opera 8.0+, Safari
    xmlHttp=new XMLHttpRequest();
    }
  catch (e)
    {
    // Internet Explorer
    try
      {
      xmlHttp=new ActiveXObject("Msxml2.XMLHTTP.3.0");
      }
    catch (e)
      {
      xmlHttp=new ActiveXObject("Microsoft.XMLHTTP");
      }
    }
    if (xmlHttp==null)
    {
        alert ("Sorry, your browser does not appear to support AJAX.");
    } 
  return(xmlHttp);
}

function getData(url,what)
{
	what = escape(what);
	url = url + "?email=" + what;
	//var d = new Date();
	//url = url + "&nocache=" + d.getTime();
	
	xmlHttp.onreadystatechange = stateChanged;
	xmlHttp.open("GET",url,true);
	xmlHttp.send(null);
}

function stateChanged()
{ 
	if (xmlHttp.readyState==4)
	{ 
		returnedData = xmlHttp.responseText;
		updateDisplay();
		
		//Re-get a new object for IE 7
		xmlHttp=GetXmlHttpObject();
	}
}

function updateDisplay()
{
	switch(returnedData)
	{
		case "ok":
			show("forgotten_2");
			break;
		case "fail":
			show("forgottenEmailError");
			break;
		default:
			show("forgottenEmailError");
	}
}

function checkAvail(what)	
{
	var pane;
	var tab;
	var tabLink;
	var prefix = what;
	
	for (i=1;i<=6;i++)
	{
		pane = document.getElementById(prefix+"_"+i);
		if (pane)
		{
			if (pane.innerHTML.indexOf("<") < 0)
			{
				tab=document.getElementById(prefix+"_"+i+"_tab");
				if (tab)
				{
					tab.className = tab.className + " grey";
					//Hack to perminantly display tab for - requested by Future. Disabled.
					//if (i==1)
					//{
					//	show(prefix+"_4");	
					//}
				}
				tabLink=document.getElementById(prefix+"_"+i+"_link");
				if (tabLink)
				{
					tabLink.href = "javascript:void;";
				}
			}
		}
	}
}
var active_tab = null; // the active tab at startup
var tmp_active_tab = null; // the tab that is being hovered over

function navMainInit(navId,navArray)
{
	if ((navId !== undefined) && (typeof(navId) == "string")) {
		var navMain = document.getElementById(navId);
	} else {
		var navMain = document.getElementById('navMain');
	}
	var navs = navMain.getElementsByTagName('li');
	if ((navArray !== undefined) && (typeof(navArray) == "object")) {
		var top_nav = navArray;
	} else {
		var top_nav = Array('news','reviews','blogs','forums','software','magazines','advertorial');
	}
	for(i=0;i<navs.length;i++) {
		current_nav = navs[i];
		nav_id = current_nav.id;
		if(nav_id != '' && top_nav.toString().indexOf(nav_id.toString()) !== -1) {			
			current_nav.onmouseover = navMainUpdate;		
			current_nav.onmouseout = navMainReset;
			
			if(current_nav.className.indexOf('active') !== -1) {
				active_tab = current_nav;
				tmp_active_tab = current_nav;
			}	
		}
	}
	
	if(tmp_active_tab==null) {
		tmp_active_tab = document.getElementById(top_nav[0]);
	}
}

function navMainUpdate()
{
	tmp_active_tab.className = tmp_active_tab.className.replace(/active/,'');	
	var subnav = tmp_active_tab.getElementsByTagName('ul')[0];
	if(subnav) {
		hide(subnav.id);
	}
	
	this.className = this.className+' active';
	tmp_active_tab = this;
	
	var subnav = this.getElementsByTagName('ul')[0];
	if(subnav) {
		show(subnav.id);
	}
	/* Additional code for advertorial navigation */
	var navBar = document.getElementById('navBar1');
	
	if(this.id == 'advertorial') {
		navBar.className = navBar.className.replace(/solidBlue/,'solidMagenta');
	} else {
		navBar.className = navBar.className.replace(/solidMagenta/,'solidBlue');
	}
}

function navMainReset()
{
	tmp_active_tab.className = tmp_active_tab.className.replace(/active/,'');
	var subnav = tmp_active_tab.getElementsByTagName('ul')[0];
	if(subnav) {
		hide(subnav.id);
	}
	if(active_tab) {
		active_tab.className = active_tab.className+' active';
		tmp_active_tab = active_tab;
		
		subnav = tmp_active_tab.getElementsByTagName('ul')[0];
		if(subnav) {
			show(subnav.id);
		}
		
		/* Additional code for advertorial navigation */
		var navBar = document.getElementById('navBar1');
		
		if(tmp_active_tab.id == 'advertorial') {
			navBar.className = navBar.className.replace(/solidBlue/,'solidMagenta');
		} else {
			navBar.className = navBar.className.replace(/solidMagenta/,'solidBlue');
		}
	}
}

//Event.observe(window, "load", navMainInit, false);