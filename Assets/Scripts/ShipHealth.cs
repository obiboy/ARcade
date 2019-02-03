using UnityEngine;
using UnityEngine.UI;

public class ShipHealth : MonoBehaviour
{
	public float StartingHealth = 100f;               // The amount of health each tank starts with.
	public Slider Slider;                             // The slider to represent how much health the tank currently has.
	public Image FillImage;                           // The image component of the slider.
	public Color FullHealthColor = Color.green;       // The color the health bar will be when on full health.
	public Color ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
	public GameObject ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
	public float CurrentHealth;                      // How much health the tank currently has.

	private ParticleSystem ExplosionParticles;        // The particle system the will play when the tank is destroyed.
	private bool Dead;                                // Has the tank been reduced beyond zero health yet?


	private void Awake ()
	{
		// Instantiate the explosion prefab and get a reference to the particle system on it.
		ExplosionParticles = Instantiate (ExplosionPrefab).GetComponent<ParticleSystem> ();

		// Disable the prefab so it can be activated when it's required.
		ExplosionParticles.gameObject.SetActive (false);
	}


	private void OnEnable()
	{
		// When the tank is enabled, reset the tank's health and whether or not it's dead.
		CurrentHealth = StartingHealth;
		Dead = false;

		// Update the health slider's value and color.
		SetHealthUI();
	}


	public void TakeDamage ()
	{
		// Reduce current health by the amount of damage done.
		CurrentHealth -= 10.0f;

		// Change the UI elements appropriately.
		SetHealthUI ();

		// If the current health is at or below zero and it has not yet been registered, call OnDeath.
		if (CurrentHealth <= 0f && !Dead)
		{
			OnDeath ();
		}
	}


	private void SetHealthUI ()
	{
		// Set the slider's value appropriately.
		Slider.value = CurrentHealth;

		// Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
		FillImage.color = Color.Lerp (ZeroHealthColor, FullHealthColor, CurrentHealth / StartingHealth);
	}


	private void OnDeath ()
	{
		// Set the flag so that this function is only called once.
		Dead = true;

		// Move the instantiated explosion prefab to the tank's position and turn it on.
		ExplosionParticles.transform.position = transform.position;
		ExplosionParticles.gameObject.SetActive (true);

		// Play the particle system of the tank exploding.
		ExplosionParticles.Play ();


		// Turn the tank off.
		gameObject.SetActive (false);
	}

	void OnTriggerEnter(Collider other)
	{
		if(GameObject.FindWithTag("Enemy"))
		{
			Destroy (other.gameObject);
			TakeDamage ();
		}
	}
}