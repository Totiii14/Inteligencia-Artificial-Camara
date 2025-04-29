using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public static class Randoms 
{

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        //Se usa < para tener la misma probabilidad de chances (50%)
    //        Debug.Log(Random.value < 0.5f ? "Cara" : "Seca"); 
    //        Debug.Log(Random.value * 7 < numeroBase); 
    //        Debug.Log(Random.value * (7 - 2) + 2); 
    //    }
    //}

    public static bool CoinTests()
    {
        return Random.value < 0.5f;
    }

    public static bool Chance(float chance)
    {
        return Random.value < chance;
    }

    public static float RandomRange(float min, float max)
    {
        return Random.value * (max - min) + min;
    }

    public static int WeightedRandom(IEnumerable<int> weights)
    {
        int totalWeight = 0;
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        float rnd = Random.value + totalWeight;
        foreach (int weight in weights)
        {
            rnd -= weight;
            if (rnd <= 0) return weight;
        }
        return -1;
    }

    public static T RandomWeightedObject<T>(Dictionary<T, int> objectWeight)
    {
        int totalWeight = 0;
        foreach (int weight in objectWeight.Values)
        {
            totalWeight += weight;
        }

        float rnd = Random.value + totalWeight;
        foreach (var pair in objectWeight)
        {
            rnd -= pair.Value;
            if (rnd <= 0) return pair.Key;
        }

        return default;
    }

    public static T RandomFromArray<T>(T[] array)
    {
        return array[(int)Random.value * array.Length];
    }

    public static T RandomFromMatrix<T>(this T[,] matrix)
    {
        int rndX = (int) Random.value * matrix.GetLength(0);
        int rndY = (int) Random.value * matrix.GetLength(1);

        return matrix[rndX, rndY];

        //int lengthX = matrix.GetLength(0);
        //int rnd = (int)Random.value * matrix.Length;
        //int x = rnd % lengthX;

        //return matrix[x, (rnd - x) / lengthX];
    }

    public static Vector3 NoY(this Vector3 v3)
    {
        v3.y = 0;
        return v3;
    }
}
