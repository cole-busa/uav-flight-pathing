using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Game : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            GameState gameState = new GameState();
            Debug.Log(gameState.test());
        }

        // Update is called once per frame
        void Update() {

        }
    }
}


