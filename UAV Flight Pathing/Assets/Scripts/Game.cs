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


        // Start is called before the first frame update
        void Start() {
            width = 10;
            height = 10;
            gameState = new GameState(width, height, "GAME_SOLO_UNINFORMED");
            drone = new Drone(width, height);
            moveCount = 0;
            movesPerIteration = new ArrayList();
            drones = new ArrayList();
            iterations = 0;
            maxIterations = 10;
            droneCount = 4;
            start = true;
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (gameState.getState() == "GAME_SOLO_UNINFORMED") {
                soloUninformedGame();
            } else if (gameState.getState() == "GAME_MULTI_ORIGIN_UNINFORMED") {
                multiUninformedOriginGame();
            } else if (gameState.getState() == "GAME_MULTI_CORNER_UNINFORMED") {
                multiUninformedCornerGame();
            } else if (gameState.getState() == "GAME_MULTI_ORIGIN_PERFECTLY_INFORMED") {
                multiPerfectlyInformedOriginGame();
            } else if (gameState.getState() == "GAME_MULTI_ORIGIN_DECENTLY_INFORMED") {
                multiDecentlyInformedOriginGame();
            } else if (gameState.getState() == "GAME_WIN") {
                gameState.setState("GAME_OVER");
            }
        }

        //Helper function to calculate the average moves over all iterations
        int findAverageMoves() {
            int sum = 0;
            foreach (int i in movesPerIteration) {
                sum += i;
            }
            int averageMoves = sum / movesPerIteration.Count;
            return averageMoves;
        }

        //Helper function to reset the current iteration
        void ResetIteration(string state) {
            movesPerIteration.Add(moveCount);
            gameState = new GameState(width, height, state);
            if (state == "GAME_SOLO_UNINFORMED") {
                drone = new Drone(width, height);
            } else if (state.Contains("ORIGIN")) {
                for (int i = 0; i < drones.Count; i++) {
                    drones[i] = new Drone(width, height);
                }
            } else if (state.Contains("CORNER")) {
                drones[0] = new Drone(width, height, 0, 0);
                drones[1] = new Drone(width, height, width - 1, 0);
                drones[2] = new Drone(width, height, 0, height - 1);
                drones[3] = new Drone(width, height, width - 1, height - 1);
            }
            moveCount = 0;
            iterations++;
        }

        //The Solo Uninformed scenario involves one drone that starts at (0, 0)
        //and chooses randomly between adjacent tiles, only avoiding those already explored.
        void soloUninformedGame() {
            if (iterations == maxIterations) {
                int averageMoves = findAverageMoves();
                Debug.Log("THE UNINFORMED DRONE AT ORIGIN FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                ResetIteration("GAME_MULTI_ORIGIN_UNINFORMED");
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
                    ResetIteration("GAME_SOLO_UNINFORMED");
                }
            }
        }

        //The Multi Origin Uninformed scenario involves four drones that all start at (0, 0)
        //and choose randomly between adjacent tiles, only avoiding those each has explored (no shared memory).
        void multiUninformedOriginGame() {
            if (start) {
                start = false;
                for (int i = 0; i < droneCount; i++) {
                    Drone n = new Drone(width, height);
                    drones.Add(n);
                }
            }
            if (iterations == maxIterations) {
                int averageMoves = findAverageMoves();
                Debug.Log("THE UNINFORMED DRONES AT ORIGIN FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                gameState.setState("GAME_MULTI_CORNER_UNINFORMED");
                movesPerIteration = new ArrayList();
                iterations = 0;
                drones[0] = new Drone(width, height, 0, 0);
                drones[1] = new Drone(width, height, width - 1, 0);
                drones[2] = new Drone(width, height, 0, height - 1);
                drones[3] = new Drone(width, height, width - 1, height - 1);
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
                        ResetIteration("GAME_MULTI_ORIGIN_UNINFORMED");
                        break;
                    }
                }
            }
        }

        //The Multi Corner Uninformed scenario involves four drones that start at each of the corners
        //and choose randomly between adjacent tiles, only avoiding those each has explored (no shared memory).
        void multiUninformedCornerGame() {
            if (iterations == maxIterations) {
                int averageMoves = findAverageMoves();
                Debug.Log("THE UNINFORMED DRONES AT CORNERS FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                gameState.setState("GAME_MULTI_ORIGIN_PERFECTLY_INFORMED");
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
                        ResetIteration("GAME_MULTI_CORNER_UNINFORMED");
                        break;
                    }
                }
            }
        }

        //The Multi Origin Perfect Informed scenario involves four drones that all start at (0, 0)
        //and choose between adjacent tiles based on the perfect Manhattan Distance of each tile 
        //and the goal. They also avoid tiles all of them have explored (shared memory).
        void multiPerfectlyInformedOriginGame() {
            if (iterations == maxIterations) {
                int averageMoves = findAverageMoves();
                Debug.Log("THE PERFECTLY INFORMED DRONES FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
                gameState.setState("GAME_MULTI_ORIGIN_DECENTLY_INFORMED");
                movesPerIteration = new ArrayList();
                iterations = 0;
                for (int i = 0; i < drones.Count; i++) {
                    drones[i] = new Drone(width, height);
                }
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
                        ResetIteration("GAME_MULTI_ORIGIN_PERFECTLY_INFORMED");
                        break;
                    }
                }
            }
        }


        //The Multi Origin Decently Informed scenario involves four drones that all start at (0, 0)
        //and choose between adjacent tiles based on the decent Manhattan Distance (plus or minus random noise) 
        //of each tile and the goal. They also avoid tiles all of them have explored (shared memory).
        void multiDecentlyInformedOriginGame() {
            if (iterations == maxIterations) {
                int averageMoves = findAverageMoves();
                Debug.Log("THE DECENTLY INFORMED DRONES FOUND THE GOAL IN " + averageMoves + " AVERAGE MOVES!");
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
                        ResetIteration("GAME_MULTI_ORIGIN_DECENTLY_INFORMED");
                        break;
                    }
                }
            }
        }
    }
}


