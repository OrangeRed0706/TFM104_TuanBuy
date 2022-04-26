using System.Collections.Generic;
using System.Linq;
using Business.IServices;
using Business.Services.Utility;
using Data;
using Data.Entities;
using TuanBuy.Models;

namespace Business.Services
{
    public class UsersService : IUserService
    {
        private readonly TuanBuyContext _dbContext;

        public UsersService(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(User instance)
        {
            _dbContext.User.Add(instance);
        }

        public void Update(User instance)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(User instance)
        {
            throw new System.NotImplementedException();
        }

        public User GeUser(string email)
        {
            return _dbContext.User.FirstOrDefault(x => x.Email == email);
        }

        public string ForgetPassword(string email)
        {
            var curUser = _dbContext.User.FirstOrDefault(x => x.Email == email);
            if (curUser != null)
            {
                return curUser.Password = EncrytionString.encrytion(curUser.Email);
            }
            else throw new System.NotImplementedException();

        }
        /// <summary>
        /// 新增OAUTH使用者
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CreateOAuthUser(User user)
        {
            user.State = UserState.普通會員.ToString();
            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
            return true;
        }
        /// <summary>
        /// 新增團buy聊天室
        /// </summary>
        /// <param name="MemberId"></param>
        public void CreateTuanButChat(int MemberId)
        {
            var result = _dbContext.ChatRooms.FirstOrDefault(x => x.ChatRoomTitle == "團Buy廣場");
            if (result == null)
            {
                ChatRoom chatRoom = new ChatRoom() { ChatRoomTitle = "團Buy廣場" };
                List<ChatRoomMember> chatRoomMembers = new List<ChatRoomMember>()
                    {
                        new ChatRoomMember() {MemberId=MemberId ,ChatRoomId=chatRoom.ChatRoomId},
                    };
                chatRoom.ChatRoomMembers = chatRoomMembers;
                _dbContext.ChatRooms.Add(chatRoom);
                _dbContext.SaveChanges();
            }
            else
            {
                ChatRoomMember chatRoomMembers = new ChatRoomMember()
                { MemberId = MemberId, ChatRoomId = result.ChatRoomId };
                _dbContext.Member_Chats.Add(chatRoomMembers);
                _dbContext.SaveChanges();
            }

        }


        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}