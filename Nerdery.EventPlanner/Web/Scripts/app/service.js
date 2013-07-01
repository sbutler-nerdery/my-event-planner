var Service = {
    call: function(url, args, successCallback) {
        $.ajax({
            url: url,
            type: "POST",
            data: args,
            dataType: "json",
            /*NOTE: if you use complete instead of success, you will get back a different json object!!! 
            Took me a while to figure that out. */
            success: function(response) {
                successCallback(response);
            }
        });
    }
};