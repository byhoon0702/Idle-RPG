using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public void AssignTarget(Transform target)
    {
        virtualCamera.Follow = target;
    }

    void Update()
    {
        
    }
}
