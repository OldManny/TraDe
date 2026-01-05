export interface BookLevel {
  price: number;
  quantity: number;
  total: number;
}

export interface Trade {
  id: string;
  price: number;
  quantity: number;
  time: string;
  side: 'buy' | 'sell';
}

export interface MarketStats {
  lastPrice: number;
  change: string;
  volume: string;
  high24h: number;
  low24h: number;
  spread?: string;
}