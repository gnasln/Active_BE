using Bff.Application.Common.Interfaces;
using MediatR;

namespace Bff.Application.Tenants.Common;

public record CheckOwner : IRequest<bool>
{
    public Guid Id { set; get; }
}

public class CheckOwnerHandle(IUser user) : IRequestHandler<CheckOwner, bool>
{
    private readonly IUser _user = user;

    public async Task<bool> Handle(CheckOwner check, CancellationToken cancellationToken)
    {
        await Task.Delay(0);
        if (!string.IsNullOrEmpty(_user.Id))
        {
            if (Guid.Parse(_user.Id) == check.Id)
            {
                return true;
            }
        }

        return false;
    }
}