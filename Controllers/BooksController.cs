using Hometask2.Models;
using Hometask2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hometask2.Controllers
{
    [Route("api")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private BooksService _booksService;
        private IOptions<Config> _options;
        public BooksController(BooksService booksService, IOptions<Config> options)
        {
            _booksService = booksService;
            _options = options;
        }

        [HttpPost]
        [Route("books/fill")]
        public async Task<ActionResult> Fill(IEnumerable<Book> books)
        {
            await _booksService.Fill(books);
            return CreatedAtRoute(null, null);
        }

        [HttpGet]
        [Route("books")]
        public async Task<ActionResult> GetBooks([FromQuery]string? order)
        {
            IQueryable<BookDTO> query = await _booksService.GetBooks(order);
            return Ok(query);
        }

        [HttpGet]
        [Route("recommended")]
        public async Task<ActionResult> GetRecommended([FromQuery]string? genre)
        {
            IEnumerable<BookDTO> query = await _booksService.GetRecommended(genre);
            return Ok(query);
        }

        [HttpGet]
        [Route("books/{id:int}")]
        public async Task<ActionResult> GetBook(int id)
        {
            BookDetailsDTO book = await _booksService.GetBook(id);
            if (book == null)
            {
                return BadRequest("Book with such id does not exist");
            }
            return Ok(book);
        }

        [HttpDelete]
        [Route("books/{id:int}")]
        public async Task<ActionResult> DeleteBook([FromQuery]string? secret, int id)
        {
            if (secret != _options.Value.SecretKey) return BadRequest("Wrong secret key");
            if (await _booksService.DeleteBook(id)) return Ok();
            return BadRequest("Book with such id does not exist");
        }

        [HttpPost]
        [Route("books/save")]
        public async Task<ActionResult> PostBook(Book book)
        {
            if (book.Id == 0)
            {
                await _booksService.CreateBook(book);
            }
            else
            {
                if (await _booksService.UpdateBook(book) == 0) return BadRequest("Book with such id does not exist");
            }
            return CreatedAtRoute(null, book.Id);
        }

        [HttpPut]
        [Route("books/{id:int}/review")]
        public async Task<ActionResult> PutReview(int id, Review review)
        {
            if((await _booksService.PutReview(id, review)) == 0) return BadRequest("Book with such id does not exist");
            return CreatedAtRoute(null, new { review.Id });
        }

        [HttpPut]
        [Route("books/{id:int}/rate")]
        public async Task<ActionResult> PutRate(int id, Rating rating)
        {
            if (rating.Score > 5 || rating.Score < 1) return BadRequest("Score can be from 1 to 5");
            if (!await _booksService.PutRate(id, rating)) return BadRequest("Book with such id does not exist");
            return CreatedAtRoute(null, null);
        }
    }
}
