using System.Collections.Generic;
using UnityEngine;
using ReproducibleRandomGenerator;

public class Test : MonoBehaviour
{
    private ValueProcessor valueProcessor;
    private List<ulong> seedList = null;

    private void Start()
    {
        if (valueProcessor == null)
        {
            valueProcessor = new ValueProcessor();
            valueProcessor.RandomProvider = new SimplePCG();

            // prepare seeds
            ulong rootSeed = 1024;

            // prepare group seeds
            valueProcessor.RandomProvider.SetSeed(rootSeed);

            seedList = new List<ulong>(10);
            for (int i = 0; i < 10; i++)
            {
                seedList.Add(valueProcessor.RandULong(i));
            }
        }
    }

    private List<float> _cache = new List<float>(32);
    private List<float> _cache2 = new List<float>(32);
    public void OnBtnClicked()
    {
        // prepare random values
        var index = UnityEngine.Random.Range(0, 10);
        Debug.Log("Index: " + index);

        var groupSeed = seedList[index];

        valueProcessor.RandomProvider.SetSeed(groupSeed);

        _cache.Clear();
        for (int i = 0; i < 10; i++)
        {
            _cache.Add(valueProcessor.Value);
        }

        // reset seed
        valueProcessor.RandomProvider.SetSeed(groupSeed);

        for (int i = 0; i < 10; i++)
        {
            _cache2.Add(valueProcessor.Value);
        }

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Value Check: " + _cache[i] + " " + _cache2[i] + " same: " + (_cache[i] == _cache2[i]));
        }
    }
}
