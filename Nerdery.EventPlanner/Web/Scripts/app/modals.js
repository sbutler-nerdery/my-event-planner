var Modals = {
    $defaultWidth: null,
    $defaultHeight: null,
    $modals: null,
    init: function() {
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
        $modals.each(function() {
            var modal = this;
            var actionKey = $(modal).data("action");
            var trigger = $("[data-open-modal=" + actionKey + "]").first();
            var longList = $("[data-list=long]");

            longList.height($defaultHeight - 200);
            longList.css({ "overflow-y": "scroll" });
            trigger.click(function() {
                $(modal).dialog("open");
            });
        });
    },
    showLoader: function(actionKey) {
        $("[data-action=" + actionKey + "] .loader").show();
        $("[data-action=" + actionKey + "] :input[type=submit]").hide();
    },
    hideLoader: function(actionKey) {
        $("[data-action=" + actionKey + "] .loader").hide();
        $("[data-action=" + actionKey + "] :input[type=submit]").show();
    },
    open: function(actionKey) {
        var targetModal = Modals.getModelByActionKey(actionKey);
        $(targetModal).dialog("open");
    },
    dismiss: function(actionKey) {
        var targetModal = Modals.getModelByActionKey(actionKey);
        $(targetModal).dialog("close");
    },
    getModelByActionKey: function(actionKey) {
        return $.grep($modals, function(modal) {
            return $(modal).attr("data-action") == actionKey;
        });
    }
};