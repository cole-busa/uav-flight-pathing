using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Game : MonoBehaviour {
        private GameState gameState;
        private Drone drone;
        private int moveCount;
        // Start is called before the first frame update
        void Start() {
            this.gameState = new GameState();
            this.drone = new Drone();
            Debug.Log("start");
            Debug.Log(drone.getPosX());
            Debug.Log(gameState.getState());
            this.moveCount = 0;
        }

        // Update is called once per frame
        void Update() {
            if (gameState.getState() == "GAME_ACTIVE") {
                int[,] map = gameState.getMap();
                bool[,] explored = gameState.getExplored();
                ArrayList adjacent = drone.adjacent(map);
                ArrayList move = new ArrayList();
                (int x, int y) decision = (0, 0);
                foreach ((int x, int y) pos in adjacent) {
                    if (map[pos.y, pos.x] == 1) {
                        move.Clear();
                        move.Add(pos);
                        break;
                    } else if (!explored[pos.y, pos.x]) {
                        move.Add(pos);
                    }
                }
                decision = ((int x, int y))move[Random.Range(0, move.Count)];
                drone.move(decision);
                explored[decision.y, decision.x] = true;
                Debug.Log("MOVE TO " + decision);
                if (map[decision.y, decision.x] == 1) {
                    gameState.setState("GAME_WIN");
                }
                moveCount++;
            } else if (gameState.getState() == "GAME_WIN") {
                Debug.Log("YOU WIN IN " + moveCount + " MOVES!");
                gameState.setState("GAME_OVER");
            }
        }
    }
}


