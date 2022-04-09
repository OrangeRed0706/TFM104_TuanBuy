﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TuanBuy.Models.Entities;

namespace TuanBuy.Migrations
{
    [DbContext(typeof(TuanBuyContext))]
    [Migration("20220408141730_2080")]
    partial class _2080
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TuanBuy.Models.ChatMessages", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ChatRoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageImage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MessageId");

                    b.HasIndex("ChatRoomId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("TuanBuy.Models.ChatRoom", b =>
                {
                    b.Property<Guid>("ChatRoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChatRoomTitle")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChatRoomId");

                    b.ToTable("ChatRooms");
                });

            modelBuilder.Entity("TuanBuy.Models.ChatRoomMember", b =>
                {
                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<Guid>("ChatRoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("MemberId", "ChatRoomId");

                    b.HasIndex("ChatRoomId");

                    b.ToTable("Member_Chats");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnType("bit");

                    b.Property<int>("PaymentType")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StateId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StateId");

                    b.HasIndex("UserId");

                    b.ToTable("Order");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Address = "送貨地址",
                            CreateDate = new DateTime(2022, 4, 8, 22, 17, 30, 2, DateTimeKind.Local).AddTicks(7815),
                            Description = "訂單描述",
                            Disable = false,
                            PaymentType = 1,
                            Phone = "091234567",
                            StateId = 1,
                            UserId = 1
                        },
                        new
                        {
                            Id = 2,
                            Address = "送貨地址",
                            CreateDate = new DateTime(2022, 4, 8, 22, 17, 30, 4, DateTimeKind.Local).AddTicks(6649),
                            Description = "訂單描述",
                            Disable = false,
                            PaymentType = 1,
                            Phone = "091234567",
                            StateId = 2,
                            UserId = 2
                        },
                        new
                        {
                            Id = 3,
                            Address = "送貨地址",
                            CreateDate = new DateTime(2022, 4, 8, 22, 17, 30, 4, DateTimeKind.Local).AddTicks(6756),
                            Description = "訂單描述",
                            Disable = false,
                            PaymentType = 1,
                            Phone = "091234567",
                            StateId = 3,
                            UserId = 3
                        },
                        new
                        {
                            Id = 4,
                            Address = "送貨地址",
                            CreateDate = new DateTime(2022, 4, 8, 22, 17, 30, 4, DateTimeKind.Local).AddTicks(6801),
                            Description = "Benny跟Lynn購買產品",
                            Disable = false,
                            PaymentType = 1,
                            Phone = "091234567",
                            StateId = 1,
                            UserId = 5
                        });
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.OrderDetail", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<bool>("Disable")
                        .HasColumnType("bit");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ProductId", "OrderId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetail");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            OrderId = 1,
                            Count = 18,
                            Disable = false,
                            Price = 500m
                        },
                        new
                        {
                            ProductId = 2,
                            OrderId = 2,
                            Count = 10,
                            Disable = false,
                            Price = 1000m
                        },
                        new
                        {
                            ProductId = 3,
                            OrderId = 3,
                            Count = 10,
                            Disable = false,
                            Price = 500m
                        },
                        new
                        {
                            ProductId = 5,
                            OrderId = 4,
                            Count = 20,
                            Disable = false,
                            Price = 8000m
                        },
                        new
                        {
                            ProductId = 6,
                            OrderId = 4,
                            Count = 40,
                            Disable = false,
                            Price = 1500m
                        });
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.OrderState", b =>
                {
                    b.Property<int>("StateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StateId");

                    b.ToTable("OrderState");

                    b.HasData(
                        new
                        {
                            StateId = 1,
                            State = "購物車"
                        },
                        new
                        {
                            StateId = 2,
                            State = "未付款"
                        },
                        new
                        {
                            StateId = 3,
                            State = "已付款"
                        },
                        new
                        {
                            StateId = 4,
                            State = "完成"
                        },
                        new
                        {
                            StateId = 5,
                            State = "取消"
                        });
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disable")
                        .HasColumnType("bit");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Product");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Category = "食品",
                            Content = "不知道可不可以吃的貓咪",
                            CreateTime = new DateTime(2022, 4, 8, 22, 17, 29, 984, DateTimeKind.Local).AddTicks(7575),
                            Description = "不知道可不可以吃                                                                                                        ",
                            Disable = false,
                            EndTime = new DateTime(2022, 4, 18, 22, 17, 29, 985, DateTimeKind.Local).AddTicks(4574),
                            Name = "貓貓",
                            Price = 50m,
                            Total = 1000m,
                            UserId = 1
                        },
                        new
                        {
                            Id = 2,
                            Category = "食品",
                            Content = "擁有水中珍品美譽的智力鮭魚，富含對人體有益的魚油，產地捕撈後隨即低溫急速冷凍鎖住新鮮與營養，美味直送到家！",
                            CreateTime = new DateTime(2022, 4, 8, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(485),
                            Description = "擁有水中珍品美譽的智力鮭魚，富含對人體有益的魚油，產地捕撈後隨即低溫急速冷凍鎖住新鮮與營養，美味直送到家！              ",
                            Disable = false,
                            EndTime = new DateTime(2022, 4, 18, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(505),
                            Name = "鮭魚",
                            Price = 50m,
                            Total = 1000m,
                            UserId = 2
                        },
                        new
                        {
                            Id = 3,
                            Category = "3C",
                            Content = "便宜好用ㄉ記憶體",
                            CreateTime = new DateTime(2022, 4, 8, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(653),
                            Description = "記憶體是要描述什麼                                                                                                      ",
                            Disable = false,
                            EndTime = new DateTime(2022, 4, 18, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(654),
                            Name = "記憶體",
                            Price = 300m,
                            Total = 10000m,
                            UserId = 3
                        },
                        new
                        {
                            Id = 4,
                            Category = "食品",
                            Content = "吃的到蝦仁的月亮蝦餅",
                            CreateTime = new DateTime(2022, 4, 8, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(717),
                            Description = "吃的到蝦仁的月亮蝦餅                                                                                                  ",
                            Disable = false,
                            EndTime = new DateTime(2022, 4, 18, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(718),
                            Name = "月亮蝦餅",
                            Price = 100m,
                            Total = 10000m,
                            UserId = 3
                        },
                        new
                        {
                            Id = 5,
                            Category = "食品",
                            Content = "厚切達3公分！精選Prime極佳級，原塊現切牛肉，大理石紋路般的油花分布，讓人為之瘋狂～口感柔嫩多汁，絕對滿足想大口吃肉的你",
                            CreateTime = new DateTime(2022, 4, 8, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(774),
                            Description = "厚切達3公分！精選Prime極佳級，原塊現切牛肉，大理石紋路般的油花分布，讓人為之瘋狂～口感柔嫩多汁，絕對滿足想大口吃肉的你",
                            Disable = false,
                            EndTime = new DateTime(2022, 4, 18, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(774),
                            Name = "Prime-原塊現切牛肉",
                            Price = 200m,
                            Total = 10000m,
                            UserId = 4
                        },
                        new
                        {
                            Id = 6,
                            Category = "食品",
                            Content = "這款雪糕你吃過沒？格子脆皮餅乾裡面有香甜綿密的雪糕，百吃不厭的香草口味，配上酥脆餅皮口感，絕對大滿足～還有多種口味任選",
                            CreateTime = new DateTime(2022, 4, 8, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(859),
                            Description = "這款雪糕你吃過沒？格子脆皮餅乾裡面有香甜綿密的雪糕，百吃不厭的香草口味，配上酥脆餅皮口感，絕對大滿足～還有多種口味任選",
                            Disable = false,
                            EndTime = new DateTime(2022, 4, 18, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(860),
                            Name = "脆餅雪糕",
                            Price = 50m,
                            Total = 10000m,
                            UserId = 4
                        },
                        new
                        {
                            Id = 7,
                            Category = "食品",
                            Content = "堅持手工製作，外酥內Q的迷人口感，多種披薩口味任選，簡單加熱就能享用，香氣濃郁成份單純，點心宵夜絕對便利的美味～",
                            CreateTime = new DateTime(2022, 4, 8, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(906),
                            Description = "堅持手工製作，外酥內Q的迷人口感，多種披薩口味任選，簡單加熱就能享用，香氣濃郁成份單純，點心宵夜絕對便利的美味～",
                            Disable = false,
                            EndTime = new DateTime(2022, 4, 18, 22, 17, 29, 994, DateTimeKind.Local).AddTicks(907),
                            Name = "手工製作披薩",
                            Price = 300m,
                            Total = 10000m,
                            UserId = 4
                        });
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.ProductMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("MessageContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductMessages");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.ProductPic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PicPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductPics");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PicPath = "DEMO喵喵.jpg",
                            ProductId = 1
                        },
                        new
                        {
                            Id = 2,
                            PicPath = "DEMO鮭魚.jpg",
                            ProductId = 2
                        },
                        new
                        {
                            Id = 3,
                            PicPath = "DEMO記憶體.jpg",
                            ProductId = 3
                        },
                        new
                        {
                            Id = 4,
                            PicPath = "DEMO月亮蝦餅.jpg",
                            ProductId = 4
                        },
                        new
                        {
                            Id = 5,
                            PicPath = "DEMO牛排.jpg",
                            ProductId = 5
                        },
                        new
                        {
                            Id = 6,
                            PicPath = "DEMO雪糕.jpg",
                            ProductId = 6
                        },
                        new
                        {
                            Id = 7,
                            PicPath = "DEMO披薩.jpg",
                            ProductId = 7
                        });
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.ProductSellerReply", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("MessageContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductMessageId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductMessageId");

                    b.ToTable("ProductSellerReplies");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Birth")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Disable")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Friend")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NickName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PicPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Disable = false,
                            Email = "123@gmail.com",
                            Name = "小王",
                            NickName = "賣貓的小王",
                            Password = "123456",
                            PicPath = "637843188933582087init.jpg",
                            Sex = 1,
                            State = "正式會員"
                        },
                        new
                        {
                            Id = 2,
                            Disable = false,
                            Email = "456@gmail.com",
                            Name = "小明",
                            NickName = "賣鮭魚的小明",
                            Password = "123456",
                            PicPath = "637843188933582087init.jpg",
                            Sex = 1,
                            State = "正式會員"
                        },
                        new
                        {
                            Id = 3,
                            Disable = false,
                            Email = "789@gmail.com",
                            Name = "小張",
                            NickName = "賣記憶體的小張",
                            Password = "123456",
                            PicPath = "637843188933582087init.jpg",
                            Sex = 1,
                            State = "正式會員"
                        },
                        new
                        {
                            Id = 4,
                            Disable = false,
                            Email = "Lynn@gmail.com",
                            Name = "Lynn",
                            NickName = "Lynn",
                            Password = "123456",
                            PicPath = "637843188933582087init.jpg",
                            Sex = 1,
                            State = "正式會員"
                        },
                        new
                        {
                            Id = 5,
                            Disable = false,
                            Email = "Benny@gmail.com",
                            Name = "Benny",
                            NickName = "Benny",
                            Password = "123456",
                            PicPath = "637843188933582087init.jpg",
                            Sex = 1,
                            State = "正式會員"
                        },
                        new
                        {
                            Id = 6,
                            Disable = false,
                            Email = "Harry@gmail.com",
                            Name = "Harry",
                            NickName = "Harry",
                            Password = "123456",
                            PicPath = "637843188933582087init.jpg",
                            Sex = 1,
                            State = "正式會員"
                        },
                        new
                        {
                            Id = 7,
                            Disable = false,
                            Email = "GGAA@gmail.com",
                            Name = "GGAA",
                            NickName = "GGAA",
                            Password = "123456",
                            PicPath = "637843188933582087init.jpg",
                            Sex = 1,
                            State = "正式會員"
                        });
                });

            modelBuilder.Entity("TuanBuy.Models.ChatMessages", b =>
                {
                    b.HasOne("TuanBuy.Models.ChatRoom", "ChatRoom")
                        .WithMany()
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatRoom");
                });

            modelBuilder.Entity("TuanBuy.Models.ChatRoomMember", b =>
                {
                    b.HasOne("TuanBuy.Models.ChatRoom", "ChatRoom")
                        .WithMany("ChatRoomMembers")
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TuanBuy.Models.Entities.User", "User")
                        .WithMany("ChatRoom")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatRoom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.Order", b =>
                {
                    b.HasOne("TuanBuy.Models.Entities.OrderState", "OrderState")
                        .WithMany()
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TuanBuy.Models.Entities.User", "User")
                        .WithMany("Order")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderState");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.OrderDetail", b =>
                {
                    b.HasOne("TuanBuy.Models.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("TuanBuy.Models.Entities.Product", "Product")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.Product", b =>
                {
                    b.HasOne("TuanBuy.Models.Entities.User", "User")
                        .WithMany("Product")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.ProductMessage", b =>
                {
                    b.HasOne("TuanBuy.Models.Entities.Product", "Product")
                        .WithMany("ProductMessage")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.ProductPic", b =>
                {
                    b.HasOne("TuanBuy.Models.Entities.Product", "Product")
                        .WithMany("ProductPics")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.ProductSellerReply", b =>
                {
                    b.HasOne("TuanBuy.Models.Entities.ProductMessage", "ProductMessage")
                        .WithMany()
                        .HasForeignKey("ProductMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductMessage");
                });

            modelBuilder.Entity("TuanBuy.Models.ChatRoom", b =>
                {
                    b.Navigation("ChatRoomMembers");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.Product", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("ProductMessage");

                    b.Navigation("ProductPics");
                });

            modelBuilder.Entity("TuanBuy.Models.Entities.User", b =>
                {
                    b.Navigation("ChatRoom");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });
#pragma warning restore 612, 618
        }
    }
}