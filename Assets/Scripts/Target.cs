using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MoveTowards))]
[RequireComponent (typeof(RotateTowards))]
public class Target : MonoBehaviour {
    public TargetZone[] targetZones;
    public GameObject geometryContainer;
    public GameObject destructionParticleContainer;

    private TargetManager targetManager;
    private MoveTowards moveTowards;
    private RotateTowards rotateTowards;
    private GameObject player;
    private ParticleSystem[] destructionParticles;
    private float pointsValueLoss;
    private Vector3 startPosition;
    private PointsDisplay pointsDisplay;

    public TargetManager Targetmanager
    {
        set { targetManager = value; }
    }

    public GameObject Player
    {
        set { player = value; }
    }

    public void InitTarget()
    {
        //Get components
        moveTowards = GetComponent<MoveTowards>();
        rotateTowards = GetComponent<RotateTowards>();
        destructionParticles = destructionParticleContainer.GetComponentsInChildren<ParticleSystem>();

        //Set points value loss
        pointsValueLoss = targetManager.pointsValueLoss;

        //Set target transform
        moveTowards.target = player.transform;
        rotateTowards.target = player.transform;

        //Set points display
        pointsDisplay = targetManager.pointsDisplay;

        //Enable scripts
        moveTowards.enabled = true;
        rotateTowards.enabled = true;
    }

    //YOU'RE CHECKING FOR NULL HERE SO IF SOMETHING'S FUCKED UP, CHECK HERE FIRST
    public void Reset()
    {
        //Set components visibility
        if (destructionParticleContainer != null)
        {
            destructionParticleContainer.SetActive(false);
        }
        if(geometryContainer != null)
        {
            geometryContainer.SetActive(true);
        }

        //Add to inactive targets list
        targetManager.InactiveTargets.Enqueue(this);
        //Disable target
        if(this != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        //store start position for value loss calculations
        startPosition = transform.position;
        //enable target
        gameObject.SetActive (true);
    }

    public void Hit(RaycastHit hit)
    {
        //Get points
        int points = GetPoints(hit.collider);
        pointsDisplay.SetText(points);
		GameManager.instance.AddPoints (points);

        StartCoroutine(Destroy ());
    }

    private IEnumerator Destroy()
    {
        //Disable geometry
        geometryContainer.SetActive(false);

        //Enable particles
        destructionParticleContainer.SetActive(true);

        //Total time for particles to finish
        float maxParticleDuration = 0;

        //Play particle effect
        foreach(ParticleSystem particles in destructionParticles)
        {
            maxParticleDuration = Mathf.Max(maxParticleDuration, particles.duration);
            particles.Play();
        }

        //Move points display to stopping position
        pointsDisplay.transform.position = moveTowards.StoppingPosition;
        pointsDisplay.transform.LookAt(player.transform);

        //Show points display
        pointsDisplay.gameObject.SetActive(true);

        //wait untile particles have finished
        yield return new WaitForSeconds(maxParticleDuration);

        //Reset game object and hide it
        Reset();

        yield return new WaitForEndOfFrame();
    }

    private int GetPoints(Collider hitTargetZone)
    {
        foreach(TargetZone targetZone in targetZones)
        {
            if (targetZone.collider != hitTargetZone)
                continue;
            
            return CalculatePointLosses(targetZone.points);
        }

        return 0;
    }

    private int CalculatePointLosses(int pointsBase)
    {
        //Get distances to compare
        float startDistanceToTarget = Vector3.Distance(startPosition, moveTowards.StoppingPosition);
        float currentDistanceToTarget = Vector3.Distance(transform.position, moveTowards.StoppingPosition);

        //Get distance percentage
        float distancePercentage = (startDistanceToTarget * currentDistanceToTarget) / 100;
        distancePercentage = Mathf.Max(0, distancePercentage);

        //Hold min and max point values
        float maxPoints = pointsBase;
        float minPoints = maxPoints - (pointsBase * pointsValueLoss);

        //Calculate linear points loss
        float pointsValue = Mathf.Lerp(minPoints, maxPoints, distancePercentage);
        pointsValue = Mathf.Max(0, pointsValue);

        //Round to whole number and return new points
        return Mathf.RoundToInt(pointsValue);
    }
    [System.Serializable]
    public struct TargetZone
    {
        public Collider collider;
        public int points;
    }
}
