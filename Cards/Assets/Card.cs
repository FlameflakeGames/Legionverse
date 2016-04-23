using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
	public Texture2D[] cardBacks,cardMain;
	public int selectedBack = 0;
	public Renderer rend;
	public Texture2D cardTex;
	public TextMesh nameText,typeText,statAT,statBT,descripText,flavorText,Color1Text,NCText;
	public string name,type,description,flavor;
	public int statA,statB,redCost,greenCost,blueCost,yellowCost,whiteCost,blackCost,neutralCost,playerID;
	public Rigidbody rb;
	public enum CardType {Creature=0,Hero=1}
	public CardType cardType;
	public int cardId;
	public bool boolA,boolB;

	void Start()
	{
		rend.materials[0].mainTexture=cardBacks[selectedBack];
		rend.materials[1].mainTexture=cardMain[selectedBack];;
		rend.materials[3].mainTexture=cardTex;
		nameText.text=name;
		typeText.text=type;
		descripText.text=description;
		flavorText.text=flavor;
		statAT.text=statA.ToString();
		statBT.text=statB.ToString();
		if(neutralCost>0)
		{
			NCText.gameObject.SetActive(true);
			NCText.text=neutralCost.ToString();
		}
		if(redCost>0 && Color1Text.gameObject.activeSelf==false)
		{
			Color1Text.gameObject.SetActive(true);
			Color1Text.text=redCost.ToString();
		}
		if(yellowCost>0 && Color1Text.gameObject.activeSelf==false)
		{
			Color1Text.gameObject.SetActive(true);
			Color1Text.text=yellowCost.ToString();
		}
		if(greenCost>0 && Color1Text.gameObject.activeSelf==false)
		{
			Color1Text.gameObject.SetActive(true);
			Color1Text.text=greenCost.ToString();
		}
		if(blueCost>0 && Color1Text.gameObject.activeSelf==false)
		{
			Color1Text.gameObject.SetActive(true);
			Color1Text.text=blueCost.ToString();
		}
		if(blackCost>0 && Color1Text.gameObject.activeSelf==false)
		{
			Color1Text.gameObject.SetActive(true);
			Color1Text.text=blackCost.ToString();
		}
		if(whiteCost>0 && Color1Text.gameObject.activeSelf==false)
		{
			Color1Text.gameObject.SetActive(true);
			Color1Text.text=whiteCost.ToString();
		}
	}

	void Update()
	{
		if(this.GetComponent<NetworkView>().isMine==false)
		{
			rb.isKinematic=true;
			Quaternion qat = transform.rotation;
			Vector3 eulers = qat.eulerAngles;
			eulers = new Vector3(270, eulers.y, eulers.z);
			transform.rotation = qat;
		}
	}
}