using FluentValidation;

namespace DAM.Core.Validations
{
    public static class ValidatorsExtensions
    {

        public static IRuleBuilderOptions<T, ICollection<TElement>> ListCountMustBeInRange<T, TElement>(this IRuleBuilder<T, ICollection<TElement>> ruleBuilder, int min, int max)
        {
            return ruleBuilder.Must(list => list.Count >= min && list.Count <= max).WithMessage($"The collection count should be between {min} and {max}");
        }
    }
}
