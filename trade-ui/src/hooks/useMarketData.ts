import { useEffect, useState, useRef } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import type { Trade, BookLevel, MarketStats } from '../types/market';

interface SignalRTrade {
  id?: string;
  Id?: string;
  executionPrice?: number;
  ExecutionPrice?: number;
  executionQuantity?: number;
  ExecutionQuantity?: number;
  executionTime?: string;
  ExecutionTime?: string;
  buyOrderId?: string;
  BuyOrderId?: string;
}

export const useMarketData = () => {
  const [trades, setTrades] = useState<Trade[]>([]);
  const [bids, setBids] = useState<BookLevel[]>([]);
  const [asks, setAsks] = useState<BookLevel[]>([]);
  const [stats, setStats] = useState<MarketStats>({
    lastPrice: 100.00,
    change: '0.00%',
    volume: '0',
    high24h: 100.00,
    low24h: 100.00,
    spread: '0.05', 
  });
  const [isConnected, setIsConnected] = useState(false);

  // Refs
  const volRef = useRef(0);
  const highRef = useRef(100);
  const lowRef = useRef(100);
  const openPriceRef = useRef(100);
  const lastTradePriceRef = useRef(100);

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:8080/hubs/marketdata")
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveTrades", (incomingTrades: SignalRTrade[]) => {
      // Process trades sequentially to determine direction
      const newTrades = incomingTrades.map(t => {
        const price = t.executionPrice || t.ExecutionPrice || 0;
        // If price >= last price, assume Buy (Green). Else Sell (Red).
        const side = price >= lastTradePriceRef.current ? 'buy' : 'sell';
        lastTradePriceRef.current = price;

        return {
          id: t.id || t.Id || '',
          price: price,
          quantity: t.executionQuantity || t.ExecutionQuantity || 0,
          time: new Date(t.executionTime || t.ExecutionTime || Date.now()).toLocaleTimeString(),
          side: side
        } as Trade;
      });

      if (newTrades.length === 0) return;

      const latestPrice = newTrades[newTrades.length - 1].price;

      // Stats Update
      volRef.current += newTrades.reduce((sum, t) => sum + t.quantity, 0);
      highRef.current = Math.max(highRef.current, ...newTrades.map(t => t.price));
      lowRef.current = Math.min(lowRef.current, ...newTrades.map(t => t.price));
      
      const changePct = ((latestPrice - openPriceRef.current) / openPriceRef.current) * 100;

      setStats({
        lastPrice: latestPrice,
        change: `${changePct >= 0 ? '+' : ''}${changePct.toFixed(2)}%`,
        volume: volRef.current.toLocaleString(),
        high24h: highRef.current,
        low24h: lowRef.current
      });

      setTrades(prev => [...newTrades.reverse(), ...prev].slice(0, 25));
      
      // Update Book: Generate more levels (12) to fill the height
      setBids(generateLevels(latestPrice, 'buy', 16));
      setAsks(generateLevels(latestPrice, 'sell', 16));
    });

    connection.start()
      .then(() => setIsConnected(true))
      .catch(err => console.error("SignalR Connection Error: ", err));

    return () => {
      connection.stop();
    };
  }, []);

  return { trades, bids, asks, stats, isConnected };
};

// Helper to accept 'count'
const generateLevels = (centerPrice: number, side: 'buy' | 'sell', count: number): BookLevel[] => {
  // If price is 0 or NaN, default to 100
  const safePrice = centerPrice || 100; 
  
  return Array.from({ length: count }).map((_, i) => {
    const offset = (i + 1) * 0.05;
    const price = side === 'buy' ? safePrice - offset : safePrice + offset;
    const quantity = Math.floor(Math.random() * 400) + 20;
    return {
      price,
      quantity,
      total: price * quantity 
    };
  });
};