using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour {
    public GameObject targetPrefab;
    public float spawnDelay = 2f;
    public float timeBetweenSpawnsMin = 1f;
    public float timeBetweenSpawnsMax = 5f;
    public float spawnRadius = 10f;
    public float maxSpawnHeight = 40f;
    public int maxNumTargets = 20;

    [Range(0, 1), Tooltip("How much % of point value is removed when target is at stopping distance (0 = 0%, 1 = 100%")]
    public float pointsValueLoss;

    public PointsDisplay pointsDisplay;

    private List<Target> spawnedTargets = new List<Target>();
    private Queue<Target> inactiveTargets = new Queue<Target>();

    public Queue<Target> InactiveTargets
    {
        get { return inactiveTargets; }
    }

	void Awake(){
		//disable on game start because this is controlled by game manager
		this.enabled = false;
	}

    void OnEnable()
    {
        //InitTargets();
        StartCoroutine(SpawnTarget());
    }

    void OnDisable()
    {
        StopCoroutine(SpawnTarget());
        ResetAllTargets();
    }

    public void InitTargets()
    {
        //TEMP: Store player, later make sure there can only be one player
        //GameObject player = GameObject.FindGameObjectsWithTag("Player")[0] as GameObject;

        //Create target parent game object (for a cleaner outline)
        GameObject targetParent = new GameObject();
        targetParent.name = "Targets";

        //Insantiate all targets
        for(int i = 0; i < maxNumTargets; i++)
        {
            Target targetInstance = (Instantiate(targetPrefab) as GameObject).GetComponent<Target>();

            //Register target to manager
            targetInstance.Targetmanager = this;

            //Set parent
            targetInstance.transform.parent = targetParent.transform;

            //Set player
			targetInstance.Player = GameManager.instance.player;

            //Initialize target
            targetInstance.InitTarget();

            //Add to target lists
            spawnedTargets.Add(targetInstance);
        }
        ResetAllTargets();
    }

    private IEnumerator SpawnTarget()
    {
        //wait spawn delay time
        yield return new WaitForSeconds(spawnDelay);
        //spawning loop
        while (this.isActiveAndEnabled)
        {
            if(inactiveTargets.Count > 0)
            {
                //Get inactive target from queue
                Target target = inactiveTargets.Dequeue();
                //Move target to position and make sure it is visible for the player
                Vector3 position;
                do
                {
                    position = transform.position + Random.onUnitSphere * spawnRadius;
                } while (position.y < transform.position.y || position.y > maxSpawnHeight);

                target.transform.position = position;

                //Activate target
                target.Activate();
            }

            //Get random wait time
            float waitTime = Random.Range(timeBetweenSpawnsMin, timeBetweenSpawnsMax);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void ResetAllTargets()
    {
        //clear targets queue
        inactiveTargets.Clear();

        //reset each target
        foreach(Target target in spawnedTargets)
        {
            target.Reset();
        }
    }
}
