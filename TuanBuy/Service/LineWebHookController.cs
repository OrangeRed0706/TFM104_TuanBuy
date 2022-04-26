using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Line;

namespace TuanBuy.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineWebHookController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;
        private readonly TuanBuyContext _dbContext;

        //建構子注入
        public LineWebHookController(IHttpClientFactory factory, TuanBuyContext db)
        {
            _factory = factory;
            _dbContext = db;
            Console.WriteLine($"HttpClient工廠:{_factory.ToString()}");
        }
        //跟Line Bot掛鉤的服務（替代Line Bot腦袋）
        [Route("Line/webhook")]
        [HttpPost]
        [Consumes("application/json")]
        public void ReceiveMessage([FromBody] WebHookEventData data)
        {
            #region 發送字串
            //{
            //    "destination": "string",
            //    "events": [
            //    {
            //        "replyToken": "string",
            //        "type": "message",
            //        "mode": "string",
            //        "timestamp": 0,
            //        "source": {
            //            "type": "user",
            //            "userId": "U35de2fbf9d2a6d87e9d5e21877664ac2"
            //        },
            //        "message": {
            //            "id": "string",
            //            "type": "text",
            //            "text": "string",
            //            "emojis": [
            //            {
            //                "index": 0,
            //                "length": 0,
            //                "productId": "string",
            //                "emojiId": "string"
            //            }
            //            ],
            //            "mention": {
            //                "mentionees": [
            //                {
            //                    "index": 0,
            //                    "length": 0,
            //                    "userId": "string"
            //                }
            //                ]
            //            }
            //        }
            //    }
            //    ]
            //}


            #endregion

            //先取出type(判斷WebHook)
            string type = data.events[0].type;
            string userid = null;
            string ResponseText = null;
            switch (type)
            {
                //處理前端送進來訊息
                case "message":

                    if (data.events[0].source.type.Equals("user"))
                    {
                        userid = data.events[0].source.userId;
                    }

                    //取出Line前端使用聊天資訊(文字類型)
                    String text = null;
                    if (data.events[0].message.type.Equals("text"))
                    {
                        text = data.events[0].message.text;
                    }

                    //進行處理
                    if (text != null)
                    {
                        //TODO 推向AI 進行語意解析(NLP Natual Language Process)
                        //處理好了 鸚鵡說
                        ResponseText = $"誰:{userid} 說:{text}";
                        Console.WriteLine(ResponseText);
                    }
                    else
                    {

                    }

                    break;
                //處理前端加入好友 或者解除封鎖
                case "follow":
                    {
                        //TODO 加入好友的資料新增作業
                        //取出相關傳遞資訊 封裝成Entity物件
                        userid = data.events[0].source.userId;
                        String userType = data.events[0].source.type;
                        //時間點(時間戳記)
                        Int64 timestamp = data.events[0].timestamp;
                        //如何轉換常整數為日期與時間???
                        TimeSpan time = TimeSpan.FromMilliseconds(timestamp);
                        DateTime curDate = new DateTime(1970, 1, 1) + time;


                        //查看看是否既定的解除封鎖
                        //封裝成Entity物件
                        var result = (from m in _dbContext.LineMember
                                      where m.userid == userid
                                      select m).FirstOrDefault();
                        if (result != null)
                        {
                            result.status = true; //離開了
                            result.unfollowdatetime = null;
                            //.那一個Entity物件內有一個看不到前端維護狀態RowState 
                            //同步更新回資料庫資料表
                            _dbContext.SaveChanges();
                            ResponseText = $"誰:{userid} \n歡迎你重新加入。";

                        }
                        LineMember member = new LineMember()
                        {
                            //沒有維護全球唯一識別碼欄位UniqueIdentifier 
                            userid = userid,
                            timestamp = curDate,
                            status = true,
                            type = userType
                        };
                        //透過DbContext加入物件到Persistence 中
                        _dbContext.LineMember.Add(member);
                        //同步更新到資料庫去
                        Int32 r = _dbContext.SaveChanges();

                        //透過前端Persistence(DbContext)進行物件新增 在同步更新回資料庫
                        ResponseText = $"誰:{userid} \n歡迎你加入這一個群組。";
                        break;
                    }
                //封鎖帳號引發事件
                case "unfollow":
                    {
                        //取出相關傳遞資訊 封裝成Entity物件
                        userid = data.events[0].source.userId;
                        String userType = data.events[0].source.type;
                        //時間點(時間戳記)
                        Int64 timestamp = data.events[0].timestamp;
                        //呼叫自訂模組類別成員 進行轉換DateTime
                        DateTime curDate = AppUtility.convertLongToDateTime(timestamp);
                        //透過Entity Framework查詢 找出這一個物件模組
                        var result = (from m in _dbContext.LineMember
                                      where m.userid == userid
                                      select m).FirstOrDefault();
                        //變更物件屬性內容(連動RowSate狀態 由Browse->Modify 模式)
                        result.status = false; //離開了
                        result.unfollowdatetime = curDate;
                        //.那一個Entity物件內有一個看不到前端維護狀態RowState 
                        //同步更新回資料庫資料表
                        _dbContext.SaveChanges();

                        break;
                    }
            }
            //TODO ---處理好了(send push message)回給使用者訊息（已讀已回）
            //呼叫自訂類別的成員（Utliity）取得特定的configuration file配置組態物件
            IConfiguration config = AppUtility.getConfiguration("appsettings.json");
            //取出send push message URL
            string urlString = config.GetValue<string>("Line:pushMessage");
            string token = config.GetValue<string>("Line:token");

            //跟Http工廠要一個HttpClient物件
            HttpClient client = _factory.CreateClient();
            //請求的位址
            client.BaseAddress = new Uri(urlString);
            //http Header:Authorization
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            //建構HttpRequestMessage
            HttpRequestMessage request = new HttpRequestMessage();
            //設定請求傳送方法
            request.Method = HttpMethod.Post;
            //設定Body內容 Json String
            //使用物件化 再來序列化
            Models.Line.PushMessageData pushData = new Models.Line.PushMessageData()
            {
                to = userid,
                messages =
                    new Models.Line.PushMessage[]
                    {
                        new Models.Line.PushMessage()
                        {
                            type = "text",
                            text = ResponseText
                        }
                    }
            };
            //序列化成Json String
            string jsonPush = JsonConvert.SerializeObject(pushData);
            //建立HttpContent物件
            StringContent content = new StringContent(jsonPush);
            request.Content = content;
            //加入Request Header-Content-Type application/json

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //設定Headers(Content-Type)
            Console.WriteLine($"request{request}");
            HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
            string res = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Console.WriteLine($"Res：{res}");
        }
        [Route("Line/LinePushMessage")]
        [HttpPost]
        public void LinePushMessage()
        {
            //設計HttpClient for .net framework
            //1建構一個HttpClient物件
            HttpClient client = new HttpClient();
            //設定服務位址(line send push Mssage)
            String urlString = "https://api.line.me/v2/bot/message/push";
            client.BaseAddress = new Uri(urlString);
            //需要進行Header Authorization
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "KAvTOeGvQOxfbd8baVHkJaHuFwWuQE6puk5dsNS8CrmjEp6hhOY+kZC00/IhI0BHocPD9QveYllD9wXAwgbFRaKO8xEr2WDcOEXrwO2Lz0hc0EJKODdHtNwqA21TyoaRBz22XGhXtZh00dxehl7z/gdB04t89/1O/w1cDnyilFU=");
            //建構Push Message物件
            LineWebHookController.PushMessageData pushMessageData = new PushMessageData()
            {
                to = "U35de2fbf9d2a6d87e9d5e21877664ac2",
                messages = new Message[]
                {
                    new Message()
                    {
                        type="text",
                        text=$"我來了 來自服務通知...{DateTime.UtcNow.AddHours(8)} "
                    }
                }
            };
            //序列化成Json字串
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(pushMessageData);

            //建構HttpContent物件
            HttpContent content = new StringContent(jsonString);
            //設定Header Content-Type
            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //正式提出請求採用Post
            HttpResponseMessage response =
                client.PostAsync("", content).GetAwaiter().GetResult();

        }
        public class PushMessageData
        {
            public string to { get; set; }
            public Message[] messages { get; set; }
        }

        public class Message
        {
            public string type { get; set; }
            public string text { get; set; }
        }
    }
}

