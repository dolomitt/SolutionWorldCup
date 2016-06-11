$(function () {
    function pageLoad() {

        //teach select2 to accept data-attributes
        $(".chzn-select").each(function () {
            $(this).select2($(this).data());
        });

        //changing default parsley behaviour so it adds error messages to labels.
        //label - is a parent of element
        $("#user-form").parsley({
            errors: {
                container: function (elem, isRadioOrCheckbox) {
                    return elem.parents(".control-group").children("label");
                }
            }
        });

    };

    pageLoad();
    PjaxApp.onPageLoad(pageLoad);
});