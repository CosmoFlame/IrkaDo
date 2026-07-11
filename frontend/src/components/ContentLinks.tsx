import type { ContentLink } from "@/types/api";

/**
 * Renders editor-authored outbound links (to Instagram/Telegram posts, etc.) attached to a
 * news article, guide, or collaboration. Falls back to the raw URL when no title is set.
 */
export function ContentLinks({
  links,
  className,
  compact = false,
}: {
  links: ContentLink[];
  className?: string;
  compact?: boolean;
}) {
  if (links.length === 0) return null;

  return (
    <ul className={className}>
      {links.map((link) => (
        <li key={link.url}>
          <a
            href={link.url}
            target="_blank"
            rel="noopener noreferrer"
            className={`inline-flex items-center gap-1 font-medium text-amber-600 hover:text-amber-500 ${
              compact ? "text-sm" : ""
            }`}
          >
            {link.title || link.url}
            <span aria-hidden>↗</span>
          </a>
        </li>
      ))}
    </ul>
  );
}
