const API_URL = 'http://localhost:5269/api/v1/orders';

export const placeOrder = async (side: 'buy' | 'sell', price: number, quantity: number) => {
  // Map 'buy' -> 0, 'sell' -> 1 for the C# Enum
  const sideEnum = side === 'buy' ? 0 : 1;
  
  const response = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      price,
      quantity,
      side: sideEnum
    })
  });

  if (!response.ok) {
    throw new Error('Failed to place order');
  }
  
  return await response.json();
};