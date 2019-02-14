using UnityEngine;

[CreateAssetMenu(fileName = "New Settings", menuName = "Settings")]
public class GameSettings : ScriptableObject
{
// поменяй в местах, где юзается, и убери потом эти комментарии
    [Header("Color")]
    public Color failCylinderColor;
// поменяй в местах, где юзается
    [Header("Cylinder Value")]
    public float increaseDecreaseStep = 0.04f;
// поменяй в местах, где юзается
    [Header("Other Value")]
    public float waitWhenPerfectWave = 1f;
}
