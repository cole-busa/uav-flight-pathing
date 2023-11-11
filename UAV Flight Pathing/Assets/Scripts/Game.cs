using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Game : MonoBehaviour {
        private GameState gameState;
        private Drone drone;
        private int moveCount;
        private int iterations;
        private int maxIterations;
        private ArrayList movesPerIteration;
        // Start is called before the first frame update
        void Start() {
            this.gameState = new GameState(10, 10);
            this.drone = new Drone();
            this.movesPerIteration = new ArrayList();
            this.moveCount = 0;
            this.iterations = 0;
            this.maxIterations = 10;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (gameState.getState() == "GAME_UNINFORMED") {
                if (iterations == maxIterations) {
                    gameState.setState("GAME_WIN");
                } else {
                    int[,] map = gameState.getMap();
                    bool[,] explored = gameState.getExplored();
                    float[,] heuristics = gameState.getHeuristics();
                    ArrayList adjacent = drone.adjacent(map);
                    (int x, int y) decision = (0, 0);
                    decision = drone.findMove(map, explored, heuristics, adjacent);
                    drone.move(decision);
                    explored[decision.y, decision.x] = true;
                    moveCount++;
                    if (map[decision.y, decision.x] == 1) {
                        movesPerIteration.Add(moveCount);
                        moveCount = 0;
                        gameState = new GameState(10, 10);
                        drone = new Drone();
                        iterations++;
                    }
                }
            } else if (gameState.getState() == "GAME_WIN") {
                int sum = 0;
                foreach (int i in movesPerIteration) {
                    sum += i;
                }
                int averageMoves = sum / movesPerIteration.Count;
                Debug.Log("THE DRONE FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                gameState.setState("GAME_OVER");
            }
        }
    }
}


