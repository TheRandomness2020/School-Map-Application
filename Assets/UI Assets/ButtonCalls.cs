using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ButtonCalls : MonoBehaviour
{
    private GameObject highlightObject;
    private Animation highlightAnimation;
    private Animator highlightAnimator;
    public ExitStartup startUpPanelExit;

    private void Awake()
    {
        InitializeHighlightingObjects();
        //startUpPanelExit = transform.parent.GetComponent<ExitStartup>();
    }
    // This Singular Script Contains public functions for all buttons.
    // Each button will use this script, but likely only call one method 
    // from this script. 

    public void HandleUploadButton()
    {
        // Code for Uploading a File
        // Wait like a second for button transition
        Highlight();
        UploadFile();
    }

    private void UploadFile()
    {
        // Actually functionality to Upload a file
    }

    public void ShouldExitStartup()
    {
        startUpPanelExit.enabled = true;
    }

    public void AboutButton()
    {
        Highlight();
    }

    private void InitializeHighlightingObjects()
    {
        highlightObject = transform.GetChild(0).gameObject;
        highlightAnimator = highlightObject.GetComponent<Animator>();
        highlightAnimator.StopPlayback();
    }
    private void Highlight()
    {
        highlightAnimator.Play("BlueHighlighting");
    }
}
