using UnityEngine;

public class WallMovement : MonoBehaviour
{
    public enum MoveMode
    {
        XAxis,
        YAxis,
        ZAxis,
        DiagonalXY,
        DiagonalXZ,
        DiagonalYZ
    }

    [Header("Movement")]
    public MoveMode moveMode = MoveMode.XAxis;
    public float distance = 3f;
    public float speed = 2f;
    public bool useLocalSpace = false;

    public bool randomStartOffset = false;

    private Vector3 startPosition;
    private float phaseOffset;

    private void Start()
    {
        startPosition = useLocalSpace ? transform.localPosition : transform.position;
        phaseOffset = randomStartOffset ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    private void Update()
    {
        Vector3 direction = GetDirection(moveMode).normalized;

        // Smooth back-and-forth motion in range [-distance, distance].
        float offset = Mathf.Sin(Time.time * speed + phaseOffset) * distance;
        Vector3 nextPosition = startPosition + direction * offset;

        if (useLocalSpace)
            transform.localPosition = nextPosition;
        else
            transform.position = nextPosition;
    }

    private static Vector3 GetDirection(MoveMode mode)
    {
        switch (mode)
        {
            case MoveMode.XAxis:
                return Vector3.right;
            case MoveMode.YAxis:
                return Vector3.up;
            case MoveMode.ZAxis:
                return Vector3.forward;
            case MoveMode.DiagonalXY:
                return new Vector3(1f, 1f, 0f);
            case MoveMode.DiagonalXZ:
                return new Vector3(1f, 0f, 1f);
            case MoveMode.DiagonalYZ:
                return new Vector3(0f, 1f, 1f);
            default:
                return Vector3.right;
        }
    }
}
