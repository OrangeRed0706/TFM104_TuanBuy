using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace TuanBuy.Models.AppUtlity
{
    public class RedisProvider
    {
        private IDatabase Db { get; set; }
        private ConnectionMultiplexer Redis { get; set; }
        public RedisProvider()
        {
            //   Redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
               Redis = ConnectionMultiplexer.Connect("tuanbuyredis.redis.cache.windows.net:6380,password=BFvMijldKRhmI0C1dcrCltsS1BYLwNLi3AzCaLDKszg=,ssl=True,abortConnect=False");
        }

        public IDatabase GetRedisDb(int dbNumber)
        {
            if (!(dbNumber >= 0 & dbNumber < 15)) return null;
            Db = Redis.GetDatabase(dbNumber);
            return Db;

        }
        /// <summary>
        /// 存取通知
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>



        #region 將物件轉換成HashEntry[]

        public static HashEntry[] ToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select
                (
                    property =>
                    {
                        object propertyValue = property.GetValue(obj);
                        string hashValue;

                        // This will detect if given property value is 
                        // enumerable, which is a good reason to serialize it
                        // as JSON!
                        if (propertyValue is IEnumerable<object>)
                        {
                            // So you use JSON.NET to serialize the property
                            // value as JSON
                            hashValue = JsonConvert.SerializeObject(propertyValue);
                        }
                        else
                        {
                            hashValue = propertyValue.ToString();
                        }

                        return new HashEntry(property.Name, hashValue);
                    }
                )
                .ToArray();
        }


        #endregion

        #region 將HashEntry[]轉換成物件
        public static T ConvertFromRedis<T>(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }



        #endregion

        #region 將Dictionary轉HashEntry[]
        public static HashEntry[] ToHashEntryArray(IDictionary<string, string> items)
        {
            var entries = new HashEntry[items.Count];
            int i = 0;
            foreach (var item in items)
            {
                entries[i++] = new HashEntry(item.Key, item.Value);
            }
            return entries;
        }
        public static HashEntry[] ToHashEntryArray(IDictionary<int, int> items)
        {
            var entries = new HashEntry[items.Count];
            int i = 0;
            foreach (var item in items)
            {
                entries[i++] = new HashEntry(item.Key, item.Value);
            }
            return entries;
        }
        #endregion

        #region 將HashEntry[] 轉Dictionary

        public static IDictionary<string, string> ConvertToDictionaryString(HashEntry[] entries)
        {
            var stringMap = new Dictionary<string, string>();
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    stringMap.Add(entry.Name, entry.Value.ToString());
                }
            }
            return stringMap;
        }
        public static IDictionary<int, int> ConvertToDictionaryInt(HashEntry[] entries)
        {
            var IntMap = new Dictionary<int, int>();
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    IntMap.Add((int)entry.Name, (int)entry.Value);
                }
            }
            return IntMap;
        }
        #endregion

    }
}