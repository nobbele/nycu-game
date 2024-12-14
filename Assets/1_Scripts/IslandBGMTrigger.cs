using UnityEngine;
using System.Collections.Generic;

public class IslandBGMTrigger : MonoBehaviour
{
    public AudioClip islandBGM;
    public int priority;
    public float volume = 1.0f;

    private static List<IslandBGMTrigger> activeTriggers = new List<IslandBGMTrigger>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeTriggers.Add(this);
            UpdateBGM();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeTriggers.Remove(this);
            UpdateBGM();
        }
    }

    private void UpdateBGM()
    {
        IslandBGMTrigger highestPriorityTrigger = null;
        foreach (var trigger in activeTriggers)
        {
            if (highestPriorityTrigger == null || trigger.priority > highestPriorityTrigger.priority)
            {
                highestPriorityTrigger = trigger;
            }
        }

        if (highestPriorityTrigger != null)
        {
            BGMManager.Instance.PlayBGM(highestPriorityTrigger.islandBGM, highestPriorityTrigger.volume);
            BGMManager.Instance.CurrentPriority = highestPriorityTrigger.priority;
        }
        else
        {
            BGMManager.Instance.StopBGM();
            BGMManager.Instance.CurrentPriority = 0;
        }
    }
}
