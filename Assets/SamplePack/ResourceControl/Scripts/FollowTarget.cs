using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    
    public Transform target;
 
    private void FixedUpdate()
    {
        transform.localPosition = target.localPosition;
        transform.localRotation = target.localRotation;
    }
}
