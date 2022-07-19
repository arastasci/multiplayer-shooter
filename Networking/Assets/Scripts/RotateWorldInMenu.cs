using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWorldInMenu : MonoBehaviour
{
    public static RotateWorldInMenu instance;
    [SerializeField] float rotationSpeed = 5f;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    private void OnDestroy()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
