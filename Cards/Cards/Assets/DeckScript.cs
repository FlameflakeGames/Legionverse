using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckScript : MonoBehaviour
{
	public GameObject cardPrefab;
	public int[] cards;
	public bool spawnedCards;
	public int playerID;
	public int cardsLeft = 59;

	void Start()
	{
		int[] newCards = new int[cards.Length];
		List<int> usedNums = new List<int>();
		for(int i = 0; i<cards.Length; i++)
		{
			int randInt = Random.Range(0, cards.Length);
			while(usedNums.Contains(randInt))
			{
				randInt = Random.Range(0, cards.Length);
			}
			newCards[randInt]=cards[i];
			usedNums.Add(randInt);
		}
		cards=newCards;
		cardsLeft=cards.Length-1;
	}

	public void DrawCard()
	{
		if(this.GetComponent<NetworkView>().isMine)
		{
			GameObject go = (GameObject)Network.Instantiate(cardPrefab, transform.position+(Vector3.up*0.007812499f*cardsLeft), transform.rotation, 8);
			this.GetComponent<NetworkView>().RPC("SpawnCard", RPCMode.AllBuffered, cardsLeft, go.GetComponent<NetworkView>().viewID);
			cardsLeft-=1;
			transform.position+=Vector3.down*0.007812499f;
		}
	}

	/*void Update()
	{
		if(GetComponent<NetworkView>().isMine && !spawnedCards)
		{
			spawnedCards=true;
			for(int i = 0; i<cards.Length; i++)
			{

			}
		}
	}*/

	[RPC] void SpawnCard(int i, NetworkViewID viewID)
	{
		GameObject go = NetworkView.Find(viewID).gameObject;
		//if(networkView.isMine)
		{
			Card c = go.GetComponent<Card>();
			print(i);
			print(cards[i]);
			c.name=CardDatabase.instance.cards[cards[i]].name;
			c.type=CardDatabase.instance.cards[cards[i]].type;
			c.cardTex=CardDatabase.instance.cards[cards[i]].cardPicture;
			string desc = CardDatabase.instance.cards[cards[i]].descrip.Replace("\\n", "\n");
			c.description=desc;
			string flav = CardDatabase.instance.cards[cards[i]].flavor.Replace("\\n", "\n");
			c.flavor=flav;
			c.statA=CardDatabase.instance.cards[cards[i]].statA;
			c.statB=CardDatabase.instance.cards[cards[i]].statB;
			c.neutralCost=CardDatabase.instance.cards[cards[i]].neutralCost;
			c.redCost=CardDatabase.instance.cards[cards[i]].redCost;
			c.yellowCost=CardDatabase.instance.cards[cards[i]].yellowCost;
			c.greenCost=CardDatabase.instance.cards[cards[i]].greenCost;
			c.blueCost=CardDatabase.instance.cards[cards[i]].blueCost;
			c.whiteCost=CardDatabase.instance.cards[cards[i]].whiteCost;
			c.blackCost=CardDatabase.instance.cards[cards[i]].blackCost;
			c.cardType=CardDatabase.instance.cards[cards[i]].ct;
			c.playerID=this.playerID;
			c.cardId=cards[i];
			c.rb.isKinematic=true;
			c.boolA=CardDatabase.instance.cards[cards[i]].boolA;
			c.boolB=CardDatabase.instance.cards[cards[i]].boolB;
		}
	}
}

[System.Serializable]
public class CardType
{
	public string name,type,descrip,flavor;
	public Texture2D cardPicture;
	public int statA,statB,redCost,greenCost,blueCost,yellowCost,whiteCost,blackCost,neutralCost;
	public Card.CardType ct;
	public bool boolA,boolB;
	public AudioClip soundA,soundB,soundC;
	public enum CardAbilities {Airborne=0,Ranged=1}
	public List<CardAbilities> abilities;
}