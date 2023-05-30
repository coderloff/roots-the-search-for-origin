using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField] Animator Scene;
    // Start is called before the first frame update
    void Start()
    {
        Scene.SetTrigger("Exit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
