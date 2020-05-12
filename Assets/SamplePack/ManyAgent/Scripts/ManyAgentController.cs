using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ManyAgentController : Bolt.EntityEventListener<IManyAgentState>
{
    public Vector3 WP;
    public NavMeshAgent NMA;
    NavMeshHit NMH = new NavMeshHit();

    public override void Attached()
    {
        if (BoltNetwork.IsServer)
        {

            NMA = GetComponent<NavMeshAgent>();
            Navi();

        }
        //else GetComponent<NavMeshAgent>().enabled = false;

        state.SetTransforms(state.transform, transform);
        WP = transform.position;
    }


    void Update()
    {
        if (entity.IsOwner == false)
        {
            transform.GetChild(0).position = Vector3.Lerp(WP, transform.position, 0.1f);
            WP = transform.GetChild(0).position;
        }
    }
    public void Navi()
    {


        bool a = false;

        while (a == false)
        {

            Vector2 c = Random.insideUnitCircle;
            Vector3 d = new Vector3(c.x, 0, c.y);
            Vector3 b = d * 30f + transform.position;



            NavMesh.SamplePosition(b, out NMH, 10f, NavMesh.AllAreas);

            if (NMH.position.x != Mathf.Infinity)
                a = true;
        }

        NMA.SetDestination(NMH.position);
        Invoke("Navi", 5f);
    }

    // Use this for initialization
    void Start()
    {

    }

}
