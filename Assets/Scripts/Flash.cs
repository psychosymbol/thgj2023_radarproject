using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flash : MonoBehaviour
{

	public static Flash instance;

	public float totalSeconds;  // The total of seconds the flash wil last
	public float flashNumber = 2;
	public float maxIntensity;  // The maximum intensity the flash will reach
	public Light myLight;       // Your light
	public Light[] myLights;       // Your lights
	private List<float> lightOriginalIntensity;

    private void Start()
    {
		if (!instance) instance = this;
		else if (instance != this) Destroy(gameObject);
    }

    public void doflash()
    {
		StartCoroutine(flashNow());
    }

	public IEnumerator flashNow()
	{
		float waitTime = totalSeconds / 2;
		// Get half of the seconds (One half to get brighter and one to get darker)
		//while (myLight.intensity < maxIntensity)
		//{
		//	myLight.intensity += Time.deltaTime / waitTime;     // Increase intensity
		//	yield return null;
		//}
		//while (myLight.intensity > 0)
		//{
		//	myLight.intensity -= Time.deltaTime / waitTime;     //Decrease intensity
		//	yield return null;
		//}
		float intensity = 1;
		int flashTime = 0;
		lightOriginalIntensity = new List<float>();
        foreach (var item in myLights)
        {
			lightOriginalIntensity.Add(item.intensity);
        }
		// Get half of the seconds (One half to get brighter and one to get darker)
		while (flashTime < flashNumber)
        {
			while (intensity < maxIntensity)
			{
				intensity += Time.deltaTime / waitTime;     // Increase intensity
				for (int i = 0; i < myLights.Length; i++)
                {
					myLights[i].intensity = intensity.Remap(0, 1, 0, lightOriginalIntensity[i]);
                }
				yield return null;
			}
			while (intensity > 0)
			{
				intensity -= Time.deltaTime / waitTime;     //Decrease intensity
				for (int i = 0; i < myLights.Length; i++)
				{
					myLights[i].intensity = intensity.Remap(0, 1, 0, lightOriginalIntensity[i]);
				}
				yield return null;
			}
			flashTime++;
			yield return null;
		}

		while (intensity < maxIntensity)
		{
			intensity += Time.deltaTime / waitTime;     // Increase intensity
			for (int i = 0; i < myLights.Length; i++)
			{
				myLights[i].intensity = intensity.Remap(0, 1, 0, lightOriginalIntensity[i]);
			}
			yield return null;
		}

		yield return null;

		for (int i = 0; i < myLights.Length; i++)
		{
			myLights[i].intensity = lightOriginalIntensity[i];
		}
	}
}