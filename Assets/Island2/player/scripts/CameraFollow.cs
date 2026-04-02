using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum CameraMode { ThirdPerson, Overview, DragonEncounter }

    // ── inspector ──────────────────────────────────────────────────
    [Header("Target")]
    public Transform target;

    [Header("Mode 1 — Third Person")]
    public Vector3 offset        = new Vector3(0, 3, -5);   // world-space, same as original
    public float   smoothSpeed   = 10f;

    [Header("Mode 2 — Scene Overview")]
    public float overviewHeight  = 30f;
    public float overviewPadding = 5f;

    [Header("Mode 3 — Dragon Encounter")]
    public float dragonSpotRange     = 15f;
    public float encounterHeight     = 8f;
    public float encounterDistance   = 12f;

    [Header("FOV Transitions")]
    public float normalFOV      = 60f;
    public float encounterFOV   = 75f;
    public float overviewFOV    = 90f;
    public float fovSmoothSpeed = 5f;

    // ── private state ──────────────────────────────────────────────
    public CameraMode currentMode = CameraMode.ThirdPerson;

    private Camera       cam;
    private Transform    nearestDragon;
    private GameObject[] dragons = new GameObject[0];

    // ── lifecycle ──────────────────────────────────────────────────
    void Start()
    {
        cam = GetComponent<Camera>();
        RefreshDragonList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchMode(CameraMode.ThirdPerson);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchMode(CameraMode.Overview);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchMode(CameraMode.DragonEncounter);
        if (Input.GetKeyDown(KeyCode.R))      RefreshDragonList();
    }

    void LateUpdate()
    {
        if (target == null) return;

        switch (currentMode)
        {
            case CameraMode.ThirdPerson:     DoThirdPerson();  break;
            case CameraMode.Overview:        DoOverview();     break;
            case CameraMode.DragonEncounter: DoEncounter();    break;
        }
    }

    // ── camera modes ───────────────────────────────────────────────

    // EXACTLY like the original — fixed world-space offset, LookAt player
    void DoThirdPerson()
    {
        // World-space offset — camera does NOT rotate with player, same as original
        Vector3 desired = target.position + offset;

        SmoothMove(desired);
        SmoothLookAt(target.position);
        SmoothFOV(normalFOV);
    }

    // Bird's-eye view framing entire scene
    void DoOverview()
    {
        Vector3 center       = ComputeSceneCenter(out float sceneRadius);
        float   requiredDist = sceneRadius / Mathf.Tan(Mathf.Deg2Rad *
                               (cam ? cam.fieldOfView * 0.5f : 45f));
        Vector3 desired      = center + new Vector3(0,
                               overviewHeight + requiredDist * 0.5f,
                               -requiredDist * 0.5f);

        SmoothMove(desired);
        SmoothLookAt(center);
        SmoothFOV(overviewFOV);
    }

    // Cinematic mid-shot between player and nearest dragon
    void DoEncounter()
    {
        nearestDragon = GetNearestDragon();

        if (nearestDragon != null &&
            Vector3.Distance(target.position, nearestDragon.position) <= dragonSpotRange)
        {
            Vector3 midPoint = (target.position + nearestDragon.position) * 0.5f;
            Vector3 desired  = midPoint + new Vector3(0, encounterHeight, -encounterDistance);

            SmoothMove(desired);
            SmoothLookAt(midPoint);
            SmoothFOV(encounterFOV);
        }
        else
        {
            // No dragon in range — behave exactly like third person
            DoThirdPerson();
        }
    }

    // ── helpers ────────────────────────────────────────────────────

    void SwitchMode(CameraMode mode)
    {
        currentMode = mode;
        if (mode == CameraMode.Overview) RefreshDragonList();
    }

    void SmoothMove(Vector3 destination)
    {
        transform.position = Vector3.Lerp(
            transform.position, destination, smoothSpeed * Time.deltaTime);
    }

    void SmoothLookAt(Vector3 point)
    {
        Quaternion goal    = Quaternion.LookRotation(point - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, goal, smoothSpeed * Time.deltaTime);
    }

    void SmoothFOV(float targetFOV)
    {
        if (cam == null) return;
        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView, targetFOV, fovSmoothSpeed * Time.deltaTime);
    }

    Vector3 ComputeSceneCenter(out float radius)
    {
        Vector3 min = target.position;
        Vector3 max = target.position;

        foreach (GameObject d in dragons)
        {
            if (d == null) continue;
            min = Vector3.Min(min, d.transform.position);
            max = Vector3.Max(max, d.transform.position);
        }

        Vector3 center = (min + max) * 0.5f;
        radius = Vector3.Distance(min, max) * 0.5f + overviewPadding;
        return center;
    }

    Transform GetNearestDragon()
    {
        Transform nearest = null;
        float     minDist = Mathf.Infinity;

        foreach (GameObject dragon in dragons)
        {
            if (dragon == null) continue;
            float dist = Vector3.Distance(target.position, dragon.transform.position);
            if (dist < minDist) { minDist = dist; nearest = dragon.transform; }
        }
        return nearest;
    }

    void RefreshDragonList()
    {
        dragons = GameObject.FindGameObjectsWithTag("Dragon");
    }
}