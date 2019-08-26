<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="CogApi.aspx.cs" Inherits="BotClientWeb.CogApi" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <script language="JavaScript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <%--<script language="JavaScript" src="//ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js"></script>--%>
    <script src="http://ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js"></script>
    <script src="Scripts/scriptcam/scriptcam.js"></script>
    <script language="JavaScript">
			$(document).ready(function() {
				$("#webcam").scriptcam({
					showMicrophoneErrors:false,
					onError:onError,
					cornerRadius:20,
					cornerColor:'e3e5e2',
					onWebcamReady:onWebcamReady,
					uploadImage:'upload.gif',
					onPictureAsBase64:base64_tofield_and_image
				});
			});
			function base64_tofield() {
				$('#formfield').val($.scriptcam.getFrameAsBase64());
			};
			function base64_toimage() {
				$('#image').attr("src","data:image/png;base64,"+$.scriptcam.getFrameAsBase64());
			};
			function base64_tofield_and_image(b64) {
				$('#formfield').val(b64);
				$('#image').attr("src","data:image/png;base64,"+b64);
			};
			function changeCamera() {
				$.scriptcam.changeCamera($('#cameraNames').val());
			}
			function onError(errorId,errorMsg) {
				$( "#btn1" ).attr( "disabled", true );
				$( "#btn2" ).attr( "disabled", true );
				alert(errorMsg);
			}
			function onWebcamReady(cameraNames,camera,microphoneNames,microphone,volume) {
				$.each(cameraNames, function(index, text) {
					$('#cameraNames').append( $('<option></option>').val(index).html(text) )
				});
				$('#cameraNames').val(camera);
			}
    </script> 

    <br />

<div style="width:330px;float:left;">
    <div id="webcam">
        </div><br />
    <p><button class="btn btn-small" id="btn1" onclick="base64_tofield()">Take Picture</button></p>
    <p><textarea id="formfield" name="formfield" style="width:190px;height:70px;" hidden="hidden"></textarea></p>
</div>
        <p>
          <asp:Label ID="label1" runat="server" Text="Result are shown here:"></asp:Label> <br />
          <asp:TextBox ID="TextBox1" runat="server" Width="350px" Height="222px" 
              TextMode="MultiLine" style="overflow:scroll"></asp:TextBox>
        </p>


</asp:Content>
