using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Queries.Departments.GetDepartmentsWithTopPositions;

public sealed class
    GetDepartmentsWithTopPositionsHandler : IQueryHandler<IReadOnlyList<DepartmentByPositionDto>,
    GetDepartmentsWithTopPositionsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetDepartmentsWithTopPositionsHandler(IDbConnectionFactory connectionFactory) =>
        _connectionFactory = connectionFactory;

    public async Task<Result<IReadOnlyList<DepartmentByPositionDto>, ErrorList>> Handle(
        GetDepartmentsWithTopPositionsQuery query,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new DynamicParameters();

        parameters.Add("count", query.PositionsCount, DbType.Int32);

        var departments = await connection.QueryAsync<DepartmentByPositionDto>(
            """
            SELECT d.id, d.depth, d.is_active, d.created_at, d.updated_at, d.identifier, d.name, d.path, COUNT(dp.id) as positionsCount
            FROM public.departments d
                     LEFT JOIN public.department_positions dp ON dp.department_id = d.id
            GROUP BY d.path, d.name, d.identifier, d.updated_at, d.created_at, d.is_active, d.depth, d.id
            ORDER BY positionsCount DESC
            LIMIT @count
            """,
            parameters);

        // Пример с оконной функцией, работает медлеленее из за тяжёлых этапов: окно + DISTINCT 
        // var departments = await connection.QueryAsync<DepartmentDto>(
        //     """
        //     SELECT DISTINCT d.id, d.depth, d.is_active, d.created_at, d.updated_at, d.identifier, d.name, d.path, COUNT(dp.id) OVER(PARTITION BY d.id) as positions
        //     FROM public.departments d
        //              LEFT JOIN public.department_positions dp ON dp.department_id = d.id
        //     ORDER BY positions DESC
        //     LIMIT 5
        //     """,
        //     parameters);

        return departments.ToList();
    }
}

public record GetDepartmentsWithTopPositionsQuery(int? PositionsCount = 5) : IQuery;