using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSLocationScript : MonoBehaviour
{
        public Text GPSStatus;
    public Text latitudeValue;
    public Text longitudevalue;
    public Text altitudeValue;
    public Text horizontalAccuracyValue;
    public Text timeStampValue;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPSLoc());
    }

    // Update is called once per frame
    IEnumerator GPSLoc()
    {
        //if (UNITY_EDITOR)
        
        // No permission handling needed in Editor
        //else if (UNITY_ANDROID)
        //{
            //Check if user has location service enabled
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation)) {
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
            }

            // First, check if user has location service enabled
            if (!UnityEngine.Input.location.isEnabledByUser) {
                // TODO Failure
                Debug.LogFormat("Android and Location not enabled");
                yield break;
            }
        //}

        // wait till service starts
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service Didnt Start in 20 seconds
        if (maxWait < 1)
        {
            GPSStatus.text = "Did Not Start";
            yield break;
        }

        // Connection Failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            GPSStatus.text = "Cannot determine device location";
            yield break;
        }
        else
        {
            //Access granted
            GPSStatus.text = "Running";
            InvokeRepeating("UpdateGPSData",0.5f,1f);
        }
    }

    private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            //running 
            GPSStatus.text = "Running";
            latitudeValue.text = Input.location.lastData.latitude.ToString();
            longitudevalue.text = Input.location.lastData.longitude.ToString();
            altitudeValue.text = Input.location.lastData.altitude.ToString();
            horizontalAccuracyValue.text = Input.location.lastData.horizontalAccuracy.ToString();
            timeStampValue.text = Input.location.lastData.timestamp.ToString();
        }
        else
        {
            //Stoped
            GPSStatus.text = "Stoped";
        }
    }
}
