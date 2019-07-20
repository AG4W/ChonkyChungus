using UnityEngine;

using System;

public class JsonHelper
{
    public static T[] GetJsonArray<T>(string data)
    {
        return JsonUtility.FromJson<Wrapper<T>>("{ \"array\": " + data + "}").array;
    }
    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;

        public override string ToString()
        {
            string s = "";

            for (int i = 0; i < array.Length; i++)
                s += array[i].ToString();

            return s;
        }
    }
}
