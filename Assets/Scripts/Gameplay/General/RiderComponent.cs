using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RiderComponent : MonoBehaviour
{
    [SerializeField] RiderData riderData;
    [SerializeField] bool isShowcase;
    public List<GameObject> models { get; private set; }
    Animator animator;
    
    public RiderData RiderData { get { return riderData; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        models = new List<GameObject>();
        Transform geometryRoot = transform.GetChild(0);
        foreach (Transform model in geometryRoot)
            models.Add(model.gameObject);
    }
    private void OnEnable()
    {
        if(isShowcase)
            animator.SetBool("isIKControlled", false);
    }
}
