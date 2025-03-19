using Unity.Netcode;
using System.Collections.Generic;

public class EconomyObject : NetworkBehaviour
{
	public StockPile stockPile = new();
	public void StockPile(Good good)
	{
		stockPile.Add(good);
	}
	public void StockPile(List<Good> stockPile) 
	{
		this.stockPile.Add(stockPile.ToArray());
	}
}
