var EventPlanner = EventPlanner || {};

(function ($, APP) {
    // DOM Ready Function
    $(function () {
        APP.Calendars.init();
        APP.Autocomplete.init();
        APP.Tabs.init();
        APP.Modals.init();
    });

    APP.Events = {
        addFoodItem: function (controlId, response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var id = response.Data.FoodItemId;
            var title = response.Data.Title;
            var description = response.Data.Description;
            var text = title + " " + description;
            var value = id;
            $("#" + controlId).append("<option value='" + value + "'>" + text + "</option>");
            APP.Autocomplete.addSelectedItem(controlId, value);

            //Clear the fields...
            $("[data-action=add-food-item] :input").each(function() {
                $(this).val("");
            });  
        },
        addGame: function (controlId, data) {
            if (data.Error) {
                alert(data.Message);
                return;
            }
            
            var id = response.Data.FoodItemId;
            var title = response.Data.Title;
            var description = response.Data.Description;
            var text = title + " " + description;
            var value = id;
            $("#" + controlId).append("<option value='" + value + "'>" + text + "</option>");
            APP.Autocomplete.addSelectedItem(controlId, value);
            
            //Clear the fields...
            $("[data-action=add-game-item] :input").each(function () {
                $(this).val("");
            });
        },
        addEmailInvite: function (controlId) {
            var emailField = $("#EmailInvite_Email");
            var firstNameField = $("#EmailInvite_FirstName");
            var lastNameField = $("#EmailInvite_LastName");
            var name = firstNameField.val() + " " + lastNameField.val();
            var value = emailField.val() + "|" + firstNameField.val() + "|" + lastNameField.val();
            $("#" + controlId).append("<option value='" + value + "'>" + name + "</option>");
            APP.Autocomplete.addSelectedItem(controlId, value);
            
            //Clear the fields...
            emailField.val("");
            firstNameField.val("");
            lastNameField.val("");
        },
        addFacebookInvites: function(controlId) {
            var selectedCheckBoxes = $("input:checkbox:checked.facebook-invites");
            var facebookInviteValues = selectedCheckBoxes.map(function() {
                return $(this).val();
            });
            var names = selectedCheckBoxes.map(function() {
                return $(this).attr("Title");
            });

            for (var i = 0; i < facebookInviteValues.length; i++) {
                var facebookId = facebookInviteValues[i];
                var facebookName = names[i];
                var value = facebookId + "|" + facebookName;
                $("#" + controlId).append("<option value='" + value + "'>" + facebookName + "</option>");
                APP.Autocomplete.addSelectedItem(controlId, value);
            }

            //Clear the fields...
            selectedCheckBoxes.each(function () {
                $(this).prop("checked",false);
            });
        }
    },

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
        $selectControls: null,
        init: function () {
            //Setup select 2 stuff...
            $selectControls = $('.fancy-list-box');
            var friendLists = $.grep($selectControls, function (select) {
                return $(select).data("placeholder-type") == "friend-list";
            });
            
            $(friendLists).select2({ placeholder: "Click here to see a list of your friends... " });

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
                }
            });
        },
        addSelectedItem: function (controlId, newVal) {
            //Get the select control
            var control = $.grep($selectControls, function (select) {
                return $(select).attr("id") == controlId;
            });

            //Get the previously selected values
            var previousValues = $(control).select2("val");
            
            //Set the new values
            previousValues.push(newVal);
            $(control).select2("val", previousValues);
        }
    };
}(jQuery, EventPlanner));