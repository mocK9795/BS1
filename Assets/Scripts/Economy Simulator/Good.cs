using System.Collections.Generic;

[System.Serializable]
public struct Good
{
	public string name;
	public int quantity;

	public bool Equals(Good other) { return other.name == name; }
	public bool Contains(Good other) { return other.name == name && quantity >= other.quantity;}

	
	/// <summary>
	/// Returns true if addition was valid
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Add(Good other) 
	{
		if (Equals(other)) { quantity += other.quantity; return true; }
		return false;
	}

	/// <summary>
	/// Returns true if subtraction was valid
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Subtract(Good other)
	{
		if (!Contains(other)) { return false; }
		quantity -= other.quantity; return true;
	}
}

[System.Serializable]
public struct StockPile
{
	public List<Good> pile;

	public void Add(Good good)
	{
		foreach (Good other in pile) {
			if (other.Add(good)) return;
		}
		pile.Add(good);
	}
	public bool Subtract(Good other)
	{
		if (!Contains(other)) return false;
		foreach (var good in pile)
		{
			good.Subtract(other);
		}
		return false;
	}
	public bool Contains(Good other)
	{
		foreach (var good in pile)
		{
			if (good.Contains(other)) return true;
		}
		return false;
	} 
	public bool Contains(IEnumerable<Good> other)
	{
		foreach (var good in other)
		{
			if (!Contains(good)) return false;
		}
		return true;
	}
	public void Subtract(IEnumerable<Good> cost)
	{
		foreach (var good in cost)
		{
			if (Subtract(good)) return;
		}
	}
	public void Add(IEnumerable<Good> goods)
	{
		foreach (Good good in goods)
		{
			Add(good);
		}
	}
	public void Add(StockPile otherPile)
	{
		Add(otherPile.pile);
	}
}
