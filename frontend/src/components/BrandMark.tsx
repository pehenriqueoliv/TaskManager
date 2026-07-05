export function BrandMark({ className = 'brand__mark' }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 32 32" width="26" height="26" aria-hidden="true">
      <rect width="32" height="32" rx="8" fill="#5B3DF5" />
      <rect x="7" y="8" width="4" height="16" rx="1.5" fill="#ffffff" opacity="0.55" />
      <rect x="14" y="8" width="4" height="11" rx="1.5" fill="#ffffff" opacity="0.8" />
      <rect x="21" y="8" width="4" height="7" rx="1.5" fill="#ffffff" />
    </svg>
  )
}
