using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_ScriptableObject", menuName = "MyScriptable/Player_ScriptableObject", order = 0)]
public class Player_ScriptableObject : ScriptableObject 
{
    public int speed;
    public int GunNum;
    public GameObject GunPrefab;
    public int GunSpeed;
    public int GunBounceNum;
}

