using CSharpFunctionalExtensions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Application.Abstractions;

public interface IDepartmentRepository
{
    Task<Result<Guid, Error>> CreateDepartment(Department department, CancellationToken cancellationToken);
    Task<Result<Department, Error>> GetDepartmentById(Guid id, CancellationToken cancellationToken);
    Task<UnitResult<Error>> LockDescendantsRecursive(Guid departmentId, CancellationToken cancellationToken);
    Task<UnitResult<Error>> LockDescendantsByLtree(Guid departmentId, CancellationToken cancellationToken);
    Task<Result<bool, Error>> CheckIfAllDepartmentsExist(Guid[] ids, CancellationToken cancellationToken);
    Task<UnitResult<Error>> Save(CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateChildrenPathAndDepth(Department department, DepartmentPath oldPath, DateTime updateAt,
        CancellationToken cancellationToken);

    Task<Result<bool, Error>> IsDescendant(Guid departmentId, Guid departmentIdForCheck,
        CancellationToken cancellationToken);
}