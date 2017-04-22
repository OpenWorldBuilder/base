using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        CameraManager cameraMan = Camera.main.GetComponent<CameraManager>();
        cameraMan.EnterDeleteMode();
    }
}
