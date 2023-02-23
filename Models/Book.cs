using FluentValidation;

namespace Hometask2.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Rating> Ratings { get; set; } = new List<Rating> { };

        public Book(int id, string title, string cover, string content, string author, string genre)
        {
            Id = id;
            Title = title;
            Cover = cover;
            Content = content;
            Author = author;
            Genre = genre;
        }
    }

    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title can not be empty").MaximumLength(100).WithMessage("Title length must be 1-100 symbols");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content can not be empty").MaximumLength(1500).WithMessage("Content length must be <1500 symbols");
            RuleFor(x => x.Author).NotEmpty().WithMessage("Author can not be empty").MaximumLength(100).WithMessage("Author length must be 1-100 symbols");
            RuleFor(x => x.Genre).NotEmpty().WithMessage("Genre must be specified");
        }
    }
}
