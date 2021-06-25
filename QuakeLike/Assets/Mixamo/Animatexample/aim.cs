using UnityEngine;
using System.Collections;

public class aim : MonoBehaviour {

	protected Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		// The bottom-left of the screen or window is at (0, 0). The top-right of the screen or window is at (Screen.width, Screen.height).
		Vector3 mousePosition = Input.mousePosition;

		float h = Mathf.Lerp(-1.0f, 1.0f, ((float)mousePosition.x) / ((float)Screen.width));
		float v = Mathf.Lerp(-1.0f, 1.0f, ((float)mousePosition.y) / ((float)Screen.height));

		animator.SetFloat("AimHorizontal", h);
		animator.SetFloat("AimVertical", v);

		
	}
}
