using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TuanBuy.Migrations
{
    public partial class test123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatRoomTitle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.ChatRoomId);
                });

            migrationBuilder.CreateTable(
                name: "LineMember",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unfollowdatetime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineMember", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "NotifyCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotifyCategory", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "OrderState",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderState", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PicPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    Friend = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoucherDescribe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PicPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountDescribe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VouchersDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VouchersAvlAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "ChatRoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member_Chats",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member_Chats", x => new { x.MemberId, x.ChatRoomId });
                    table.ForeignKey(
                        name: "FK_Member_Chats_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "ChatRoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member_Chats_User_MemberId",
                        column: x => x.MemberId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_OrderState_StateId",
                        column: x => x.StateId,
                        principalTable: "OrderState",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotify",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotify", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotify_NotifyCategory_Category",
                        column: x => x.Category,
                        principalTable: "NotifyCategory",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotify_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVouchers",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVouchers", x => new { x.MemberId, x.VoucherId });
                    table.ForeignKey(
                        name: "FK_UserVouchers_User_MemberId",
                        column: x => x.MemberId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVouchers_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMessages_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PicPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPics_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSellerReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductMessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSellerReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSellerReplies_ProductMessages_ProductMessageId",
                        column: x => x.ProductMessageId,
                        principalTable: "ProductMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "OrderState",
                columns: new[] { "StateId", "State" },
                values: new object[,]
                {
                    { 1, "待付款" },
                    { 2, "待出貨" },
                    { 3, "待收貨" },
                    { 4, "完成" },
                    { 5, "不成立" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "BankAccount", "Birth", "Disable", "Email", "Friend", "Name", "NickName", "Password", "Phone", "PicPath", "Sex", "State" },
                values: new object[,]
                {
                    { 1, null, null, null, false, "123@gmail.com", null, "小王", "賣貓的小王", "123456", null, "637843188933582087init.jpg", 1, "正式會員" },
                    { 2, null, null, null, false, "456@gmail.com", null, "小明", "賣鮭魚的小明", "123456", null, "637843188933582087init.jpg", 1, "正式會員" },
                    { 3, null, null, null, false, "789@gmail.com", null, "小張", "賣記憶體的小張", "123456", null, "637843188933582087init.jpg", 1, "正式會員" },
                    { 4, null, null, null, false, "Lynn@gmail.com", null, "Lynn", "Lynn", "123456", null, "637843188933582087init.jpg", 1, "系統管理員" },
                    { 5, null, null, null, false, "Benny@gmail.com", null, "Benny", "Benny", "123456", null, "637843188933582087init.jpg", 1, "系統管理員" },
                    { 6, null, null, null, false, "Harry@gmail.com", null, "Harry", "Harry", "123456", null, "637843188933582087init.jpg", 1, "系統管理員" },
                    { 7, null, null, null, false, "GGAA@gmail.com", null, "GGAA", "GGAA", "123456", null, "637843188933582087init.jpg", 1, "系統管理員" }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "Id", "Address", "CreateDate", "Description", "Disable", "PaymentType", "Phone", "StateId", "UserId" },
                values: new object[,]
                {
                    { "1", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 814, DateTimeKind.Local).AddTicks(1442), "訂單描述", false, 1, "091234567", 1, 1 },
                    { "8", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 815, DateTimeKind.Local).AddTicks(9974), "小王跟Lynn購買產品", false, 1, "0987654", 2, 1 },
                    { "5", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 815, DateTimeKind.Local).AddTicks(9808), "Benny跟Harry購買產品", false, 1, "0987654", 2, 5 },
                    { "2", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 815, DateTimeKind.Local).AddTicks(9597), "訂單描述", false, 1, "091234567", 2, 2 },
                    { "9", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 816, DateTimeKind.Local).AddTicks(22), "小明跟Lynn購買產品", false, 1, "0987654", 2, 2 },
                    { "4", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 815, DateTimeKind.Local).AddTicks(9759), "Benny跟Lynn購買產品", false, 1, "091234567", 2, 5 },
                    { "3", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 815, DateTimeKind.Local).AddTicks(9704), "訂單描述", false, 1, "091234567", 3, 3 },
                    { "7", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 815, DateTimeKind.Local).AddTicks(9924), "Harry跟Lynn購買產品", false, 1, "0987654", 2, 6 },
                    { "6", "送貨地址", new DateTime(2022, 4, 20, 15, 21, 11, 815, DateTimeKind.Local).AddTicks(9862), "Lynn跟Harry購買產品", false, 1, "0987654", 2, 4 }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "Category", "Content", "CreateTime", "Description", "Disable", "EndTime", "Name", "Price", "Total", "UserId" },
                values: new object[,]
                {
                    { 7, "美食", "堅持手工製作，外酥內Q的迷人口感，多種披薩口味任選，簡單加熱就能享用，香氣濃郁成份單純，點心宵夜絕對便利的美味～", new DateTime(2022, 4, 20, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6750), "堅持手工製作，外酥內Q的迷人口感，多種披薩口味任選，簡單加熱就能享用，香氣濃郁成份單純，點心宵夜絕對便利的美味～", false, new DateTime(2022, 4, 30, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6751), "手工製作披薩", 300m, 10000m, 4 },
                    { 6, "美食", "這款雪糕你吃過沒？格子脆皮餅乾裡面有香甜綿密的雪糕，百吃不厭的香草口味，配上酥脆餅皮口感，絕對大滿足～還有多種口味任選", new DateTime(2022, 4, 20, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6702), "這款雪糕你吃過沒？格子脆皮餅乾裡面有香甜綿密的雪糕，百吃不厭的香草口味，配上酥脆餅皮口感，絕對大滿足～還有多種口味任選", false, new DateTime(2022, 4, 30, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6703), "脆餅雪糕", 50m, 10000m, 4 },
                    { 4, "美食", "吃的到蝦仁的月亮蝦餅", new DateTime(2022, 4, 20, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6593), "吃的到蝦仁的月亮蝦餅                                                                                                  ", false, new DateTime(2022, 4, 30, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6594), "月亮蝦餅", 100m, 10000m, 3 },
                    { 3, "3C", "便宜好用ㄉ記憶體", new DateTime(2022, 4, 20, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6522), "記憶體是要描述什麼                                                                                                      ", false, new DateTime(2022, 4, 30, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6523), "記憶體", 300m, 10000m, 3 },
                    { 2, "美食", "擁有水中珍品美譽的智力鮭魚，富含對人體有益的魚油，產地捕撈後隨即低溫急速冷凍鎖住新鮮與營養，美味直送到家！", new DateTime(2022, 4, 20, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6315), "擁有水中珍品美譽的智力鮭魚，富含對人體有益的魚油，產地捕撈後隨即低溫急速冷凍鎖住新鮮與營養，美味直送到家！              ", false, new DateTime(2022, 4, 30, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6335), "鮭魚", 50m, 1000m, 2 },
                    { 1, "美食", "不知道可不可以吃的貓咪", new DateTime(2022, 4, 20, 15, 21, 11, 802, DateTimeKind.Local).AddTicks(971), "不知道可不可以吃                                                                                                        ", false, new DateTime(2022, 4, 30, 15, 21, 11, 802, DateTimeKind.Local).AddTicks(7818), "貓貓", 50m, 1000m, 1 },
                    { 5, "美食", "厚切達3公分！精選Prime極佳級，原塊現切牛肉，大理石紋路般的油花分布，讓人為之瘋狂～口感柔嫩多汁，絕對滿足想大口吃肉的你", new DateTime(2022, 4, 20, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6651), "厚切達3公分！精選Prime極佳級，原塊現切牛肉，大理石紋路般的油花分布，讓人為之瘋狂～口感柔嫩多汁，絕對滿足想大口吃肉的你", false, new DateTime(2022, 4, 30, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6652), "Prime-原塊現切牛肉", 200m, 10000m, 4 },
                    { 8, "乾糧", "堅持手工製作，外酥內Q的迷人口感，多種披薩口味任選，簡單加熱就能享用，香氣濃郁成份單純，點心宵夜絕對便利的美味～", new DateTime(2022, 4, 20, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6799), "堅持手工製作，外酥內Q的迷人口感，多種披薩口味任選，簡單加熱就能享用，香氣濃郁成份單純，點心宵夜絕對便利的美味～", false, new DateTime(2022, 4, 30, 15, 21, 11, 810, DateTimeKind.Local).AddTicks(6800), "魔法仗", 300m, 20000m, 6 }
                });

            migrationBuilder.InsertData(
                table: "OrderDetail",
                columns: new[] { "OrderId", "Count", "Disable", "Price", "ProductId" },
                values: new object[,]
                {
                    { "1", 18, false, 500m, 1 },
                    { "7", 20, false, 1000m, 5 },
                    { "2", 10, false, 1000m, 2 },
                    { "5", 10, false, 1000m, 6 },
                    { "3", 10, false, 500m, 3 },
                    { "4", 20, false, 8000m, 5 },
                    { "8", 20, false, 1000m, 5 },
                    { "6", 5, false, 1000m, 8 },
                    { "9", 20, false, 1000m, 6 }
                });

            migrationBuilder.InsertData(
                table: "ProductPics",
                columns: new[] { "Id", "PicPath", "ProductId" },
                values: new object[,]
                {
                    { 7, "DEMO披薩.jpg", 7 },
                    { 6, "DEMO雪糕.jpg", 6 },
                    { 5, "DEMO牛排.jpg", 5 },
                    { 4, "DEMO月亮蝦餅.jpg", 4 },
                    { 3, "DEMO記憶體.jpg", 3 },
                    { 2, "DEMO鮭魚.jpg", 2 },
                    { 1, "DEMO喵喵.jpg", 1 },
                    { 9, "DEMO牛排1.jpg", 5 },
                    { 8, "Demo二哈.jpg", 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatRoomId",
                table: "ChatMessages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Chats_ChatRoomId",
                table: "Member_Chats",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_StateId",
                table: "Order",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_UserId",
                table: "Product",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMessages_ProductId",
                table: "ProductMessages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPics_ProductId",
                table: "ProductPics",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSellerReplies_ProductMessageId",
                table: "ProductSellerReplies",
                column: "ProductMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotify_Category",
                table: "UserNotify",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotify_UserId",
                table: "UserNotify",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVouchers_VoucherId",
                table: "UserVouchers",
                column: "VoucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "LineMember");

            migrationBuilder.DropTable(
                name: "Member_Chats");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ProductPics");

            migrationBuilder.DropTable(
                name: "ProductSellerReplies");

            migrationBuilder.DropTable(
                name: "UserNotify");

            migrationBuilder.DropTable(
                name: "UserVouchers");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "ProductMessages");

            migrationBuilder.DropTable(
                name: "NotifyCategory");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "OrderState");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
