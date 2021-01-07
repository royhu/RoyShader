/********************************************************
 * Copyright(C) 2019 by RoyApp All rights reserved.
 * FileName:    GameUtils.cs
 * Author:      Roy Hu
 * Version:     1.0
 * Date:        2020-09-04 12:02
 * Description: 
 * Usage:       
 *******************************************************/

namespace RoyUnity
{
    public static class GameUtils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}