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
        addFoodItem: function (response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var actionKey = "add-food-item";
            var list = $(".food-list").first();
            list.html(response.Data);

            //Clear the fields...
            $("[data-action=" + actionKey + "] :input[type=text]").each(function () {
                $(this).val("");
            });
            
            APP.Modals.dismiss(actionKey);
        },
        addExistingFoodItem: function (foodItemId, personId, eventId) {
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var list = $(".food-list").first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/AddExistingFoodItem", { eventId: eventId, foodItemId: foodItemId, personId: personId }, callback);
        },
        removeFoodItem: function (eventId, foodItemId) {
            var callback = function(response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var list = $(".food-list").first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/RemoveFoodItem", {eventId : eventId, foodItemId : foodItemId}, callback);
        },
        addGame: function (response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var actionKey = "add-game-item";
            var list = $(".game-list").first();
            list.html(response.Data);

            //Clear the fields...
            $("[data-action=" + actionKey + "] :input[type=text]").each(function () {
                $(this).val("");
            });
            
            APP.Modals.dismiss(actionKey);
        },
        addExistingGame: function (gameId, personId, eventId) {
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var list = $(".game-list").first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/AddExistingGame", { eventId: eventId, gameId: gameId, personId: personId }, callback);
        },
        removeGame: function (response) {
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var actionKey = "remove-game-item";
                var list = $(".game-list").first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/RemoveGame", { eventId: eventId, foodItemId: foodItemId }, callback);
        },
        addEmailInvite: function (response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var actionKey = "invite-person-to-event";
            var controlId = response.Data.InviteControlId;
            var id = response.Data.PersonId;
            var email = response.Data.Email;
            var text = (response.Data.FirstName == null || response.Data.LastName == null)
                ? response.Data.UserName
                : response.Data.FirstName + " " + response.Data.LastName;
            var value = (id == 0) ? email + "╫" + response.Data.FirstName + "╫" + response.Data.LastName : id;
            $("#" + controlId).append("<option value='" + value + "'>" + text + "</option>");
            APP.Autocomplete.addSelectedItem(controlId, value);

            //Clear the fields...
            $("[data-action=" + actionKey + "] :input[type=text]").each(function () {
                $(this).val("");
            });

            EventPlanner.Modals.dismiss(actionKey);
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
            $defaultWidth = 800;
            $defaultHeight = 400;
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
    APP.Ajax = {
        call: function (url, args, successCallback) {
            $.ajax({
                url: url,
                type: "POST",
                data: args,
                dataType: "json",
                /*NOTE: if you use complete instead of success, you will get back a different json object!!! 
                Took me a while to figure that out. */
                success: function (response) {
                    successCallback(response);
                }
            });
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
            var foodLists = $.grep($selectControls, function (select) {
                return $(select).data("placeholder-type") == "food-list";
            });
            var gameLists = $.grep($selectControls, function (select) {
                return $(select).data("placeholder-type") == "game-list";
            });
            $(friendLists).select2({ placeholder: "Click here to see a list of your friends... " });
            $(foodLists).select2({ placeholder: "Click here to see a list of your munchies... " });
            $(gameLists).select2({ placeholder: "Click here to see a list of your games... " });

            //jQuery autocomplete
            var callback = function(response) {
                if (!response.Error) {
                    $("[data-autocomplete=true]").autocomplete({ source: JSON.parse(response.Data) });
                } else {
                    alert(response.Message);
                }
            };
            
            APP.Ajax.call("/Service/GetTimeList", null, callback);
                       
            var eventId = $("#EventId").val();
            var personId = $("#PersonId").val();
            var isSelecting = false;
            $("[data-autocomplete-list=food]").autocomplete({
                minLength:0,
                source: function (request, response) {
                    callback = function (serverResponse) {
                        if (!serverResponse.Error) {
                            response($.map(serverResponse.Data, function(item) {
                                return { label: item.Title, value: item.FoodItemId };
                            }));
                        } else {
                            alert(serverResponse.Message);
                        }
                    };
                    APP.Ajax.call("/Service/GetPersonFoodList", { personId: personId, eventId: eventId, contains: request.term }, callback);
                },
                select: function (event, ui) {
                    this.value = "";
                    
                    //Add the existing item to the list of items
                    APP.Events.addExistingFoodItem(ui.item.value, personId, eventId);

                    return false;
                }
            }).on("click", function () {
                $(this).autocomplete("search", "");
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