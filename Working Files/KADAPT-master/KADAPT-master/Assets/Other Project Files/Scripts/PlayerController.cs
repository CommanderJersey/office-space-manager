using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

private Animator theAnim;
private Rigidbody rigid;
public float groundDistance = 0.3f;
public float JumpForce = 500;
public LayerMask whatIsGround;

	// Use this for initialization
	void Start () {
		theAnim = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		var v = Input.GetAxis ("Vertical");
		var h = Input.GetAxis ("Horizontal");
		theAnim.SetFloat ("Speed", v);
		theAnim.SetFloat ("TurningSpeed", h);
		
		if (Input.GetButtonDown ("Jump")) {
			rigid.AddForce (Vector3.up * JumpForce);
			theAnim.SetTrigger ("Jump");
		}
		if (Physics.Raycast (transform.position + (Vector3.up * 0.1f), Vector3.down, groundDistance, whatIsGround)) {
			theAnim.SetBool ("grounded", true);
			theAnim.applyRootMotion = true;
		} else {
			theAnim.SetBool ("grounded", false);
		}
	}
}
