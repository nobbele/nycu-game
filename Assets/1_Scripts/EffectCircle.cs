using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCircle : MonoBehaviour
{
    [SerializeField] private Effect effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EffectManager effectManager))
        {
            effectManager.ApplyEffect(effect);
            Debug.Log($"Applying {effect.effectName}");
        }
    }
}
