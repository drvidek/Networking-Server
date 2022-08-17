using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _camProxy;
    [SerializeField] private float _gravity;
    [SerializeField] private float _playerSpd, _jumpHeight;

    private float _grvAccel, _moveSpd, _jumpSpd;

    private bool[] _inputs;
    private float _yVelocity;

    void OnValidate()
    {
        if (_controller == null)
            _controller = GetComponent<CharacterController>();
        if (_player == null)
            _player = GetComponent<Player>();
        Inititialise();
    }

    private void Start()
    {
        _inputs = new bool[6];
        Inititialise();
    }

    private void FixedUpdate()
    {
        Vector2 inputDir = Vector2.zero;
        if (_inputs[0])
            inputDir.y += 1;
        if (_inputs[1])
            inputDir.y -= 1;
        if (_inputs[2])
            inputDir.x -= 1;
        if (_inputs[3])
            inputDir.x += 1;

        Move(inputDir, _inputs[4], _inputs[5]);

    }

    private void Inititialise()
    {
        _grvAccel = _gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        _moveSpd = _playerSpd * Time.fixedDeltaTime;
        _jumpSpd = Mathf.Sqrt(_jumpHeight * -2f * _grvAccel);
    }

    private void Move(Vector2 inputDir, bool jump, bool sprint)
    {
        Vector3 moveDir = Vector3.Normalize(_camProxy.right * inputDir.x + Vector3.Normalize(FlattenVector3(_camProxy.forward)) * inputDir.y);
        moveDir *= _moveSpd;

        if (sprint)
        {
            moveDir *= 2f;
        }

        if (_controller.isGrounded)
        {
            _yVelocity = 0f;
            if (jump)
                _yVelocity = _jumpSpd;
        }
        _yVelocity += _grvAccel;

        moveDir.y = _yVelocity;

        _controller.Move(moveDir);

        SendMovement();
    }

    private Vector3 FlattenVector3(Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }

    private void SendMovement()
    {
        if (NetworkManager.NetworkManagerInstance.CurrentTick % 2 != 0)
            return;

        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientID.playerMovement);
        message.AddUShort(_player.Id);
        message.AddUShort(NetworkManager.NetworkManagerInstance.CurrentTick);
        message.AddVector3(transform.position);
        message.AddVector3(_camProxy.forward);
        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(message);
    }

    public void SetInput(bool[] inputs, Vector3 forward)
    {
        this._inputs = inputs;
        _camProxy.forward = forward;
    }
}
