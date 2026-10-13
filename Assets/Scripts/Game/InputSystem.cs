using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public bool isDragging = false;

    private void Update()
    {
        //ClickInput();
    }

    public void ClickInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GridSystem.Instance.spawnSand.SpawnSandBlock(mousePos);
        }
    }

    
}
