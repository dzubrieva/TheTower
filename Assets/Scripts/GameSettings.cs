using UnityEngine;


[CreateAssetMenu(fileName = "New Settings", menuName = "Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Colors")]
    public Color failCylinderColor;


    [Header("Cylinder Values")]
    public float increaseDecreaseStep = 0.04f;

    [Header("Other Values")]
    public float waitWhenPerfectWave = 1f;
}
