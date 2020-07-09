// Decompiled with JetBrains decompiler
// Type: EnumUtils
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E20EE8E7-4E23-4A85-927C-65F4005A4EC2
// Assembly location: D:\Games\Cuphead\Cuphead_Data\Managed\Assembly-CSharp.dll

using System;

public static class EnumUtils
{
    public static T[] GetValues<T>()
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enum type");
        return (T[])Enum.GetValues(typeof(T));
    }

    public static string[] GetValuesAsStrings<T>()
    {
        T[] values = EnumUtils.GetValues<T>();
        string[] strArray = new string[values.Length];
        for (int index = 0; index < values.Length; ++index)
            strArray[index] = values[index].ToString();
        return strArray;
    }

    public static int GetCount<T>()
    {
        return EnumUtils.GetValues<T>().Length;
    }

    public static T Random<T>()
    {
        T[] values = EnumUtils.GetValues<T>();
        return values[UnityEngine.Random.Range(0, values.Length)];
    }

    public static T Parse<T>(string name)
    {
        T[] values = EnumUtils.GetValues<T>();
        for (int index = 0; index < values.Length; ++index)
        {
            if (name == values[index].ToString())
                return values[index];
        }
        return values[0];
    }

    public static bool TryParse<T>(string name, out T result)
    {
        T[] values = EnumUtils.GetValues<T>();
        for (int index = 0; index < values.Length; ++index)
        {
            if (name == values[index].ToString())
            {
                result = values[index];
                return true;
            }
        }
        result = values[0];
        return false;
    }
}
