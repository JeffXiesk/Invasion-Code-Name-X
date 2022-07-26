using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner networkRunner = null;

    [SerializeField]
    private NetworkPrefabRef playerPrefabBlue;

    [SerializeField]
    private NetworkPrefabRef TurretBlue;

    [SerializeField]
    private NetworkPrefabRef TurretRed;

    [SerializeField]
    private NetworkPrefabRef CrystalBlue;

    [SerializeField]
    private NetworkPrefabRef CrystalRed;

    [SerializeField]
    private NetworkPrefabRef MinionBlue;

    [SerializeField]
    private NetworkPrefabRef MinionRed;


    [SerializeField]
    private NetworkPrefabRef playerPrefabRed;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();

    private void Start()
    {
        StartGame(GameMode.AutoHostOrClient);
    }
    async void StartGame(GameMode mode)
    {
        networkRunner.ProvideInput = true;

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2
        });
        GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(false);
    }

    public void OnConnectedToServer(NetworkRunner runner){
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){ }

    public void OnDisconnectedFromServer(NetworkRunner runner){}

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}

    public void OnInput(NetworkRunner runner, NetworkInput input){
        var data = new NetworkInputData();
        data.mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        data.buttons.Set(InputButtons.W,Input.GetKey(KeyCode.W));
        data.buttons.Set(InputButtons.A, Input.GetKey(KeyCode.A));
        data.buttons.Set(InputButtons.S, Input.GetKey(KeyCode.S));
        data.buttons.Set(InputButtons.D, Input.GetKey(KeyCode.D));
        data.buttons.Set(InputButtons.Mouse0, Input.GetKey(KeyCode.Mouse0));
        data.buttons.Set(InputButtons.B,Input.GetKey(KeyCode.B));
        data.buttons.Set(InputButtons.M, Input.GetKey(KeyCode.M));
        data.buttons.Set(InputButtons.Q, Input.GetKey(KeyCode.Q));
        data.buttons.Set(InputButtons.Esc, Input.GetKey(KeyCode.Escape));
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition = new Vector3(0,0,0);
        if (player.PlayerId == 1) {
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefabBlue, new Vector3(0,0,0), Quaternion.identity, player);
            NetworkObject networkPlayerObject1 = runner.Spawn(TurretBlue, new Vector3(0.58f, -1.6f, 0f), Quaternion.identity);
            NetworkObject networkPlayerObject2 = runner.Spawn(TurretBlue, new Vector3(-11.44f, 2.36f, 0f), Quaternion.identity);
            NetworkObject networkPlayerObject3 = runner.Spawn(CrystalBlue, new Vector3(-20f, -0.4899f, 0f), Quaternion.identity);
            playerList.Add(player, networkPlayerObject);
        }
        else
        {
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefabRed, new Vector3(15,0,0), Quaternion.identity, player);
            NetworkObject networkPlayerObject1 = runner.Spawn(TurretRed, new Vector3(14.45f, 2.36f, 0f), Quaternion.identity);
            NetworkObject networkPlayerObject2 = runner.Spawn(TurretRed, new Vector3(27.5f, -1.6f, 0f), Quaternion.identity);
            NetworkObject networkPlayerObject3 = runner.Spawn(CrystalRed, new Vector3(38.95f, -0.4899f, 0f), Quaternion.identity);
            playerList.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerList.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data){}

    public void OnSceneLoadDone(NetworkRunner runner){}

    public void OnSceneLoadStart(NetworkRunner runner){}

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
}
