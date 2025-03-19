using System;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    public static Inspector Instance;

    public void Inspect(GameObject inspectedObject)
    {
        Inspectable[] allInspectableObject = inspectedObject.GetComponentsInChildren<Inspectable>();
        throw(new NotImplementedException());
    }
}
