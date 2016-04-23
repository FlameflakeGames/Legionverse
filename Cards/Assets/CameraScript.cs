using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour
{
	public Transform[] pos;
	public int playerID;
	public bool spawnedDeck = false;
	public Transform[] deckPos,heroPos;
	public GameObject deckPrefab;
	public Texture2D[] emptyCrystals,fullCrystals;
	public bool drawnCardThisTurn;
	public CreatureScript selectedCreature;
	public List<CreatureScript> crets = new List<CreatureScript>();
	public GameObject heroPrefab;
	public int maxPlayers = 2;
	public Vector3[] angles;
	public int watchedPlayer = 0;
	public GameObject heroObj;

	void OnGUI()
	{
		for(int i = 0; i<TurnManager.instance.rawManaSlots[playerID]; i++)
		{
			GUI.color=new Color(1.0f, 1.0f, 1.0f, 0.25f);
			GUI.DrawTexture(new Rect(16, 16+(64*i), 64, 64), emptyCrystals[0]);
			GUI.color=Color.white;
		}
		for(int i = 0; i<TurnManager.instance.rawMana[playerID]; i++)
		{
			GUI.DrawTexture(new Rect(16, 16+(64*i), 64, 64), fullCrystals[0]);
		}
		for(int i = 0; i<maxPlayers; i++)
		{
			if(GUI.Button(new Rect(Screen.width-128, 16+(32*i), 96, 24), "Watch: #"+i))
			{
				watchedPlayer=i;
			}
		}
		if(TurnManager.currentTurn==this.playerID)
		{
			if(GUI.Button(new Rect(Screen.width-128, Screen.height/2-16, 96, 32), "End Turn"))
			{
				int newTurn = playerID+1;
				newTurn%=maxPlayers;
				TurnManager.networkView.RPC("SetTurn", RPCMode.All, newTurn);
				TurnManager.networkView.RPC("AddMana", RPCMode.All, 0, 1, newTurn);
				TurnManager.networkView.RPC("RefreshMana", RPCMode.All, 0, newTurn);
				TurnManager.networkView.RPC("DrawCard", RPCMode.All, newTurn);
				drawnCardThisTurn=false;
				foreach(CreatureScript cret in crets)
				{
					if(cret)
					{
						cret.attacked=false;
					}
				}
			}
		}
		if(TurnManager.currentTurn==-1)
		{
			if(GUI.Button(new Rect(Screen.width-128, Screen.height/2-16, 96, 32), "Start Game"))
			{
				int newTurn = 0;
				TurnManager.networkView.RPC("SetTurn", RPCMode.All, newTurn);
				TurnManager.networkView.RPC("AddMana", RPCMode.All, 0, 1, newTurn);
				TurnManager.networkView.RPC("RefreshMana", RPCMode.All, 0, newTurn);
				for(int player = 0; player<TurnManager.instance.rawMana.Length; player++)
				{
					for(int drawn = 0; drawn<5; drawn++)
					{
						TurnManager.networkView.RPC("DrawCard", RPCMode.All, player);
					}
				}
				drawnCardThisTurn=false;
			}
		}
		//if()
	}

	void FixedUpdate()
	{
		transform.position=pos[watchedPlayer].position;
		transform.rotation=pos[watchedPlayer].rotation;
		if(Network.isClient || Network.isServer)
		{
			if(!spawnedDeck)
			{
				playerID=Random.Range(0, maxPlayers);
				while(TurnManager.instance.playerDecks[playerID])
					playerID=Random.Range(0, maxPlayers);
				GameObject go = (GameObject)Network.Instantiate(deckPrefab, deckPos[playerID].position, deckPos[playerID].rotation, 16);
				NetworkView deck = go.GetComponent<NetworkView>();
				CardDatabase.instance.GetComponent<NetworkView>().RPC("SpawnDeck", RPCMode.AllBuffered, deck.viewID, playerID);
				GameObject go2 = (GameObject)Network.Instantiate(heroPrefab, heroPos[playerID].position, heroPos[playerID].rotation, 8);
				CardDatabase.instance.GetComponent<NetworkView>().RPC("SpawnHero", RPCMode.AllBuffered, go2.GetComponent<NetworkView>().viewID, playerID);
				spawnedDeck=true;
				watchedPlayer=playerID;
			}
			if(TurnManager.currentTurn==this.playerID)
			{
				Ray r = this.GetComponent<Camera>().ViewportPointToRay(new Vector3(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height));
				RaycastHit rh;
				if(Physics.Raycast(r, out rh) && Input.GetButton("Fire1"))
				{
					//print("THING!");
					if(rh.collider.tag=="Card")
					{
						Card c = rh.collider.GetComponent<Card>();
						if((TurnManager.instance.rawMana[playerID]>=c.neutralCost || c.rb.isKinematic) && drawnCardThisTurn==false)
						{
							//if(c.rb.isKinematic)
								//drawnCardThisTurn=true;
							c.rb.isKinematic=false;
							//print("THING TWO!");
							Ray r2 = this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
							RaycastHit rh2;
							Physics.Raycast(r2, out rh2);
							Vector3 vec = rh2.point;
							vec.y=2f;
							rh.transform.position = vec;
							rh.rigidbody.velocity=Vector3.zero;
							Quaternion q = Quaternion.identity;
							q.eulerAngles = angles[playerID];
							rh.transform.rotation = q;
						}
					}
					if(rh.collider.tag=="Creature")
					{
						CreatureScript cs = rh.collider.GetComponent<CreatureScript>();
						if(cs.playerID==this.playerID)
						{
							if(!crets.Contains(cs))
								crets.Add(cs);
							selectedCreature=cs;
						}
						else
						{
							if(selectedCreature && selectedCreature.attacked==false)
							{
								selectedCreature.attacked=true;
								selectedCreature.networkView.RPC("AttackOther", RPCMode.All, cs.networkView.viewID);

							}
						}
					}
				}
				/*if(Physics.Raycast(r, out rh) && Input.GetButtonUp("Fire2"))
				{
					Collider[] cols = Physics.OverlapSphere(rh.point, 2.5f);
					foreach(Collider c in cols)
					{
						if(c.attachedRigidbody)
						{
							c.attachedRigidbody.AddForce(Vector3.up*500 + (new Vector3(Random.Range(-25, 25), Random.Range(-25, 25), Random.Range(-25, 25))));
							c.attachedRigidbody.AddTorque(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
						}
					}
				}*/
			}
		}
	}
}