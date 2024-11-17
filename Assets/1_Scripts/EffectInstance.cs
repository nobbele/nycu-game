using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInstance
{
    public Effect effectData;
    public float remainingDuration;
    public GameObject particleEffect;

    public EffectInstance(Effect effect)
    {
        effectData = effect;
        remainingDuration = effect.duration;
    }

    public void UpdateEffect(float deltaTime)
    {
        remainingDuration -= deltaTime;
    }
}
