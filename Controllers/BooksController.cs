using FluentValidation;
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
        private IValidator<Rating> _ratingValidator;
        private IValidator<Review> _reviewValidator;
        private IValidator<Book> _bookValidator;
        private IValidator<string> _orderValidator;
        public BooksController(BooksService booksService, IOptions<Config> options, IValidator<Rating> ratingValidator, IValidator<Review> reviewValidator, IValidator<Book> bookValidator, IValidator<string> orderValidator)
        {
            _booksService = booksService;
            _options = options;
            _ratingValidator = ratingValidator;
            _reviewValidator = reviewValidator;
            _bookValidator = bookValidator;
            _orderValidator = orderValidator;
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
            var v = _orderValidator.Validate(order??"");
            if (!v.IsValid) return BadRequest(v.ToString());
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
            var v = _bookValidator.Validate(book);
            if (!v.IsValid) return BadRequest(v.ToString());
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
            var v = _reviewValidator.Validate(review);
            if (!v.IsValid) return BadRequest(v.ToString());
            if ((await _booksService.PutReview(id, review)) == 0) return BadRequest("Book with such id does not exist");
            return CreatedAtRoute(null, new { review.Id });
        }

        [HttpPut]
        [Route("books/{id:int}/rate")]
        public async Task<ActionResult> PutRate(int id, Rating rating)
        {
            var v = _ratingValidator.Validate(rating);
            if (!v.IsValid) return BadRequest(v.ToString());
            if (!await _booksService.PutRate(id, rating)) return BadRequest("Book with such id does not exist");
            return CreatedAtRoute(null, null);
        }
    }
}
