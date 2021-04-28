<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplicationSignalR_NEW._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     <h3>Log Items</h3>
    <asp:listview id="logListView" runat="server" itemplaceholderid="itemPlaceHolder" clientidmode="Static" enableviewstate="false">
        <layouttemplate>
            <ul id="logUl">
                <li runat="server" id="itemPlaceHolder"></li>
            </ul>
        </layouttemplate>
        <itemtemplate>
                <li><span class="logItem"><%#Container.DataItem.ToString() %></span></li>
        </itemtemplate>
    </asp:listview>
    <script src="Scripts/jquery-3.3.1.min.js"></script>
    <script src="Scripts/jquery.signalR-2.4.1.min.js"></script>
     <script src="signalr/hubs"></script>
    <script type="text/javascript">

        $(function() {

            var logger = $.connection.logHub;

            logger.client.logMessage = function(msg) {

                $("#logUl").append("<li>" + msg + "</li>");

            };

            $.connection.hub.start();

        });

    </script>


</asp:Content>
