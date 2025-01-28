using UnityEngine;
using DG.Tweening;

public class ObjectShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float duration = 1f; // Duration of the shake
    [SerializeField] private float strength = 1f; // Intensity of the shake
    [SerializeField] private int vibrato = 10; // How much the object vibrates
    [SerializeField] private float randomness = 90f; // Randomness of the shake
    [SerializeField] private bool fadeOut = true; // If the shake intensity fades over time

    [Header("Shake Type")]
    [SerializeField] private ShakeType shakeType = ShakeType.Position; // Type of shake

    public enum ShakeType
    {
        Position,
        Rotation,
        Scale
    }

    // Public method to trigger the shake
    public void Shake()
    {
        switch (shakeType)
        {
            case ShakeType.Position:
                transform.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
                break;
            case ShakeType.Rotation:
                transform.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut);
                break;
            case ShakeType.Scale:
                transform.DOShakeScale(duration, strength, vibrato, randomness, fadeOut);
                break;
        }
    }

    // Example: Trigger the shake when the script starts
    private void Start()
    {
        Shake();
    }
}
