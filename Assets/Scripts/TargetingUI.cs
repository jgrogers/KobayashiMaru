using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.UIElements;
public class TargetingUI : MonoBehaviour
{
    RectTransform rectTransform;
    Canvas mCanvas;
    [SerializeField] private Color[] teamColorAssignments;
    private List<UILineRenderer> lineRenderers = new List<UILineRenderer>();
    private int rendererToUse = 0;
    private Vector3 initialRT = Vector3.zero;
    private float initialWidth = 0;
    private float initialHeight = 0;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialRT = rectTransform.rect.position;
        initialWidth = rectTransform.rect.width;
        initialHeight = rectTransform.rect.height;
        mCanvas = GetComponent<Canvas>();
    }
    private void DrawEdgeArrow(Vector2 target_pos, int team) { 
        if (lineRenderers.Count == rendererToUse)
        {// Add another renderer
            GameObject lrGO = new GameObject("UILineRenderer");
            lrGO.transform.parent = this.transform;
            lineRenderers.Add(lrGO.AddComponent<UILineRenderer>());
        }
        if (lineRenderers.Count > rendererToUse)
        {
            // Find the right on screen position near the edge
            Vector2 off_screen_direction = (target_pos - new Vector2(Screen.width / 2, Screen.height / 2)).normalized;
            Vector2 on_screen_direction = off_screen_direction* Mathf.Min(Screen.width/2, Screen.height/2);
            Vector2 on_screen_endpt = on_screen_direction + new Vector2(Screen.width/2, Screen.height/2);
            Vector2 off_screen_perp = new Vector2(off_screen_direction.y, -off_screen_direction.x);
            
            Vector2[] points = new Vector2[4];
            points[0] = on_screen_endpt;
            points[1] = on_screen_endpt - 10.0f * off_screen_direction + 8.0f * off_screen_perp;
            points[2] = on_screen_endpt - 10.0f * off_screen_direction - 8.0f * off_screen_perp;
            points[3] = on_screen_endpt;
            lineRenderers[rendererToUse].Points = points;
            lineRenderers[rendererToUse].LineThickness = 1f;
            lineRenderers[rendererToUse].color = teamColorAssignments[team];
            lineRenderers[rendererToUse].enabled = true;
            rendererToUse++;
        } else
        {
            Debug.LogError("cant find my renderer I just created...");
        }

    }
    private void DrawHealthbar(Vector2 center, float width, float prop)
    {
        float endpt1 = 0f;
        float ht = 20f;
        if (lineRenderers.Count == rendererToUse)
        {// Add another renderer
            GameObject lrGO = new GameObject("UILineRenderer");
            lrGO.transform.parent = this.transform;
            lineRenderers.Add(lrGO.AddComponent<UILineRenderer>());
        }
        if (lineRenderers.Count > rendererToUse)
        {
            Vector2[] points = new Vector2[2];
            points[0] = center + new Vector2(-width / 2, ht);
            points[1] = points[0] + new Vector2(width * prop, 0f);
            endpt1 = -width / 2 + width * prop;
            lineRenderers[rendererToUse].Points = points;
            lineRenderers[rendererToUse].color = Color.green;
            lineRenderers[rendererToUse].LineThickness = 6f;
            lineRenderers[rendererToUse].enabled = true;
            rendererToUse++;
        }
        if (lineRenderers.Count == rendererToUse)
        {// Add another renderer
            GameObject lrGO = new GameObject("UILineRenderer");
            lrGO.transform.parent = this.transform;
            lineRenderers.Add(lrGO.AddComponent<UILineRenderer>());
        }
        if (lineRenderers.Count > rendererToUse)
        {
            Vector2[] points = new Vector2[2];
            points[0] = center + new Vector2(endpt1, ht);
            points[1] = center + new Vector2(width/2, ht);
            lineRenderers[rendererToUse].Points = points;
            lineRenderers[rendererToUse].color = Color.red;
            lineRenderers[rendererToUse].LineThickness = 6f;
            lineRenderers[rendererToUse].enabled = true;
            rendererToUse++;
        }
    }
    private void DrawReticle(Vector2 center, float width, float height, int team) { 
        if (lineRenderers.Count == rendererToUse)
        {// Add another renderer
            GameObject lrGO = new GameObject("UILineRenderer");
            lrGO.transform.parent = this.transform;
            lineRenderers.Add(lrGO.AddComponent<UILineRenderer>());
        }
        if (lineRenderers.Count > rendererToUse)
        {
            Vector2[] points = new Vector2[5];
            points[0] = center + new Vector2(-width / 2, -height / 2);
            points[1] = center + new Vector2(width / 2, -height / 2);
            points[2] = center + new Vector2(width / 2, height / 2);
            points[3] = center + new Vector2(-width / 2, height / 2);
            points[4] = center + new Vector2(-width / 2, -height / 2);
            lineRenderers[rendererToUse].Points = points;
            lineRenderers[rendererToUse].color = teamColorAssignments[team];
            lineRenderers[rendererToUse].LineThickness = 1f;
            lineRenderers[rendererToUse].enabled = true;
            rendererToUse++;
        } else
        {
            Debug.LogError("cant find my renderer I just created...");
        }

    }
    private void ClearLineRenderers()
    {
        foreach (var lr in lineRenderers) {
            lr.enabled = false;
        }
    }
    public static Vector2 GetScreenPositionFromTransform(Camera pCamera, Vector3 pTransformPosition)
    {
        var worldToScreenPoint = pCamera.WorldToScreenPoint(pTransformPosition);
        return new Vector2(worldToScreenPoint.x, Screen.height - worldToScreenPoint.y);
    }
    // Update is called once per frame
    void Update()
    {
        Dictionary<int, List<GameObject>> ships = GameManager.Instance.GetAllShips();
        rendererToUse = 0;
        ClearLineRenderers();
        foreach (int team in ships.Keys)
        {
            foreach (GameObject go in ships[team])
            {
                Vector3 target_position = go.transform.position;
                Vector3 screen_coordinates = Camera.main.WorldToScreenPoint(target_position);
/*                float refWidth = 800f; // reference resolution - width - set in Canvas Scaler 
                float refHeight = 600; // reference resolution - height - set in Canvas Scaler
                bool matchWidth = false; //true if screen match mode is set to match the width, 
                                        //false if is set to match the height
                if (matchWidth)
                {
                    screen_coordinates.x *= refWidth / Screen.width;
                    screen_coordinates.y *= refWidth / Screen.width;
                }
                else
                {
                    screen_coordinates.x *= refHeight / Screen.height;
                    screen_coordinates.y *= refHeight / Screen.height;
                }*/
                Vector3 image_coordinates = screen_coordinates;
                if (screen_coordinates.z <= 0) continue;
                if (image_coordinates.x < 0 || image_coordinates.x >= Screen.width || image_coordinates.y < 0 || image_coordinates.y >= Screen.height)
                {// Off screen, draw an arrow of appropriate color to point to it just inside the screen
                    DrawEdgeArrow(image_coordinates, team);
                } else
                {
                    // Draw a reticle with the right color around this ship
                    DrawReticle(image_coordinates, 30f, 30f, team);
                    float healthProp = go.GetComponent<ShipDamage>().hitPoints / go.GetComponent<ShipDamage>().maxHitPoints;
                    DrawHealthbar(image_coordinates, 30f, healthProp);
                }
            }
        }
    }
}
