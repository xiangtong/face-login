$(document).ready(function(){
    var canvas = $("#canvas")[0];
    var context = canvas.getContext("2d");
    // hide the canvas
    //context.style.display = "none";
    var image = new Image();
    var video = $("#video")[0];
    var videoObj = { "video": true };
    var errBack = function (error) {
                console.log("Video capture error: ", error.code);
    };

    // Prefer camera resolution nearest to 1280x720.
    var constraints = { audio: false, video: { width: 1280, height: 720 } };

    navigator.mediaDevices.getUserMedia(constraints)
            .then(function (mediaStream) {
                var video = $("#video")[0];
                video.srcObject = mediaStream;
                video.onloadedmetadata = function (e) {
                    video.play();
                };
            })
            .catch(function (err) { console.log(err.name + ": " + err.message); }); 
    $("#snap").click(function(){
        if (video.readyState == 4) {
            var canvas = $("#canvas")[0];
            // canvas.style.visibility = "visible";
            var context = canvas.getContext("2d");
            context.drawImage(video, 0, 0, 640, 480);
            var image = new Image();
            image.src = canvas.toDataURL("image/png",0.1);
            var base64 = image.src;
            $.post('/uploadregimg', {img:base64}, function(data){
                        if(data.result=="success"){
                            console.log("**********");
                            var image=$("#faceimage")[0];
                            image.style.visibility = "visible";
                            image.src=data.imageurl;
                            $("#videodiv").css('display','none');
                            if($("#noimage")!=null){
                                $("#noimage").css('display','none');
                                $("#haveimage").css('display','block');
                            }
                        }
                    },'json')
        }
    })
    $("#displayvideo").click(function(){
        $("#videodiv").css('display','block');
    })
    $("#displayvideo1").click(function(){
        $("#videodiv").css('display','block');
    })
    $("#displayvideo2").click(function(){
        $("#noimage").css('display','none');
        $("#videodiv").css('display','block');
    })

    $("#verify").click(function(){
      if (video.readyState == 4) {
            var canvas = $("#canvas")[0];
            // canvas.style.visibility = "visible";
            var context = canvas.getContext("2d");
            context.drawImage(video, 0, 0, 640, 480);
            var image = new Image();
            image.src = canvas.toDataURL("image/png",0.1);
            var base64 = image.src;
            $.post('/imglogin', {img:base64}, function(data){
                        if(data.result=="success"){
                            window.location.href = "/"
                        }
                        else{
                            $("#loginresult").text("Login fail.Please use username and password to login")
                        }
                    },'json')
        }
    })
    // $("#verify").click(function(){
    //     $.get('/verify', function(data){
    //         if(data.result=="success"){
    //             console.log("**********");
    //         }
    //     },'json')
    // })

})	


function snapandupload() {
       
}



