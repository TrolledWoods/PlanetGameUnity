using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    Stone,
    Wood,
    Iron,

    ITEM_COUNT
}

[RequireComponent(typeof(SpriteRenderer))]
public class ItemEntity : EntityBehaviour {

    public Item Type;
    public int Count;

    public override void EntityStart()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        renderer.sprite = ResourceManager.singleton.GetItemSprite(Type);
    }

}
