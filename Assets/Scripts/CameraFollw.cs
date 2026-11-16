using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollw : MonoBehaviour
{ 
    public Transform Player;
   public Vector3 offSet;
   public float cameraSpeed=3;
    public float min;
    public float max;
    void LateUpdate()
    {
    

           Vector3 Target=Player.position+offSet;
        Target.x = Mathf.Clamp(Target.x, min, max);
         transform.position=Vector3.Lerp(transform.position,Target,cameraSpeed*Time.deltaTime);
          
    }
}
