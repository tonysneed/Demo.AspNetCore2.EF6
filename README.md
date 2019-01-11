# Using Entity Framework 6 with ASP.NET Core 2.0

1. Create a solution with Visual Studio 2017 and add a new .NET Class Library project.
    - Select Windows Classic Desktop, Class Library

    ![add-class-lib](https://user-images.githubusercontent.com/2836367/29745523-91a86380-8a82-11e7-954a-ef85ada764dc.png)

    - Specify a .NET Framework version (for ex, 4.7)    
    - Add the EF6 NuGet package.

    ![add-ef6-package](https://user-images.githubusercontent.com/2836367/29745543-5d08ff80-8a83-11e7-8740-358f4c341388.png)

2. Add a `Product` entity class to a Models folder.

    ```csharp
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
    }
    ```

3. Add a `SampleDbContext` to a Contexts folder and inherit from `DbContext`.
    - Add a `Products` property of type `DbSet<Product>`

        ```csharp
        public class SampleDbContext : DbContext
        {
            public SampleDbContext(string connectionName) :
                base(connectionName) { }

            public DbSet<Product> Products { get; set; }
        }
        ```

4. Add a SampleDbContextFactory class to the Contexts folder.
    - Implement IDbContextFactory<SampleDbContext>
    - Return a new SampleDbContext in the Create method
    - Specify a connection string for the SampleDb database

        ```csharp
        class SampleDbContextFactory : IDbContextFactory<SampleDbContext>
        {
            public SampleDbContext Create()
            {
                return new SampleDbContext(@"Server=(localdb)\mssqllocaldb;Database=SampleDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }
        ```

5. In Package Manager Console select the class library project
    - Run the following command: `Enable-Migrations`
    - Add code to the `Seed` method in the `Configuration` class in the Migrations folder

        ```csharp
        context.Products.AddOrUpdate(
            new Product { Id = 1, ProductName = "Chai", UnitPrice = 10 },
            new Product { Id = 2, ProductName = "Chang", UnitPrice = 11 },
            new Product { Id = 3, ProductName = "Aniseed Syrup", UnitPrice = 12 }
        );
        ```
    
    - Run the `Add-Migration Initial` command
    - Run the `Update-Database` command
    - Add a data connection to the Server Explorer
    - Verify that the SampleDb database exists and that the Products table contains data

6. Add a new Web project to the solution.
    - Select .NET Core, ASP.NET Core Web Application    

    ![add-new-web-project](https://user-images.githubusercontent.com/2836367/29745538-1431a6d6-8a83-11e7-8d3a-0c46b1ce2d90.png)
    
    - Select Web API targeting the full .NET Framework
    
    ![new-aspnet-core](https://user-images.githubusercontent.com/2836367/29745542-42b05908-8a83-11e7-9cde-8a97b91cbc58.png)
    
    - Set the web project as the startup project for the solution
    - Add the EF6 Nuget package to the web project
    - Add a project reference to the Data class library project
    
    ![add-project-ref](https://user-images.githubusercontent.com/2836367/29745555-a01d7972-8a83-11e7-8d7f-106780eff979.png)
    
    - Build the solution.

7. Open appsettings.json in the Web project and add the connection string

    ```json
    "ConnectionStrings": {
        "SampleDb": "Server=(localdb)\\mssqllocaldb;Database=SampleDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

8. In the Web project register the context for DI in Startup.cs.
    - Add following code after `services.AddMvc()` in the `ConfigureServices` method

        ```csharp
        services.AddScoped(provider =>
        {
            var connectionString = Configuration.GetConnectionString("SampleDb");
            return new SampleDbContext(connectionString);
        });
        ```

9. Add a `ProductsController` that extends `Controller`

    ![add-products-controller](https://user-images.githubusercontent.com/2836367/29745554-8eaa765e-8a83-11e7-9b10-60d3e7647edf.png)

    - Pass `SampleDbContext` to the ctor
    - Add actions for GET, POST, PUT and DELETE

        ```csharp
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
        ```

10. Test the controller by running the app and submitting some requests.
    - Use Postman or Fiddler
    - Set Content-Type header to application/json for POST and PUT.
    - The database should be created automatically

        ```
        GET: http://localhost:50781/api/products
        POST: http://localhost:50781/api/products
          - Body: {"productName":"Ikura","unitPrice":12}
        GET: http://localhost:50781/api/products/4
        PUT: http://localhost:50781/api/products/4
          - Body: {"id":4,"productName":"Ikura","unitPrice":13}
        DELETE: http://localhost:50781/api/products/4
        ```
