using UnityEngine;

public class ProductionManager : BaseManager
{
    public override void Tick() 
    {
        ExtractProducts();
    }

    void ExtractProducts()
    {
        var extratorObjects = FindObjectsByType<Extractor>(FindObjectsSortMode.None);

        foreach (var extractor in extratorObjects)
        {
            ExtractProducts(extractor);
        }
    }

    void ExtractProducts(Extractor extractor)
    {
        extractor.Produce();
		Nationality nationality = extractor.GetComponent<Nationality>();
        if (nationality == null) return;

        var nearestStoragePoint = PoliticalSearch.Find<Storage>(nationality.net_nation.Value, extractor.transform.position);
        if (nearestStoragePoint == null) return;

        nearestStoragePoint.stockPile.Add(extractor.stockPile);
        extractor.stockPile.pile.Clear();
	}
}
