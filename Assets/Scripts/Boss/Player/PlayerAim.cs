using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;

    [Header("Aim Settings")]
    public float aimRange = 3f;          // length of visible aim line
    public float rotationSpeed = 15f;    // smoothing for aim rotation
    public float stickDeadzone = 0.2f;

    [Header("Line Settings")]
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.05f;

    private Camera mainCam;
    private LineRenderer line;

    private Vector2 aimDir = Vector2.right;   // default aiming right
    private bool usingController = false;
    private Vector2 lastStickDir = Vector2.right;

    void Start()
    {
        mainCam = Camera.main;

        // Setup line renderer
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;
    }

    void Update()
    {
        UpdateAimDirection();
        DrawAimLine();
    }

    // ----------------------------------------------------------
    //  AIM DIRECTION CALCULATION
    // ----------------------------------------------------------
    void UpdateAimDirection()
    {
        // --- Read controller input ---
        float x = Input.GetAxis("RightStickHorizontal");
        float y = -Input.GetAxis("RightStickVertical");  // invert Y for controller

        Vector2 stick = new Vector2(x, y);
        bool stickActive = stick.sqrMagnitude > stickDeadzone * stickDeadzone;

        // --- Detect mouse movement ---
        bool mouseMoved =
            Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f ||
            Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f;

        // -------- SWITCH INPUT SOURCE --------
        if (stickActive)
        {
            usingController = true;
            lastStickDir = stick.normalized;
        }
        else if (mouseMoved)
        {
            usingController = false;
        }

        // -------- USE THE RIGHT INPUT SOURCE --------
        if (usingController)
        {
            aimDir = lastStickDir;
        }
        else
        {
            Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
            aimDir = (mouseWorld - transform.position).normalized;
        }

        // -------- ROTATE FIREPOINT --------
        if (firePoint)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            firePoint.rotation = Quaternion.Lerp(firePoint.rotation, rot, Time.deltaTime * rotationSpeed);
        }
    }

    // ----------------------------------------------------------
    //  DRAW AIM LINE
    // ----------------------------------------------------------
    void DrawAimLine()
    {
        if (!firePoint) return;

        Vector3 start = firePoint.position;
        Vector3 end = start + (Vector3)(aimDir.normalized * aimRange);

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    // ----------------------------------------------------------
    //  PUBLIC: Get aim direction for PlayerAttack
    // ----------------------------------------------------------
    public Vector2 GetAimDirection()
    {
        return aimDir;
    }
}
