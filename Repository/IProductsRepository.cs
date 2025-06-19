
public interface IProductsRepository
{
    List<Product> GetAll();
    Product GetById(int id);
    void Post(Product product);
    void Put(int id, Product product);
    void Delete(int id);
}