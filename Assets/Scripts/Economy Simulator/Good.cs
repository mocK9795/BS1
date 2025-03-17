using System.Collections.Generic;

[System.Serializable]
public struct Good
{
	public string name;
	public int quantity;

	public bool Equals(Good other) { return other.name == name; }
	public bool Contains(Good other) { return other.name == name && quantity >= other.quantity;}

	// Summary:
	//		Returns true if addition was valid
	public bool Add(Good other) 
	{
		if (Equals(other)) { quantity += other.quantity; return true; }
		return false;
	}
}

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

	public void Add(Good[] goods)
	{
		foreach (Good good in goods)
		{
			Add(good);
		}
	}
}
