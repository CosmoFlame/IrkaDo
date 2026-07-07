"use client";

import { LazyMotion, MotionConfig, domAnimation } from "framer-motion";

/**
 * App-wide motion setup.
 *
 * - `LazyMotion` with `domAnimation` loads only the animation/gesture/inView features
 *   (~15kb) instead of the full `motion` bundle, and `strict` forbids the heavy
 *   `motion.*` component so only the lightweight `m.*` component is used anywhere.
 * - `MotionConfig reducedMotion="user"` makes every animation honour the OS
 *   "prefers-reduced-motion" setting automatically (transform/layout animations are
 *   disabled, opacity still fades), so we don't have to branch per-component.
 */
export function MotionProvider({ children }: { children: React.ReactNode }) {
  return (
    <MotionConfig reducedMotion="user">
      <LazyMotion features={domAnimation} strict>
        {children}
      </LazyMotion>
    </MotionConfig>
  );
}
