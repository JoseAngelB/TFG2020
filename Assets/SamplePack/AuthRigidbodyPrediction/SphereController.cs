using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public GameObject mirrorSelf;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        mirrorSelf.transform.localPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
