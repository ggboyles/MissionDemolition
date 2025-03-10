using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // fields set in Inspector pane
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public Transform leftArm;
    public Transform rightArm;
    public AudioClip snapSound;
    private AudioSource audioSource;

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private LineRenderer lineRenderer;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, leftArm.position);
        lineRenderer.SetPosition(1, launchPos);
        lineRenderer.SetPosition(2, rightArm.position);

        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        // player has pressed mouse down while over slingshot
        aimingMode = true;

        // instantiate a projectile
        projectile = Instantiate(projectilePrefab) as GameObject;

        // start it at the launchPoint
        projectile.transform.position = launchPos;

        // set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        lineRenderer.positionCount = 3;
        UpdateLineRenderer();
    }

    void Update()
    {
        // if slingshot is not in aiming mode, dont run this code
        if (!aimingMode) return;

        // gets current mouse pos in 2d screen coords
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        // limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        // move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        lineRenderer.positionCount = 3;
        UpdateLineRenderer();

        if (Input.GetMouseButtonUp(0))
        {
            // mouse has been released
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -mouseDelta * velocityMult;

            // switch to slingshot view immediately before setting POI
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile; // set the _MainCamera POI
            Instantiate<GameObject>(projLinePrefab, projectile.transform); // adds projectileLine
            projectile = null;
            MissionDemolition.SHOT_FIRED();

            // plays snapping audio
            audioSource.PlayOneShot(snapSound);

            // reset rubber band when projectile is launched
            lineRenderer.positionCount = 2;
            UpdateLineRenderer();
        }
    }

    void UpdateLineRenderer()
    {
        if (projectile != null)
        {
            lineRenderer.positionCount = 3; 
            lineRenderer.SetPosition(0, leftArm.position);
            lineRenderer.SetPosition(1, projectile.transform.position);
            lineRenderer.SetPosition(2, rightArm.position);
        }
        else
        {
            lineRenderer.positionCount = 2; 
            lineRenderer.SetPosition(0, leftArm.position);
            lineRenderer.SetPosition(1, rightArm.position);
        }
    }
}
