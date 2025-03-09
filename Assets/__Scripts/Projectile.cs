using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    const int LOOKBACK_COUNT = 10;

    [SerializeField]
    private bool _awake = true;
    public bool awake
    {
        get {return _awake;}
        private set {_awake = value;}
    }

    private Vector3 prevPos;
    // this private list stores history of projectiles move distance
    private List<float> deltas = new List<float>();
    private Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        awake = true;
        prevPos = new Vector3(1000, 1000, 0);
        deltas.Add(1000);
    }

    void FixedUpdate()
    {
        if(rigid.isKinematic || !awake) return;

        Vector3 deltaV3 = transform.position - prevPos;
        deltas.Add(deltaV3.magnitude);
        prevPos = transform.position;

        // limit look back
        while(deltas.Count > LOOKBACK_COUNT)
        {
            deltas.RemoveAt(0);
        }

        // iterate over deltas and find greatest one
        float maxDelta = 0;
        foreach(float f in deltas)
        {
            if(f > maxDelta) maxDelta = f;
        }

        // if the projectile hasn't moved more than the sleepThreshold
        if(maxDelta <= Physics.sleepThreshold)
        {
            awake = false;
            rigid.Sleep();
        }
    }

}
