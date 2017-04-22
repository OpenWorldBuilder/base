using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject selectedTile;

    void Update ()
    {
        // Check for movement.
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        transform.position = transform.position + new Vector3(x, y, 0.0f);

        // Check for scrolling.
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera cam = GetComponent<Camera>();
            if (cam.orthographicSize + 1 < 24)
            {
                cam.orthographicSize += 1;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera cam = GetComponent<Camera>();
            if (cam.orthographicSize - 1 > 4)
            {
                cam.orthographicSize -= 1;
            }
        }


        // Have we clicked the world?
        if (selectedTile != null && Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.mousePosition);
            Vector3 objectPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            objectPos.x = Mathf.Round(objectPos.x / 1.0f) * 1.0f;
            objectPos.y = Mathf.Round(objectPos.y / 1.0f) * 1.0f;
            objectPos.z = 1.0f;
            Instantiate(selectedTile, objectPos, Quaternion.identity);
        }
    }

    internal void setPosition(Vector2 vec)
    {
        transform.position = new Vector3(vec.x, vec.y, transform.position.z);
    }
}
