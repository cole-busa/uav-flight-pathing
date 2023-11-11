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

        void ResetIteration(string state) {
            gameState = new GameState(10, 10, state);
            drone = new Drone();
            moveCount = 0;
        }
        // Start is called before the first frame update
        void Start() {
            ResetIteration("GAME_UNINFORMED");
            movesPerIteration = new ArrayList();
            iterations = 0;
            maxIterations = 10;
        }

        int findAverageMoves() {
            int sum = 0;
            foreach (int i in movesPerIteration) {
                sum += i;
            }
            int averageMoves = sum / movesPerIteration.Count;
            return averageMoves;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (gameState.getState() == "GAME_UNINFORMED") {
                if (iterations == maxIterations) {
                    int averageMoves = findAverageMoves();
                    Debug.Log("THE UNINFORMED DRONE FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                    ResetIteration("GAME_INFORMED");
                    movesPerIteration = new ArrayList();
                    iterations = 0;
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
                        ResetIteration("GAME_UNINFORMED");
                        iterations++;
                    }
                }
            } else if (gameState.getState() == "GAME_INFORMED") {
                if (iterations == maxIterations) {
                    int averageMoves = findAverageMoves();
                    Debug.Log("THE INFORMED DRONE FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
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
                        ResetIteration("GAME_INFORMED");
                        iterations++;
                    }
                }
            } if (gameState.getState() == "GAME_WIN") {
                gameState.setState("GAME_OVER");
            }
        }
    }
}


