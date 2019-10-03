﻿
window.contextBridge = {

    connectivity: {

        _contextBridgeURI: "%_contextBridgeURI%",
        _contextBridgeIsOpen: false,
        _contextBridgeSocket: null,
        getOrOpenConnection: function (onSuccess, onError) {
            //First connection
            if (window.contextBridge.connectivity._contextBridgeSocket === null) {
                console.log("BlazorMobile: trying to open a new connection");
                window.contextBridge.connectivity.openWSConnection(window.contextBridge.connectivity._contextBridgeURI, onSuccess, onError);
            }
            else if (window.contextBridge.connectivity._contextBridgeIsOpen === true) {
                //Common behavior
                onSuccess(window.contextBridge.connectivity._contextBridgeSocket);
            }
            else {
                //On case of error, the socket is cleared and contextBridgeIsOpen bool value to false
                //So if we are here, that mean a new socket has been instanciated, but is not yet open

                setTimeout(function () {
                    //Second chance interval, if the connection is busy
                    //This should be rare if everything is working
                    if (window.contextBridge.connectivity._contextBridgeIsOpen === true) {
                        onSuccess(window.contextBridge.connectivity._contextBridgeSocket);
                    }
                    else {
                        //If we are here, returning the current task as failed
                        onError();
                    }
                }, 500);
            }
        },
        openWSConnection: function (uri, onOpen, onError) {
            var webSocketURL = null;
            webSocketURL = uri;
            console.log("BlazorMobile: Connecting to websocket server: " + webSocketURL);
            try {
                window.contextBridge.connectivity._contextBridgeSocket = new WebSocket(webSocketURL);
                window.contextBridge.connectivity._contextBridgeSocket.onopen = function (openEvent) {
                    console.log("BlazorMobile: Connected to websocket server");
                    window.contextBridge.connectivity._contextBridgeIsOpen = true;
                    onOpen(window.contextBridge.connectivity._contextBridgeSocket);
                };
                window.contextBridge.connectivity._contextBridgeSocket.onclose = function (closeEvent) {
                    if (closeEvent.code == 3001) {
                        console.log("BlazorMobile: Socket connection to native closed");
                        //The onError is not returned here, as the initial event was typically called at the time we wanted to launch the remote method
                    } else {
                        //If we fall in this case, that mean that the initial connection failed
                        console.log("BlazorMobile: Unable to connect socket to native");
                        onError();
                    }

                    window.contextBridge.connectivity._contextBridgeSocket = null;
                    window.contextBridge.connectivity._contextBridgeIsOpen = false;
                };
                window.contextBridge.connectivity._contextBridgeSocket.onerror = function (errorEvent) {
                    console.log("BlazorMobile: Socket error: " + JSON.stringify(errorEvent, null, 4));
                };
                window.contextBridge.connectivity._contextBridgeSocket.onmessage = function (messageEvent) {
                    var wsMsg = messageEvent.data;
                    window.contextBridge.receive(wsMsg);
                };
            } catch (exception) {
                console.error(exception);
            }
        }
    },
    metadata: {
        GetBlazorMobileWebAssemblyName: function () {
            return 'BlazorMobile.Web';
        },
        GetBlazorMobileReceiveMethodName: function () {
            return 'Receive';
        },
         GetBlazorMobileReceiveFromXamarinMethodName: function () {
             return 'ReceiveFromXamarin';
        }
    },
    send: function (csharpProxy) {
        //TODO: Manage connexion error in the ContextBridge in order to return a failed Task;

        //If window.blazorContextBridgeURI is not null when using 'send', we must use his url instead of the configured one
        //as window.blazorContextBridgeURI reflect the URI generated by the mobile application. This remove any debug/override ip adress
        //during external debugging when launched from a real device

        if (window.blazorContextBridgeURI !== undefined && window.blazorContextBridgeURI !== null) {
            window.contextBridge.connectivity._contextBridgeURI = window.blazorContextBridgeURI;
        }

        window.contextBridge.connectivity.getOrOpenConnection(
            function (wsSocket) {
                //On success
                wsSocket.send(csharpProxy);

        }, function () {
                //On Error
                console.log("BlazorMobile: Unable to connect to native, faulting current task");
                DotNet.invokeMethodAsync(window.contextBridge.metadata.GetBlazorMobileWebAssemblyName(),
                    window.contextBridge.metadata.GetBlazorMobileReceiveMethodName(), csharpProxy, false);
        });
    },
    receive: function (csharpProxy) {
        if (csharpProxy.indexOf("ClientMethodProxyIdentifier") > -1) {
            DotNet.invokeMethodAsync(window.contextBridge.metadata.GetBlazorMobileWebAssemblyName(),
                window.contextBridge.metadata.GetBlazorMobileReceiveFromXamarinMethodName(), csharpProxy, true);
        }
        else {
            DotNet.invokeMethodAsync(window.contextBridge.metadata.GetBlazorMobileWebAssemblyName(),
                window.contextBridge.metadata.GetBlazorMobileReceiveMethodName(), csharpProxy, true);
        }
    }
};

window.contextBridgeSend = function (data) {
    window.contextBridge.send(data);
    return true;
};


window.contextBridgeSendClient = function (assembly, method, params) {
    DotNet.invokeMethodAsync(assembly, method, params);
    return true;
};
