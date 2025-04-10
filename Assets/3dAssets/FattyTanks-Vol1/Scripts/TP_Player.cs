using UnityEngine;

public enum PlayerWeaponType{KNIFE,PISTOL,NULL}

public class TP_Player : MonoBehaviour {

	Rigidbody myRigidBody;
	public float moveSpeed=10.0f;
	public Transform hitTestPivot,gunPivot;
	public GameObject mousePointer,proyectilePrefab;
	//public Animator animator;
	int hashSpeed;
	float attackTime=0.4f;
	 PlayerWeaponType currentWeapon=PlayerWeaponType.NULL;
    TP_Timer attackTimer = new TP_Timer();

    //Tank
    public Transform turretTr;

    public float m_TurnSpeed = 180f;
    private float m_MovementInputValue;
    private float m_TurnInputValue;

    //cam test
    private float camDist;
    

    // Use this for initialization
    void Awake() {

	}
	void Start () {
		SetWeapon (PlayerWeaponType.PISTOL);
		myRigidBody = GetComponent<Rigidbody> ();
		hashSpeed = Animator.StringToHash ("Speed");
		attackTimer.StartTimer (0.1f);

        //id3644
        camDist = Vector3.Distance(Camera.main.transform.position, transform.position);

        //	Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
		//animator.SetFloat (hashSpeed, myRigidBody.velocity .magnitude);
		float inputHorizontal = Input.GetAxis ("Horizontal");
		float inputVertical = Input.GetAxis ("Vertical");

        float speedY = inputVertical > 0.1 ? Mathf.Clamp ((inputVertical * moveSpeed), moveSpeed / 2.0f, moveSpeed) : 0.0f;
		float speedX = inputHorizontal > 0.1 ? Mathf.Clamp ((inputHorizontal * moveSpeed), moveSpeed / 2.0f, moveSpeed) : 0.0f;
		Vector3 newVelocity=new Vector3(inputVertical*moveSpeed, 0.0f, inputHorizontal*-moveSpeed);
        myRigidBody.linearVelocity = newVelocity;

        m_MovementInputValue = inputVertical;   //id3644
        m_TurnInputValue = inputHorizontal;   //id3644

        //transform.rotation = Quaternion.Euler(newVelocity);
        float deltaY = -inputHorizontal;
        float deltaX = inputVertical;
        float angleInDegrees = Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI;
        transform.eulerAngles = new Vector3(0, -angleInDegrees, 0);

        switch (currentWeapon) {
			case PlayerWeaponType.KNIFE:
				if (Input.GetMouseButton (0) && attackTimer.IsFinished()) {
					Attack();
				}
			break;
			case PlayerWeaponType.PISTOL:
				if (Input.GetMouseButtonDown (0) && attackTimer.IsFinished()) {

					Attack();
				}
			break;
		}

        /*
		if (Input.GetKeyDown (KeyCode.Alpha1))
			SetWeapon (PlayerWeaponType.KNIFE);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			SetWeapon (PlayerWeaponType.PISTOL);
        */

		attackTimer.UpdateTimer ();
		UpdateAim ();
	}

    private void FixedUpdate()
    {
        //Move();
        //Turn();
    }
    public void DamagePlayer(){
		//animator.SetBool ("Dead", true);
		//animator.transform.parent = null;
		this.enabled = false;
		myRigidBody.isKinematic = true;
        TP_Manager.RegisterPlayerDeath ();
		gameObject.GetComponent<Collider> ().enabled = false;
        TP_Cam.ToggleShake (0.3f);
		//Vector3 pos = animator.transform.position;
		//pos.y = 0.2f;
		//animator.transform.position = pos;
	}
	void UpdateAim(){

        Vector3 mousePosOffset = Input.mousePosition + new Vector3(0,0, camDist);   //id3644
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosOffset);   //id3644
        Vector3 mousePosRevision = new Vector3(mousePos.x + mousePos.y, transform.position.y, mousePos.z);
        mousePointer.transform.position = mousePosRevision;

        //Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        //mousePos.y = transform.position.y;
        //mousePointer.transform.position = mousePos;
        float deltaY = mousePos.z - transform.position.z;
		float deltaX = mousePos.x - transform.position.x;
		float angleInDegrees = Mathf.Atan2 (deltaY, deltaX) * 180 / Mathf.PI;
		//transform.eulerAngles = new Vector3 (0, -angleInDegrees, 0);

        turretTr.eulerAngles = new Vector3(0, -angleInDegrees + 90, 0);
    }
	public void Attack(){
		switch (currentWeapon) {
			case PlayerWeaponType.KNIFE:							
				Invoke ("DoHitTest",0.2f);				
			break;
			case PlayerWeaponType.PISTOL:
                TP_Cam.ToggleShake (0.1f);
				GameObject bullet=GameObject.Instantiate(proyectilePrefab, gunPivot.position,gunPivot.rotation) as GameObject;
				bullet.transform.LookAt(mousePointer.transform);
				bullet.transform.Rotate(0,Random.Range(-7.5f,7.5f),0);
				AlertEnemies();
			break;
		}
		//animator.SetBool ("Attack", true);
		CancelInvoke ("AttackOver");
		Invoke ("AttackOver", attackTime);
		attackTimer.StartTimer (attackTime);

	}
	void AlertEnemies(){
		RaycastHit[] hits=Physics.SphereCastAll (hitTestPivot.position,20.0f, hitTestPivot.up);
		foreach (RaycastHit hit in hits) {
			if (hit.collider != null && hit.collider.tag == "Enemy") {
				hit.collider.GetComponent<NPC_Enemy>().SetAlertPos(transform.position);
			}
		}
	}
	public void DoHitTest(){




		RaycastHit[] hits=Physics.SphereCastAll (hitTestPivot.position,2.0f, hitTestPivot.up);
		foreach(RaycastHit hit in hits){
			if (hit.collider!=null && hit.collider.tag == "Enemy") {
				RaycastHit forwarHit= new RaycastHit();
				Physics.Raycast(hitTestPivot.position,hit.transform.position-transform.position,out forwarHit);
				if (forwarHit.collider!=null && forwarHit.collider.tag == "Enemy") {
					forwarHit.collider.GetComponent<NPC_Enemy>().Damage();
				}
			}
		}
	}
	void AttackOver(){
		//animator.SetBool ("Attack", false);
	}
	
	void SetWeapon(PlayerWeaponType weaponType){
		if (weaponType != currentWeapon) {
			currentWeapon = weaponType;
			//animator.SetTrigger ("WeaponChange");
			switch (weaponType) {
			case PlayerWeaponType.KNIFE:
				attackTime=0.4f;
				//animator.SetInteger ("WeaponType", 0);
				break;
			case PlayerWeaponType.PISTOL:
				attackTime=0.1f;
				//animator.SetInteger ("WeaponType", 3);
				break;
			}
		}
        TP_Manager.SelectWeapon (weaponType);
	}

    private void Move()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        //Vector3 movement = transform.forward * m_MovementInputValue * moveSpeed * Time.deltaTime;
        Vector3 movement = transform.right * m_MovementInputValue * moveSpeed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        myRigidBody.MovePosition(myRigidBody.position + movement);
    }

    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        myRigidBody.MoveRotation(myRigidBody.rotation * turnRotation);
    }
}
