using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	private const string typeName = "FlameflakeShipSim";
	private string gameName = "Testing server";
	private string directIP = "cliffracers.thruhere.net";
	public int port = 25569;
	public int maxPlayers = 5;
	private HostData[] hostList;
	public GameObject playerPrefab;
	public GameObject[] playerObjects;

	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived)
		{
			hostList = MasterServer.PollHostList();
		}
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        /*for(int i = 0; i<maxPlayers; i++)
		{
			if(playerObjects[i].networkView.owner==player)
			{
				playerObjects[i].networkView.RPC("DestroyPlayer", RPCMode.All);
			}
		}*/
    }

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void OnConnectedToServer()
	{
		//SpawnPlayer();
		Debug.Log("Clickity click, connected.");
	}

	private void StartServer()
	{
		Network.InitializeServer(maxPlayers, port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server is UP!  Sweet!");
		//SpawnPlayer();
	}

	private void SpawnPlayer()
	{
		Network.Instantiate(playerPrefab, transform.position, Quaternion.identity, 1337);
	}

	void OnGUI()
	{
		if(!Network.isClient && !Network.isServer)
		{
			if(GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
			{
				StartServer();
			}

			if(GUI.Button(new Rect(100, 250, 250, 100), "Refresh hosts"))
			{
				RefreshHostList();
			}

			if(GUI.Button(new Rect(750, 250, 250, 100), "Connect to IP specified"))
			{
				Network.Connect(directIP, port);
			}

			gameName = GUI.TextField(new Rect(100, 400, 200, 20), gameName, 30);
			port = int.Parse(GUI.TextField(new Rect(100, 450, 200, 20), port.ToString(), 30));
			maxPlayers = int.Parse(GUI.TextField(new Rect(100, 500, 200, 20), maxPlayers.ToString(), 30));
			directIP = GUI.TextField(new Rect(100, 550, 200, 20), directIP, 30);

			if(hostList != null)
			{
				for(int i=0; i<hostList.Length; i++)
				{
					if(GUI.Button (new Rect(400, 100+(110*i), 300, 100), hostList[i].gameName))
					{
						JoinServer(hostList[i]);
					}
				}
			}
		}
	}
}
