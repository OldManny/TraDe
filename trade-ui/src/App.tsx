import { useState } from 'react';
import { Activity, Clock } from 'lucide-react';
import { StatsBar } from './components/StatsBar';
import { OrderBook } from './components/OrderBook';
import type { BookLevel, Trade, MarketStats } from './types/market';

export default function App() {
  const [bids] = useState<BookLevel[]>([
    { price: 100.50, quantity: 150, total: 15075 },
    { price: 100.45, quantity: 200, total: 20090 },
    { price: 100.40, quantity: 175, total: 17570 },
    { price: 100.35, quantity: 225, total: 22578.75 },
    { price: 100.30, quantity: 180, total: 18054 }
  ]);

  const [asks] = useState<BookLevel[]>([
    { price: 100.55, quantity: 120, total: 12066 },
    { price: 100.60, quantity: 160, total: 16096 },
    { price: 100.65, quantity: 140, total: 14091 },
    { price: 100.70, quantity: 190, total: 19133 },
    { price: 100.75, quantity: 155, total: 15616.25 }
  ]);

  const [trades] = useState<Trade[]>([
    { id: '1', price: 100.52, quantity: 50, time: '14:32:15', side: 'buy' },
    { id: '2', price: 100.51, quantity: 75, time: '14:32:12', side: 'sell' },
    { id: '3', price: 100.53, quantity: 100, time: '14:32:08', side: 'buy' },
    { id: '4', price: 100.50, quantity: 60, time: '14:32:05', side: 'sell' },
    { id: '5', price: 100.52, quantity: 85, time: '14:32:01', side: 'buy' }
  ]);

  const [stats] = useState<MarketStats>({
    lastPrice: 100.52, change: '+0.45%', volume: '1.2M', high24h: 101.20, low24h: 99.80
  });

  const [orderForm, setOrderForm] = useState({ side: 'buy' as 'buy' | 'sell', price: '', qty: '' });

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-950 via-slate-900 to-slate-950 text-slate-100 p-6">
      {/* Header */}
      <div className="mb-6">
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 bg-gradient-to-br from-emerald-500 to-cyan-500 rounded-xl flex items-center justify-center">
              <Activity className="w-7 h-7 text-white" />
            </div>
            <div>
              <h1 className="text-3xl font-bold bg-gradient-to-r from-emerald-400 to-cyan-400 bg-clip-text text-transparent">
                TraDe Engine
              </h1>
              <p className="text-slate-400 text-sm">High-Performance Limit Order Book</p>
            </div>
          </div>
          <div className="text-right">
            <div className="text-sm text-slate-400">Last Price</div>
            <div className="text-2xl font-bold text-emerald-400">${stats.lastPrice}</div>
            <div className="text-sm text-emerald-400">{stats.change}</div>
          </div>
        </div>

        <StatsBar stats={stats} />
      </div>

      <div className="grid grid-cols-12 gap-6">
        <div className="col-span-8">
          <OrderBook bids={bids} asks={asks} />
        </div>

        <div className="col-span-4 space-y-6">
          {/* Order Entry */}
          <div className="bg-slate-900/50 border border-slate-800 rounded-xl backdrop-blur-sm overflow-hidden">
            <div className="p-4 border-b border-slate-800">
              <h2 className="text-lg font-semibold">Place Order</h2>
            </div>
            <div className="p-4 space-y-4">
              <div className="grid grid-cols-2 gap-2 p-1 bg-slate-950 rounded-lg">
                <button
                  onClick={() => setOrderForm({...orderForm, side: 'buy'})}
                  className={`py-2 rounded-md font-semibold transition-all ${
                    orderForm.side === 'buy' ? 'bg-emerald-500 text-white shadow-lg shadow-emerald-500/50' : 'text-slate-400 hover:text-slate-300'
                  }`}
                >BUY</button>
                <button
                  onClick={() => setOrderForm({...orderForm, side: 'sell'})}
                  className={`py-2 rounded-md font-semibold transition-all ${
                    orderForm.side === 'sell' ? 'bg-rose-500 text-white shadow-lg shadow-rose-500/50' : 'text-slate-400 hover:text-slate-300'
                  }`}
                >SELL</button>
              </div>
              <input type="number" placeholder="Price (USD)" value={orderForm.price} onChange={e => setOrderForm({...orderForm, price: e.target.value})} className="w-full bg-slate-950 border border-slate-700 rounded-lg px-4 py-3 text-slate-100 focus:border-cyan-500 focus:ring-2 focus:ring-cyan-500/20 outline-none transition-all font-mono" />
              <input type="number" placeholder="Quantity" value={orderForm.qty} onChange={e => setOrderForm({...orderForm, qty: e.target.value})} className="w-full bg-slate-950 border border-slate-700 rounded-lg px-4 py-3 text-slate-100 focus:border-cyan-500 focus:ring-2 focus:ring-cyan-500/20 outline-none transition-all font-mono" />
              <button className={`w-full py-3 rounded-lg font-semibold transition-all text-white ${
                orderForm.side === 'buy' ? 'bg-gradient-to-r from-emerald-500 to-emerald-600 shadow-emerald-500/30' : 'bg-gradient-to-r from-rose-500 to-rose-600 shadow-rose-500/30'
              } shadow-lg`}>
                {orderForm.side === 'buy' ? 'Place Buy Order' : 'Place Sell Order'}
              </button>
            </div>
          </div>

          {/* Recent Trades */}
          <div className="bg-slate-900/50 border border-slate-800 rounded-xl backdrop-blur-sm overflow-hidden">
            <div className="p-4 border-b border-slate-800">
              <h2 className="text-lg font-semibold flex items-center gap-2">
                <Clock className="w-5 h-5 text-cyan-400" />
                Recent Trades
              </h2>
            </div>
            <div className="divide-y divide-slate-800">
              {trades.map((trade) => (
                <div key={trade.id} className="p-3 hover:bg-slate-800/30 transition-colors">
                  <div className="flex justify-between items-center mb-1">
                    <span className={`font-mono font-semibold ${trade.side === 'buy' ? 'text-emerald-400' : 'text-rose-400'}`}>
                      ${trade.price.toFixed(2)}
                    </span>
                    <span className="text-xs text-slate-500 font-mono">{trade.time}</span>
                  </div>
                  <div className="flex justify-between text-sm text-slate-400">
                    <span>Qty: {trade.quantity}</span>
                    <span className={`text-xs px-2 py-0.5 rounded ${trade.side === 'buy' ? 'bg-emerald-500/20 text-emerald-400' : 'bg-rose-500/20 text-rose-400'}`}>
                      {trade.side.toUpperCase()}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}