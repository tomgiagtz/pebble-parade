using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionObject : MonoBehaviour
{
    [SerializeField]
    private string _name;
    public string Name => _name;
    private string _description;
    public string Description => _description;
    // Start is called before the first frame update

    void Start()
    {
        if (_name == null || _name == ""){
            AssignName();
            Debug.Log("DESCRIPTION OBJECT WARNING: " + _name + " is assigned DescriptionObject.cs but does not have the attribute \"title.\" The Game Object name is assigned");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos(){

    }

    public void AssignName(){
        _name = gameObject.name;
    }
}
