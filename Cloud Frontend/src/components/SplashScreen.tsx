import { BRAND } from '../config/brand';
import { BrandFloor } from './BrandFloor';

const PARTICLES = Array.from({ length: 14 }, (_, i) => i);
const ORBITS = Array.from({ length: 6 }, (_, i) => i);

type Props = {
  exiting?: boolean;
};

export function SplashScreen({ exiting = false }: Props) {
  return (
    <div
      className={`splash-screen ${exiting ? 'splash-screen--out' : ''}`}
      role="presentation"
      aria-hidden={exiting}
    >
      <div className="splash-bg" aria-hidden>
        <div className="splash-mesh" />
        <div className="splash-aurora splash-aurora-a" />
        <div className="splash-aurora splash-aurora-b" />
        <div className="splash-grid" />
        <div className="splash-ambient">
          <span className="app-orb app-orb-1 splash-orb" />
          <span className="app-orb app-orb-2 splash-orb" />
          <span className="app-orb app-orb-3 splash-orb" />
          <span className="splash-orb splash-orb-extra" />
        </div>
        <div className="splash-particles">
          {PARTICLES.map((i) => (
            <span key={i} className="splash-particle" />
          ))}
        </div>
      </div>
      <div className="splash-vignette" aria-hidden />

      <div className="splash-core">
        <div className="splash-eye-stage">
          <div className="splash-rings" aria-hidden>
            <span className="splash-ring splash-ring-1" />
            <span className="splash-ring splash-ring-2" />
            <span className="splash-ring splash-ring-3" />
          </div>
          <div className="splash-orbit" aria-hidden>
            {ORBITS.map((i) => (
              <span key={i} className="splash-orbit-dot" />
            ))}
          </div>
          <div className="splash-eye-halo" aria-hidden />
          <div className="splash-eye-wrap">
            <span className="cloudeye-mark splash-eye" aria-hidden>
              <span className="cloudeye-iris" />
              <span className="cloudeye-shine" />
            </span>
          </div>
        </div>

        <div className="splash-brand">
          <span className="brand-mark splash-rr" aria-hidden>
            RR
          </span>
          <p className="splash-client">{BRAND.clientName}</p>
          <div className="splash-branches" aria-hidden>
            <span>Salem</span>
            <span className="splash-branch-dot" />
            <span>Namakkal</span>
            <span className="splash-branch-dot" />
            <span>Kolathur</span>
          </div>
        </div>

        <div className="splash-loader" aria-hidden>
          <div className="splash-loader-track">
            <span className="splash-loader-fill" />
            <span className="splash-loader-glow" />
          </div>
        </div>
        <p className="splash-status">
          <span className="splash-status-text">Waking every branch</span>
          <span className="splash-status-dots" aria-hidden>
            <span />
            <span />
            <span />
          </span>
        </p>
      </div>

      <BrandFloor className="brand-floor--splash" />
    </div>
  );
}
