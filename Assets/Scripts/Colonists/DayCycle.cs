using UnityEngine;

/// <summary>
/// Manages day/night cycle and game time.
/// Controls time speed (1x, 2x, 4x) and pause.
/// </summary>
public class DayCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Current game hour (0-23)")]
    [Range(0, 23)]
    public int hour = 12;

    [Tooltip("Current game minute (0-59)")]
    [Range(0, 59)]
    public int minute;

    [Tooltip("Current day number")]
    public int day = 1;

    [Tooltip("Current season: 0=Spring, 1=Summer, 2=Autumn, 3=Winter")]
    [Range(0, 3)]
    public int season;

    [Header("Speed")]
    [Tooltip("Game speed multiplier")]
    [Range(0f, 4f)]
    public float gameSpeed = 1f;

    [Tooltip("Real seconds per game minute at 1x speed")]
    public float secondsPerMinute = 1f;

    [Header("Lighting")]
    [Tooltip("Directional light for sun")]
    public Light sunLight;

    [Tooltip("Sunrise hour")]
    public int sunriseHour = 6;

    [Tooltip("Sunset hour")]
    public int sunsetHour = 20;

    // Public accessors
    public bool IsPaused => Mathf.Approximately(gameSpeed, 0f);
    public bool IsNight => hour < sunriseHour || hour >= sunsetHour;
    public float DayProgress => (hour * 60f + minute) / 1440f; // 0-1

    private float _timeAccumulator;

    // Singleton-like access
    public static DayCycle Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        HandleInput();
        if (IsPaused) return;

        _timeAccumulator += Time.deltaTime * gameSpeed;
        while (_timeAccumulator >= secondsPerMinute)
        {
            _timeAccumulator -= secondsPerMinute;
            AdvanceMinute();
        }

        UpdateLighting();
    }

    /// <summary>
    /// Handles keyboard input for time controls.
    /// Space = pause, 1/2/3 = speed.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameSpeed = Mathf.Approximately(gameSpeed, 0f) ? 1f : 0f;
        }
        // Only NumPad for speed (Alpha 1-3 used by BuildManager)
        if (Input.GetKeyDown(KeyCode.Keypad1))
            gameSpeed = 1f;
        if (Input.GetKeyDown(KeyCode.Keypad2))
            gameSpeed = 2f;
        if (Input.GetKeyDown(KeyCode.Keypad3))
            gameSpeed = 4f;
    }

    /// <summary>
    /// Advances game time by one minute.
    /// </summary>
    private void AdvanceMinute()
    {
        minute++;
        if (minute >= 60)
        {
            minute = 0;
            hour++;
            if (hour >= 24)
            {
                hour = 0;
                day++;
                CheckSeasonChange();
            }
        }
    }

    /// <summary>
    /// Changes season every 15 days.
    /// </summary>
    private void CheckSeasonChange()
    {
        if (day % 15 == 0)
        {
            season = (season + 1) % 4;
            Debug.Log($"[DayCycle] Season changed to {(Season)season}, Day {day}");
        }
    }

    /// <summary>
    /// Updates sun position based on time of day.
    /// </summary>
    private void UpdateLighting()
    {
        if (sunLight == null) return;

        float t = (hour * 60f + minute - sunriseHour * 60f) / ((sunsetHour - sunriseHour) * 60f);
        float sunAngle = Mathf.Lerp(5f, 175f, Mathf.Clamp01(t));
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 30f, 0f);

        // Intensity peaks at noon
        float noonProgress = Mathf.Sin(DayProgress * Mathf.PI);
        sunLight.intensity = Mathf.Lerp(0.2f, 1.2f, noonProgress);
        sunLight.color = IsNight ? new Color(0.3f, 0.4f, 0.8f) : Color.white;
    }

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter,
    }


}
