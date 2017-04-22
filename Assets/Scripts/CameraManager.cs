using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int minZoom = 4;
    public int maxZoom = 24;
    private GameObject selectedTile;
    private bool deleteMode;

    void Update()
    {
        // Check for movement.
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        transform.position = transform.position + new Vector3(x, y, 0.0f);

        // Check for scrolling.
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera cam = GetComponent<Camera>();
            if (cam.orthographicSize + 1 < maxZoom)
            {
                cam.orthographicSize += 1;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera cam = GetComponent<Camera>();
            if (cam.orthographicSize - 1 > minZoom)
            {
                cam.orthographicSize -= 1;
            }
        }


        // Have we clicked the world?
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedTile != null)
            {
                // Check there is not a grounditem here first. If there is we must remove it prior to building.
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.tag == "Unbuildable")
                    {
                        return;
                    }
                }

                // Round up position.
                Vector3 objectPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                objectPos.x = Mathf.Round(objectPos.x / 1.0f) * 1.0f;
                objectPos.y = Mathf.Round(objectPos.y / 1.0f) * 1.0f;
                objectPos.z = 1.0f;

                // Instantiate an object here.
                GameObject obj = Instantiate(selectedTile, objectPos, Quaternion.identity);
                obj.tag = "Owned";

                return;
            }

            if (deleteMode)
            {
                // Find an object here, if we do, delete it.
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null && hit.collider.gameObject.tag == "Owned")
                    {
                        Destroy(hit.collider.gameObject);
                        break;
                    }
                }
            }
        }

        // Have we right clicked?
        if (Input.GetMouseButtonDown(1))
        {
            selectedTile = null;
            deleteMode = false;
        }
    }

    internal void EnterDeleteMode()
    {
        selectedTile = null;
        deleteMode = true;
    }

    internal void EnterBuildMode(GameObject tile)
    {
        selectedTile = tile;
        deleteMode = false;
    }

    internal void SetPosition(Vector2 vec)
    {
        transform.position = new Vector3(vec.x, vec.y, transform.position.z);
    }
}
