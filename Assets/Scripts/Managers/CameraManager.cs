using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBuilder
{
    public class CameraManager : MonoBehaviour
    {
        public int minZoom = 4;
        public int maxZoom = 24;
        private GameObject selectedTile;
        private bool deleteMode;
        private Vector3 lastMousePos;

        void LateUpdate()
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
            if (Input.GetMouseButton(0))
            {
                // Translate mouse location to world location.
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.x = Mathf.Round(mousePos.x / 1.0f) * 1.0f;
                mousePos.y = Mathf.Round(mousePos.y / 1.0f) * 1.0f;
                mousePos.z = 1.0f;
                if (mousePos != lastMousePos)
                {
                    OnMouseDown(mousePos);
                    lastMousePos = mousePos;
                }
            }

            // Have we right clicked?
            if (Input.GetMouseButtonDown(1))
            {
                selectedTile = null;
                deleteMode = false;
            }
        }

        /**
         * Called when the mouse is down in the given frame, and we have moved.
         */
        internal void OnMouseDown(Vector3 mousePos)
        {
            // Are we building?
            if (selectedTile != null)
            {
                String layerName = selectedTile.GetComponent<SpriteRenderer>().sortingLayerName;

                // Check there is not a grounditem here first. If there is we must remove it prior to building.
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.tag == "Unbuildable" || hit.collider.gameObject.GetComponent<SpriteRenderer>().sortingLayerName == layerName)
                    {
                        return;
                    }
                }

                // Instantiate an object here.
                GameObject obj = GameManager.instance.AddObjectAt(selectedTile, mousePos);
                obj.tag = "Owned";

                return;
            }

            // Are we deleting?
            if (deleteMode)
            {
                GameManager.instance.RemoveObjectAt(Camera.main.ScreenToWorldPoint(Input.mousePosition), "Owned");
            }
        }

        /**
         * Enter delete mode.
         */
        internal void EnterDeleteMode()
        {
            selectedTile = null;
            deleteMode = true;
        }

        /**
         * Enter build mode.
         */
        internal void EnterBuildMode(GameObject tile)
        {
            selectedTile = tile;
            deleteMode = false;
        }

        /**
         * Set the position of the camera.
         */
        internal void SetPosition(Vector2 vec)
        {
            transform.position = new Vector3(vec.x, vec.y, transform.position.z);
        }
    }
}