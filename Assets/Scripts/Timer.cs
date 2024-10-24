using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class Timer : MonoBehaviour
{
    public float MaxTimerDuration = 10;
    public bool _isLooping = true;
    [HideInInspector] public bool Tick { get; private set; }
    [HideInInspector] public bool IsActive = false;

    private Image img;
    private float _currentTime;

    void Start()
    {
        img = GetComponent<Image>();
        if (!img)
        {
            Debug.LogWarning("Timer: Image component not found!");
            enabled = false;
            return;
        }

        ResetTimer();
    }

    void Update()
    {
        Tick = false;
        IsActive = false;

        if (_currentTime != -2)
        {
            IsActive = true;
            _currentTime -= Time.deltaTime;

            if (_currentTime <= 0)
            {
                Tick = true;
                _currentTime = _isLooping ? MaxTimerDuration : -2;
            }

            img.fillAmount = Mathf.Clamp01(1 - _currentTime / MaxTimerDuration);
        }
    }

    public void ResetTimer()
    {
        _currentTime = _isLooping ? MaxTimerDuration : -2;
        if (img)
            img.fillAmount = 1;
    }

    public void StartTimer() => _currentTime = MaxTimerDuration;
}
