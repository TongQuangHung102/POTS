import * as signalR from "@microsoft/signalr";

let hubConnection = null;

export const initSignalR = (setNewNotification) => {
    if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) return; 

    const token = sessionStorage.getItem("token"); 

    hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7259/notificationHub", {
            accessTokenFactory: () => token,
            transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect()
        .build();

    hubConnection.start()
        .then(() => console.log("SignalR Connected"))
        .catch(err => console.error("Lỗi kết nối SignalR:", err));

    hubConnection.on("ReceiveNotification", () => {
        console.log("Nhận thông báo mới!");
        setNewNotification(prev => prev + 1);
    });
};


export const stopSignalR = async () => {
    if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
        await hubConnection.stop();
        console.log("SignalR Disconnected");
        hubConnection = null;
    }
};
