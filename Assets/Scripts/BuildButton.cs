using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour {
    public Button yourButton;
    public GameObject tile;

    // Use this for initialization
    void Start ()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        CameraManager cameraMan = Camera.main.GetComponent<CameraManager>();
        cameraMan.selectedTile = tile;
    }
}
