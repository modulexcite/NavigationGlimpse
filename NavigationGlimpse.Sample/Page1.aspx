﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page1.aspx.cs" Inherits="NavigationGlimpse.Sample.Page1" %>
<%@ Register assembly="Navigation" namespace="Navigation" tagprefix="nav" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:HyperLink runat="server" NavigateUrl="{RefreshLink Sort}" Text="Sort" />
		<asp:HyperLink runat="server" NavigateUrl="{NavigationLink Next}" Text="Next" />
		<asp:Button runat="server" Text="Submit" />
    </div>
    </form>
</body>
</html>
