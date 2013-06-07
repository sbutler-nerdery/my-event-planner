var EventPlanner = EventPlanner || {};

(function ($, APP) {
    // DOM Ready Function
    $(function () {
        APP.Calendars.init();
        APP.Events.init();
        APP.Autocomplete.init();
        APP.Tabs.init();
        APP.Modals.init();
    });

    APP.Events = {
        $delimieter: null,
        $guestListClass: null,
        init: function () {
            $delimieter = "╫";
            $guestListClass = ".guest-lists";
        },
        getSingleFoodItem: function (foodItemId, callback) {
            APP.Ajax.call("/Service/GetSingleFoodItem", { foodItemId: foodItemId }, callback);
        },
        viewFoodItem: function (foodItemId, actionKey) {
            //Get single item from the database
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var foodItem = response.Data;

                //Set the appropriate form values
                var dialog = $("[data-action=view-food-item]").first();
                dialog.find(".food-title").html(foodItem.Title);
                dialog.find(".food-description").html(foodItem.Description);

                //Open the dialog box for the specified action key
                APP.Modals.open(actionKey);
            };

            APP.Events.getSingleFoodItem(foodItemId, callback);
        },
        refreshFoodItems: function (actionKey, response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            //var actionKey = "add-food-item";
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
        updateFoodItem: function (foodItemId, actionKey) {
            //Get single item from the database
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var foodItem = response.Data;

                //Set the appropriate form values
                $("#UpdateFoodItem_FoodItemId").val(foodItemId);
                $("#UpdateFoodItem_Title").val(foodItem.Title);
                $("#UpdateFoodItem_Description").val(foodItem.Description);

                //Open the dialog box for the specified action key
                APP.Modals.open(actionKey);
            };

            APP.Events.getSingleFoodItem(foodItemId, callback);
        },
        removeFoodItem: function (eventId, foodItemId) {
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var list = $(".food-list").first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/RemoveFoodItem", { eventId: eventId, foodItemId: foodItemId }, callback);
        },
        getSingleGame: function (gameId, callback) {
            APP.Ajax.call("/Service/GetSingleGame", { gameId: gameId }, callback);
        },
        viewGame: function (gameId, actionKey) {
            //Get single item from the database
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var game = response.Data;

                //Set the appropriate form values
                var dialog = $("[data-action=view-game]").first();
                dialog.find(".game-title").html(game.Title);
                dialog.find(".game-description").html(game.Description);

                //Open the dialog box for the specified action key
                APP.Modals.open(actionKey);
            };

            APP.Events.getSingleGame(gameId, callback);
        },
        refreshGames: function (actionKey, response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            //var actionKey = "add-game-item";
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
        removeGame: function (eventId, gameId) {
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var actionKey = "remove-game-item";
                var list = $(".game-list").first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/RemoveGame", { eventId: eventId, gameId: gameId }, callback);
        },
        updateGame: function (gameId, actionKey) {
            //Get single item from the database
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var foodItem = response.Data;

                //Set the appropriate form values
                $("#UpdateGameItem_GameId").val(gameId);
                $("#UpdateGameItem_Title").val(foodItem.Title);
                $("#UpdateGameItem_Description").val(foodItem.Description);

                //Open the dialog box for the specified action key
                APP.Modals.open(actionKey);
            };

            APP.Events.getSingleGame(gameId, callback);
        },
        /**/
        getSingleGuest: function (guestId, callback) {
            APP.Ajax.call("/Service/GetEventGuest", { guestId: guestId }, callback);
        },
        viewGuest: function (guestId, actionKey) {
            //Get single item from the database
            //var callback = function (response) {
            //    if (response.Error) {
            //        alert(response.Message);
            //        return;
            //    }

            //    var game = response.Data;

            //    //Set the appropriate form values
            //    var dialog = $("[data-action=view-guest]").first();
            //    dialog.find(".game-title").html(game.Title);
            //    dialog.find(".game-description").html(game.Description);

            //    //Open the dialog box for the specified action key
            //    APP.Modals.open(actionKey);
            //};

            //APP.Events.getSingleGuest(guestId, callback);
        },
        refreshGuests: function (actionKey, response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            //var actionKey = "add-game-item";
            var list = $($guestListClass).first();
            list.html(response.Data);

            //Clear the fields...
            $("[data-action=" + actionKey + "] :input[type=text]").each(function () {
                $(this).val("");
            });

            APP.Modals.dismiss(actionKey);
        },
        addRegisteredGuest: function (guestId, personId, eventId) {
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var list = $($guestListClass).first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/AddPreviousGuest", { eventId: eventId, guestId: guestId, personId: personId }, callback);
        },
        removeGuest: function (eventId, guestId) {
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var actionKey = "remove-guest";
                var list = $($guestListClass).first();
                list.html(response.Data);
            };

            APP.Ajax.call("/Service/RemoveGuest", { eventId: eventId, guestId: guestId }, callback);
        },
        updateGuest: function (guestId, actionKey) {
            //Get single item from the database
            var callback = function (response) {
                if (response.Error) {
                    alert(response.Message);
                    return;
                }

                var guest = response.Data;

                //Set the appropriate form values
                $("#UpdateGuest_PersonId").val(guestId);
                $("#UpdateGuest_FirstName").val(guest.FirstName);
                $("#UpdateGuest_LastName").val(guest.LastName);
                $("#UpdateGuest_Email").val(guest.Email);

                //Open the dialog box for the specified action key
                APP.Modals.open(actionKey);
            };

            APP.Events.getSingleGuest(guestId, callback);
        },
        /**/
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
            var value = (id == 0) ? email + $delimieter + response.Data.FirstName + $delimieter + response.Data.LastName : id;
            $("#" + controlId).append("<option value='" + value + "'>" + text + "</option>");
            APP.Autocomplete.addSelectedItem(controlId, value);

            //Clear the fields...
            $("[data-action=" + actionKey + "] :input[type=text]").each(function () {
                $(this).val("");
            });

            EventPlanner.Modals.dismiss(actionKey);
        },
        //This is not being used at the moment because there is no way to send a facebook invitation via the Graph API
        addFacebookInvites: function (controlId) {
            var selectedCheckBoxes = $("input:checkbox:checked.facebook-invites");
            var facebookInviteValues = selectedCheckBoxes.map(function () {
                return $(this).val();
            });
            var names = selectedCheckBoxes.map(function () {
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
                $(this).prop("checked", false);
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
        init: function () {
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
                longList.css({ "overflow-y": "scroll" });
                trigger.click(function () {
                    $(modal).dialog("open");
                });
            });
        },
        showLoader: function (actionKey) {
            $("[data-action=" + actionKey + "] .loader").show();
            $("[data-action=" + actionKey + "] :input[type=submit]").hide();
        },
        hideLoader: function (actionKey) {
            $("[data-action=" + actionKey + "] .loader").hide();
            $("[data-action=" + actionKey + "] :input[type=submit]").show();
        },
        open: function (actionKey) {
            var targetModal = APP.Modals.getModelByActionKey(actionKey);
            $(targetModal).dialog("open");
        },
        dismiss: function (actionKey) {
            var targetModal = APP.Modals.getModelByActionKey(actionKey);
            $(targetModal).dialog("close");
        },
        getModelByActionKey: function (actionKey) {
            return $.grep($modals, function (modal) {
                return $(modal).attr("data-action") == actionKey;
            });
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

            $(friendLists).select2({ placeholder: "Click here to see a list of your friends... " });

            //jQuery autocomplete
            var callback = function (response) {
                if (!response.Error) {
                    $("[data-autocomplete=true]").autocomplete({ source: JSON.parse(response.Data) });
                } else {
                    alert(response.Message);
                }
            };

            APP.Ajax.call("/Service/GetTimeList", null, callback);

            var eventId = $("#EventId").val();
            var personId = $("#PersonId").val();
            $("[data-autocomplete-list=food]").autocomplete({
                minLength: 0,
                source: function (request, response) {
                    callback = function (serverResponse) {
                        if (!serverResponse.Error) {
                            response($.map(serverResponse.Data, function (item) {
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

            $("[data-autocomplete-list=games]").autocomplete({
                minLength: 0,
                source: function (request, response) {
                    callback = function (serverResponse) {
                        if (!serverResponse.Error) {
                            response($.map(serverResponse.Data, function (item) {
                                return { label: item.Title, value: item.GameId };
                            }));
                        } else {
                            alert(serverResponse.Message);
                        }
                    };
                    APP.Ajax.call("/Service/GetPersonGameList", { personId: personId, eventId: eventId, contains: request.term }, callback);
                },
                select: function (event, ui) {
                    this.value = "";

                    //Add the existing item to the list of items
                    APP.Events.addExistingGame(ui.item.value, personId, eventId);

                    return false;
                }
            }).on("click", function () {
                $(this).autocomplete("search", "");
            });
            
            $("[data-autocomplete-list=guests]").autocomplete({
                minLength: 0,
                source: function (request, response) {
                    callback = function (serverResponse) {
                        if (!serverResponse.Error) {
                            response($.map(serverResponse.Data, function (item) {
                                return { label: item.FirstName + " " + item.LastName, value: item.PersonId };
                            }));
                        } else {
                            alert(serverResponse.Message);
                        }
                    };
                    APP.Ajax.call("/Service/GetGuestList", { eventId: eventId, contains: request.term }, callback);
                },
                select: function (event, ui) {
                    this.value = "";

                    //Add the existing item to the list of items
                    APP.Events.addRegisteredGuest(ui.item.value, personId, eventId);

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