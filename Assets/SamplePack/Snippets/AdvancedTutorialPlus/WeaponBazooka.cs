#if false

using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
    public class WeaponBazooka : WeaponBase
    {

        [SerializeField]
        public GameObject shellFakePrefab;

        [SerializeField]
        public GameObject shellEntityPrefab;

        [SerializeField]
        public float shellForce = 100;

        public override void OnOwner(PlayerCommand cmd, BoltEntity entity)
        {
            if (entity.IsOwner)
            {
                IPlayerState state = entity.GetState<IPlayerState>();
                PlayerController controller = entity.GetComponent<PlayerController>();

                Vector3 pos;
                Quaternion look;

                // this calculate the looking angle for this specific entity
                PlayerCamera.instance.CalculateCameraAimTransform(entity.transform, state.pitch, out pos, out look);

                // display debug
                Debug.DrawRay(pos, look * Vector3.forward);

                //check lag compensation to see if we should have hit someone at the frame fired

                bool gotHit = false;
                using (var hits = BoltNetwork.OverlapSphereAll(pos, 10f, cmd.ServerFrame))
                {
                    for (int i = 0; i < hits.count; ++i)
                    {
                        var hit = hits.GetHit(i);
                        var serializer = hit.body.GetComponent<PlayerController>();

                        if ((serializer != null) && (serializer.state.team != state.team))
                        {
                            serializer.ApplyDamage(controller.activeWeapon.damagePerBullet);
                            gotHit = true;
                        }
                    }

                }

                //otherwise fire bazooka

                if (!gotHit)
                {
                    // calculate where we are aiming
                    Vector3 aimPos;
                    Quaternion aimRot;
                    PlayerCamera.instance.CalculateCameraAimTransform(entity.transform, entity.GetState<IPlayerState>().pitch, out aimPos, out aimRot);

                    // extrapolate that 1024 units forward
                    Vector3 aimPosDistant = aimPos + (aimRot * Vector3.forward * 1024f);

                    // then get the direction from the muzzle to the distant aim point
                    Vector3 fireDirection = (aimPosDistant - muzzleFlash.position).normalized;

                    // create shell
                    BoltEntity shellGo = BoltNetwork.Instantiate(shellEntityPrefab, muzzleFlash.position + transform.forward * 4, Quaternion.LookRotation(fireDirection));

                    // launch it
                    shellGo.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, shellForce), ForceMode.VelocityChange);
                }


            }
        }


        public override void Fx(BoltEntity entity)
        {


        }
    }
}
#endif