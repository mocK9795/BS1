using Unity.Netcode;
using UnityEngine;

public class Follower : Infantry
{
    Leader leader;
    bool isFollowing;

    public void Link(Leader newLeader) 
    { 
        leader.follow -= OnCommand;
        newLeader.follow += OnCommand;
        leader = newLeader;
    }

    public void UnLink() { 
        leader.follow -= OnCommand;
        leader = null;
    }

    public void OnCommand(Vector3 position, Infantry enemey)
    {

    }
}
