using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    /*
   We want to make sure there can be only ONE instance of our network manager
   We are creating a private static instance of our NetworkManager and a public static Property to control it
   */
    private static GameLogic _gameLogicInstance;
    public static GameLogic GameLogicInstance
    {
        //Property Read is the instance, public by default
        get => _gameLogicInstance;
        //private means only this instance of the class can access set
        private set
        {
            //set the instance to the value if the instance is null
            if (_gameLogicInstance == null)
            {
                _gameLogicInstance = value;
            }
            //if it is not null, check if the value is stored as the static instance
            else if (_gameLogicInstance != value)
            {
                //if not, throw a warning and destroy that instance

                //$ is to identify the string as containing an interpolated value
                Debug.LogWarning($"{nameof(GameLogic)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    [SerializeField] private GameObject _playerPrefab;
    public GameObject PlayerPrefab => _playerPrefab;

    private void Awake()
    {
        GameLogicInstance = this;
    }

}
