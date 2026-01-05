import { useState } from 'react';
import { Activity, Clock, Wifi, WifiOff } from 'lucide-react';
import { StatsBar } from './components/StatsBar';
import { OrderBook } from './components/OrderBook';
import { useMarketData } from './hooks/useMarketData';
import { placeOrder } from './services/api';

export default function App() {
  // Hook into the Live Data
  const { trades, bids, asks, stats, isConnected } = useMarketData();
  
  const [orderForm, setOrderForm] = useState({ side: 'buy' as 'buy' | 'sell', price: '', qty: '' });
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Handle Order Submission
  const handlePlaceOrder = async () => {
    if (!orderForm.price || !orderForm.qty) return;
    setIsSubmitting(true);
    try {
      await placeOrder(orderForm.side, parseFloat(orderForm.price), parseFloat(orderForm.qty));
      setOrderForm(prev => ({ ...prev, price: '', qty: '' }));
      // Visual feedback or toast could go here
    } catch {
      alert("Failed to connect to Matching Engine");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-950 via-slate-900 to-slate-950 text-slate-100 p-6">
      {/* Header */}
      <div className="mb-6">
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 bg-gradient-to-br from-emerald-500 to-cyan-500 rounded-xl flex items-center justify-center shadow-lg shadow-emerald-500/20">
              <Activity className="w-7 h-7 text-white" />
            </div>
            <div>
              <h1 className="text-3xl font-bold bg-gradient-to-r from-emerald-400 to-cyan-400 bg-clip-text text-transparent">
                TraDe Engine
              </h1>
              <div className="flex items-center gap-2">
                <p className="text-slate-400 text-sm">High-Performance Limit Order Book</p>
                {isConnected ? 
                  <span className="flex items-center gap-1 text-emerald-500 text-xs bg-emerald-500/10 px-2 py-0.5 rounded-full"><Wifi className="w-3 h-3" /> Live</span> : 
                  <span className="flex items-center gap-1 text-rose-500 text-xs bg-rose-500/10 px-2 py-0.5 rounded-full"><WifiOff className="w-3 h-3" /> Offline</span>
                }
              </div>
            </div>
          </div>
          <div className="text-right">
            <div className="text-sm text-slate-400">Last Price</div>
            <div className="text-2xl font-bold text-emerald-400 transition-all duration-300">${stats.lastPrice.toFixed(2)}</div>
            <div className={`text-sm ${stats.change.startsWith('+') ? 'text-emerald-400' : 'text-rose-400'}`}>{stats.change}</div>
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
              <input 
                type="number" placeholder="Price (USD)" 
                value={orderForm.price} onChange={e => setOrderForm({...orderForm, price: e.target.value})} 
                className="w-full bg-slate-950 border border-slate-700 rounded-lg px-4 py-3 text-slate-100 focus:border-cyan-500 focus:ring-2 focus:ring-cyan-500/20 outline-none transition-all font-mono" 
              />
              <input 
                type="number" placeholder="Quantity" 
                value={orderForm.qty} onChange={e => setOrderForm({...orderForm, qty: e.target.value})} 
                className="w-full bg-slate-950 border border-slate-700 rounded-lg px-4 py-3 text-slate-100 focus:border-cyan-500 focus:ring-2 focus:ring-cyan-500/20 outline-none transition-all font-mono" 
              />
              <button 
                onClick={handlePlaceOrder}
                disabled={isSubmitting || !isConnected}
                className={`w-full py-3 rounded-lg font-semibold transition-all text-white ${
                  orderForm.side === 'buy' ? 'bg-gradient-to-r from-emerald-500 to-emerald-600 shadow-emerald-500/30' : 'bg-gradient-to-r from-rose-500 to-rose-600 shadow-rose-500/30'
                } shadow-lg disabled:opacity-50 disabled:cursor-not-allowed`}
              >
                {isSubmitting ? 'Sending...' : (orderForm.side === 'buy' ? 'Place Buy Order' : 'Place Sell Order')}
              </button>
            </div>
          </div>

          {/* Recent Trades */}
          <div className="bg-slate-900/50 border border-slate-800 rounded-xl backdrop-blur-sm overflow-hidden">
            <div className="p-4 border-b border-slate-800 flex items-center gap-2">
              <Clock className="w-5 h-5 text-cyan-400" />
              <h2 className="text-lg font-semibold">Recent Trades</h2>
            </div>
            <div className="divide-y divide-slate-800 max-h-[400px] overflow-y-auto">
              {trades.map((trade, idx) => (
                <div key={trade.id || idx} className="p-3 hover:bg-slate-800/30 transition-colors flex justify-between items-center">
                   <div>
                    <span className={`font-mono font-bold block ${trade.side === 'buy' ? 'text-emerald-400' : 'text-rose-400'}`}>
                      ${trade.price.toFixed(2)}
                    </span>
                    <span className="text-xs text-slate-500 font-mono">{trade.time}</span>
                   </div>
                   <div className="text-right">
                    <span className="text-slate-300 font-mono text-sm block">{trade.quantity}</span>
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