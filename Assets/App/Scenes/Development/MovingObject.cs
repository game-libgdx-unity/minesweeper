using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        this.OnBecameVisibleAsObservable().Subscribe(_ => print("Visible")).AddTo(gameObject);
        this.OnBecameInvisibleAsObservable().Subscribe(_ => print("Invisible")).AddTo(gameObject);
    }
}