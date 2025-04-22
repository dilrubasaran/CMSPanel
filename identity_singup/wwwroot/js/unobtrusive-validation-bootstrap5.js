(function ($) {
    // Make ASP's unobtrusive validation compatible with Bootstrap 5 styling. See this for more details: https://stackoverflow.com/a/19006517/576153
    $.validator.addMethod("noonlyspace", function(value, element) {
        return this.optional(element) || value.trim().length > 0;
    }, "Bu alan boş bırakılamaz");

    // Add unobtrusive adapter for noonlyspace rule
    $.validator.unobtrusive.adapters.add("noonlyspace", [], function (options) {
        options.rules["noonlyspace"] = true;
        options.messages["noonlyspace"] = options.message;
    });

    $.validator.setDefaults({
        errorElement: "span",
        errorClass: "invalid-feedback",
        onfocusout: function(element) {
            // Enable validation on blur
            if (!this.checkable(element)) {
                // Trim whitespace from the input value
                var $element = $(element);
                var value = $element.val();
                if (value && value.trim() === '') {
                    $element.val('');
                }
                this.element(element);
            }
        },
        highlight: function (element, errorClass, validClass) {
            // Only validation controls
            if (!$(element).hasClass('novalidation')) {
                $(element).closest('.form-control').removeClass('is-valid').addClass('is-invalid');
            }
        },
        unhighlight: function (element, errorClass, validClass) {
            // Only validation controls
            if (!$(element).hasClass('novalidation')) {
                $(element).closest('.form-control').removeClass('is-invalid').addClass('is-valid');
            }
        },
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            }
            else if (element.prop('type') === 'radio' && element.parent('.radio-inline').length) {
                error.insertAfter(element.parent().parent());
            }
            else if (element.prop('type') === 'checkbox' || element.prop('type') === 'radio') {
                error.appendTo(element.parent().parent());
            }
            else {
                error.insertAfter(element);
            }
        }
    });
}(jQuery));