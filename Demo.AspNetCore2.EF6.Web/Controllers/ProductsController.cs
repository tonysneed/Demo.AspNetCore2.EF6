using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Demo.AspNetCore2.EF6.Data.Contexts;
using Demo.AspNetCore2.EF6.Data.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Demo.AspNetCore2.EF6.Web.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly SampleDbContext _dbContext;

        public ProductsController(SampleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var products = await _dbContext.Products
                .OrderBy(e => e.ProductName)
                .ToListAsync();
            return Ok(products);
        }

        // GET api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var product = await _dbContext.Products
                .SingleOrDefaultAsync(e => e.Id == id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // POST api/products
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Product product)
        {
            _dbContext.Entry(product).State = EntityState.Added;

            await _dbContext.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = product.Id }, product);
        }

        // PUT api/products
        [HttpPut]
        public async Task<ActionResult> Put([FromBody]Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        // DELETE api/products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _dbContext.Products
                .SingleOrDefaultAsync(e => e.Id == id);
            if (product == null) return Ok();

            _dbContext.Entry(product).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
