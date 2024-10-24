using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{ 
    // Please modify the script such that the compassNeedle
    // will always point in the direction the user needs to go
    // (rotate the rectTransform)
    public RectTransform compassNeedle;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        compassNeedle.transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y,Input.gyro.attitude.z);
        
    }

    
}
