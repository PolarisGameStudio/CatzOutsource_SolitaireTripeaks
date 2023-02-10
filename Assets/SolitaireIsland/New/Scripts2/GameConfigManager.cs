using com.F4A.MobileThird;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigManager : SingletonMonoDontDestroy<GameConfigManager>
{
    public Sprite[] MapSprites;

    public GameObject[] MapDetails;

    protected override void Initialization()
    {
    }
}
