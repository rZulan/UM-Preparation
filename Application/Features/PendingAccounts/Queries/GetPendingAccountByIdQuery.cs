using System.Net;
using Application.DTO.PendingAccount;
using Application.Interfaces;
using Application.Results;
using MediatR;

namespace Application.Features.PendingAccounts.Queries;

/// <summary>Query to retrieve a single pending account by its ID.</summary>
/// <param name="Id">The unique identifier of the pending account to retrieve.</param>
public record GetPendingAccountByIdQuery(int Id) : IRequest<Result<GetPendingAccountDto>>;

public class GetPendingAccountByIdQueryHandler(IPendingAccountRepository pendingAccountRepository)
    : IRequestHandler<GetPendingAccountByIdQuery, Result<GetPendingAccountDto>>
{
    private readonly IPendingAccountRepository _pendingAccountRepository = pendingAccountRepository;

    public async Task<Result<GetPendingAccountDto>> Handle(GetPendingAccountByIdQuery request,
        CancellationToken cancellationToken)
    {
        var pendingAccount = await _pendingAccountRepository.GetByIdAsync(request.Id, cancellationToken);

        if (pendingAccount == null)
            return Result<GetPendingAccountDto>.Failure("Pending Account not found", HttpStatusCode.NotFound);

        var result = new GetPendingAccountDto
        {
            Id = pendingAccount.Id,
            IsActive = pendingAccount.IsActive,
            EmployeePrefix = pendingAccount.EmployeePrefix,
            EmployeeId = pendingAccount.EmployeeId,
            Username = pendingAccount.Username,
            FirstName = pendingAccount.FirstName,
            MiddleName = pendingAccount.MiddleName ?? "N/A",
            LastName = pendingAccount.LastName,
            Suffix = pendingAccount.Suffix ?? "N/A"
        };

        return Result<GetPendingAccountDto>.Success(result);
    }
}