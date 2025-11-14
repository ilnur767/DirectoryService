using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Commands.Departments.MoveDepartment;

public sealed class MoveDepartmentHandler : ICommandHandler<MoveDepartmentCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<MoveDepartmentHandler> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly ITransactionManager _transactionManager;

    public MoveDepartmentHandler(
        ITransactionManager transactionManager,
        IDepartmentRepository departmentRepository,
        ILogger<MoveDepartmentHandler> logger, TimeProvider timeProvider)
    {
        _transactionManager = transactionManager;
        _departmentRepository = departmentRepository;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task<UnitResult<ErrorList>> Handle(MoveDepartmentCommand command, CancellationToken cancellationToken)
    {
        var transaction = await _transactionManager.BeginTransaction(cancellationToken);
        if (transaction.IsFailure)
        {
            return transaction.Error.ToErrorList();
        }

        using var transactionScope = transaction.Value;
        try
        {
            var departmentResult =
                await _departmentRepository.GetDepartmentById(command.DepartmentId, cancellationToken);
            if (departmentResult.IsFailure)
            {
                transactionScope.Rollback();

                return departmentResult.Error.ToErrorList();
            }

            var lockDescendantsResult =
                //await _departmentRepository.LockDescendantsRecursive(command.DepartmentId, cancellationToken);
                await _departmentRepository.LockDescendantsByLtree(command.DepartmentId, cancellationToken);
            if (lockDescendantsResult.IsFailure)
            {
                transactionScope.Rollback();

                return lockDescendantsResult.Error.ToErrorList();
            }

            Department? parentDepartment = null;
            if (command.ParentDepartmentId != null)
            {
                var parentDepartmentResult =
                    await _departmentRepository.GetDepartmentById(command.ParentDepartmentId.Value, cancellationToken);
                if (parentDepartmentResult.IsFailure)
                {
                    transactionScope.Rollback();

                    return parentDepartmentResult.Error.ToErrorList();
                }

                var isDescendantResult = await _departmentRepository.IsDescendant(command.DepartmentId,
                    command.ParentDepartmentId.Value, cancellationToken);
                if (isDescendantResult.IsFailure || isDescendantResult.Value)
                {
                    transactionScope.Rollback();

                    return Errors.Department.CannotAssignAsAParentOrItsDescendants().ToErrorList();
                }

                parentDepartment = parentDepartmentResult.Value;
            }

            var oldPath = departmentResult.Value.Path;
            var updateAt = _timeProvider.GetUtcNow().UtcDateTime;
            var changeParentResult = departmentResult.Value.ChangeParent(parentDepartment, updateAt);
            if (changeParentResult.IsFailure)
            {
                transactionScope.Rollback();

                return changeParentResult.Error.ToErrorList();
            }

            var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveResult.IsFailure)
            {
                transactionScope.Rollback();

                return saveResult.Error.ToErrorList();
            }

            var updateChildrenResult =
                await _departmentRepository.UpdateChildrenPathAndDepth(
                    departmentResult.Value,
                    oldPath,
                    updateAt,
                    cancellationToken);
            if (updateChildrenResult.IsFailure)
            {
                return updateChildrenResult.Error.ToErrorList();
            }

            var commitResult = transactionScope.Commit();
            if (commitResult.IsFailure)
            {
                transactionScope.Rollback();

                return commitResult.Error.ToErrorList();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not move department '{id}'", command.DepartmentId);

            transactionScope.Rollback();
        }

        return new UnitResult<ErrorList>();
    }
}

public record MoveDepartmentCommand(Guid DepartmentId, Guid? ParentDepartmentId) : ICommand;