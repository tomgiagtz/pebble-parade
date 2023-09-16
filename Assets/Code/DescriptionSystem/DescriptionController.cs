using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class DescriptionController : MonoBehaviour
{
    public List<DescriptionObject> descriptionObjects = new List<DescriptionObject>();
    public InputAction playerControls;
    public float descriptionRadius = 50f;

    private void OnEnable(){
        playerControls.Enable();

    }

    private void OnDisable(){
        playerControls.Disable();
    }
    void Awake(){
        // var actions = new DefaultInputActions();
        //  //moveAction = actions.FindActionMap("Gameplay").FindAction("DescriptionOverlay");
        // actions.FindActionMap("Gameplay").FindAction("DescriptionOverlay").performed += OnDescriptionOverlay;
        // actions.FindActionMap("Gameplay").FindAction("DescriptionStart").performed += OnDescriptionStart;
        // actions.FindActionMap("Gameplay").FindAction("DescriptionStop").performed += OnDescriptionStop;
    }

    void Start(){

    }

    public void OnDescriptionOverlay(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Time.timeScale = 0.05f;
            //activate high contrast
            descriptionObjects.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, descriptionRadius);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                GameObject obj = hitColliders[i].gameObject;
                if((obj.GetComponent("DescriptionObject") as DescriptionObject) != null){
                    descriptionObjects.Add(obj.GetComponent<DescriptionObject>());
                    //assign location relative to player
                    Debug.Log(descriptionObjects[descriptionObjects.Count - 1].name);
                }
            }
        }
        else if (context.canceled)
        {
            Time.timeScale = 1;
            //deactivate high contrast
            //Debug.Log("DescriptionOverlayDeactivated");
        }
    }
    public void OnDescriptionStart(){

    }
    public void OnDescriptionStop(){

    }

    public void OnDrawGizmosSelected(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, descriptionRadius);
    }
    
}
