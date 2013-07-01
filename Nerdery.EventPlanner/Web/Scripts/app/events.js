//dependancies: Modal, Ajax, Autocomplete
var Events = {
    $delimieter: null,
    $guestListClass: null,
    init: function() {
        $delimieter = "╫";
        $guestListClass = ".guest-lists";
    },
    getSingleFoodItem: function(foodItemId, callback) {
        Service.call("/Service/GetSingleFoodItem", { foodItemId: foodItemId }, callback);
    },
    viewFoodItem: function(foodItemId, actionKey) {
        //Get single item from the database
        var callback = function(response) {
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
            Modals.open(actionKey);
        };

        Events.getSingleFoodItem(foodItemId, callback);
    },
    refreshFoodItems: function(actionKey, response) {
        if (response.Error) {
            alert(response.Message);
            return;
        }

        //var actionKey = "add-food-item";
        var list = $(".food-list").first();
        list.html(response.Data);

        //Clear the fields...
        $("[data-action=" + actionKey + "] :input[type=text]").each(function() {
            $(this).val("");
        });

        Modals.dismiss(actionKey);
    },
    addExistingFoodItem: function(foodItemId, personId, eventId) {
        var callback = function(response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var list = $(".food-list").first();
            list.html(response.Data);
        };

        Service.call("/Service/AddExistingFoodItem", { eventId: eventId, foodItemId: foodItemId, personId: personId }, callback);
    },
    updateFoodItem: function(foodItemId, actionKey) {
        //Get single item from the database
        var callback = function(response) {
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

        Events.getSingleFoodItem(foodItemId, callback);
    },
    removeFoodItem: function(eventId, foodItemId) {
        var callback = function(response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var list = $(".food-list").first();
            list.html(response.Data);
        };

        Service.call("/Service/RemoveFoodItem", { eventId: eventId, foodItemId: foodItemId }, callback);
    },
    getSingleGame: function(gameId, callback) {
        Service.call("/Service/GetSingleGame", { gameId: gameId }, callback);
    },
    viewGame: function(gameId, actionKey) {
        //Get single item from the database
        var callback = function(response) {
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

        Events.getSingleGame(gameId, callback);
    },
    refreshGames: function(actionKey, response) {
        if (response.Error) {
            alert(response.Message);
            return;
        }

        //var actionKey = "add-game-item";
        var list = $(".game-list").first();
        list.html(response.Data);

        //Clear the fields...
        $("[data-action=" + actionKey + "] :input[type=text]").each(function() {
            $(this).val("");
        });

        Modals.dismiss(actionKey);
    },
    addExistingGame: function(gameId, personId, eventId) {
        var callback = function(response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var list = $(".game-list").first();
            list.html(response.Data);
        };

        Service.call("/Service/AddExistingGame", { eventId: eventId, gameId: gameId, personId: personId }, callback);
    },
    removeGame: function(eventId, gameId) {
        var callback = function(response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var actionKey = "remove-game-item";
            var list = $(".game-list").first();
            list.html(response.Data);
        };

        Service.call("/Service/RemoveGame", { eventId: eventId, gameId: gameId }, callback);
    },
    updateGame: function(gameId, actionKey) {
        //Get single item from the database
        var callback = function(response) {
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

        Events.getSingleGame(gameId, callback);
    },
    /**/
    getSingleGuest: function(guestId, callback) {
        Service.call("/Service/GetEventGuest", { guestId: guestId }, callback);
    },
    viewGuest: function(guestId, actionKey) {
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
    refreshGuests: function(actionKey, response) {
        if (response.Error) {
            alert(response.Message);
            return;
        }

        //var actionKey = "add-game-item";
        var list = $($guestListClass).first();
        list.html(response.Data);

        //Clear the fields...
        $("[data-action=" + actionKey + "] :input[type=text]").each(function() {
            $(this).val("");
        });

        Modals.dismiss(actionKey);
    },
    addRegisteredGuest: function(guestId, personId, eventId) {
        var callback = function(response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var list = $($guestListClass).first();
            list.html(response.Data);
        };

        Service.call("/Service/AddPreviousGuest", { eventId: eventId, guestId: guestId, personId: personId }, callback);
    },
    removeGuest: function(eventId, guestId) {
        var callback = function(response) {
            if (response.Error) {
                alert(response.Message);
                return;
            }

            var actionKey = "remove-guest";
            var list = $($guestListClass).first();
            list.html(response.Data);
        };

        Service.call("/Service/RemoveGuest", { eventId: eventId, guestId: guestId }, callback);
    },
    updateGuest: function(guestId, actionKey) {
        //Get single item from the database
        var callback = function(response) {
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

        Events.getSingleGuest(guestId, callback);
    },
    /**/
    addEmailInvite: function(response) {
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
        $("[data-action=" + actionKey + "] :input[type=text]").each(function() {
            $(this).val("");
        });

        Modals.dismiss(actionKey);
    },
    //This is not being used at the moment because there is no way to send a facebook invitation via the Graph API
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
            Autocomplete.addSelectedItem(controlId, value);
        }

        //Clear the fields...
        selectedCheckBoxes.each(function() {
            $(this).prop("checked", false);
        });
    }
};