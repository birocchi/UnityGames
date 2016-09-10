using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour {
    public AudioSource MusicSound;

    [SerializeField]
    KeyCode SpeedUp = KeyCode.Space;
    [SerializeField]
    KeyCode SpeedDown = KeyCode.C;
    [SerializeField]
    KeyCode Forward = KeyCode.W;
    [SerializeField]
    KeyCode Back = KeyCode.S;
    [SerializeField]
    KeyCode Left = KeyCode.A;
    [SerializeField]
    KeyCode Right = KeyCode.D;
    [SerializeField]
    KeyCode TurnLeft = KeyCode.Q;
    [SerializeField]
    KeyCode TurnRight = KeyCode.E;
    [SerializeField]
    KeyCode MusicOffOn = KeyCode.M;
    
    private KeyCode[] availableKeyCodes;

	//Açao que deve ser tomada quando uma tecla e pressionada
    public Action<PressedKeyCode[]> KeyPressedAction;


    private void Awake() {

		//Cria uma lista de todas as teclas disponiveis para o jogador
        availableKeyCodes = new[] {
                            SpeedUp,
                            SpeedDown,
                            Forward,
                            Back,
                            Left,
                            Right,
                            TurnLeft,
                            TurnRight
                        };

    }

	void FixedUpdate() {

		//Lista de todos os botoes pressionados
		List<PressedKeyCode> pressedKeyCodes = new List<PressedKeyCode>();

		//Percorre toda a lista de botoes disponiveis, vendo se foram pressionados ou nao
	    for (int index = 0; index < availableKeyCodes.Length; index++) {
	        var keyCode = availableKeyCodes[index];
	        if (Input.GetKey(keyCode))
                pressedKeyCodes.Add((PressedKeyCode)index);
	    }

	    if (KeyPressedAction != null)
	        KeyPressedAction(pressedKeyCodes.ToArray());

        // for test
        if (Input.GetKey(MusicOffOn))
        {
           if (  MusicSound.volume == 0.5f) return;
/*            if (MusicSound.isPlaying)
                MusicSound.Stop();
            else*/
                MusicSound.volume = 0.5f;
                MusicSound.Play();
        }
      
	}
}
