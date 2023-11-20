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
        private ArrayList states;
        private int stateIndex;
        private int width;
        private int height;
        private Transform[] linkedObjects;
        private float unitWidth;
        private float unitHeight;
        private bool withGraphics;
        private bool renderingSolo;
        private bool renderingMulti;
        private Vector3 unrendered;
        private Vector3 spawn;
        private Vector3 topLeftCorner;
        private Vector3 topRightCorner;
        private Vector3 bottomLeftCorner;
        private Vector3 bottomRightCorner;


        // Start is called before the first frame update
        void Start() {
            //Game settings
            width = 20;
            height = 20;
            unitWidth = 19f / width;
            unitHeight = 7.6f / height;
            droneCount = 4;
            maxIterations = 10;
            withGraphics = true;

            //Game initialization
            gameState = new GameState(width, height, "GAME_SOLO_UNINFORMED");
            drone = new Drone(width, height);
            drones = new ArrayList();
            for (int i = 0; i < droneCount; i++) {
                Drone n = new Drone(width, height);
                drones.Add(n);
            }
            movesPerIteration = new ArrayList();
            moveCount = 0;
            iterations = 0;
            renderingSolo = true;
            renderingMulti = false;

            stateIndex = 0;
            states = new ArrayList();

            unrendered = new Vector3(90f, 90f, 0f);
            spawn = new Vector3(0f, 0f, 0f);
            topLeftCorner = new Vector3(-9.5f, 3.8f, 0f);
            topRightCorner = new Vector3(9.5f, 3.8f, 0f);
            bottomLeftCorner = new Vector3(-9.5f, -3.8f, 0f);
            bottomRightCorner = new Vector3(-9.5f, -3.8f, 0f);

            linkedObjects = new Transform[4];
            linkedObjects[0] = GameObject.Find("drone 1").transform;
            linkedObjects[1] = GameObject.Find("drone 2").transform;
            linkedObjects[2] = GameObject.Find("drone 3").transform;
            linkedObjects[3] = GameObject.Find("drone 4").transform;

            linkedObjects[1].position = unrendered;
            linkedObjects[2].position = unrendered;
            linkedObjects[3].position = unrendered;


            //The Solo Uninformed scenario involves one drone that starts at (0, 0)
            //and chooses randomly between adjacent tiles, only avoiding those already explored.
            states.Add("GAME_SOLO_UNINFORMED");

            //The Multi Origin Uninformed scenario involves four drones that all start at (0, 0)
            //and choose randomly between adjacent tiles, only avoiding those each has explored (no shared memory).
            states.Add("GAME_MULTI_ORIGIN_UNINFORMED");

            //The Multi Corner Uninformed scenario involves four drones that start at each of the corners
            //and choose randomly between adjacent tiles, only avoiding those each has explored (no shared memory).
            states.Add("GAME_MULTI_CORNER_UNINFORMED");

            //The Multi Random Uninformed scenario involves four drones that start at random tiles
            //and choose randomly between adjacent tiles, only avoiding those each has explored (no shared memory).
            states.Add("GAME_MULTI_RANDOM_UNINFORMED");

            //The Multi Random Quadrant Uninformed scenario involves four drones that start at random spots in each of the four quadrants
            //and choose randomly between adjacent tiles, only avoiding those each has explored (no shared memory).
            states.Add("GAME_MULTI_RANDOM_QUADRANT_UNINFORMED");

            //The Multi Origin Manhattan Informed scenario involves four drones that all start at (0, 0)
            //and choose between adjacent tiles based on the perfect Manhattan Distance of each tile 
            //and the goal. They also avoid tiles all of them have explored (shared memory).
            states.Add("GAME_MULTI_ORIGIN_MANHATTAN_INFORMED");

            //The Multi Origin Euclidean Informed scenario involves four drones that all start at (0, 0)
            //and choose between adjacent tiles based on the perfect Euclidean Distance of each tile 
            //and the goal. They also avoid tiles all of them have explored (shared memory).
            states.Add("GAME_MULTI_ORIGIN_EUCLIDEAN_INFORMED");

            //The Multi Corner Perfectly Informed scenario involves four drones that start at each of the corners
            //and choose between adjacent tiles based on the perfect Euclidean Distance of each tile 
            //and the goal. They also avoid tiles all of them have explored (shared memory).
            states.Add("GAME_MULTI_CORNER_PERFECTLY_INFORMED");

            //The Multi Corner Decently Informed scenario involves four drones that start at each of the corners
            //and choose between adjacent tiles based on the decent Euclidean Distance (plus or minus random noise) 
            //of each tile and the goal. They also avoid tiles all of them have explored (shared memory).
            states.Add("GAME_MULTI_CORNER_DECENTLY_INFORMED");

            //The Multi Corner Badly Informed scenario involves four drones that start at each of the corners
            //and choose between adjacent tiles based on the perfect Euclidean Distance of each tile
            //and a random tile chosen at runtime. They also avoid tiles all of them have explored (shared memory).
            states.Add("GAME_MULTI_CORNER_BADLY_INFORMED");
            states.Add("GAME_WIN");
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (withGraphics) {
                if (renderingSolo) {
                    Vector3 pos1 = new Vector3(drone.getPosX() * unitWidth - 9.5f, drone.getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[0].position = Vector3.MoveTowards(linkedObjects[0].position, pos1, 5f * Time.deltaTime);

                    if (pos1 == linkedObjects[0].position) {
                        renderingSolo = false;
                    }
                } else if (renderingMulti) {
                    //Top left corner
                    Vector3 pos1 = bottomLeftCorner;
                    linkedObjects[0].position = Vector3.MoveTowards(linkedObjects[0].position, pos1, 1f * Time.deltaTime);

                    //Top right corner
                    Vector3 pos2 = topRightCorner;
                    linkedObjects[1].position = Vector3.MoveTowards(linkedObjects[1].position, pos2, 1f * Time.deltaTime);

                    //Bottom left corner
                    Vector3 pos3 = bottomLeftCorner;
                    linkedObjects[2].position = Vector3.MoveTowards(linkedObjects[2].position, pos3, 1f * Time.deltaTime);

                    //Bottom right corner
                    Vector3 pos4 = bottomRightCorner;
                    linkedObjects[3].position = Vector3.MoveTowards(linkedObjects[3].position, pos4, 1f * Time.deltaTime);

                    if (pos1 == linkedObjects[0].position && pos2 == linkedObjects[1].position
                        && pos3 == linkedObjects[2].position && pos4 == linkedObjects[3].position) {
                        renderingMulti = false;
                    }
                }
            }
            if (!renderingSolo && !renderingMulti) {
                string state = gameState.getState();
                GenericGame(state);
            }
        }

        //Helper function to calculate the average moves over all iterations
        int FindAverageMoves() {
            int sum = 0;
            foreach (int i in movesPerIteration) {
                sum += i;
            }
            int averageMoves = sum / movesPerIteration.Count;
            return averageMoves;
        }

        //Helper function to reset the current iteration
        void ResetIteration(string state, bool hardReset) {
            if (hardReset) {
                movesPerIteration = new ArrayList();
                iterations = 0;
            } else {
                movesPerIteration.Add(moveCount);
                iterations++;
            }
            moveCount = 0;
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
            } else if (state.Contains("RANDOM")) {
                if (state.Contains("QUADRANT")) {
                    drones[0] = new Drone(width, height, Random.Range(0, width / 2), Random.Range(0, height / 2));
                    drones[1] = new Drone(width, height, Random.Range(width / 2, width), Random.Range(0, height / 2));
                    drones[2] = new Drone(width, height, Random.Range(0, width / 2), Random.Range(height / 2, height));
                    drones[3] = new Drone(width, height, Random.Range(width / 2, width), Random.Range(height / 2, height));
                } else {
                    for (int i = 0; i < drones.Count; i++) {
                        drones[i] = new Drone(width, height, Random.Range(0, width), Random.Range(0, height));
                    }
                }
            }
        }

        //Solo game helper function that decides the moves of one drone
        void PlaySoloGame() {
            moveCount++;

            int[,] map = gameState.getMap();
            bool[,] explored = gameState.getGlobalExplored();
            float[,] heuristics = gameState.getHeuristics();

            ArrayList adjacent = drone.adjacent(map);
            (int x, int y) decision = (0, 0);
            decision = drone.findMove(map, explored, heuristics, adjacent);
            drone.move(decision);
            explored[decision.y, decision.x] = true;

            if (map[decision.y, decision.x] == 1) {
                ResetIteration("GAME_SOLO_UNINFORMED", false);
            }
            renderingSolo = true;
        }

        //Multi game helper function that decides the moves of four drones
        void PlayMultiGame(string state) {
            moveCount++;

            int[,] map = gameState.getMap();
            bool[,] explored = gameState.getGlobalExplored();
            float[,] heuristics = gameState.getHeuristics();

            foreach (Drone d in drones) {
                if (state.Contains("UNINFORMED"))
                    explored = d.getExplored();

                ArrayList adjacent = d.adjacent(map);
                (int x, int y) decision = (0, 0);
                decision = d.findMove(map, explored, heuristics, adjacent);
                d.move(decision);
                explored[decision.y, decision.x] = true;

                if (map[decision.y, decision.x] == 1) {
                    ResetIteration(state, false);
                    return;
                }
            }
        }

        void RenderSoloGame() {

        }

        void RenderMultiGame() {

        }

        //Generic game helper function that deals with resetting iterations and telling which game to play
        void GenericGame(string state) {
            if (iterations == maxIterations) {
                if (state == "GAME_WIN")
                    return;
                int averageMoves = FindAverageMoves();
                Debug.Log("THE " + state + " SCENARIO TOOK " + averageMoves + " AVERAGE MOVES!");
                stateIndex++;
                ResetIteration((string)states[stateIndex], true);
                return;
            }
            if (state.Contains("SOLO"))
                PlaySoloGame();
            else
                PlayMultiGame(state);
        }
    }
}


