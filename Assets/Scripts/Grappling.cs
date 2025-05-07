using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovementAdvanced pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance = 40f;
    public float grappleDelayTime = 0.1f;
    public float overshootYAxis = 5f;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd = 2f;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMovementAdvanced>();
        if (pm == null)
            Debug.LogError("Grappling: PlayerMovementAdvanced reference not found.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            TryStartGrapple();
        }

        if (grapplingCdTimer > 0f)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void TryStartGrapple()
    {
        if (grapplingCdTimer > 0f || grappling)
        {
            Debug.Log("Grappling: On cooldown or already grappling.");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            grappling = true;
            pm.freeze = true;
            Debug.Log("Grapple started. Will execute in delay.");
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            Debug.Log("No grappleable surface hit.");
            // Optional: play error sound or animation
        }
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;
        Debug.Log("Executing Grapple. Launching to point.");

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;

        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;
        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f); // Adjust timing based on jump arc duration
    }

    public void StopGrapple()
    {
        Debug.Log("Grapple finished. Resetting state.");
        pm.freeze = false;
        grappling = false;
        grapplingCdTimer = grapplingCd;

        // Optionally reset LineRenderer
        // lr.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
