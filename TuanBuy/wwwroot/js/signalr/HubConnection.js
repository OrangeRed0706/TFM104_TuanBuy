////var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
////// 傳送通知事件
//////connection.on("ReceiveMessage", function (user, message) {
//////    var msg = message;
//////    document.getElementById("notifyBell").click();
//////    layout.message = user+"：" + msg;
//////    console.log("message:" + msg);
//////});
////// 連接事件
//////
////connection.start().then((res) => {
////    console.log("連線成功");
////    connection.invoke("GetAllNotify").then(() => {
////        console.log("取得通知成功！");

////    });
////}).catch(function (err) {
////    return console.error(err.toString());
////});