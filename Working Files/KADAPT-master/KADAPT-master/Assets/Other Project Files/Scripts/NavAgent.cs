using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavAgent : MonoBehaviour {

	public Transform target;
	
	public Transform bathroom;
	public Transform desk;
	public Transform vendingMachine;
	public Transform waterCooler;
	public int agentNum;
	
	public GameObject AgentObject;
	
	UnityEngine.AI.NavMeshAgent agent;



	
	int waitTimer = 0;
	int moveTimer = 0;
	int destination = 0;
	
	
	
	int hunger = 0; //Random(; //SHould be random later
	int thirst = 0; //Same here
	int bladder = 0; //Same here
	
	//GoToDeskAction deskNode = new GoToDeskAction();
	
	
	// Use this for initialization
	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		PlayerPrefs.SetInt("bathroom", 0);
		PlayerPrefs.SetInt("kitchen", 0);
		moveTimer = Random.Range(610, 640);
		
		thirst = Random.Range(0, 20);
		thirst = Random.Range(0, 15);
		bladder = Random.Range(0, 5);
		
		
		
		 PlayerPrefs.SetInt("Money", 100);
		 PlayerPrefs.SetInt("Workers", 2);
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (PlayerPrefs.GetInt("Workers", 0) < agentNum) {
			//AgentObject.SetActive(false);
			//AgentObject.gameObject.transform.localScale = new Vector3(0.01f,0.01f,0.01f);
//AgentObject.GetComponent<Renderer>(false); // = false;
			agent.speed = 0;
			
		} else {
			//AgentObject.gameObject.transform.localScale = new Vector3(1,1,1);
			//AgentObject.renderer.enabled = false;
			agent.speed = 8;
			//AgentObject.SetActive(true);
		}
		
		
		//For calling a node
		if (moveTimer == 600){
			if ((bladder >= 50) && (PlayerPrefs.GetInt("bathroom", 0) == 0)){
				GoToBathroom();
			} else if ((hunger >= 45) && (PlayerPrefs.GetInt("kitchen", 0) == 0)){
				GoToKitchen();
			} else if ((thirst > 65) && PlayerPrefs.GetInt("waterCooler", 0) == 0) {
				GoToWaterCooler();
			} else {
				GoToDesk();
			}
		}
		
		//Resetting the move timer when it gets to 0
		if (moveTimer == 0) {
			if (target == bathroom){
				PlayerPrefs.SetInt("bathroom", 0);
			} else if (target == waterCooler){
				PlayerPrefs.SetInt("waterCooler", 0);
			} else if (target == vendingMachine){
				PlayerPrefs.SetInt("kitchen", 0);
			}
			
			moveTimer = Random.Range(601, 900);
		}
		
		
		if (moveTimer > 0){
			moveTimer--;
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
		if ((moveTimer)%50 == 1){
			if ((!agent.pathPending) && (target == desk)){
				 if (agent.remainingDistance <= agent.stoppingDistance){
					 if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f){
						 PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)+1);
					 }
				 }
			 }
		}
		
	}
	
	void GoToDesk(){
		target = desk;
		moveTimer = 400;
		agent.SetDestination(target.position);
	}
	
	void GoToBathroom(){
		target = bathroom;
		PlayerPrefs.SetInt("bathroom", 1);
		bladder = 0;
		moveTimer = 280;
		agent.SetDestination(target.position);
	}
	
	void GoToWaterCooler(){
		target = waterCooler;
		PlayerPrefs.SetInt("waterCooler", 1);
		thirst = 0;
		moveTimer = 280;
		agent.SetDestination(target.position);
	}
	
	void GoToKitchen(){
		target = vendingMachine;
		PlayerPrefs.SetInt("kitchen", 1);
		hunger = 0;
		moveTimer = 180;
		agent.SetDestination(target.position);
	}
	
}
