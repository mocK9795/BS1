using Unity.Netcode;
using System.Collections.Generic;

public class EconomyObject : NetworkBehaviour, Inspectable
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

	public string GetInspectableData()
	{
		return "Economy Object\n" +
			"Stockpile has " + stockPile.pile.Count.ToString() + " items left";
	}
}
