using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SideScrolling1 : MonoBehaviour
{   
    
    private Transform player;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        
    }
    // Start is called before the first frame update
    private void LateUpdate()
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
        
        
        
        
        transform.position = cameraPosition;
    }

}
