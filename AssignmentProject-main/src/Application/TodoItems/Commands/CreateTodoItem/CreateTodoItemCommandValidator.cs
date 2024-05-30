using Assignment.Application.Common.Interfaces;

namespace Assignment.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    private readonly IApplicationDbContext _context;
    private record BriefTodoItem(int ListId, string? Title);

    public CreateTodoItemCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(v => new BriefTodoItem(v.ListId, v.Title))
            .MustAsync(BeUniqueTitle)
                .WithMessage("Item 'Title' in list must be unique.")
                .WithErrorCode("Unique");
    }

    private async Task<bool> BeUniqueTitle(BriefTodoItem todoItem, CancellationToken cancellationToken)
    {
        return await _context.TodoItems
            .AllAsync(l => l.Title != todoItem.Title || l.ListId != todoItem.ListId, cancellationToken);
    }
}
