import { BarChart3, TrendingUp, TrendingDown, DollarSign } from 'lucide-react';
import type { MarketStats } from '../types/market';

export const StatsBar = ({ stats }: { stats: MarketStats }) => {
  const items = [
    { label: '24h Volume', value: stats.volume, icon: BarChart3, color: 'cyan' },
    { label: '24h High', value: `$${stats.high24h}`, icon: TrendingUp, color: 'emerald' },
    { label: '24h Low', value: `$${stats.low24h}`, icon: TrendingDown, color: 'rose' },
    { label: 'Spread', value: '$0.05', icon: DollarSign, color: 'amber' }
  ];

  return (
    <div className="grid grid-cols-4 gap-4 mb-6">
      {items.map((stat, i) => (
        <div key={i} className="bg-slate-900/50 border border-slate-800 rounded-xl p-4 backdrop-blur-sm">
          <div className="flex items-center justify-between mb-2">
            <span className="text-slate-400 text-sm">{stat.label}</span>
            <stat.icon className={`w-4 h-4 text-${stat.color}-400`} />
          </div>
          <div className={`text-xl font-bold text-${stat.color}-400`}>{stat.value}</div>
        </div>
      ))}
    </div>
  );
};