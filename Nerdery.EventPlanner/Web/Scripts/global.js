var EventPlanner = EventPlanner || {};

(function ($, APP) {
    // DOM Ready Function
    $(function () {
        APP.Calendars.init();
        APP.Autocomplete.init();
        APP.Tabs.init();
        APP.Modals.init();
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
    APP.Modals = {
        $defaultWidth: null,
        $defaultHeight: null,
        $modals: null,
        init: function () {
            $defaultWidth = $(window).width() - 200;
            $defaultHeight = $(window).height() - 400;
            //Set up modals
            $modals = $("[data-dialog=true]"); //invite-person-to-event
            $modals.dialog({
                autoOpen: false,
                width: $defaultWidth,
                height: $defaultHeight,
                modal: true
            });
            
            //Set up triggers to open modals
            $modals.each(function () {
                var modal = this;
                var actionKey = $(modal).data("action");
                var trigger = $("[data-open-modal=" + actionKey + "]").first();
                var longList = $("[data-list=long]");
                longList.height($defaultHeight - 200);
                longList.css({ "overflow-y":"scroll" });
                trigger.click(function () {
                    $(modal).dialog("open");
                });
            });
        },
        dismiss: function (actionKey) {
            var targetModal = $.grep($modals, function (modal) {
                return $(modal).attr("data-action") == actionKey;
            });
            
            $(targetModal).dialog("close");
        }
    },
    APP.Tabs = {
        init: function () {
            $("[data-tabs=true]").tabs();
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