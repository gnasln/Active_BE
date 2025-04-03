using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.KeyResultMember.Commands;

public class DeleteMemberFromKeyResultCommand : IRequest<ResultCustom<string>>
{
    public required Guid Id { get; set; }
}
public class DeleteMemberFromKeyResultCommandHandler : IRequestHandler<DeleteMemberFromKeyResultCommand, ResultCustom<string>>
{
    private readonly IKeyResultMemberRepository _keyResultMemberRepository;

    public DeleteMemberFromKeyResultCommandHandler(IKeyResultMemberRepository keyResultMemberRepository)
    {
        _keyResultMemberRepository = keyResultMemberRepository;
        
    }

    public async Task<ResultCustom<string>> Handle(DeleteMemberFromKeyResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _keyResultMemberRepository.DeleteKeyResultMember(request.Id, cancellationToken);
            return new ResultCustom<string>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Delete member from key result successfully" }
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<string>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}