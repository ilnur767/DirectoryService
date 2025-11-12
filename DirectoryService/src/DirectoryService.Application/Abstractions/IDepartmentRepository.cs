using CSharpFunctionalExtensions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Abstractions;

public interface IDepartmentRepository
{
    Task<Result<Guid, Error>> CreateDepartment(Department department, CancellationToken cancellationToken);
    Task<Result<Department, Error>> GetDepartmentById(Guid id, CancellationToken cancellationToken);
    Task<Result<bool, Error>> CheckIfAllDepartmentsExist(Guid[] ids, CancellationToken cancellationToken);
    Task<UnitResult<Error>> Save(CancellationToken cancellationToken);
}