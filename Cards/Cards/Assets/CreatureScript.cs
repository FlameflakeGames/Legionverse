using UnityEngine;
using System.Collections;

public class CreatureScript : MonoBehaviour
{
	public Texture2D[] cardBacks,cardMain;
	public int selectedBack = 0;
	public Renderer rend;
	public Texture2D cardTex;
	public TextMesh nameText,typeText,attackT,healthT,descripText,flavorText;
	public string name,type,description,flavor;
	public Rigidbody rb;
	public int attack,health,maxhealth,playerID;
	public bool canAttack,attacked,attackedAirborne;
	public NetworkView networkView,secondView;
	public GameObject soundPrefab;
	public AudioClip summonSound,attackSound,dieSound;
	public GameObject currentSound;
	public GameObject attackedOutline;

	[RPC] void TakeDamage(int amount)
	{
		if(networkView.isMine)
			this.health-=amount;
	}

	[RPC] void PlaySound(int type)
	{
		GameObject go = (GameObject)Instantiate(soundPrefab, transform.position, transform.rotation);
		if(type==0)
		{
			go.GetComponent<AudioSource>().clip=summonSound;
			Destroy(currentSound);
		}
		else if(type==1)
		{
			go.GetComponent<AudioSource>().clip=attackSound;
			Destroy(currentSound);
		}
		else if(type==2)
		{
			go.GetComponent<AudioSource>().clip=dieSound;
			Destroy(currentSound);
		}
		go.GetComponent<AudioSource>().Play();
		currentSound=go;
	}

	[RPC] void AttackOther(NetworkViewID nvid)
	{
		NetworkView nv = NetworkView.Find(nvid);
		CreatureScript cs = nv.GetComponent<CreatureScript>();
		if(networkView.isMine)
		{
			networkView.RPC("PlaySound", RPCMode.All, 1);
			if(!CardDatabase.instance.cards[this.cardID].abilities.Contains(CardType.CardAbilities.Ranged))
				networkView.RPC("TakeDamage", RPCMode.All, cs.attack);
			nv.RPC("TakeDamage", RPCMode.All, attack);
		}
		if(CardDatabase.instance.cards[this.cardID].abilities.Contains(CardType.CardAbilities.Airborne))
		{
			attackedAirborne=true;
		}
	}

	public int cardID;

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(stream.isReading)
		{
			stream.Serialize(ref health);
			stream.Serialize(ref maxhealth);
			stream.Serialize(ref attack);
			stream.Serialize(ref cardID);
		}
		else
		{
			stream.Serialize(ref health);
			stream.Serialize(ref maxhealth);
			stream.Serialize(ref attack);
			stream.Serialize(ref cardID);
		}
	}

	void Start()
	{
		rend.materials[0].mainTexture=cardBacks[selectedBack];
		rend.materials[1].mainTexture=cardMain[selectedBack];;
		rend.materials[3].mainTexture=cardTex;
		nameText.text=name;
		typeText.text=type;
		descripText.text=description;
		flavorText.text=flavor;
		if(networkView.isMine)
			networkView.RPC("PlaySound", RPCMode.All, 0);
	}
	
	void Update()
	{
		if(this.health<=0)
		{
			if(networkView.isMine)
				networkView.RPC("PlaySound", RPCMode.All, 2);
			Network.Destroy(secondView.viewID);
			Network.Destroy(networkView.viewID);
		}
		if(this.health>this.maxhealth)
			this.health=this.maxhealth;
		if(this.attacked==false)
		{
			attackedOutline.SetActive(true);
		}
		else
		{
			attackedOutline.SetActive(false);
		}
		attackT.text=attack.ToString();
		healthT.text=health.ToString();
		if(this.GetComponent<NetworkView>().isMine==false)
		{
			rb.isKinematic=true;
		}
	}
}