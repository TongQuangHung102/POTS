import * as signalR from "@microsoft/signalr";

let hubConnection = null;

export const initSignalR = (setNewNotification) => {
    const apiUrl = process.env.REACT_APP_API_URL;
    if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) return; 

    const token = localStorage.getItem("token"); 

    hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${apiUrl}/notificationHub`, {
            accessTokenFactory: () => token,
            transport: signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect()
        .build();

    hubConnection.start()
        .then(() => console.log("SignalR Connected"))
        .catch(err => console.error("Lỗi kết nối SignalR:", err));

    hubConnection.on("ReceiveNotification", () => {
      
        setNewNotification(prev => prev + 1);
    });
};


export const stopSignalR = async () => {
    if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
        await hubConnection.stop();
 
        hubConnection = null;
    }
};
