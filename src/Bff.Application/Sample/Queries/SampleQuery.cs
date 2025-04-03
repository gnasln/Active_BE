using Bff.Application.Common.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bff.Application.Sample.Queries;

public record SampleQuery : IRequest<string>
{
    public required string YourName { get; set; }
};

public class SampleQueryHandler(IUser user) : IRequestHandler<SampleQuery, string>
{
    private readonly IUser _user = user;
    public async Task<string> Handle(SampleQuery request, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return $"This is sample Api for id: {_user.Id}, un: {_user.UserName}";
    }
}
