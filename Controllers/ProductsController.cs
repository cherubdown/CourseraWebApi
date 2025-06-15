using Microsoft.AspNetCore.Mvc;

namespace CourseraWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        public List<Product> Products { get; set; } = new List<Product>()
        {
            new Product(1, "Laptop", "High performance laptop", 1200.00m),
            new Product(2, "Smartphone", "Latest model smartphone", 800.00m),
            new Product(3, "Tablet", "Portable tablet with stylus", 600.00m)
        };

        [HttpGet]
        [Route("api/products")]
        public ActionResult<List<Product>> Get()
        {
            return Ok(Products);
        }

        [HttpGet]
        [Route("api/products/{id:int}")]
        public ActionResult<Product> GetById(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [Route("api/products")]
        public ActionResult<string> Create([FromBody] Product product)
        {
            product.Id = Products.Count + 1; // Assign a new ID
            Products.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut]
        [Route("api/products/{id:int}")]
        public ActionResult<string> Update(int id, [FromBody] Product product)
        {
            var existingProduct = Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Update the existing product
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;

            return Ok($"Updated product {id} to: {existingProduct}");
        }

        [HttpDelete]
        [Route("api/products/{id:int}")]
        public ActionResult<string> Delete(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            Products.Remove(product);
            return NoContent();
        }
    }
}