using UnityEngine;
using System.Collections.Generic;

public class Extractor : EconomyObject
{
	[SerializeField] StockPile extractionProducts;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
	}

	public List<Good> GetProduction()
	{
		stockPile.Add(extractionProducts);
		List<Good> result = new List<Good>(stockPile.pile);
		stockPile.pile.Clear();
		return result;
	}
}
