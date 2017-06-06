// Write your Javascript code.
jQuery(document).ready(function() {

	/*
	    Fullscreen background
	*/
	$.backstretch("/images/backgrounds/1.jpg");

	$('.msf-form form fieldset:first-child').fadeIn('slow');

	if($("#fromloginform").length){
		$("#facelogindiv").css('display','none');
		$("#loginform").css('display','block');
	} else {
		$("#facelogindiv").css('display','block');
		$("#loginform").css('display','none');		
	}
	if(!$("#facelogindiv").length){
		$("#loginform").css('display','block');
		$("#changelogin2").css('display','none');
	}

});
