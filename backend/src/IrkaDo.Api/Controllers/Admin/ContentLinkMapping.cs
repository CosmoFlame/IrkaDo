using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    /// <summary>
    /// Replaces an already-tracked parent's links: deletes the current rows and inserts the new set.
    /// The new links are added through the <see cref="DbSet{TEntity}"/> (not just the parent's
    /// navigation) so EF marks them <c>Added</c> — otherwise their client-generated key makes the
    /// change tracker treat them as existing rows and emit an UPDATE that matches nothing.
    /// </summary>
    public static void ReplaceLinks(
        IAppDbContext db,
        ICollection<ContentLink> existing,
        IEnumerable<AdminLinkDto>? links,
        Action<ContentLink> assignParent)
    {
        if (existing.Count > 0)
            db.ContentLinks.RemoveRange(existing);

        foreach (var link in ToEntities(links))
        {
            assignParent(link);
            db.ContentLinks.Add(link);
        }
    }
}
