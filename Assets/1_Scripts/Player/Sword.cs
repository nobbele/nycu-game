using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sword : MonoBehaviour
{
    private List<Collider> colliders = new List<Collider>();
    public IEnumerable<Collider> OverlappingColliders => colliders;

    private void OnTriggerEnter(Collider other) {
        if (!colliders.Contains(other)) { colliders.Add(other); }
    }

    private void OnTriggerExit(Collider other) {
        colliders.Remove(other);
    }
}
