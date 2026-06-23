using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string sceneName;
    public string benchID;

    public bool hasShield;
    public bool hasGroundSlam;

    public List<string> collectedPickups = new List<string>();
}