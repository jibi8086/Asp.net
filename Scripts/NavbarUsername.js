 function displyname()
        {
            if('@Session["name"]'!=null){
            document.getElementById('uname').innerHTML+='@Session["name"]';
                }
        }