(function ($) {

    var actions;

    $.fn.dnnPopup = function (options) {

        // Extend our default options with those provided.
        // Note that the first argument to extend is an empty
        // object – this is to keep from overriding our "defaults" object.
        var opts = $.extend({}, $.fn.dnnPopup.defaults, options);

        var action = opts.action;
        var actionUrl = opts.actionUrl;
        var data = opts.data;

        // Our plugin implementation code goes here.
        if (action === "open") {
            this.click(function () {
                $.ajax({
                    url: actionUrl,
                    datatype: "html",
                    contenttyep: "application/html; charset=utf-8",
                    type: "GET",
                    data: data,
                    success: function (result) {
                        var $modal = $(result);
                        $("body").append($modal);
                        $modal.filter(".modal").modal();
                    },
                    error: function (xhr, status) {

                    }
                });
            });
        }

        if (action === "close") {
            // Close popup code.
        }
        return this;
    };

    // Plugin defaults – added as a property on our plugin function.
    $.fn.dnnPopup.defaults = {
        action: "open",
        buttons : { action: "close", text: "close", actionUrl: "" },
        actionUrl: "/",
        methods: {},
        data: {}
    };
}(jQuery));