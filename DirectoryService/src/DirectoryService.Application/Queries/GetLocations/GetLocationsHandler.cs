using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Common;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Queries.GetLocations;

public sealed class GetLocationsHandler : IQueryHandler<PagedList<LocationDto>, GetLocationsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetLocationsHandler(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<Result<PagedList<LocationDto>, ErrorList>> Handle(GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var parameters = new DynamicParameters();

        parameters.Add("pageSize", query.Pagination.PageSize, DbType.Int32);
        parameters.Add("offset", (query.Pagination.Page - 1) * query.Pagination.PageSize, DbType.Int32);

        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("l.name ILIKE '%' || @search || '%'");
            parameters.Add("search", query.Search, DbType.String);
        }

        if (query.DepartmentIds != null && query.DepartmentIds.Length > 0)
        {
            var departmentIds = string.Join(",", query.DepartmentIds.Select(i => $"'{i}'"));
            conditions.Add($"dl.department_id in({departmentIds})");
        }

        if (query.IsActive != null)
        {
            conditions.Add("l.is_active = @isActive");
            parameters.Add("isActive", query.IsActive, DbType.Boolean);
        }

        var whereClause = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
        long? totalCount = null;
        var locations = await connection.QueryAsync<LocationDto, long, LocationDto>(
            $"""
             SELECT l.id, l.name, l.address, l.time_zone, l.is_active, l.created_at, l.updated_at,
                    COUNT(*) OVER() as total_count
             FROM locations l
             LEFT JOIN department_locations dl ON  dl.location_id = l.id
             {whereClause} 
             LIMIT @pageSize OFFSET @offset
             """,
            param: parameters,
            map: (l, count) =>
            {
                totalCount ??= count;

                return l;
            },
            splitOn: "total_count");


        return new PagedList<LocationDto>
        {
            TotalCount = totalCount ?? 0,
            Items = locations
                .OrderBy(l => l.Name)
                .ThenBy(l => l.CreatedAt)
                .ToList(),
            Page = query.Pagination.Page,
            PageSize = query.Pagination.PageSize
        };
    }
}

public record GetLocationsQuery(Guid[]? DepartmentIds, string? Search, bool? IsActive, PaginationRequest Pagination)
    : IQuery;