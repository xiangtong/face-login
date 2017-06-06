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
            var button=$(this)
            button.prop("disabled",true)
            // var displayvideo=$("#video")[0]
            // displayvideo.style.visibility="hidden"; 
            $("#video").css('display','none')          
            $("#progress").css('display','block');       
            $.post('/uploadregimg', {img:base64}, function(data){
                        if(data.result=="success"){
                            console.log("**********");
                            var image=$("#faceimage")[0];
                            image.style.visibility = "visible";
                            image.src=data.imageurl;
                            button.prop("disabled",false)
                            // displayvideo.style.visibility="visible";
                            $("#progress").css('display','none'); 
                            $("#video").css('display','block')  
                            $("#videodiv").css('display','none');
                            if($("#noimage")!=null){
                                $("#noimage").css('display','none');
                                $("#haveimage").css('display','block');
                            }
                        }
                        else if(data.result=="refused"){
                            alert("You could not change Peck's image!")
                            var image=$("#faceimage")[0];
                            image.style.visibility = "visible";
                            button.prop("disabled",false)
                            // displayvideo.style.visibility="visible";
                            $("#progress").css('display','none'); 
                            $("#video").css('display','block')  
                            $("#videodiv").css('display','none');
                            if($("#noimage")!=null){
                                $("#noimage").css('display','none');
                                $("#haveimage").css('display','block');
                            }
                        }
                    },'json')
        }
    })
    // $("#displayvideo").click(function(){
    //     $("#videodiv").css('display','block');
    // })
    $("#displayvideo1").click(function(){
        $("#haveimage").css('display','none');
        $("#videodiv").css('display','block');
    })
    $("#cancelsnap").click(function(){
        var ifnoimage=$("#faceimage").attr("src")
        if(!ifnoimage){
            $("#noimage").css('display','block');
        } else {
            $("#haveimage").css('display','block');
        }

        $("#videodiv").css('display','none');
    })
    $("#displayvideo2").click(function(){
        $("#noimage").css('display','none');
        $("#videodiv").css('display','block');
    })
    $("#changelogin").click(function(){
        if($(".field-validation-error").length){
            $(".field-validation-error").remove()
        }
        $("#loginresult").css('display','none')
        $("#facelogindiv").css('display','none');
        $("#loginform").css('display','block');
    })
    $("#changelogin2").click(function(){
        $("#facelogindiv").css('display','block');
        $("#loginform").css('display','none');
        if($("#fromloginform").length){
            $("#fromloginform").remove();
        }

    })

    $("#verify").click(function(){
      if (video.readyState == 4) {
            var canvas = $("#canvas")[0];
            var progress =$("#progress")[0];
            // canvas.style.visibility = "visible";
            var context = canvas.getContext("2d");
            context.drawImage(video, 0, 0, 640, 480);
            var image = new Image();
            image.src = canvas.toDataURL("image/png",0.1);
            var base64 = image.src;
            var button=$(this)
            button.prop("disabled",true)
            var videodiv=$("#videodiv")[0]
            videodiv.style.visibility="hidden";           
            $("#progress").css('display','block');
            $.post('/imglogin', {img:base64}, function(data){
                        if(data.result=="success"){
                            $("#progress").css('display','none');
                            window.location.href = "/"
                        }
                        else{
                            $("#loginresult").css('display','block')
                            $("#loginresult").text("Login fail.Please use username and password to login")
                            button.prop("disabled",false)
                            $("#progress").css('display','none');
                            videodiv.style.visibility="visible";
                        }
                    },'json')
        }
    })

  $("#deleteimage").click(function(){
        var button=$(this)
        button.prop("disabled",true)
        // var displayvideo=$("#video")[0]
        // displayvideo.style.visibility="hidden";
        var imgurl= $("#faceimage").attr('src')
        $("#faceimage").css('display','none')          
        $("#deleteprogress").css('display','block');       
        $.get('/deleteimg', function(data){
                    if(data.result=="success"){
                        console.log("**********");
                        var image=$("#faceimage")[0];
                        image.style.visibility = "hidden";
                        image.src="";
                        button.prop("disabled",false)
                        $("#deleteprogress").css('display','none'); 
                        $("#faceimage").css('display','block')  
                        $("#noimage").css('display','block');
                        $("#haveimage").css('display','none');
                    }
                    else if(data.result=="refused"){
                        alert("You could not delete Peck's image!")
                        button.prop("disabled",true)
                        var image=$("#faceimage")[0];
                        $("#faceimage").attr('src',imgurl)
                        image.style.visibility = "visible";
                        $("#faceimage").css('display','block') 
                        $("#deleteprogress").css('display','none'); 
                    }
                },'json')
    })

    // $("#verify").click(function(){
    //     $.get('/verify', function(data){
    //         if(data.result=="success"){
    //             console.log("**********");
    //         }
    //     },'json')
    // })

})	


