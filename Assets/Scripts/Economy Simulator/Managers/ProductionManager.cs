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
            Nationality nationality = extractor.GetComponent<Nationality>();
            if (nationality == null)
            {
                extractor.StockPile(extractor.GetProduction());
                continue;
            }
        }
    }
}
