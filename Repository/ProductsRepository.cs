
public class ProductsRepository : IProductsRepository
{
    private List<Product> _products { get; set; } = new List<Product>()
    {
        new Product(1, "Laptop", "High performance laptop", 1200.00m),
        new Product(2, "Smartphone", "Latest model smartphone", 800.00m),
        new Product(3, "Tablet", "Portable tablet with stylus", 600.00m)
    };

    public ProductsRepository()
    {
        // Initialize _latestId based on the highest existing product ID
        int highestInt = 1;
        foreach(var product in _products)
        {
            if (product.Id > highestInt)
            {
                highestInt = product.Id;
            }
        }
        _latestId = highestInt;
    }

    private int _latestId;

    public List<Product> GetAll()
    {
        return _products;
    }

    public Product GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public void Post(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }
        product.Id = _latestId + 1;
        _latestId++;
        _products.Add(product);
    }

    public void Put(int id, Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }
        var existingProduct = GetById(id);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
    }
    public void Delete(int id)
    {
        var product = GetById(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }
        _products.Remove(product);
    }
}