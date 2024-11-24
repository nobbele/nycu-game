using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    private List<PlayerStatus> statuses = new List<PlayerStatus>();

    public void AddOrUpdateStatus(StatusType type, float value, float duration)
    {
        var existingStatus = statuses.Find(s => s.Type == type);
        if (existingStatus != null)
        {
            existingStatus.Value = Mathf.Max(existingStatus.Value, value);
            existingStatus.ExpirationTime = Mathf.Max(existingStatus.ExpirationTime, Time.time + duration);
        }
        else
        {
            statuses.Add(new PlayerStatus(type, value, Time.time + duration));
        }
    }

    public void RemoveExpiredStatuses()
    {
        statuses.RemoveAll(s => s.ExpirationTime != -1 && Time.time >= s.ExpirationTime);
    }

    public float GetStatusValue(StatusType type)
    {
        var status = statuses.Find(s => s.Type == type && (s.ExpirationTime == -1 || Time.time < s.ExpirationTime));
        return status != null ? status.Value : 0f;
    }

    public bool HasStatus(StatusType type)
    {
        return statuses.Exists(s => s.Type == type && (s.ExpirationTime == -1 || Time.time < s.ExpirationTime));
    }

    private void Update()
    {
        foreach (var status in statuses)
        {
            ApplyStatusEffect(status);
        }

        RemoveExpiredStatuses();
    }

    private void ApplyStatusEffect(PlayerStatus status)
    {
        Player player = GetComponent<Player>();
        if (player == null) return;

        if (status.ExpirationTime != -1 && Time.time >= status.ExpirationTime)
        {
            return;
        }

        switch (status.Type)
        {
            case StatusType.LingeringDamage:
                status.DamageTimer += Time.deltaTime;
                if (status.DamageTimer >= 1f)
                {
                    player.OnDamage(gameObject, Mathf.RoundToInt(status.Value));
                    status.DamageTimer = 0f;
                }
                break;
        }
    }
}
