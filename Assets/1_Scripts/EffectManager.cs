using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EffectManager : MonoBehaviour
{
    private MovementController movementController;
    private List<EffectInstance> activeEffects = new List<EffectInstance>();

    private void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            effect.UpdateEffect(deltaTime);

            // Remove expired effects
            if (effect.remainingDuration <= 0)
            {
                RemoveEffect(effect);
            }
        }
    }

    public void ApplyEffect(Effect newEffect)
    {
        if (!newEffect.isStackable && activeEffects.Exists(e => e.effectData == newEffect))
        {
            Debug.Log($"{newEffect.effectName} is already active and cannot stack.");
            return;
        }
        
        var instance = new EffectInstance(newEffect);
        
        instance.particleEffect = Instantiate(newEffect.particleEffect, gameObject.transform);
        
        //Applying effect
        if (newEffect.effectType == EffectType.Buff && newEffect.effectName == "Speed Buff")
        {
            ApplySpeedEffect(newEffect.magnitude, newEffect.duration);
        }
        if (newEffect.effectType == EffectType.Debuff && newEffect.effectName == "Speed Debuff")
        {
            ApplySpeedEffect(newEffect.magnitude, newEffect.duration);
        }
        
        activeEffects.Add(instance);

        Debug.Log($"Applied effect: {newEffect.effectName}");
    }
    
    private void ApplySpeedEffect(float magnitude, float duration)
    {
        movementController.MultiplySpeedMultiplier(magnitude); // Increase speed by multiplier
        Debug.Log($"Speed Effect applied: x{magnitude} for {duration} seconds");
        
    }

    public void RemoveEffect(EffectInstance effect)
    {
        Destroy(effect.particleEffect);
        
        if (effect.effectData.effectName == "Speed Buff" || effect.effectData.effectName == "Speed Debuff")
        {
            movementController.DivideSpeedMultiplier(effect.effectData.magnitude); // Divide the multiplier
        }
        
        activeEffects.Remove(effect);
        Debug.Log($"Removed effect: {effect.effectData.effectName}");
    }
}
