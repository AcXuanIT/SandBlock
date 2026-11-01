using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    public float liftOffset = 1.5f;

    private Camera cam;
    private Item target;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Begin(mouse);
        }
        if (target != null && Input.GetMouseButton(0))
        {
            Drag(mouse);
        }    
        if (Input.GetMouseButtonUp(0))
        {
            End();
        }
    }

    private void Begin(Vector2 mouse)
    {
        if (GameManager.Instance.isUpColor) return;

        Collider2D hit = Physics2D.OverlapPoint(mouse);
        if (hit != null && hit.TryGetComponent(out Item item))
        {
            target = item;
            target.BeginDrag();
            Drag(mouse);
        }
    }

    private void Drag(Vector2 mouse)
    {
        GridSystem grid = GridSystem.Instance;
        Vector2 half = target.LocalHalfSize;

        Vector2 center = mouse + Vector2.up * liftOffset;
        center.x = Mathf.Clamp(center.x, -grid.GridWidth + half.x, grid.GridWidth - half.x);
        center.y = Mathf.Min(center.y, grid.GridLength - half.y);

        target.transform.position = center - target.LocalCenter;
    }

    private void End()
    {
        if (target == null) return;

        Item item = target;
        target = null;

        if (item.TryPlace())
            ItemSystem.Instance.OnItemPlaced(item);
    }
}
