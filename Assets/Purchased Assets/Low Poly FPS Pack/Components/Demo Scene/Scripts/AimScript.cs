using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AimScript : MonoBehaviour
{

    float mouseX;
    float mouseY;
    Quaternion rotationSpeed;

    [Header("Gun Options")]
    //How fast the gun moves on the x and y
    //axis when aiming
    public float aimSpeed = 6.5f;
    //How fast the gun moves to the new position
    public float moveSpeed = 15.0f;

    [Header("Gun Positions")]
    //Default gun position
    public Vector3 defaultPosition;
    //Aim down the sight position
    public Vector3 zoomPosition;

    [Header("Camera")]
    //Main gun camera
    public Camera gunCamera;

    [Header("Camera Options")]
    //How fast the camera field of view changes
    public float fovSpeed = 15.0f;
    //Camera FOV when zoomed in
    public float zoomFov = 30.0f;
    //Default camera FOV
    public float defaultFov = 60.0f;

    [Header("Audio")]
    public AudioSource mainAudioSource;
    public AudioClip aimSound;
    //Used to check if the audio has played
    bool soundHasPlayed = false;


    private Quaternion currentRotation;
    public bool isSnipper;
    public GameObject zoomTexture;
    void Start()
    {
        //Hide the cursor at start
        Cursor.visible = true;
    }

    void Update()
    {
        #if !MOBILE_INPUT 
            CheckZoom();
        #else 

        #endif
        //When right click is held down
        

        //Rotate the gun based on the mouse input
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        //Rotate the gun on the x and y axis
        rotationSpeed = Quaternion.Euler(-mouseY, mouseX, 0);
        transform.localRotation = Quaternion.Slerp
            (transform.localRotation, rotationSpeed, aimSpeed * Time.deltaTime);
        // Debug.Log(transform.rotation);
    }

    void CheckZoom() {
        if (Input.GetButton("Fire2")) {
            GameObject.FindWithTag("Player").transform.GetChild(2).GetChild(8).gameObject.SetActive(false);
            Zoom();
        }
        else {
            GameObject.FindWithTag("Player").transform.GetChild(2).GetChild(8).gameObject.SetActive(true);
            DisableZoom();
        }
    }
    void CheckMobileZoom(){
        DisableZoom();
    }
    public void Zoom()
    {
        if (isSnipper) {
            zoomTexture.SetActive(true);
        }
        //Move the gun to the zoom position
        transform.localPosition = Vector3.Lerp(transform.localPosition,
                                                zoomPosition, Time.deltaTime * moveSpeed);
        //Change the camera field of view
        gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
                                            zoomFov, fovSpeed * Time.deltaTime);
        //If the aim sound has not played, play it
        if (!soundHasPlayed)
        {
            mainAudioSource.clip = aimSound;
            mainAudioSource.Play();
            //The sound has played
            soundHasPlayed = true;
        }
    }
    void DisableZoom()
    {
         //When right click is released
            //Move the gun back to the default position
            transform.localPosition = Vector3.Lerp(transform.localPosition,
                                                   defaultPosition, Time.deltaTime * moveSpeed);
            //Change back the camera field of view
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView,
                                               defaultFov, fovSpeed * Time.deltaTime);

            soundHasPlayed = false;
            if (isSnipper) {
                zoomTexture.SetActive(false);
            }
    }
}