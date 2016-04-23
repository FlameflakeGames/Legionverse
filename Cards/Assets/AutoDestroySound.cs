using UnityEngine;
using System.Collections;

public class AutoDestroySound : MonoBehaviour
{
	public float ticksUntilDestroy = 10;

	void Update()
	{
		ticksUntilDestroy-=Time.deltaTime;
		if(ticksUntilDestroy<=0)
		{
			Destroy(this.gameObject);
		}
	}
}