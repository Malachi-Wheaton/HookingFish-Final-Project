using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CamRecoil : MonoBehaviour
{
    public float rotationspeed;
    public float retunrSpeed;

    public Vector3 recoilrotation = new Vector3(2f, 2f, 2f);

   private Vector3 currentRotation;
   private Vector3 rot;
    private void Update()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, rotationspeed*Time.deltaTime); ;
        rot = Vector3.Slerp(rot, currentRotation, rotationspeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }
    public void Fire()
    {
        currentRotation += new Vector3(-recoilrotation.x, Random.Range(-recoilrotation.y, recoilrotation.y), Random.Range(-recoilrotation.z, recoilrotation.z));

    }
}
