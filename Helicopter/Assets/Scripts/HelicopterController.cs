using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour {

    public AudioSource HelicopterSound;
    public ControlPanel ControlPanel;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;

    public float TurnForce = 3f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float MaximumHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 1.3f;

	private WindZone windZone;

    private float engineForce;
    public float EngineForce {

        get { return engineForce; }
        set {
			//Alterar a forca do motor afeta os 2 rotores e o som da helice
            MainRotorController.RotorSpeed = value * 120;
            SubRotorController.RotorSpeed = value * 60;
            HelicopterSound.pitch = Mathf.Clamp(value / 20, 0, 1.2f);

            engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    private bool IsOnGround = true;
	private float originalDrag;


	void Start () {

		//Coloca o metodo "OnKeyPressed" como açao a ser tomada quando se pressiona um botao
        ControlPanel.KeyPressedAction += OnKeyPressed;

		originalDrag = HelicopterModel.drag;
		windZone = transform.FindChild("Windzone").GetComponent<WindZone>();
	}

	void Update () {

		//Confere se o helicoptero esta no chao
		RaycastHit hitInfo;
		if(Physics.Raycast(transform.position, -transform.up, out hitInfo, 2f)){
			if(hitInfo.transform.tag == "Terrain"){
				IsOnGround = true;
			}
		}
		else {
			IsOnGround = false;
		}
	}
  
    void FixedUpdate() {

		//Trata o processo de subida
        LiftProcess();

		//Trata a movimentacao lateral
        MoveProcess();

		//Trata a inclinaçao do helicoptero
        TiltProcess();
    }

	private void LiftProcess() {
		
		//A porcentagem da altura maxima "MaximumHeight" em que esta o helicoptero
		float heightLevel = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / MaximumHeight, 0, 1);
		
		//Forca do motor, levando em consideraçao a massa do helicoptero
		float upForce = Mathf.Lerp(0f, EngineForce, heightLevel) * HelicopterModel.mass;

		//Ativa ou desativa o vento
		if(engineForce <= 10){
			windZone.windTurbulence = engineForce/10 * 5;
			HelicopterModel.drag = 0;
		}
		else {
			windZone.windTurbulence = 5;
			HelicopterModel.drag = originalDrag;
		}
		
		//Aplicar a força para cima no helicoptero, em relacao ao seu eixo y local
		HelicopterModel.AddRelativeForce(Vector3.up * upForce);
		Debug.Log(engineForce);
	}

    private void MoveProcess() {
		//Move o helicoptero para frente e para tras
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));

		//TESTE de curva junto com inclinaçao
		//float turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
		//hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
		//HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
	}
	
	
	
    private void TiltProcess()
    {
		//Enclina o helicoptero para a direçao do movimento
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
		HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

	private void OnKeyPressed(PressedKeyCode[] obj) {
        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = - Time.fixedDeltaTime;
        else
            if (hMove.y < 0)
                tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else
            if (hMove.x < 0)
                tempX = Time.fixedDeltaTime;


        foreach (var pressedKeyCode in obj)
        {
            switch (pressedKeyCode)
            {
                case PressedKeyCode.SpeedUpPressed:
                    EngineForce += 0.1f;
                    break;

                case PressedKeyCode.SpeedDownPressed:
                    EngineForce -= 0.12f;
                    if (EngineForce < 0) EngineForce = 0;
                    break;

                case PressedKeyCode.ForwardPressed:
	                if (IsOnGround) break;
	                tempY = Time.fixedDeltaTime;
	                break;

                case PressedKeyCode.BackPressed:
	                if (IsOnGround) break;
	                tempY = -Time.fixedDeltaTime;
	                break;

                case PressedKeyCode.LeftPressed:
	                if (IsOnGround) break;
	                tempX = -Time.fixedDeltaTime;
	                break;

                case PressedKeyCode.RightPressed:
	                if (IsOnGround) break;
	                tempX = Time.fixedDeltaTime;
	                break;

                case PressedKeyCode.TurnRightPressed:
                {
					//Rotaciona o helicoptero no eixo Y
                    if (IsOnGround) break;
                    var force = (turnForcePercent - Mathf.Abs(hMove.y))*HelicopterModel.mass;
                    HelicopterModel.AddRelativeTorque(0f, force, 0);
                }
                break;

                case PressedKeyCode.TurnLeftPressed:
                {
                    if (IsOnGround) break;
                    
                    var force = -(turnForcePercent - Mathf.Abs(hMove.y))*HelicopterModel.mass;
                   HelicopterModel.AddRelativeTorque(0f, force, 0);
                }
                break;

            }
        }

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

    }
}