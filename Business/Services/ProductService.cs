using Business.IServices;
using Data;
using Data.Entities;

namespace Business.Services
{
    public class ProductService :IProductService
    {
        private TuanBuyContext _dbContext;
        public ProductService(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product)
        {
            _dbContext.Product.Add(product);
        }

        public void Add(ProductPic instance)
        {
            throw new System.NotImplementedException();
        }

        public void Update(ProductPic instance)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(ProductPic instance)
        {
            throw new System.NotImplementedException();
        }

        public void Add(Product instance)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Product instance)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(Product instance)
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}