using System.Collections.Generic;
using UnityEngine;

public class FlipperManager : Singleton<FlipperManager>
{
    [SerializeField] private List<Flipper> Flippers;

    public void RaiseFlippers()
    {
        for (int i = 0; i < Flippers.Count; i++)
        {
            Flippers[i].OnHold();
        }
    }

    public void ReleaseFlippers()
    {
        for (int i = 0; i < Flippers.Count; i++)
        {
            Flippers[i].OnRelease();
        }
    }
}