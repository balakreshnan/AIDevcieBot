<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChatBot.aspx.cs" Inherits="BotClientWeb.ChatBot" Async="true" %>



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
            </div>
           <%-- <div style="margin:5px;">
                <img src="webcamlogo.png" style="vertical-align:text-top" />
                <select id="cameraNames" size="1" onChange="changeCamera()" style="width:245px;font-size:10px;height:25px;"></select>
            </div>--%>
        </div>
        <div style="width:135px;float:left;">
            <p><button class="btn btn-small" id="btn1" onclick="base64_tofield()">Find Emotions</button></p>
            <%--<p><button class="btn btn-small" id="btn2" onclick="base64_toimage()">Snapshot to image</button></p>--%>
        </div>
        <div style="width:200px;float:left;">
            <p><textarea id="formfield" name="formfield" style="width:190px;height:70px;" hidden="hidden"></textarea></p>
            <%--<p><img id="image" name="image" style="width:200px;height:153px;" /></p>
            <p>
                <input type="submit" name="submit" value="submit" />
            </p>--%>
            <p>
                
                <asp:TextBox ID="TextBox1" runat="server" Width="250px"></asp:TextBox>
                
            </p>
        </div>

    <br />

    <iframe src='https://webchat.botframework.com/embed/bbjcicc?s=kO8oSW7xxOI.cwA.fh8.qYmEKKfKl25aKheS1B_ky6MO5hO3rdblcRmcDkP_hr4'
        style="height: 602px; max-height: 602px; width: 700px; max-width: 700px;">

    </iframe>
    <br />
  <p>
            <asp:TextBox ID="TextBox2" runat="server" Width="887px" Height="222px" TextMode="MultiLine"></asp:TextBox>
        </p>

</asp:Content>
