using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampManager : MonoBehaviour
{
    
     [SerializeField] Animator anim;
   
    
    
    void Start()
    {
        //anim = GetComponent<Animator>();
        anim.SetBool("lightOn", false);     
    }

   


    public void LightOn()
    {
        anim.SetBool("lightOn", true);
    }


}
