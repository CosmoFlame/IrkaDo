namespace IrkaDo.Application.Features;

public record PagedResult<T>(T[] Items, int Page, int PageSize, int TotalCount);
