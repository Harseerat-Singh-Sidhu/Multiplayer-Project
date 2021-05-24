using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class Piece : NetworkBehaviour
{
   // public Team currentTeam;
    public int currentTile = -1;

    public void Start()
    {
        RegisterEvents();

     //   int teamId = Utility.RetrieveTeamId(OwnerClientId);
     //   GetComponent<MeshRenderer>().material = GameScene.Instance.playerMats[teamId];
       // currentTeam = (Team)teamId;

        if (IsOwner)
            gameObject.AddComponent<BoxCollider>();
    }
    public void OnDestroy()
    {
        UnregisterEvents();
    }

    public void EnableInteraction()
    {
        gameObject.layer = LayerMask.NameToLayer("ActivePiece");
    }
    public void DisableInteraction()
    { 
        gameObject.layer = LayerMask.NameToLayer("Piece");
    }

    [ClientRpc]
    public void PositionClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    // Events
    private void RegisterEvents()
    {

    }
    private void UnregisterEvents()
    {

    }
}
