using UnityEngine;

public class Cloner : MonoBehaviour 
{
	public GameObject obj;
	public int waitTime;
	public int Total;
	private float timer;

	private void Awake () 
	{
		if(timer >= waitTime)
		{
			for (var i = 0; i < Total; i++)
			{
				Instantiate(obj);
			}
		}
		Destroy(this);
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > waitTime)
		{
			for (var i = 0; i < Total; i++)
			{
				Instantiate(obj);
			}
		}
		Destroy(this);
	}
}
