using StackExchange.Redis;

namespace TuanBuy.Models.Extension
{
    public static class RedisExtension
    {
        public static bool SaveMessage(this IDatabase db, string key, string value)
        {
            db.ListLeftPush(key, value);
            return true;
        }
    }
}