using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Loop : Block
{
    public int repeatTimes;
    public List<Block> blocks;

    public Loop(string n = "Loop", int r = 0,List<Block> b = null) : base(n)
    {
        repeatTimes = r;
        blocks = b;
    }
}