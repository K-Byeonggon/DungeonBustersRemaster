using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkRoomManager : NetworkRoomManager
{
    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);

        
    }

    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {


        base.OnRoomServerDisconnect(conn);
    }
}
