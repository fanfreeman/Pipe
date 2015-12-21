using UnityEngine;
using System.Collections;

public class Opponent : Player
{
    public Player player;

    protected override void Update()
    {
        base.Update();

        float progressDiff = 0;
        if (currentPipe.id == player.currentPipe.id) // in the same pipe as the player
        {
            progressDiff = progress - player.progress;
        }
        else if (currentPipe.id > player.currentPipe.id) // in the pipe in front of the pipe the player is in
        {

        }
        else if (currentPipe.id < player.currentPipe.id) // in the pipe behind the pipe the player is in
        {

        }
        avatar.GetComponent<Rigidbody>().AddForce(-centerTrackPointDirection * progressDiff * 5f, ForceMode.Acceleration);
    }
}
