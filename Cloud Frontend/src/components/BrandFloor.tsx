import { BRAND } from '../config/brand';

type Props = {
  className?: string;
};

/** Large product name + catchphrase anchored to the bottom of splash / login. */
export function BrandFloor({ className = '' }: Props) {
  return (
    <footer className={`brand-floor ${className}`.trim()} aria-hidden>
      <span className="brand-floor-name">{BRAND.productName}</span>
      <span className="brand-floor-catch">{BRAND.productTagline}</span>
    </footer>
  );
}
