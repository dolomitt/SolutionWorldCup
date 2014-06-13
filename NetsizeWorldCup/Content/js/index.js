$(function(){
    function pageLoad(){
        function close(e){
            var $settings = $("#settings"),
                $popover = $settings.siblings(".popover");
            if(!$.contains($popover[0], e.target)){
                $settings.popover('hide');
                $(document).off("click", close);
            }
        }

        $("#chat-messages").slimscroll({
            height: '240px',
            size: '5px',
            alwaysVisible: true,
            railVisible: true
        });
    }

    pageLoad();

    PjaxApp.onPageLoad(pageLoad);
});

