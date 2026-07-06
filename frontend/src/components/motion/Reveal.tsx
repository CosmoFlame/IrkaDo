"use client";

import { m } from "framer-motion";

/**
 * Scroll-reveal wrapper: fades and rises its children into view once, the first
 * time they enter the viewport. Reduced-motion is handled globally by
 * `MotionConfig reducedMotion="user"` (the y offset is dropped, leaving a plain fade).
 *
 * Server-rendered content can be passed as children — this client leaf only adds
 * the animation shell, keeping page components on the server.
 */
export function Reveal({
  children,
  className,
  delay = 0,
  y = 24,
  as = "div",
}: {
  children: React.ReactNode;
  className?: string;
  delay?: number;
  /** Initial vertical offset in px before the element rises into place. */
  y?: number;
  as?: "div" | "section" | "li" | "article";
}) {
  const MotionTag = m[as];
  return (
    <MotionTag
      className={className}
      initial={{ opacity: 0, y }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true, amount: 0.2, margin: "0px 0px -10% 0px" }}
      transition={{ duration: 0.6, delay, ease: [0.22, 1, 0.36, 1] }}
    >
      {children}
    </MotionTag>
  );
}
