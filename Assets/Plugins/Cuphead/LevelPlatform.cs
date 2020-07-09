using System.Collections.Generic;
using UnityEngine;

public class LevelPlatform : AbstractCollidableObject
{
    //public bool canFallThrough = true;

    protected List<Transform> players
    {
        get;
        private set;
    }

    public Vector3 CurrentSpeed { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        this.players = new List<Transform>();
        base.gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
    }

    protected override void OnDisable()
    {
        foreach (Transform player in this.players)
        {
            if (player != null)
            {
                player.SetParent(null);
                player.gameObject.SetActive(true);
            }
        }
    }

    public virtual void AddChild(Transform player)
    {
        if (!this.players.Contains(player))
        {
            this.players.Add(player);
        }
        player.parent = base.transform;
    }

    public virtual void OnPlayerExit(Transform player)
    {
        if (this.players.Contains(player))
        {
            this.players.Remove(player);
        }
    }

    protected override void OnDestroy()
    {
        foreach (Transform player in this.players)
        {
            if (player != null)
            {
                player.parent = null;
            }
        }
        base.OnDestroy();
    }
}
