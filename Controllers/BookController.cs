using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        
        public BookController(Database db)
        {
            Db = db;
            
        }

        // GET api/Book
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        }

        // GET api/Book/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        // POST api/Book
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]Book body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            int result=await body.InsertAsync();
            Console.WriteLine("inserted id="+result);
            return new OkObjectResult(result);
        }

        // PUT api/Book/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutOne(int id, [FromBody]Book body)
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.name = body.name;
            result.author = body.author;
            result.isbn = body.isbn;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE api/Book/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Book(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkObjectResult(result);
        }

        public Database Db { get; }
    }
}