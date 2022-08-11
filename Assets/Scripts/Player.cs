using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; private set; }
    public string Username { get; private set; }

    public static void Spawn(ushort id, string username)
    {
        //Spawn a player prefab
        Player player = Instantiate(GameLogic.GameLogicInstance.PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity).GetComponent<Player>();
        //check if the player has a username, if not, use "guest", and name the player with that and their ID
        player.name = $"Player{id}({ (string.IsNullOrEmpty(username) ? "Guest" : username) })";
        //set the player ID to id
        player.Id = id;
        //set the Username to guest or the input name
        player.Username = string.IsNullOrEmpty(username) ? "Guest" : username;
        //add the player to the dictionary
        list.Add(id, player);
    }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    [MessageHandler((ushort)ClientToServerID.name)]
    private static void Name(ushort fromCliendID, Message message)
    {
        Spawn(fromCliendID, message.GetString());
    }

}
