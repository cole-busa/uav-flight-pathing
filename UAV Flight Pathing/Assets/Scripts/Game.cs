using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Game : MonoBehaviour {
        private GameState gameState;
        private Drone drone;
        // Start is called before the first frame update
        void Start() {
            this.gameState = new GameState();
            this.drone = new Drone();
            Debug.Log("start");
            Debug.Log(drone.getPosX());
            Debug.Log(gameState.getState());
        }

        // Update is called once per frame
        void Update() {
            if (gameState.getState() == "GAME_ACTIVE") {
                Debug.Log("update");
                Debug.Log(drone.getPosX());
                Debug.Log(gameState.getState());
                gameState.setState("");
            }
        }
    }
}


