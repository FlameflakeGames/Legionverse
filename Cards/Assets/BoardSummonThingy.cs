using UnityEngine;
using System.Collections;

public class BoardSummonThingy : MonoBehaviour
{
	public int playerID;
	public GameObject creaturePrefab,heroPrefab;

	void OnTriggerEnter(Collider other)
	{
		if(other.tag=="Card" && this.GetComponent<Collider>().tag=="Summon")
		{
			Card c = other.GetComponent<Card>();
			if(c.cardType==Card.CardType.Creature && c.playerID==playerID && c.GetComponent<NetworkView>().isMine)
			{
				GameObject go = (GameObject)Network.Instantiate(creaturePrefab, c.transform.position, c.transform.rotation, 12);
				this.GetComponent<NetworkView>().RPC("SummonCreature", RPCMode.AllBuffered, go.GetComponent<NetworkView>().viewID, c.cardId, c.GetComponent<NetworkView>().viewID, false, c.playerID);
				TurnManager.networkView.RPC("UseMana", RPCMode.All, 0, c.neutralCost, playerID);
			}
			if(c.cardType==Card.CardType.Hero && c.playerID==playerID && c.GetComponent<NetworkView>().isMine)
			{
				GameObject go = (GameObject)Network.Instantiate(heroPrefab, c.transform.position, c.transform.rotation, 12);
				this.GetComponent<NetworkView>().RPC("SummonCreature", RPCMode.AllBuffered, go.GetComponent<NetworkView>().viewID, c.cardId, c.GetComponent<NetworkView>().viewID, true, c.playerID);
				//TurnManager.networkView.RPC("UseMana", RPCMode.All, 0, c.neutralCost, playerID);
			}
		}
	}

	[RPC] void SummonCreature(NetworkViewID viewID, int cretID, NetworkViewID vid2, bool isHero, int player)
	{
		Network.Destroy(vid2);
		NetworkView nv = NetworkView.Find(viewID);
		CreatureScript cs = nv.GetComponent<CreatureScript>();
		print(cretID);
		cs.playerID=this.playerID;
		if(isHero)
		{
			TurnManager.instance.heroObjects[this.playerID]=cs;
			cs.playerID=this.playerID;
		}
		else
		{
			cs.attack=CardDatabase.instance.cards[cretID].statA;
			cs.health=cs.maxhealth=CardDatabase.instance.cards[cretID].statB;
			cs.cardTex=CardDatabase.instance.cards[cretID].cardPicture;
			cs.name=CardDatabase.instance.cards[cretID].name;
			cs.description=CardDatabase.instance.cards[cretID].descrip;
			cs.type=CardDatabase.instance.cards[cretID].type;
			cs.flavor=CardDatabase.instance.cards[cretID].flavor;
			cs.canAttack=CardDatabase.instance.cards[cretID].boolA;
			cs.playerID=this.playerID;
			cs.attacked=CardDatabase.instance.cards[cretID].boolB;
			string desc = CardDatabase.instance.cards[cretID].descrip.Replace("\\n", "\n");
			cs.description=desc;
			string flav = CardDatabase.instance.cards[cretID].flavor.Replace("\\n", "\n");
			cs.flavor=flav;
			cs.summonSound=CardDatabase.instance.cards[cretID].soundA;
			cs.attackSound=CardDatabase.instance.cards[cretID].soundB;
			cs.dieSound=CardDatabase.instance.cards[cretID].soundC;
		}
	}
}