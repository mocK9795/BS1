using Unity.Netcode;
using UnityEngine;

public class Follower : Attacker
{
    Leader leader;
    bool isFollowing;

    public void Link(Leader newLeader) 
    { 
        leader.follow -= OnCommand;
        newLeader.follow += OnCommand;
    }

    public void UnLink() { 
        leader.follow -= OnCommand; 
    }

    public void OnCommand(Vector3 position, Attacker enemey)
    {

    }
}
