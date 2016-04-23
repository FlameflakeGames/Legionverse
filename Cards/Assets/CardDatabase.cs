using UnityEngine;
using System.Collections;

public class CardDatabase : MonoBehaviour
{
	public CardType[] cards;
	public static CardDatabase instance;

	void Start()
	{
		CardDatabase.instance=this;
	}

	[RPC] void SpawnDeck(NetworkViewID viewID, int id)
	{
		GameObject go = NetworkView.Find(viewID).gameObject;
		DeckScript ds = go.GetComponent<DeckScript>();
		ds.playerID=id;
		TurnManager.instance.playerDecks[id]=ds;
	}

	[RPC] void SpawnHero(NetworkViewID viewID, int id)
	{
		GameObject go = NetworkView.Find(viewID).gameObject;
		go.GetComponent<Card>().playerID=id;
	}
}