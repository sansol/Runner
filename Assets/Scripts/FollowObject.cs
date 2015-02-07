using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

	public GameObject followMe;

	void LateUpdate () {
		transform.position = followMe.transform.position;
	}
}
