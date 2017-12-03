using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using RPG.Characters; //So we can detect by type

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour //TODO rename Cursor
    {
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        const int POTENTIALLY_WALKABLE_LAYER = 8;
        float maxRaycastDepth = 100f; // Hard coded value

        Rect currentScreenRect;

        public delegate void OnMouseOverTerrain(Vector3 destination);
        public event OnMouseOverTerrain OnMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy(Enemy  enemy);
        public event OnMouseOverEnemy OnMouseOverEnemyHit;

        void Update()
        {
            currentScreenRect = new Rect(0, 0, Screen.width, Screen.height);

            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Implement UI Interaction
            } else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            if(currentScreenRect.Contains(Input.mousePosition))
            {
                //Specify layer priorities, order matters

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (RaycastForEnemy(ray))
                {
                    return;
                }

                if (RaycastForPotentiallyWalkable(ray))
                {
                    return;
                }
            }
        }

        bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
            var gameObjectHit = hitInfo.collider.gameObject;
            var enemyHit = gameObjectHit.GetComponent<Enemy>();
            
            if (enemyHit)
            {
                print(enemyHit.gameObject);
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                OnMouseOverEnemyHit(enemyHit);
                return true;
            }
            return false;
        }

        private bool RaycastForPotentiallyWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                OnMouseOverPotentiallyWalkable(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}