using UnityEngine;
using System.Collections;

public class CardDatabase : MonoBehaviour
{
	public CardType[] cards;
	public int[] numAvailable,numUsed;
	public static CardDatabase instance;
	public string user,pass,defaultLoginURL,deckA;
	public static bool loggedIn=false;

	void OnGUI()
	{
		if(Menu.screenState==1)
		{
			user = GUI.TextField(new Rect(Screen.width/2-64, Screen.height/2-32, 128, 32), user);
			pass = GUI.TextField(new Rect(Screen.width/2-64, Screen.height/2, 128, 32), pass);
			if(GUI.Button(new Rect(Screen.width/2-96, Screen.height/2+64, 96*2, 48), "Log In/Update"))
			{
				WWWForm form = new WWWForm();
				form.AddField("myform_hash", "testHash");
				form.AddField("nickforum", user);
				form.AddField("passforum", pass);
				StartCoroutine("loginenum", form);
			}
		}
		if(Menu.screenState==2)
		{
			/*for(int i = 0; i<cards.Length; i++)
			{

			}*/
			GUI.Label(new Rect(Screen.width/2-64, Screen.height/2-32, 128, 64), "Really not feelin' up to\n it now, sorry");
		}
	}

	IEnumerator loginenum(WWWForm form)
	{
		WWW w = new WWW(defaultLoginURL, form);
		yield return w;
		if(w.error!=null)
		{
			print(w.error);
		}
		else
		{
			string response = w.text;
			print(response);
			string[] lines = response.Split('\n');
			if(lines[0]=="Connected")
			{
				deckA=lines[2];
				string[] amounts = lines[1].Split(',');
				numAvailable=numUsed=new int[cards.Length];
				foreach(string s in amounts)
				{
					string[] IDToAmount = s.Split('x');
					numUsed[int.Parse(IDToAmount[0])]=numAvailable[int.Parse(IDToAmount[0])]=int.Parse(IDToAmount[1]);
				}
			}
		}
	}

	void Start()
	{
		CardDatabase.instance=this;
	}

	[RPC] void SpawnDeck(NetworkViewID viewID, int id, string deckDB)
	{
		GameObject go = NetworkView.Find(viewID).gameObject;
		DeckScript ds = go.GetComponent<DeckScript>();
		ds.playerID=id;
		string[] separatedString = deckDB.Split(',');
		int[] newCards = new int[separatedString.Length];
		for(int i = 0; i<separatedString.Length; i++)
		{
			newCards[i]=int.Parse(separatedString[i]);
		}
		ds.cards=newCards;
		TurnManager.instance.playerDecks[id]=ds;
	}

	[RPC] void SpawnHero(NetworkViewID viewID, int id)
	{
		GameObject go = NetworkView.Find(viewID).gameObject;
		go.GetComponent<Card>().playerID=id;
	}
}