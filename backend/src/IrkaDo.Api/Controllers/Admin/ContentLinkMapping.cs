using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;

namespace IrkaDo.Api.Controllers.Admin;

/// <summary>
/// Maps the inline link editor payload (sent whole on every upsert) to <see cref="ContentLink"/>
/// entities. Null and blank rows are dropped (the editor can serialize an empty row as a JSON
/// <c>null</c>) and order is taken from the incoming array position.
/// </summary>
internal static class ContentLinkMapping
{
    public static List<ContentLink> ToEntities(IEnumerable<AdminLinkDto>? links) =>
        (links ?? [])
            .Where(l => l is not null && !string.IsNullOrWhiteSpace(l.Url))
            .Select((l, i) => new ContentLink
            {
                Url = l.Url.Trim(),
                Title = string.IsNullOrWhiteSpace(l.Title) ? null : l.Title.Trim(),
                TitleEn = string.IsNullOrWhiteSpace(l.TitleEn) ? null : l.TitleEn.Trim(),
                DisplayOrder = i
            })
            .ToList();

    /// <summary>Replaces a tracked entity's links in place so EF deletes removed rows and inserts new ones.</summary>
    public static void Replace(ICollection<ContentLink> existing, IEnumerable<AdminLinkDto>? links)
    {
        existing.Clear();
        foreach (var link in ToEntities(links))
            existing.Add(link);
    }
}
