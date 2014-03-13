<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="WebRole1.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Start" Text="Start Crawling" OnClick="start" runat="server" />
        <asp:Button ID="Stop" Text="Stop Crawling" OnClick="stop" runat="server" />
    </div>
    </form>
</body>
</html>
