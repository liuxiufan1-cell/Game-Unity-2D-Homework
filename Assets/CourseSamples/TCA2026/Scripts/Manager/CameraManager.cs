using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{
    public GameObject _startCamera;
    public GameObject _playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startCamera.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerCamera != null)
        {
            var playerCameraActive = _playerCamera.activeSelf;
            if (playerCameraActive == false)
            {
                _startCamera.SetActive(true);
            }
            else
            {
                _startCamera.SetActive(false);
            }
        }
        else
        {
            _startCamera.SetActive(true);
        }

    }

    public void GetPlayerCameraObject(GameObject _camera )
    {
        _playerCamera = _camera;
        if (_playerCamera == null)
        {
            _startCamera.SetActive(true);
        }
    }
}
