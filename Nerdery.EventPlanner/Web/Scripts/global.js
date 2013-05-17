var EventPlanner = EventPlanner || {};

(function ($, APP) {
    // DOM Ready Function
    $(function () {
        APP.Calendars.init();
        APP.Autocomplete.init();
    });

    APP.Timer = {
        doOnce: function (callback, interval) {
            var myTimer = null;
            myTimer = setInterval(function () { callback(); window.clearInterval(myTimer); }, interval);
        }
    },
    APP.Calendars = {
        init: function() {
            $("[data-calendar=true]").datepicker();
        }
    },
    APP.Autocomplete = {
        init: function () {
            //Setup select 2 stuff...
            $('.fancy-list-box').select2();

            //jQuery autocomplete
            $.ajax({
                url: "/Service/GetTimeList",
                type: "POST",
                dataType: "json",
                /*NOTE: if you use complete instead of success, you will get back a different json object!!! 
                Took me a while to figure that out. */
                success: function (response) {
                    if (!response.Error) {
                        $("[data-autocomplete=true]").autocomplete({ source: JSON.parse(response.Data) });
                    } else {
                        alert(response.Message);
                    }
                },
                error: function() {
                    alert('Unable to contact service contoller. Please inform your system administrator.');
                }
            });
        },
        postJson: function (blogs, callback) {

        }
    };
}(jQuery, EventPlanner));