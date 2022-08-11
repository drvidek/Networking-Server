using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

public enum ClientToServerID
{
    name = 1
}

public class NetworkManager : MonoBehaviour
{
    /*
    We want to make sure there can be only ONE instance of our network manager
    We are creating a private static instance of our NetworkManager and a public static Property to control it
    */
    private static NetworkManager _networkmanagerInstance;
    public static NetworkManager NetworkManagerInstance
    {
        //Property Read is the instance, public by default
        get => _networkmanagerInstance;
        //private means only this instance of the class can access set
        private set
        {
            //set the instance to the value if the instance is null
            if (_networkmanagerInstance == null)
            {
                _networkmanagerInstance = value;
            }
            //if it is not null, check if the value is stored as the static instance
            else if (_networkmanagerInstance != value)
            {
                //if not, throw a warning and destroy that instance

                //$ is to identify the string as containing an interpolated value
                Debug.LogWarning($"{nameof(NetworkManager)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    public Server GameServer { get; private set; }

    [SerializeField] private ushort s_port;
    [SerializeField] private ushort s_maxClientCount;

    private void Awake()
    {
        //when the object that this is attached to in game initialises, try to set the instance to this
        NetworkManagerInstance = this;
    }

    private void Start()
    {
        //Logs what the network is doing
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        //Create new server 
        GameServer = new Server();
        //starts server at port XXXX with X amount of clients
        GameServer.Start(s_port, s_maxClientCount);
        //when a client leaves the server, run the PlayerLeft function
        GameServer.ClientDisconnected += PlayerLeft;
    }

    //Check the server activity at set intervals
    private void FixedUpdate()
    {
        GameServer.Tick();
    }

    //When the game closes, it kills the connection to the server
    private void OnApplicationQuit()
    {
        GameServer.Stop();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //when a player leaves the server, Destroy the player object and remove from list
        Destroy(Player.list[e.Id].gameObject);
    }

}
