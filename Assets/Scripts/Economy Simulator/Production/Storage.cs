using System;
using UnityEngine;

public class Storage : EconomyObject
{
	Nullable<Good> RequsetGood(Good value)
	{
		bool result = stockPile.Subtract(value);
		if (result) return value;

		PEBObject nationality = GetComponentInChildren<PEBObject>();
		Storage[] allAvailableStorage = PoliticalSearch.Find<Storage>(nationality.nation.Value);

		return null;
	}
}
