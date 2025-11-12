using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class PlayerAim : MonoBehaviour
{
    [Header("Aim Settings")]
    public Transform firePoint;          // optional: where projectiles spawn
    public float aimRange = 3f;          // visible line length
    public float rotationSpeed = 10f;
    public float inputThreshold = 0.1f;

    [Header("Line Renderer Settings")]
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.05f;

    private Camera mainCam;
    private LineRenderer line;
    private Vector2 aimDir = Vector2.right; // default facing right
    private bool usingController = false;

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
        UpdateLineRenderer();
    }

    void UpdateAimDirection()
    {
        // --- Mouse aiming ---
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir = (mouseWorld - transform.position);
        mouseDir.Normalize();

        // --- Right stick aiming (if available) ---
        float aimX = Input.GetAxis("RightStickHorizontal");
        float aimY = Input.GetAxis("RightStickVertical");
        Vector2 stickDir = new Vector2(aimX, aimY);

        if (stickDir.magnitude > inputThreshold)
        {
            aimDir = stickDir.normalized;
            usingController = true;
        }
        else
        {
            aimDir = mouseDir;
            usingController = false;
        }

        // --- Rotate object or firePoint ---
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);
        if (firePoint)
            firePoint.rotation = Quaternion.Lerp(firePoint.rotation, targetRot, Time.deltaTime * rotationSpeed);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    void UpdateLineRenderer()
    {
        // start at firePoint or player
        Vector3 startPos = firePoint ? firePoint.position : transform.position;
        Vector3 endPos = startPos + (Vector3)(aimDir * aimRange);

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }
}
