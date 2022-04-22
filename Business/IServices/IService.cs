using System;

namespace Business.IServices
{
    public interface IDataService<in T> where T : class
    {
        void Add(T instance);
        void Update(T instance);
        void Delete(T instance);
        void SaveChanges();

    }
}