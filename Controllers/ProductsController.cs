using Microsoft.AspNetCore.Mvc;

namespace CourseraWebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ProductsController(IProductsRepository productsRepo) : ControllerBase
    {

        [HttpGet]
        public ActionResult<List<Product>> Get()
        {
            return Ok(productsRepo.GetAll());
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult<Product> GetById(int id)
        {
            var product = productsRepo.GetById(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<string> Create([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null.");
            }
            productsRepo.Post(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut]
        public ActionResult<string> Update(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null.");
            }
            productsRepo.Put(id, product);
            var existingProduct = productsRepo.GetById(id);
            return Ok($"Updated product {id} to: {existingProduct}");
        }

        [HttpDelete]
        public ActionResult<string> Delete(int id)
        {
            var existingProduct = productsRepo.GetById(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            productsRepo.Delete(id);
            return NoContent();
        }
    }
}