using System.Collections.Generic;
using System.Linq;
using TuanBuy.Models.Entities;
using TuanBuy.ViewModel;

namespace TuanBuy.Models.AppUtlity
{
    public class UserMange
    {
        private TuanBuyContext _dbContext;
        public UserMange(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }


        public bool CreateOAuthUser(User user)
        {
            user.State = UserState.普通會員.ToString();
            _dbContext.User.Add(user);
            _dbContext.SaveChanges();
            return true;
        }

        #region 傳入使用者ID加入團Buy廣場
        public void CreateTuanButChat(int MemberId)
        {
            using(_dbContext)
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
        }
        #endregion
    }
}