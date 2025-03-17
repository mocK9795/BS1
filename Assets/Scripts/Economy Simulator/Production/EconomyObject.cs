using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class EconomyObject : NetworkBehaviour
{
	StockPile stockPile = new();
	public void StockPile(Good good)
	{
		stockPile.Add(good);
	}
	public void StockPile(List<Good> stockPile) 
	{
		this.stockPile.Add(stockPile.ToArray());
	}
}
