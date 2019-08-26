<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VoiceVerify.aspx.cs" Inherits="DeviceBotWeb.VoiceVerify" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    	<meta name="viewport" content="width=device-width,initial-scale=1"/>
	<title>Audio Recorder</title>

	<script src="js/audiodisplay.js"></script>
	<script src="js/recorderjs/recorder.js"></script>
	<script src="js/main.js"></script>
    <script src="js/azure-storage.common.js"></script>
    <script src="js/azure-storage.blob.js"></script>
	<style>
	html { overflow: auto; }
	body { 
		font: 14pt Arial, sans-serif; 
		background: lightgrey;
		display: flex;
		flex-direction: column;
		height: 100vh;
		width: 100%;
		margin: 0 0;
	}
	canvas { 
		display: inline-block; 
		background: #202020; 
		width: 95%;
		height: 45%;
		box-shadow: 0px 0px 10px blue;
	}
	#controls {
		display: flex;
		flex-direction: row;
		align-items: center;
		justify-content: space-around;
		height: 20%;
		width: 100%;
	}
	#record { height: 15vh; }
	#record.recording { 
		background: red;
		background: -webkit-radial-gradient(center, ellipse cover, #ff0000 0%,lightgrey 75%,lightgrey 100%,#7db9e8 100%); 
		background: -moz-radial-gradient(center, ellipse cover, #ff0000 0%,lightgrey 75%,lightgrey 100%,#7db9e8 100%); 
		background: radial-gradient(center, ellipse cover, #ff0000 0%,lightgrey 75%,lightgrey 100%,#7db9e8 100%); 
	}
	#save, #save img { height: 10vh; }
	#save { opacity: 0.25;}
	#save[download] { opacity: 1;}
	#viz {
		height: 80%;
		width: 100%;
		display: flex;
		flex-direction: column;
		justify-content: space-around;
		align-items: center;
	}
	@media (orientation: landscape) {
		body { flex-direction: row;}
		#controls { flex-direction: column; height: 100%; width: 10%;}
		#viz { height: 100%; width: 98%;}
	}

	</style>
</head>
<body>
    <form id="form1" runat="server">
        	<div id="viz">
                <br /><br />
                <br /><br /><br /><br /><br /><br /><br /><br /><br /><br />
                <asp:TextBox ID="name" Text="bala" runat="server"></asp:TextBox>
                <asp:TextBox ID="TextBox1" Text="" runat="server"></asp:TextBox>
                <asp:Button ID="ss" Text="submit" runat="server" OnClick="ss_Click" />
		        <canvas id="analyser" width="1024" height="200"></canvas>
                &nbsp;<img id="record" src="images/mic128.png" onclick="toggleRecording(this);"/>
		        <canvas id="wavedisplay" width="1024" height="200"></canvas>
                <a id="save" href="#"><img src="images/save.svg"/></a>
                <p><textarea id="formfield" name="formfield" hidden="hidden"></textarea></p>
                <p>
                    <audio id="test" controls="controls" />
                </p>
	        </div>
	        <div id="controls">
		        
		        
	        </div>
    </form>
</body>
</html>
