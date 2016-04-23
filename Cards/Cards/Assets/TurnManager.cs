using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
	public static int currentTurn = -1;
	public static NetworkView networkView;
	public static TurnManager instance;
	public int[] rawManaSlots,rawMana;
	public DeckScript[] playerDecks;
	public CreatureScript[] heroObjects;
	public bool[] beginTurns;

	[RPC] void BeginTurn(int player)
	{
		beginTurns[player]=true;
	}

	[RPC] void DrawCard(int player)
	{
		this.playerDecks[player].DrawCard();
	}
	
	[RPC] void AddMana(int type, int amount, int player)
	{
		if(type==0)
		{
			rawManaSlots[player]+=amount;
			if(rawManaSlots[player]>10)
				rawManaSlots[player]=10;
		}
	}

	[RPC] void UseMana(int type, int amount, int player)
	{
		if(type==0)
		{
			rawMana[player]-=amount;
		}
	}
	
	[RPC] void RefreshMana(int type, int player)
	{
		if(type==0)
		{
			rawMana[player]=rawManaSlots[player];
		}
	}

	void Start()
	{
		TurnManager.networkView=this.GetComponent<NetworkView>();
		TurnManager.instance=this;
	}

	[RPC] void SetTurn(int t)
	{
		TurnManager.currentTurn=t;
	}
}