using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float smoothStep = 0.25f;
    public Vector3 offset;
    private Vector3 offsetHandler;
    private Vector3 targetPosition;

    void Start()
    {

    }


    void Update()
    {
        Vector3 newPosition = targetPosition + offset;
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothStep);
    }

    public void ShowAllTower()
    {
        offsetHandler = offset;
        offset = new Vector3(offset.x, offset.y, offset.z - 2f);
    }

    public void SetDefaultValues()
    {
        offset = offsetHandler;
    }

    public void SetNewTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
