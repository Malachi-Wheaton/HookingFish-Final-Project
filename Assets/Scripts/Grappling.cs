using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip;
    public LayerMask grappleableMask;

    private PlayerMovement pm;

    [Header("Grappling Settings")]
    public float maxGrappleDistance = 40f;
    public float grappleDelay = 0.1f;
    public float overshootY = 5f;
    public float cooldown = 2f;

    private Vector3 grapplePoint;
    private bool isGrappling;
    private float cooldownTimer;

    public KeyCode grappleKey = KeyCode.Mouse1;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        if (pm == null)
            Debug.LogError("Grappling: PlayerMovement reference is missing!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey) && cooldownTimer <= 0 && !isGrappling)
        {
            StartGrapple();
        }

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    private void StartGrapple()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxGrappleDistance, grappleableMask))
        {
            Debug.Log("Grapple hit: " + hit.collider.name);
            grapplePoint = hit.point;
            isGrappling = true;
            pm.freeze = true;

            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            Debug.Log("Grapple failed: no target");
        }
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float relativeY = grapplePoint.y - lowestPoint.y;
        float arcHeight = relativeY < 0 ? overshootY : relativeY + overshootY;

        Debug.Log($"Launching to {grapplePoint}, arc height = {arcHeight}");
        pm.JumpToPosition(grapplePoint, arcHeight);

        Invoke(nameof(StopGrapple), 1f);
    }

    private void StopGrapple()
    {
        isGrappling = false;
        pm.freeze = false;
        cooldownTimer = cooldown;
        Debug.Log("Grapple ended.");
    }

    public bool IsGrappling() => isGrappling;
    public Vector3 GetGrapplePoint() => grapplePoint;
}


