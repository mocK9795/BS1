using UnityEngine;
using System.Collections.Generic;

public class Extractor : EconomyObject, IProducer
{
	[SerializeField] StockPile extractionProducts;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
	}

	public void Produce()
	{
		stockPile.Add(extractionProducts);
	}

	public void Source()
	{

	}
}
