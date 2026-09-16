import type { Statistics } from '../lib/types';
import { formatTime } from '../lib/format';

interface StatsProps {
  stats: Statistics | null;
}

export function Stats({ stats }: StatsProps) {
  if (!stats) return null;

  const items = [
    { label: 'PB', value: stats.personalBest },
    { label: 'Ao5', value: stats.ao5 },
    { label: 'Ao12', value: stats.ao12 },
    { label: 'Avg', value: stats.totalAverage },
  ];

  return (
    <div className="stats">
      {items.map(({ label, value }) => (
        <div key={label} className="stat">
          <span className="stat-label">{label}</span>
          <span className="stat-value">
            {value !== null && value >= 0 ? formatTime(value) : '—'}
          </span>
        </div>
      ))}
    </div>
  );
}
