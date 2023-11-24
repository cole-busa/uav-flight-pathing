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
        private Transform goal;
        private float unitWidth;
        private float unitHeight;
        private float moveSpeed;
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
            width = 10;
            height = 10;
            unitWidth = 19f / width;
            unitHeight = 7.6f / height;
            droneCount = 4;
            maxIterations = 1;
            withGraphics = true;
            moveSpeed = 5f;

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
            bottomRightCorner = new Vector3(9.5f, -3.8f, 0f);

            goal = GameObject.Find("person").transform;
            goal.position = new Vector3(gameState.getGoal().x * unitWidth - 9.5f, gameState.getGoal().y * unitHeight - 3.8f, 0f);

            linkedObjects = new Transform[4];
            linkedObjects[0] = GameObject.Find("drone 1").transform;
            linkedObjects[1] = GameObject.Find("drone 2").transform;
            linkedObjects[2] = GameObject.Find("drone 3").transform;
            linkedObjects[3] = GameObject.Find("drone 4").transform;
            linkedObjects[0].position = bottomLeftCorner;
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
                    linkedObjects[0].position = Vector3.MoveTowards(linkedObjects[0].position, pos1, moveSpeed * Time.deltaTime);

                    if (pos1 == linkedObjects[0].position) {
                        if (iterations == maxIterations) {
                            renderingMulti = true;
                            
                            linkedObjects[0].position = bottomLeftCorner;
                            linkedObjects[1].position = bottomLeftCorner;
                            linkedObjects[2].position = bottomLeftCorner;
                            linkedObjects[3].position = bottomLeftCorner;
                        }
                        renderingSolo = false;
                    }
                } 
                if (renderingMulti) {
                    //Drone 1
                    Vector3 pos1 = new Vector3(((Drone) drones[0]).getPosX() * unitWidth - 9.5f, ((Drone) drones[0]).getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[0].position = Vector3.MoveTowards(linkedObjects[0].position, pos1, moveSpeed * Time.deltaTime);

                    //Drone 2
                    Vector3 pos2 = new Vector3(((Drone)drones[1]).getPosX() * unitWidth - 9.5f, ((Drone)drones[1]).getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[1].position = Vector3.MoveTowards(linkedObjects[1].position, pos2, moveSpeed * Time.deltaTime);

                    //Drone 3
                    Vector3 pos3 = new Vector3(((Drone)drones[2]).getPosX() * unitWidth - 9.5f, ((Drone)drones[2]).getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[2].position = Vector3.MoveTowards(linkedObjects[2].position, pos3, moveSpeed * Time.deltaTime);

                    //Drone 4
                    Vector3 pos4 = new Vector3(((Drone)drones[3]).getPosX() * unitWidth - 9.5f, ((Drone)drones[3]).getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[3].position = Vector3.MoveTowards(linkedObjects[3].position, pos4, moveSpeed * Time.deltaTime);

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
            goal.position = new Vector3(gameState.getGoal().x * unitWidth - 9.5f, gameState.getGoal().y * unitHeight - 3.8f, 0f);
            if (state == "GAME_SOLO_UNINFORMED") {
                drone = new Drone(width, height);

                linkedObjects[0].position = bottomLeftCorner;
            } else if (state.Contains("ORIGIN")) {
                for (int i = 0; i < drones.Count; i++) {
                    drones[i] = new Drone(width, height);

                    linkedObjects[i].position = bottomLeftCorner;
                }
            } else if (state.Contains("CORNER")) {
                drones[0] = new Drone(width, height, 0, 0);
                drones[1] = new Drone(width, height, width - 1, 0);
                drones[2] = new Drone(width, height, 0, height - 1);
                drones[3] = new Drone(width, height, width - 1, height - 1);

                linkedObjects[0].position = bottomLeftCorner;
                linkedObjects[1].position = bottomRightCorner;
                linkedObjects[2].position = topLeftCorner;
                linkedObjects[3].position = topRightCorner;
            } else if (state.Contains("RANDOM")) {
                if (state.Contains("QUADRANT")) {
                    drones[0] = new Drone(width, height, Random.Range(0, width / 2), Random.Range(0, height / 2));
                    drones[1] = new Drone(width, height, Random.Range(width / 2, width), Random.Range(0, height / 2));
                    drones[2] = new Drone(width, height, Random.Range(0, width / 2), Random.Range(height / 2, height));
                    drones[3] = new Drone(width, height, Random.Range(width / 2, width), Random.Range(height / 2, height));

                    linkedObjects[0].position = new Vector3(((Drone)drones[0]).getPosX() * unitWidth - 9.5f, ((Drone)drones[0]).getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[1].position = new Vector3(((Drone)drones[1]).getPosX() * unitWidth - 9.5f, ((Drone)drones[1]).getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[2].position = new Vector3(((Drone)drones[2]).getPosX() * unitWidth - 9.5f, ((Drone)drones[2]).getPosY() * unitHeight - 3.8f, 0f);
                    linkedObjects[3].position = new Vector3(((Drone)drones[3]).getPosX() * unitWidth - 9.5f, ((Drone)drones[3]).getPosY() * unitHeight - 3.8f, 0f);
                } else {
                    for (int i = 0; i < drones.Count; i++) {
                        drones[i] = new Drone(width, height, Random.Range(0, width), Random.Range(0, height));

                        linkedObjects[i].position = new Vector3(((Drone)drones[i]).getPosX() * unitWidth - 9.5f, ((Drone)drones[i]).getPosY() * unitHeight - 3.8f, 0f);
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
            renderingMulti = true;
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
                renderingMulti = true;
                return;
            }
            if (state.Contains("SOLO"))
                PlaySoloGame();
            else
                PlayMultiGame(state);
        }
    }
}


