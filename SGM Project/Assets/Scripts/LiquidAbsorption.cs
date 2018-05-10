using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidAbsorption : MonoBehaviour {

    //public int collisionCount = 0;
    public Color currentColor;
    public BottleSmash smashScript;
    bool hasAudioStartedPlaying = false;
    bool hasBottleExploded;
    public float particleValue = 0.02f;
    public HashSet<GameObject> set = new HashSet<GameObject>();
    public LiquidVolumeAnimator LVA;
    public GameObject hydrogenPeroxideParticles;
    public GameObject waterParticles;
    public GameObject ammoniaParticles;
    public GameObject hydroChloricAcidParticles;
    public AudioSource audio;
    private bool isPotassiumInside;
    private bool isSodiumInside;
    public GameObject smokeParticles;
    public GameObject smokeParticlesForWaterAndPotassium;
    public GameObject afterExplosionSmoke;
    public GameObject ElephantToothPasteParticles;
    public delegate void waterPotassiumAction();
    public static event waterPotassiumAction reactWaterPotassium;
    public delegate void peroxideAndSodiumAction();
    public static event peroxideAndSodiumAction reactPeroxideSodium;
    public delegate void HydroChloricAndAmmoniaAction();
    public static event HydroChloricAndAmmoniaAction reactHydroChloricAndAmmonia;

    // Use this for initialization
    void Start () {
        isPotassiumInside = false;
        isSodiumInside = false;
        if (LVA == null)
        LVA = GetComponent<LiquidVolumeAnimator>();
        

    }
    
    public void ResetBottle()
    {
        Debug.Log("Reset bottle inside absorber");
        ElephantToothPasteParticles.SetActive(false);
        isPotassiumInside = false;
        isSodiumInside = false;
        LVA.level = 0;
        set.Clear();
    }
    
    
    private void OnCollisionEnter(Collision collision)
    {
        //This if statement is used to check if potassium has been put into the bottle
        if(collision.gameObject.tag=="Potassium")
        {
            isPotassiumInside = true;
            Destroy(collision.gameObject);
           
            
        }
        //This if statment is used to call the event for reaction of water and potassium
        if (isPotassiumInside==true && set.Contains(waterParticles))
        {

            if (reactWaterPotassium != null)
                reactWaterPotassium();
            

        }
        //This if statement is used to check if sodium iodide has been put into the bottle
        if (collision.gameObject.tag == "Sodium")
        {
            isSodiumInside = true;
            Destroy(collision.gameObject);

        }
        //This if statment is used to call the event for reaction of sodium iodide and hydrogen peroxide
        if (isSodiumInside == true && set.Contains(hydrogenPeroxideParticles))
        {
            if(reactPeroxideSodium!=null)
            reactPeroxideSodium();

        }

    }
    
    void OnParticleCollision(GameObject other)
    {
        set.Add(other);
        // Potassium and water reaction
        if (isPotassiumInside == true && set.Contains(waterParticles))
        {

            if (reactWaterPotassium != null)
                reactWaterPotassium();

        }
        // hydrochloric acid and ammonia reaction
        if (set.Contains(hydroChloricAcidParticles) && set.Contains(ammoniaParticles))
        {
            if (reactHydroChloricAndAmmonia!=null)
                reactHydroChloricAndAmmonia();
        }
        // Sodium Iodide and hydrogen peroxide reaction
        if (isSodiumInside == true && set.Contains(hydrogenPeroxideParticles) )
        {
            if (reactPeroxideSodium != null)
                reactPeroxideSodium();

        }


        //CODE From 2Ginge (not made by us)
        if (other.transform.parent == transform.parent)
            return;
        bool available = false;
        if (smashScript.Cork == null)
        {
            available = true;
        }
        else
        {
            //if the cork is not on!
            if (!smashScript.Cork.activeSelf)
            {

                available = true;
            }
                //or it is disabled (through kinamism)? is that even a word?
            else if (!smashScript.Cork.GetComponent<Rigidbody>().isKinematic)
            {
                available = true;
            }
        }
        if (available)
        {
            currentColor = smashScript.color;
            if (LVA.level < 1.0f - particleValue)
            {

                //essentially, take the ratio of the bottle that has liquid (0 to 1), then see how much the level will change, then interpolate the color based on the dif.
                Color impactColor = other.GetComponentInParent<BottleSmash>().color;

                if (LVA.level <= float.Epsilon * 10)
                {
                    currentColor = impactColor;
                }
                else
                {
                    currentColor = Color.Lerp(currentColor, impactColor, particleValue / LVA.level);
                }
                //collisionCount += 1;
                LVA.level += particleValue;
                smashScript.color = currentColor;
            }
        }
    }
	// Update is called once per frame
	void Update ()
	{
	    currentColor = smashScript.color;

	}
}
