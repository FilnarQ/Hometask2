using FluentValidation;

namespace Hometask2.Models
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Rating { get; set; }
        public int ReviewsNumber { get; set; }
    }
    public class BookDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Cover { get; set; }
        public string Content { get; set; }
        public decimal Rating { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
    public class OrderValidator : AbstractValidator<string>
    {
        public OrderValidator()
        {
            RuleFor(x => x).Must(x => x == "author" || x == "title" || x == "").WithMessage("Can be ordered by author or title only");
        }
    }
}
