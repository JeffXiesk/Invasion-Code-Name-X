using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum InputButtons
{
    W,
    A,
    S,
    D,
    Mouse0,
    B,
    M,
    Q,
    Esc
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector3 mousePosition;
}
