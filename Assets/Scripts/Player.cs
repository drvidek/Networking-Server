using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; private set; }
    public string Username { get; private set; }

    [SerializeField] private PlayerMovement _movement;
    public PlayerMovement Movement => _movement;

    public static void Spawn(ushort id, string username)
    {
        foreach (Player otherPlayer in list.Values)
        {
            otherPlayer.SendSpawned(id);
        }

        //Spawn a player prefab
        Player player = Instantiate(GameLogic.GameLogicInstance.PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity).GetComponent<Player>();
        //check if the player has a username, if not, use "guest", and name the player with that and their ID
        player.name = $"Player{id}({ (string.IsNullOrEmpty(username) ? "Guest" : username) })";
        //set the player ID to id
        player.Id = id;
        //set the Username to guest or the input name
        player.Username = string.IsNullOrEmpty(username) ? "Guest" : username;

        //send the spawn confirmation to the client
        player.SendSpawned();

        //add the player to the dictionary
        list.Add(id, player);
    }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    #region Messages
    private void SendSpawned()
    {
        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientID.playerSpawned)));
    }

    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.NetworkManagerInstance.GameServer.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientID.playerSpawned)), toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }

    [MessageHandler((ushort)ClientToServerID.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerID.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.Movement.SetInput(message.GetBools(6), message.GetVector3());
        }
    }

    #endregion

}
