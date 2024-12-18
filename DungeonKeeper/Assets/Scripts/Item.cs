using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items")]
public class Item : ScriptableObject
{
    public int itemID;
    public Sprite itemImg;
    public ItemType type;
    public int value;
    public int itemAmt;

    public bool stackable = true;
}

public enum ItemType
{
    Material,
    Tool,
    Weapon,
    Healing
}

public static class ScriptableObjectExtension
{
    /// <summary>
    /// Creates and returns a clone of any given scriptable object.
    /// </summary>
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = UnityEngine.Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }
}