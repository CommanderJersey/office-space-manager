using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class MyBehaviorTree3 : MonoBehaviour
{
	public Transform wanderDesk;
	//public Transform wander1;
	//public Transform wander3;
	public GameObject participant;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	
	public GameObject[] bathrooms;
	public GameObject[] water;
	public GameObject[] food;
	
	GameObject[] humans;
	
	Transform closestWater;
	Transform closestFood;
	Transform closestBathroom;
	
	UnityEngine.AI.NavMeshAgent agent;
	
	public int hunger = 0;
	public int thirst = 0;
	public int bladder = 0; 
	
	
	int moveTimer = 0;
	
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
		
		thirst = UnityEngine.Random.Range(0, 30);
		hunger = UnityEngine.Random.Range(0, 25);
		bladder = UnityEngine.Random.Range(0, 15);
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*if ((PlayerPrefs.GetInt("GameState", 0) == 1) && (moveTimer%100 == 2)){
			PlayerPrefs.SetInt("GameTime", PlayerPrefs.GetInt("GameTime", 0)+1);
			
		}
		
		if (PlayerPrefs.GetInt("GameTime", 0) >= 40){
			
			PlayerPrefs.SetInt("GameTime", 0);
			
			GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
			foreach (var hum in humans) {
				Destroy(hum);
			}
		}*/
		
		moveTimer++;
		if (moveTimer > 1000){
			moveTimer = 0;
		}
	
		if ((moveTimer)%12 == 1){
			thirst++;
		}
		if ((moveTimer)%30 == 1){
			hunger++;
		}
		if ((moveTimer)%40 == 1){
			bladder++;
		}
		
		if (Vector3.Distance(transform.position, closestWater.position) < .5f){
			thirst = 0;
		}
		
		if (Vector3.Distance(transform.position, closestFood.position) < .5f){
			hunger = 0;
		}
		
		if (Vector3.Distance(transform.position, closestBathroom.position) < .5f){
			bladder = 0;
		}
		
		//Agent will not earn money if they are distracted by hunger, thirst ect.
		if ((moveTimer%20 == 0) && (Vector3.Distance(transform.position, wanderDesk.position) < 1f) && (thirst <=110)&&(bladder<=150)&&(hunger<=120)){
			PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)+1);
		}
	}

	protected Node ST_ApproachAndWait(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}
	
	
	protected Node GetDrinkClosest()
	{

		float distance = 9999.9f;
		float newDistance;
		
		foreach (GameObject wat in water){
			newDistance = Vector3.Distance(wat.transform.position, wanderDesk.position);
			if (newDistance < distance){
				distance = newDistance;
				closestWater = wat.transform;
				
			}
		}

		Val<Vector3> position = Val.V (() => closestWater.position);
		//thirst = 0;
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(30));
			
			
	}
	
	protected Node GetFoodClosest()
	{

		float distance = 9999.9f;
		float newDistance;
		
		foreach (GameObject foo in food){
			newDistance = Vector3.Distance(foo.transform.position, wanderDesk.position);
			if (newDistance < distance){
				distance = newDistance;
				closestFood = foo.transform;
			}
		}

		Val<Vector3> position = Val.V (() => closestFood.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(30));	
	}
	
	protected Node GetBathroomClosest()
	{

		float distance = 9999.9f;
		float newDistance;
		
		foreach (GameObject bathr in bathrooms){
			newDistance = Vector3.Distance(bathr.transform.position, wanderDesk.position);
			if (newDistance < distance){
				distance = newDistance;
				closestBathroom = bathr.transform;
			}
		}

		Val<Vector3> position = Val.V (() => closestBathroom.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(30));	
	}
	
	protected Node BuildTreeRoot()
	{
		
		bathrooms = GameObject.FindGameObjectsWithTag("Bathroom");
		water = GameObject.FindGameObjectsWithTag("Water");
		food = GameObject.FindGameObjectsWithTag("Food");
		
		Node roaming = new DecoratorLoop (
						new SequenceShuffle(
						//this.WorkAtDesk(this.wander1),
						//this.GetFood(this.wander2),
						//this.ST_ApproachAndWait(this.wander1),
						this.ST_ApproachAndWait(this.wanderDesk),
						this.GetDrinkClosest(),
						
						this.GetFoodClosest()
						));
		//return roaming;
		
		//Food related nodes/conditions
		Func<bool> actFood = () => (hunger > 120);
		Node getFood = new Sequence(this.GetFoodClosest(), this.ST_ApproachAndWait(this.wanderDesk));
		Node foodTrigger = new DecoratorLoop(new LeafAssert(actFood));
		
		//Water related nodes/conditions
		Func<bool> actWater = () => (thirst > 110);
		Node getWater = new Sequence(this.GetDrinkClosest(), this.ST_ApproachAndWait(this.wanderDesk));
		Node waterTrigger = new DecoratorLoop(new LeafAssert(actWater));
		
		//Bathroom related nodes/conditions
		Func<bool> actBathroom = () => (bladder > 150);
		Node goToBathroom = new Sequence(this.GetBathroomClosest());
		Node bathroomTrigger = new DecoratorLoop(new LeafAssert(actBathroom));
		
		
		
		//Desk related nodes/conditions
		Func<bool> actDesk = () => ((thirst <=110)&&(bladder<=100)&&(hunger<=120));
		Node returnToDesk = new Sequence(this.ST_ApproachAndWait(this.wanderDesk)); //Add sitting animation
		Node deskTrigger = new DecoratorLoop(new LeafAssert(actDesk));
		
		Node root = new DecoratorLoop( new DecoratorForceStatus( RunStatus.Success, new Sequence(
			new DecoratorForceStatus(RunStatus.Success, (new SequenceParallel(foodTrigger, getFood))),
			new DecoratorForceStatus(RunStatus.Success, (new SequenceParallel(waterTrigger, getWater))),
			new DecoratorForceStatus(RunStatus.Success, (new SequenceParallel(bathroomTrigger, goToBathroom))),
			new DecoratorForceStatus(RunStatus.Success, (new SequenceParallel(deskTrigger, returnToDesk)))
			//new SequenceParallel(deskTrigger, this.ST_ApproachAndWait(this.wanderDesk))
			)));
		return root;
		
		//Might need to actually use blackboard method if I can't figure out how to make a sequence continue after
		//condition is changed
		
		
		
		//Create a simple tree with staying at desk (possibly starting there), then moving to water and back when thirsty
		
		
		
		
		/*Val<float> pp = Val.V (() => police.transform.position.z);
		Func<bool> act = () => (police.transform.position.z > 10) || (police.transform.position.z < -10);
		Node roaming = new DecoratorLoop (
			new Sequence(
				this.ST_ApproachAndWait(this.wander1),
				this.ST_ApproachAndWait(this.wander2),
				this.ST_ApproachAndWait(this.wander3)));
		Node trigger = new DecoratorLoop (new LeafAssert (act));
		Node root = new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success, new SequenceParallel(trigger, roaming)));
		return root;*/
		
		
		
		
	}
	

	
}
