using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public float detectionLevel = 0f;
    public float maxDetection = 100f;

    public float increaseRate = 20f;
    public float decreaseRate = 15f;

    public PlayerStealthController stealth;

    void Start()
    {
        detectionLevel = 0f;
    }

    void Update()
    {
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (isCrouching)
        {
            detectionLevel -= decreaseRate * Time.deltaTime;
        }
        else
        {
            detectionLevel += increaseRate * Time.deltaTime;
        }

        detectionLevel = Mathf.Clamp(detectionLevel, 0f, maxDetection);

        if (detectionLevel >= maxDetection)
        {
            Debug.Log("PLAYER DETECTED!");
        }
    }

    public float GetDetectionLevel()
    {
        return detectionLevel;
    }
}