using UnityEngine;
using System.Collections.Generic;

public class Converter : EconomyObject, IProducer
{
    public List<Good> input;
    public List<Good> output;

    public void Produce() { }
    public void Source() { }
}
