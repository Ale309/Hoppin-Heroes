using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollower : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;

    private Transform tr;

    private void Start()
    {
        tr = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
            tr.position = new Vector2(mainCamera.position.x * Time.deltaTime, mainCamera.position.y);
    }

}
