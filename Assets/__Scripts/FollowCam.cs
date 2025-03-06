using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static public GameObject POI; // the static point of interest

    [Header("Dynamic")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero; // Vector2.zero is [0,0]

    [Header("Dynamic")]
    public float camZ; // desired Z pos of the camera

    void Awake()
    {
        camZ = this.transform.position.z;
    }

    void FixedUpdate()
    {
        //if (POI == null) return; // if no POI, restart
        // get the position of the POI
        //Vector3 destination = POI.transform.position;
        // -----------------------

        Vector3 destination = Vector3.zero;

        if (POI != null)
        {
            // if the POI has a rigidbody, check to see if its sleeping
            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if ((poiRigid != null) && poiRigid.IsSleeping())
            {
                POI = null;;
            }
        }

        if (POI != null)
        {
            destination = POI.transform.position;
        }



        // limit the minimum values of destination.x and destination.y
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);
        // interpolate from the cur camera pos towards destination
        destination = Vector3.Lerp(transform.position, destination, easing);
        // force destination.z to be camZto keep the camera far enough away
        destination.z = camZ;
        // set the camera to the destination
        transform.position = destination;
        // set the orthographicSize of the Camera to keep ground in view
        Camera.main.orthographicSize = destination.y + 10;
    }
}
