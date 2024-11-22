using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Folder : MonoBehaviour
{
    public static GameObject folder;

    private void Awake()
    {
        folder = gameObject;
    }
}
