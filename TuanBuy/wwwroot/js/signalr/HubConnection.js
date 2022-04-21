var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
/* 傳送通知事件*/
//connection.on("ReceiveMessage", function (user, message) {
//    var msg = message;
//    document.getElementById("notifyBell").click();
//    layout.message = user+"：" + msg;
//    console.log("message:" + msg);
//});
// 連接事件

//先抓取目前連線使用者並註冊Hub
var userid = 0;
var userAccount = '';
var userName = '';
var userPicPath = '';
var userConnectionId = '';
//將目前登入使用者連線Hub
$.ajax({
    type: "GET",
    url: "/Member/GetOnlineMember",
    success: function (response) {
        userid = response.id;
        userAccount = response.email;
        userName = response.nickName;
        userPicPath = '/MemberPicture/' + response.picPath;
        //app.chatMessageTitle = userName + '個人聊天室';
        //app.chatMessagePicPath = userPicPath;
        if (userAccount != null) {
            connection.start().then(function () {
                //先取得當前使用者連線id
                connection.invoke('getConnectionId')
                    .then(function (connectionId) {
                        userConnectionId = connectionId;
                    }).catch(err => console.error(err.toString()));;

                // 加入群組
                connection.invoke("AddUserList", userName, userid, userAccount).catch(function (err) {
                    return console.error(err.toString());
                });
                /*        connection.invoke("GetUserList");*/

                connection.invoke("GetOnlineUserList", userid).catch(function (err) {
                    return console.error(err.toString());
                });

                connection.invoke("GetAllNotify").then(() => {
                    console.log("取得通知成功！");
                });

                connection.invoke("GetUserList").then((msg) => {
                    console.log("使用者再線清單！");
                    console.log(msg);
                });
            });
        }
    },
    error: function (response) {
        console.log('尚未登入');
    }
});





