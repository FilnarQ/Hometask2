using Hometask2.Contexts;
using Hometask2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Query.Internal;

namespace Hometask2.Services
{
    public class BooksService
    {
        private BookContext _bookContext;
        public BooksService(BookContext bookContext)
        {
            _bookContext = bookContext;
        }

        public async Task Fill(IEnumerable<Book> books)
        {
            foreach (var book in books)
            {
                _bookContext.Books.Add(book);
            }
            await _bookContext.SaveChangesAsync();
            return;
        }

        public async Task<IQueryable<BookDTO>> GetBooks(string? order)
        {
            var query = await Task.Run(() =>
            {
                var query = from res in _bookContext.Books select new BookDTO(){ Id = res.Id, Title = res.Title, Author = res.Author, Rating = AverageRating(res.Ratings), ReviewsNumber = res.Reviews.Count };
                switch (order)
                {
                    case "author": { query = query.OrderBy(el => el.Author); break; }
                    case "title": { query = query.OrderBy(el => el.Title); break; }
                    default: { query = query.OrderBy(el => el.Id); break; }
                }
                return query;
            });
            return query;
        }

        public async Task<IEnumerable<BookDTO>> GetRecommended(string? genre)
        {
            var query = await Task.Run(() => {
                var byReviews = from res in _bookContext.Books.Include(x => x.Reviews).Include(x => x.Ratings) where res.Reviews.Count > 10 select res;
                if (genre != null) { byReviews = byReviews.Where(el => el.Genre.ToLower().Contains(genre.ToLower())); }
                var query = (from res in byReviews.AsEnumerable() orderby AverageRating(res.Ratings) descending select new BookDTO() { Id = res.Id,Title = res.Title, Author = res.Author, Rating = AverageRating(res.Ratings), ReviewsNumber = res.Reviews.Count }).Take(10);
                return query;
            });
            return query;
        }

        public async Task<BookDetailsDTO> GetBook(int id)
        {
            var book = await Task.Run(() =>
            {
                try
                {
                    var book = _bookContext.Books.Include(x => x.Reviews).Include(x => x.Ratings).Where(x => x.Id == id).First();
                    return new BookDetailsDTO(){ Id = book.Id, Title = book.Title, Author = book.Author, Cover = book.Cover, Content = book.Content, Rating = AverageRating(book.Ratings), Reviews = book.Reviews };
                }
                catch(Exception e)
                {
                    return null;
                }                
            });
            return book;
        }

        public async Task<bool> DeleteBook(int id)
        {
            try
            {
                _bookContext.Books.Remove(_bookContext.Books.Find(id));
                await _bookContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<int> CreateBook(Book book)
        {
            await _bookContext.Books.AddAsync(book);
            await _bookContext.SaveChangesAsync();
            return book.Id;
        }

        public async Task<int> UpdateBook(Book book)
        {
            if (_bookContext.Books.Find(book.Id) == null) return 0;
            var record = _bookContext.Books.Find(book.Id);
            if (record == null) return 0;

            record.Title = book.Title;
            record.Cover = book.Cover;
            record.Content = book.Content;
            record.Genre = book.Genre;
            record.Author = book.Author;
            await _bookContext.SaveChangesAsync();
            return record.Id;
        }

        public async Task<int> PutReview(int id, Review review)
        {
            try
            {
                var book = _bookContext.Books.Include(x => x.Reviews).Where(x => x.Id == id).Select(x => x).First();
                book.Reviews.Add(review);
                await _bookContext.SaveChangesAsync();
                return review.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> PutRate(int id, Rating rating)
        {
            try
            {
                var book = _bookContext.Books.Include(x => x.Ratings).Where(x => x.Id == id).Select(x => x).First();
                book.Ratings.Add(rating);
                await _bookContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public decimal AverageRating(IEnumerable<Rating>? ratings)
        {
            return (decimal)(ratings == null ? 0 : (ratings.Count() == 0 ? 0 : ratings.Average(el => el.Score)));
        }
    }
}
