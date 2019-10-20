window.webChat = (function () {

    var $messageTemplate, $userTemplate, webSocket, webChatUri;

    function webChat(currentUserName) {
        webChatUri = "ws://" + window.location.host + '/api/Hub?userName=' + currentUserName;

        this.currentUserName = currentUserName;

        $messageTemplate = $('#message-template').clone().removeAttr('id').removeAttr('style');
        $userTemplate = $('#user-template').clone().removeAttr('id').removeAttr('style');

        configureSocket();

        binds();
    }

    this.sendMessage = function () {

        if (webSocket.readyState !== WebSocket.OPEN) {
            $("#msgs").text("Connection is closed");
            return;
        }

        var message = $('#MessageField').val();

        if (message && message !== '') {
            webSocket.send(message);
            $('#MessageField').val('');
        }
    };


    this.disconnect = function () {
        if (webSocket)
            webSocket.close();

        window.location = window.location;
    };

    this.proccessInput = function (e) {
        if (e.which !== 13 || e.ctrlKey) {
            return;
        }

        e.preventDefault();

        sendMessage();
    };

    this.showMessage = function (data) {

        var $messageElement = $messageTemplate.clone();
        var userTextStyle = 'label-default';

        if (data.NewUser) {
            $messageElement.find('.js-user-text').addClass('label-default').text(data.Sender);
            $messageElement.find('.js-message-text').text('entrou na sala');

            loadUsers();
        }
        else if (data.UserLeft) {
            $messageElement.find('.js-user-text').addClass('label-default').text(data.Sender);
            $messageElement.find('.js-message-text').text('saiu da sala');

            loadUsers();
        }
        else {
            var userText = data.Sender + ' disse';


            if (data.Private) {
                if (data.Sender === this.currentUserName)
                    userText = 'Voçê disse a ' + data.Receiver + ' de forma privada';
                else
                    userText = data.Sender + ' disse a voçê de forma privada';

                userTextStyle = 'label-danger';
            }
            else if (data.Tagged) {
                if (data.Receiver === this.currentUserName)
                    userText = data.Sender + ' disse a você';
                else if (data.Sender === this.currentUserName)
                    userText = 'Você disse a ' + data.Receiver;
                else
                    userText = data.Sender + ' disse a ' + data.Receiver;

                userTextStyle = 'label-primary';
            }

            $messageElement.find('.js-user-text').addClass(userTextStyle).text(userText);
            $messageElement.find('.js-message-text').text(data.TextMessage);
        }

        $('#msgs').append($messageElement);
        $('#msgs').scrollTop($('#msgs').prop('scrollHeight'));
    };

    this.loadUsers = function () {
        $.get('/api/WebChat/UsersConnected', function (data) {
            var $html = $('<div></div>');
            var $userComponent;
            var user;
            for (var i = 0; i < data.length; i++) {
                user = data[i];
                $userComponent = $userTemplate.clone();
                $userComponent.find('label').text(user.UserName);

                $html.append($userComponent);
            }

            $('#users-container').html($html);
        });
    };

    this.configureSocket = function () {
        webSocket = new WebSocket(webChatUri);

        webSocket.onopen = e => {
            $('#chat-container, #waiting-msg').toggle();
        };

        webSocket.onclose = function (e) {
            window.location = window.location;
        };

        webSocket.onmessage = function (e) {
            showMessage(JSON.parse(e.data));
        };

        webSocket.onerror = function (e) {
            window.location = window.location;
        };
    };

    this.binds = function () {
        $('#MessageField').keypress(proccessInput);

        $("#btnSend").click(sendMessage);

        $("#btnDisconnect").click(disconnect);
    };

    return webChat;
})();