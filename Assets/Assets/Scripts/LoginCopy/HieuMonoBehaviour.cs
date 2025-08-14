using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HieuMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        this.LoadComponents();
    }
    protected virtual void Reset()
    {
        this.LoadComponents();
        this.Resetvalue();
    }
    protected virtual void Resetvalue()
    {

    }
    protected virtual void LoadComponents()
    {

    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {

    }
}
