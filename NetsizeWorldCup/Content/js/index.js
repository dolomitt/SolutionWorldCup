$(function () {
    function pageLoad() {
        function close(e) {
            var $settings = $("#settings"),
                $popover = $settings.siblings(".popover");
            if (!$.contains($popover[0], e.target)) {
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

        $(function () {
            // Reference the auto-generated proxy for the hub.
            var chat = $.connection.chatHub;
            var inc = 0;
            // Create a function that the hub can call back to display messages.
            chat.client.addNewMessageToPage = function (name, pic, message) {
                // Add the message to the page.
                $('#chat-messages').append('<div class="chat-message"><div class="sender ' + ((inc % 2 == 0) ? 'pull-left' : 'pull-right') + '"><div class="icon"><img src="' + pic + '" class="img-circle" alt=""></div><div class="time"></div></div><div class="chat-message-body' + ((inc % 2 == 1) ? ' on-left' : '') + '"><span class="arrow"></span><div class="sender">' + htmlEncode(name) + '</div><div class="text">' + htmlEncode(message) + '</div></div></div>');
                inc++;
            };
            // Get the user name and store it to prepend to messages.
            $('#displayname').val("@User.Identity.Name");
            $('#displaypic').val("/Content/Img/2.jpg");
            // Set initial focus to message input box.
            $('#message').focus();
            // Start the connection.
            $.connection.hub.start().done(function () {
                $('#message').keypress(function (e) {
                    //if we press enter in the message box we simulate a click on send button
                    if (e.keyCode == 13)
                        $('#sendmessage').click();
                });
                $('#sendmessage').click(function () {

                    //ignoring empty messages
                    if ($('#message').val() == '')
                        return;

                    // Call the Send method on the hub.
                    chat.server.send($('#displaypic').val(), $('#message').val());
                    // Clear text box and reset focus for next comment.
                    $('#message').val('').focus(); 

                    $('#chat-messages').slimScroll({ scrollBy: $("#chat-messages").height() });
                });
            });
        });

        // This optional function html-encodes messages for display in the page.
        function htmlEncode(value) {
            var encodedValue = $('<div />').text(value).html();
            return encodedValue;
        }

        $.get("/Message/GetLastMessages", function (data) {
            var inc = 0;
            data.Messages.forEach(function (entry) {
                $('#chat-messages').append('<div class="chat-message"><div class="sender ' + ((inc % 2 == 0) ? 'pull-left' : 'pull-right') + '"><div class="icon"><img src="' + entry.PictureUrl + '" class="img-circle" alt=""></div><div class="time"></div></div><div class="chat-message-body' + ((inc % 2 == 1) ? ' on-left' : '') + '"><span class="arrow"></span><div class="sender">' + htmlEncode(entry.Name) + '</div><div class="text">' + htmlEncode(entry.Message) + '</div></div></div>');
                inc++;
            });

            $('#chat-messages').slimScroll({ scrollBy: $("#chat-messages").height() });
        });
    };

    pageLoad();

    PjaxApp.onPageLoad(pageLoad);
});

