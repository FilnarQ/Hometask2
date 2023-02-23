using FluentValidation;

namespace Hometask2.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Reviewer { get; set; }
    }

    public class ReviewValidator : AbstractValidator<Review>
    {
        public ReviewValidator()
        {
            RuleFor(x => x.Message).NotEmpty().WithMessage("Message can not be empty");
            RuleFor(x => x.Reviewer).NotEmpty().WithMessage("Reviewer name can not be empty");
        }
    }
}
