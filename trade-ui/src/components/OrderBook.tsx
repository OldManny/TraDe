import { BarChart3 } from 'lucide-react';
import type { BookLevel } from '../types/market';

interface Props { bids: BookLevel[]; asks: BookLevel[]; }

export const OrderBook = ({ bids, asks }: Props) => {
  const maxBidVolume = Math.max(...bids.map(b => b.quantity), 1);
  const maxAskVolume = Math.max(...asks.map(a => a.quantity), 1);

  return (
    <div className="bg-slate-900/50 border border-slate-800 rounded-xl backdrop-blur-sm overflow-hidden flex flex-col h-full">
      <div className="p-4 border-b border-slate-800 flex-none">
        <h2 className="text-lg font-semibold flex items-center gap-2">
          <BarChart3 className="w-5 h-5 text-cyan-400" />
          Order Book
        </h2>
      </div>
      <div className="grid grid-cols-2 divide-x divide-slate-800 flex-1 min-h-0">
        {/* Bids Column */}
        <div className="flex flex-col h-full">
          <div className="grid grid-cols-3 gap-2 p-3 bg-slate-900/80 text-xs font-medium text-slate-400 border-b border-slate-800 flex-none">
            <div>Price (USD)</div><div className="text-right">Size</div><div className="text-right">Total</div>
          </div>
          <div className="p-2 space-y-1 overflow-y-auto flex-1 custom-scrollbar">
            {bids.map((bid, i) => (
              <div key={i} className="relative grid grid-cols-3 gap-2 p-2 rounded-lg hover:bg-emerald-500/5 transition-colors group">
                <div className="absolute inset-0 bg-gradient-to-r from-emerald-500/10 to-transparent rounded-lg"
                  style={{ width: `${(bid.quantity / maxBidVolume) * 100}%` }} />
                <div className="relative z-10 font-mono text-emerald-400 font-semibold">{bid.price.toFixed(2)}</div>
                <div className="relative z-10 text-right font-mono text-slate-300">{bid.quantity.toFixed(0)}</div>
                <div className="relative z-10 text-right font-mono text-slate-400 text-sm">{(bid.price * bid.quantity).toLocaleString(undefined, { maximumFractionDigits: 0 })}</div>
              </div>
            ))}
          </div>
        </div>
        {/* Asks Column */}
        <div className="flex flex-col h-full">
          <div className="grid grid-cols-3 gap-2 p-3 bg-slate-900/80 text-xs font-medium text-slate-400 border-b border-slate-800 flex-none">
            <div>Price (USD)</div><div className="text-right">Size</div><div className="text-right">Total</div>
          </div>
          <div className="p-2 space-y-1 overflow-y-auto flex-1 custom-scrollbar">
            {asks.map((ask, i) => (
              <div key={i} className="relative grid grid-cols-3 gap-2 p-2 rounded-lg hover:bg-rose-500/5 transition-colors group">
                <div className="absolute inset-0 bg-gradient-to-r from-rose-500/10 to-transparent rounded-lg"
                  style={{ width: `${(ask.quantity / maxAskVolume) * 100}%` }} />
                <div className="relative z-10 font-mono text-rose-400 font-semibold">{ask.price.toFixed(2)}</div>
                <div className="relative z-10 text-right font-mono text-slate-300">{ask.quantity.toFixed(0)}</div>
                <div className="relative z-10 text-right font-mono text-slate-400 text-sm">{(ask.price * ask.quantity).toLocaleString(undefined, { maximumFractionDigits: 0 })}</div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};