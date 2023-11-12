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
        private int droneCount;
        private ArrayList movesPerIteration;
        private ArrayList drones;
        private bool start;
        private int width;
        private int height;

        void ResetIteration(string state) {
            gameState = new GameState(width, height, state);
            drone = new Drone(width, height);
            moveCount = 0;
        }
        // Start is called before the first frame update
        void Start() {
            width = 20;
            height = 20;
            ResetIteration("GAME_SOLO_UNINFORMED");
            movesPerIteration = new ArrayList();
            drones = new ArrayList();
            iterations = 0;
            maxIterations = 10;
            droneCount = 4;
            start = true;
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
            if (gameState.getState() == "GAME_SOLO_UNINFORMED") {
                if (iterations == maxIterations) {
                    int averageMoves = findAverageMoves();
                    Debug.Log("THE UNINFORMED DRONE FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                    ResetIteration("GAME_MULTI_UNINFORMED");
                    movesPerIteration = new ArrayList();
                    iterations = 0;
                } else {
                    int[,] map = gameState.getMap();
                    bool[,] explored = gameState.getGlobalExplored();
                    float[,] heuristics = gameState.getHeuristics();
                    ArrayList adjacent = drone.adjacent(map);
                    (int x, int y) decision = (0, 0);
                    decision = drone.findMove(map, explored, heuristics, adjacent);
                    drone.move(decision);
                    explored[decision.y, decision.x] = true;
                    moveCount++;
                    if (map[decision.y, decision.x] == 1) {
                        movesPerIteration.Add(moveCount);
                        ResetIteration("GAME_SOLO_UNINFORMED");
                        iterations++;
                    }
                }
            } else if (gameState.getState() == "GAME_MULTI_UNINFORMED") {
                if (start) {
                    start = false;
                    for (int i = 0; i < droneCount; i++) {
                        Drone n = new Drone(width, height);
                        drones.Add(n);
                    }
                }
                if (iterations == maxIterations) {
                    int averageMoves = findAverageMoves();
                    Debug.Log("THE UNINFORMED DRONES FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                    gameState.setState("GAME_MULTI_INFORMED");
                    movesPerIteration = new ArrayList();
                    iterations = 0;
                    for (int i = 0; i < drones.Count; i++) {
                        drones[i] = new Drone(width, height);
                    }
                } else {
                    int[,] map = gameState.getMap();
                    float[,] heuristics = gameState.getHeuristics();
                    moveCount++;
                    foreach (Drone d in drones) {
                        bool[,] explored = d.getExplored();
                        ArrayList adjacent = d.adjacent(map);
                        (int x, int y) decision = (0, 0);
                        decision = d.findMove(map, explored, heuristics, adjacent);
                        d.move(decision);
                        explored[decision.y, decision.x] = true;
                        if (map[decision.y, decision.x] == 1) {
                            movesPerIteration.Add(moveCount);
                            ResetIteration("GAME_MULTI_UNINFORMED");
                            for (int i = 0; i < drones.Count; i++) {
                                drones[i] = new Drone(width, height);
                            }
                            iterations++;
                            break;
                        }
                    }
                }
            } else if (gameState.getState() == "GAME_MULTI_INFORMED") {
                if (iterations == maxIterations) {
                    int averageMoves = findAverageMoves();
                    Debug.Log("THE INFORMED DRONES FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                    gameState.setState("GAME_WIN");
                } else {
                    int[,] map = gameState.getMap();
                    bool[,] explored = gameState.getGlobalExplored();
                    float[,] heuristics = gameState.getHeuristics();
                    moveCount++;
                    foreach (Drone d in drones) {
                        ArrayList adjacent = d.adjacent(map);
                        (int x, int y) decision = (0, 0);
                        decision = d.findMove(map, explored, heuristics, adjacent);
                        d.move(decision);
                        explored[decision.y, decision.x] = true;
                        if (map[decision.y, decision.x] == 1) {
                            movesPerIteration.Add(moveCount);
                            ResetIteration("GAME_MULTI_INFORMED");
                            iterations++;
                            for (int i = 0; i < drones.Count; i++) {
                                drones[i] = new Drone(width, height);
                            }
                            break;
                        }
                    }
                }
            } if (gameState.getState() == "GAME_WIN") {
                gameState.setState("GAME_OVER");
            }
        }
    }
}


