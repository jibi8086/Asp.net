function showImageUpload() {
    var x = document.getElementById("imgcontrols");
    if (x.style.display === "none") {
        $("#imgcontrols").fadeIn();               
    }
    $("#inputfile").click();
}


    $(document).ready(function () {
        $("#submit").click(function () {
            var formData = new FormData();         
            var file = $("#inputfile").get(0).files;
            if (file.length > 0)
            {
                formData.append("inputfile", file[0]);
            }               
            if ($('#inputfile').get(0).files.length === 0)
            {
                return false;
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "/UserRegister/ChangeProfilePicture",
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (response) {                            
                        location.href = "/CreatePost/CreatePost";
                        if(response.id==404)
                        {
                            window.location.href = "/CompanyRegister/Error_View";
                        }
                    },
                    failure: function (response) {
                        bootbox.alert("Process failed!! Please try again.");
                    },
                    error: function (response) {
                        bootbox.alert("Error!! Sorry for your inconvinience.");
                    }
                });
            }

        });
    });



     $(document).ready(function() {
         var uploader = document.getElementById("inputfile"),
         image = document.getElementById("img-result");
         uploader.type = "file";
         uploader.accept = "image/*";
         uploader.onchange = function() {
             var reader = new FileReader();
             reader.onload = function(evt) {
                 image.classList.remove("no-image");
                 image.style.backgroundImage = "url(" + evt.target.result + ")";
                 var request = {
                     itemtype: "test 1",
                     brand: "test 2",
                     images: [
                    {
                        data: evt.target.result
                    }
                     ]
                 };

                 document.querySelector(".show-button").style.display = "block";
                 document.querySelector(
                   ".upload-result__content"
                 ).innerHTML = JSON.stringify(request, null, "  ");
             };
             reader.readAsDataURL(uploader.files[0]);
         };

         document.querySelector(".hide-button").onclick = function() {
             document.querySelector(".upload-result").style.display = "none";
         };

         document.querySelector(".show-button").onclick = function() {
             document.querySelector(".upload-result").style.display = "block";
         };
     })();
