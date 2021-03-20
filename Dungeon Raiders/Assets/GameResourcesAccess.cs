using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResourcesAccess : MonoBehaviour
{
    public static GameResourcesAccess singleton { get; private set; }

    [SerializeField] GameResources resourceFile;
    public static GameResources ResourceFile => singleton.resourceFile;

    private void Awake()
    {
        if (singleton != null)
            Destroy(singleton.gameObject);

        singleton = this;
    }
}
