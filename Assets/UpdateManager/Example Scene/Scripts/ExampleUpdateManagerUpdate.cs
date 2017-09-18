using UnityEngine;
using System.Collections;

public class ExampleUpdateManagerUpdate : OverridableMonoBehaviour
{
	private int i;
	
	public override void UpdateMe()
	{
		i++;
	}
}
