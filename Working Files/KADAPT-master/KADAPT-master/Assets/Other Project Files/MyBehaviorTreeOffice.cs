using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class MyBehaviorTreeOffice : MonoBehaviour
{
	public Transform desk;
	public Transform waterCooler;
	public Transform vendingMachine;
	public Transform bathroom1;
	public Transform bathroom2;
	public GameObject participant;

	
	public int hunger = 10; //Random(; //SHould be random later
	public int thirst; //Same here
	public int bladder = 4; //Same here
	
	public GameObject[] bathrooms;
	public GameObject[] water;
	public GameObject[] food;
	
	int moveTimer = 0;
	
	int working = 0;
	
	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
		
		
		//hunger = Random.Range(0, 20);
		//thirst = Random.Range(0, 15);
		//bladder = Random.Range(0, 5);
		
		if (bathrooms == null)
            bathrooms = GameObject.FindGameObjectsWithTag("Bathroom");
		if (water == null)
            water = GameObject.FindGameObjectsWithTag("Water");
		if (food == null)
            food = GameObject.FindGameObjectsWithTag("Food");
		
		PlayerPrefs.SetInt("Money", 250);
		PlayerPrefs.SetInt("Workers", 1);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Problem might happen where update doesn't update every frame because of behaviortree
		moveTimer++;
		if (moveTimer > 1000){
			moveTimer = 0;
		}
	
		if ((moveTimer)%20 == 1){
			thirst++;
		}
		if ((moveTimer)%50 == 1){
			hunger++;
		}
		if ((moveTimer)%48 == 1){
			bladder++;
		}
	
			/*if ((!agent.pathPending) && (target == desk)){
				 if (agent.remainingDistance <= agent.stoppingDistance){
					 if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f){
						 PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)+1);
					 }
				 }
			 } */
		
	}

	protected Node ST_ApproachAndWait(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}
	
	
	protected Node WorkAtDesk(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) + 60);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(100));
	}
	
	protected Node GetDrink(Transform target)
	{
		int index = GetClosest(water);
		
		if (thirst > 80) {
			thirst = 0;
			Val<Vector3> position = Val.V (() => target.position);
			return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(30));
		} else {
			return new Sequence(new LeafWait(1));
		}
	}
	
	protected Node GetFood(Transform target, int hunger)
	{
		if (hunger > 90) {
			hunger = 0;
			Val<Vector3> position = Val.V (() => target.position);
			return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(30));
		} else {
			return new Sequence(new LeafWait(1));
		}

	}

	protected Node UseBathroom(Transform target, int bladder)
	{
		if (bladder > 90) {
			bladder = 0;
			Val<Vector3> position = Val.V (() => target.position);
			return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(30));
		} else {
			return new Sequence(new LeafWait(1));
		}
		
}
	
	protected Node BuildTreeRoot()
	{
		Node roaming = new SequenceShuffle(
						this.ST_ApproachAndWait(this.desk),
						this.ST_ApproachAndWait(this.waterCooler),
						this.ST_ApproachAndWait(this.vendingMachine)
						//this.WorkAtDesk(this.desk),
						//this.GetDrink(this.waterCooler),
						//this.GetFood(this.vendingMachine, hunger)
						//this.UseBathroom(this.bathroom1, bladder)
						);
		return roaming;
	}
	
	int GetClosest(GameObject[] list){
		int index = -1;
		float distance = -1;
		float newDistance = 0;
		
		for (int i = 0; i < list.Length; i++){
			newDistance = Vector3.Distance(list[i].transform.position, participant.transform.position);
			if (((newDistance > distance) || (distance == -1)) && (participant.transform.eulerAngles.x == 0)){
				distance = newDistance;
				index = i;
			}
		}
		
		
		
		return index;
		
	}
}
