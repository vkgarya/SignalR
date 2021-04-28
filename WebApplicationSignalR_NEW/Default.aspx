<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplicationSignalR_NEW._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <%--Vijay::Step - 7--%>
    <h3>Users GridView</h3>
    <asp:GridView ID="UserGridView" runat="server"></asp:GridView>
    <hr />
    
    <h3>Users HTML Table</h3>
    <table class="table">
        <thead>
            <tr>
                <th>Id</th>
                <th>Name</th>
                <th>City</th>
            </tr>
        </thead>

        <tbody id="tblInfo">
        </tbody>
    </table>
    <hr />
    
    <h3>Log Items</h3>
    <asp:ListView ID="LogListView" runat="server" ItemPlaceholderID="itemPlaceHolder" ClientIDMode="Static" EnableViewState="false">
        <LayoutTemplate>
            <ul id="logUl">
                <li runat="server" id="itemPlaceHolder"></li>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li><span class="logItem"><%#Container.DataItem.ToString() %></span></li>
        </ItemTemplate>
    </asp:ListView>
    <script src="Scripts/jquery-3.3.1.min.js"></script>
    <script src="Scripts/jquery.signalR-2.4.1.min.js"></script>
    <script src="signalr/hubs"></script>
    <script type="text/javascript">

        $(function () {

            // Logger
            var logger = $.connection.logHub;
            logger.client.logMessage = function (msg) {
                $("#logUl").append("<li>" + msg + "</li>");
            };
            $.connection.hub.start();

            //});

            //$(function () {

            // Vijay::Step - 7
            // User
            // Proxy created on the fly
            var userHubConnection = $.connection.userHub;
            // Declare a function on the job hub so the server can invoke it
            userHubConnection.client.displayUsers = function () {
                getuserData();
            };
            // Start the connection
            $.connection.hub.start();
            getuserData();
        });
        function getuserData() {
            var $tbl = $('#tblInfo');
            $.ajax({
                type: "POST",
                url: 'Default.aspx/GetUserDataWebMethod',
                contentType: "application/json;",
                dataType: "json",
                success: function (data) {
                    $tbl.empty();
                    $.each(data.d, function (i, model) {
                        $tbl.append
                            (
                                '<tr>' +
                                '<td>' + model.Id + '</td>' +
                                '<td>' + model.Name + '</td>' +
                                '<td>' + model.City + '</td>' +
                                '<tr>'
                            );
                    });
                },
                failure: function (error) {
                    alert(error);
                }
            });
        }

    </script>


</asp:Content>
