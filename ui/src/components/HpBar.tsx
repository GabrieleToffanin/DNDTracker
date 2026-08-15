interface Props {
  current: number;
  max: number;
  temporary?: number;
}

export default function HpBar({ current, max, temporary = 0 }: Props) {
  if (max <= 0 || current < 0) {
    return <span className="text-dim" style={{ fontSize: '.8rem' }}>Hidden</span>;
  }
  const pct = Math.min(100, Math.max(0, (current / max) * 100));
  const color = pct > 60 ? 'var(--clr-hp-high)' : pct > 30 ? 'var(--clr-hp-mid)' : 'var(--clr-hp-low)';

  return (
    <div style={{ minWidth: 80 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '.78rem', marginBottom: 2 }}>
        <span style={{ color }}>{current}/{max}</span>
        {temporary > 0 && <span className="text-gold">+{temporary}tmp</span>}
      </div>
      <div className="hp-bar-wrap">
        <div className="hp-bar" style={{ width: `${pct}%`, background: color }} />
      </div>
    </div>
  );
}
