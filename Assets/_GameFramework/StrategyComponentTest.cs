using GameFramework.Strategy;
using UnityEngine;

public interface ISomeStrategy : IStrategyContainer {}

public class BoxShape : ISomeStrategy
{
    public int BoxCount;
}
public class SphereShape : ISomeStrategy
{
    [SerializeField] private int _sphereCount;
    [SerializeField] private float[] _spheresArray = new float[50];
}
public class CapsuleShape : ISomeStrategy
{
    [SerializeField] private int _capsuleContCount;
    [SerializeField] private string[] _someStringArray;
}

public class StrategyComponentTest : MonoBehaviour
{
    public int SomeInt;
    public string SomeString;
    public bool SomeBool;
    [SerializeReference, StrategyContainer]
    public ISomeStrategy Chlen;

    public GameObject[] SomeArray;

    private void Update()
    {
        if(Input.anyKeyDown)
        {
            if (Chlen == null)
                Debug.Log("Chlen is null");
            else
                Debug.Log(Chlen.GetType().Name);
        }
    }
}
