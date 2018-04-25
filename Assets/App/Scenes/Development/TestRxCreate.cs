using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class TestRxCreate : MonoBehaviour
{
    private BoolReactiveProperty test;
    // Use this for initialization
    void Start()
    {
//        var state = CustomInt.State.Default;
//
//        var customObservable = Observable.Create<CustomInt>(observer =>
//        {
//            observer.OnNext(new CustomInt() {Value = 1});
//            observer.OnError(new Exception("Things wrong"));
//            observer.OnCompleted();
//
//            return Disposable.CreateWithState(state, customState => { print("State: " + customState.ToString()); });
//        });
//
//
//        var subscribler = customObservable.Subscribe(
//                value => { print("Value: " + value); },
//                e =>
//                {
//                    Debug.LogError("Error: " + e.Message);
//                    state = CustomInt.State.Error;
//                },
//                () => { print("Custom observable completed "); })
//            .AddTo(gameObject);


        test.Do(_=>{print("Do Enabled"); }).Subscribe(_ => { print("Sub Enabled"); }).AddTo(this);
//        this.OnDisableAsObservable().Subscribe(_ => { print("Disabled"); }).AddTo(this);
//        this.UpdateAsObservable().Subscribe(_ => { print("Updating"); }).AddTo(this);
    }


    public class CustomInt
    {
        public int Value { get; set; }

        public enum State
        {
            Default,
            Error
        }
    }
}