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

	public InspectionData GetInspectableData()
	{
		string message = "Stockpile has " 
						+ stockPile.pile.Count.ToString() 
						+ " items left";
		return new(message, new(1.4f, 1.2f));
	}
}

public interface IProducer
{
	public void Source(); // Source Goods From Storage
	public void Produce();
}
