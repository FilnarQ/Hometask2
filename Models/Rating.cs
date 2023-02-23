using FluentValidation;

namespace Hometask2.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Score { get; set; }
    }

    public class RatingValidator : AbstractValidator<Rating>
    {
        public RatingValidator()
        {
            RuleFor(x => x.Score).InclusiveBetween(1, 5).WithMessage("Score can be from 1 to 5");
        }
    }
}
