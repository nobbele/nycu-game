public class PlayerStatus
{
    public StatusType Type;
    public float Value;
    public float ExpirationTime;
    public float DamageTimer;

    public PlayerStatus(StatusType type, float value, float expirationTime)
    {
        Type = type;
        Value = value;
        ExpirationTime = expirationTime;
        DamageTimer = 0f;
    }
}
