public class Timer
{
    public float CurrentTime { get; private set; }
    public float Interval { get; set; }

    public Timer(float interval)
    {
        Interval = interval;
        CurrentTime = 0f;
    }

    public bool Update(float deltaTime)
    {
        CurrentTime += deltaTime;
        if (CurrentTime >= Interval)
        {
            CurrentTime = 0f;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        CurrentTime = 0f;
    }
}
