using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct NormalDistribution {
    public float Mean;
    public float Variance;
    public float Min;
    public float Max;

    public float Sample() {
        var u1 = Random.value;
        var u2 = Random.value;

        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return Mathf.Clamp(Min, Mean + Mathf.Sqrt(Variance) * randStdNormal, Max);
    }
}