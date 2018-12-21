using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Text mText;
	public Text tText;
	public Text sText;

	public GameObject waterPrefab;
	public GameObject foodPrefab;
	public GameObject bathroomPrefab;
	public GameObject deskPrefab;
	public GameObject personPrefab;
	
	bool placingObject;
	GameObject objectToPlace;
	int placingX = 0;
	int placingZ = 0;
	
	public int gameTime;
	int moveTimer;
	
	//GameObject floor;
	
	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt("Workers", 2);
		PlayerPrefs.SetInt("Money", 600);
		PlayerPrefs.SetInt("GameState", 1);
	}
	
	// Update is called once per frame
	void Update () {
	
		if ((PlayerPrefs.GetInt("GameState", 0) == 1) && (moveTimer%500 == 2)){
			gameTime++;
			
		}
		
		if (gameTime >= 8){
			gameTime = 0;
			PlayerPrefs.SetInt("GameState", 0);
			PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)-(100*PlayerPrefs.GetInt("Workers", 0)));
			
			//For later: Get Game over if can't pay the workers salary
			GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
			foreach (var hum in humans) {
				Destroy(hum);
			}
		}
		
		if (PlayerPrefs.GetInt("GameState", 0) == 1){
			moveTimer++;
		}
		if (moveTimer > 10000){
			moveTimer = 0;
		}
		
		if (Input.GetKey("m")){
				PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)+100);
		}
		
		
		mText.text = "Money: $" + PlayerPrefs.GetInt("Money", 0);
		tText.text = "Time Left in Day: " + (8 - gameTime) + " Hours";
		sText.text = "Salary to Pay at End of Day: $" + (100*PlayerPrefs.GetInt("Workers", 0));
		
		if (placingObject){
			if (Input.GetKey("up")){
				placingZ += 1;
			}
			if (Input.GetKey("down")){
				placingZ -= 1;
			}
			if (Input.GetKey("left")){
				placingX -= 1;
			}
			if (Input.GetKey("right")){
				placingX += 1;
			}
			objectToPlace.transform.position = new Vector3(placingX/2f, 0.2f, placingZ/2f);
			if (Input.GetKey("p")){
				placingX = 0;
				placingZ = 0;
				placingObject = false;
			}
			
			//Vector3 mouse = new Vector3(hit.point.x,1.9f,hit.point.z);
            //mouse = Camera.main.ScreenToWorldPoint(mouse);
            
			//Raycast object
			//On click make placing object false & Bake Navmesh
			
		}
	}
	
	public void BuyWorker(){
		if (PlayerPrefs.GetInt("Money", 0) >= 350 && PlayerPrefs.GetInt("GameState", 0) == 0){
			PlayerPrefs.SetInt("Workers", PlayerPrefs.GetInt("Workers", 0)+1);
			PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)-350);
			objectToPlace = Instantiate(deskPrefab,new Vector3(0f,2f, 0f), Quaternion.identity);
			placingObject = true;		
		}
	}
	
	public void BuyWater(){
		if (PlayerPrefs.GetInt("Money", 0) >= 140 && PlayerPrefs.GetInt("GameState", 0) == 0){
			PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)-140);
			objectToPlace = Instantiate(waterPrefab,new Vector3(0f,2f, 0f), Quaternion.identity);
			placingObject = true;
		}
	}
	
	public void BuyFood(){
		if (PlayerPrefs.GetInt("Money", 0) >= 180 && PlayerPrefs.GetInt("GameState", 0) == 0){
			PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)-180);
			objectToPlace = Instantiate(foodPrefab,new Vector3(0f,2f, 0f), Quaternion.identity);
			placingObject = true;
		}
	}
	
	public void BuyBathroom(){
		if (PlayerPrefs.GetInt("Money", 0) >= 250 && PlayerPrefs.GetInt("GameState", 0) == 0){
			PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0)-250);
			objectToPlace = Instantiate(bathroomPrefab,new Vector3(0f,2f, 0f), Quaternion.identity);
			placingObject = true;
		}
	}
	
	
	public void ContinueWork(){
		if (PlayerPrefs.GetInt("GameState", 0) == 0){
			for (int i = 0; i < PlayerPrefs.GetInt("Workers", 0); i++){
				objectToPlace = Instantiate(personPrefab,new Vector3(4.64f, 0f, -12.44f), Quaternion.identity);
				
				GameObject[] desks = GameObject.FindGameObjectsWithTag("Desk");
				//foreach (var des in desks) {
					objectToPlace.GetComponent<MyBehaviorTree3>().wanderDesk = desks[i].transform;
				//}
				//Need to assign one worker per desk
			}
			PlayerPrefs.SetInt("GameState", 1);		
		}
	}
	
}
