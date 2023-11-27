using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game {
    public class Game : MonoBehaviour {
        //Game settings variables
        private int width;
        private int height;
        private float unitWidth;
        private float unitHeight;
        private int droneCount;
        private int maxIterations;
        private bool withGraphics;
        private float moveSpeed;

        //Game core variables
        private GameState gameState;
        private Drone drone;
        private ArrayList drones;
        private ArrayList movesPerIteration;
        private int moveCount;
        private int iterations;
        private bool renderingSolo;
        private bool renderingMulti;
        
        //Locations on the map
        private Vector3 unrendered;
        private Vector3 spawn;
        private Vector3 topLeftCorner;
        private Vector3 topRightCorner;
        private Vector3 bottomLeftCorner;
        private Vector3 bottomRightCorner;
        
        //GameObject variables
        private Transform goal;
        private Transform[] linkedObjects;

        //Scenario variables
        private int stateIndex;
        private ArrayList states;
        

        // Start is called before the first frame update.
        void Start() {
            //Game settings.
            width = 30;
            height = 30;
            unitWidth = 19f / width;
            unitHeight = 7.6f / height;
            droneCount = 4;
            maxIterations = 1;
            withGraphics = true;
            moveSpeed = 10f;

            //Game initialization.
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

            //Positions that will be useful for moving GameObjects.
            unrendered = new Vector3(90f, 90f, 0f);
            spawn = new Vector3(0f, 0f, 0f);
            topLeftCorner = new Vector3(-9.5f, 3.8f, 0f);
            topRightCorner = new Vector3(9.5f, 3.8f, 0f);
            bottomLeftCorner = new Vector3(-9.5f, -3.8f, 0f);
            bottomRightCorner = new Vector3(9.5f, -3.8f, 0f);

            //Accessing the GameObjects and setting them to their initial positions if we want to show graphics.
            if (withGraphics) {
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
            }

            //Set up the states ArrayList that contains the list of scenarios to explore in order.
            stateIndex = 0;
            states = new ArrayList();

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

            //The Multi Corner Decently Informed Moving Goal scenario involves four drones that start at each of the corners
            //and choose between adjacent tiles based on the decent Euclidean Distance (plus or minus random noise) 
            //of each tile and the initial goal. They also avoid tiles all of them have explored (shared memory).
            //The goal will also start to move in after a random interval.
            states.Add("GAME_MULTI_CORNER_DECENTLY_INFORMED_MOVING_GOAL");


            //The Multi Quadrant Limited Corner Decently Informed Moving Goal scenario involves four drones that start at each of the corners
            //and choose between adjacent tiles based on the decent Euclidean Distance (plus or minus random noise) 
            //of each tile and the initial goal. Each drone will be confined to its own quadrant.
            //The goal will also start to move in after a random interval.
            states.Add("GAME_MULTI_QUADRANT_LIMITED_CORNER_DECENTLY_INFORMED_MOVING_GOAL");

            //The Multi Quadrant Limited Corner Decently Informed Information Decay Moving Goal scenario 
            //involves four drones that start at each of the corners and choose between adjacent tiles based 
            //on the decent Euclidean Distance (plus or minus random noise) of each tile and the initial goal 
            //if a random float between 0 and 1 is less than 1 / sqrt(moveCount). Otherwise it will choose randomly.
            //Each drone will be confined to its own quadrant. The goal will also start to move in after a random interval.
            states.Add("GAME_MULTI_QUADRANT_LIMITED_CORNER_DECENTLY_INFORMED_INFORMATION_DECAY_MOVING_GOAL");

            //Generic state for the end of the game.
            states.Add("GAME_WIN");
        }

        // Update is called once per frame
        void FixedUpdate() {
            //If graphics are enabled, render the respective games.
            if (withGraphics) {
                if (renderingSolo) {
                    RenderSoloGame();
                } 
                if (renderingMulti) {
                    RenderMultiGame();
                }
            }

            //If we are not currently moving drones, find the next move
            if (!renderingSolo && !renderingMulti) {
                string state = gameState.getState();
                GenericGame(state);
            }
        }

        void RenderSoloGame() {
            //Move the first drone GameObject toward the drone's actual position.
            Vector3 pos1 = new Vector3(drone.getPosX() * unitWidth - 9.5f, drone.getPosY() * unitHeight - 3.8f, 0f);
            linkedObjects[0].position = Vector3.MoveTowards(linkedObjects[0].position, pos1, moveSpeed * Time.deltaTime);

            //If the GameObject has reached its destination
            if (pos1 == linkedObjects[0].position) {
                //If we are at the goal, reset the current iteration.
                if (linkedObjects[0].position == goal.position) {
                    ResetIteration("GAME_SOLO_UNINFORMED", false);
                }
                //If we have reached the max number of iterations, move on to the multi game by rendering every drone.
                if (iterations == maxIterations) {
                    linkedObjects[0].position = bottomLeftCorner;
                    linkedObjects[1].position = bottomLeftCorner;
                    linkedObjects[2].position = bottomLeftCorner;
                    linkedObjects[3].position = bottomLeftCorner;
                }
                //Stop rendering and allow the game to decide the next move.
                renderingSolo = false;
            }
        }

        void RenderMultiGame() {
            //Move the drone GameObjects toward each drone's actual position.
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

            //If every drone has reached its destination
            if (pos1 == linkedObjects[0].position && pos2 == linkedObjects[1].position
                && pos3 == linkedObjects[2].position && pos4 == linkedObjects[3].position) {
                //If one of the drones is at the goal, reset the current iteration, stop rendering, and return.
                if (linkedObjects[0].position == goal.position || linkedObjects[1].position == goal.position
                || linkedObjects[2].position == goal.position || linkedObjects[3].position == goal.position) {
                    ResetIteration(gameState.getState(), false);
                    renderingMulti = false;
                    return;
                }

                //If we are in the Moving Goal scenario, move the goal.
                if (gameState.getState().Contains("MOVING_GOAL")) {
                    Vector3 pos0 = new Vector3(drone.getPosX() * unitWidth - 9.5f, drone.getPosY() * unitHeight - 3.8f, 0f);
                    goal.position = Vector3.MoveTowards(goal.position, pos0, moveSpeed * Time.deltaTime);

                    if (pos0 == goal.position) {
                        //Stop rendering and allow the game to decide the next move.
                        renderingMulti = false;
                    }
                } else {
                    //Stop rendering and allow the game to decide the next move.
                    renderingMulti = false;
                }
            }
        }

        //Generic game helper function that deals with resetting iterations and telling which game to play
        void GenericGame(string state) {
            //If the game is over, return
            if (state == "GAME_WIN")
                    return;

            //If we are at the max iterations
            if (iterations == maxIterations) {
                //Calculate the average moves over the scenario and print them to the console.
                int averageMoves = FindAverageMoves();
                Debug.Log("THE " + state + " SCENARIO TOOK " + averageMoves + " AVERAGE MOVES!");

                //Move on to the next state by hard resetting.
                stateIndex++;
                ResetIteration((string)states[stateIndex], true);
                
                //Turn on rendering and return.
                renderingMulti = true;
                return;
            }

            //Play the game depending on the state.
            if (state.Contains("SOLO"))
                PlaySoloGame();
            else
                PlayMultiGame(state);
        }

        //Helper function to calculate the average moves over all iterations
        int FindAverageMoves() {
            int sum = 0;

            foreach (int i in movesPerIteration)
                sum += i;

            int averageMoves = sum / movesPerIteration.Count;
            return averageMoves;
        }

        //Helper function to reset the current iteration
        void ResetIteration(string state, bool hardReset) {
            if (state == "GAME_WIN") {
                //If the game is over, move all GameObjects to the unrendered position if we are using graphics. Return either way.
                if (withGraphics) {
                    goal.position = unrendered;
                    linkedObjects[0].position = unrendered;
                    linkedObjects[1].position = unrendered;
                    linkedObjects[2].position = unrendered;
                    linkedObjects[3].position = unrendered;
                }
                return;
            }

            if (hardReset) {
                //If we are doing a hard reset, empty the list of moves and set iterations back to zero.
                movesPerIteration.Clear();
                iterations = 0;
            } else {
                //Otherwise, add the current move count to the ArrayList and increment the iteration count.
                movesPerIteration.Add(moveCount);
                iterations++;
            }

            //Reset the move count and the game state object, which will create a new goal position.
            moveCount = 0;
            gameState = new GameState(width, height, state);

            //Move the goal to the new goal position.
            if (withGraphics)
                goal.position = new Vector3(gameState.getGoal().x * unitWidth - 9.5f, gameState.getGoal().y * unitHeight - 3.8f, 0f);

            //If statement for determining spawn locations for the drones.
            if (state == "GAME_SOLO_UNINFORMED") {
                //In the solo uninformed scenario, we want to move the drone to the origin.
                drone = new Drone(width, height);
                
                //Move the GameObject too if graphics are enabled.
                if (withGraphics) 
                    linkedObjects[0].position = bottomLeftCorner;

            } else if (state.Contains("ORIGIN")) {
                for (int i = 0; i < drones.Count; i++) {
                    //In any multi origin scenario, we want to move the drones to the origin.
                    drones[i] = new Drone(width, height);

                    //Move the GameObjects too if graphics are enabled.
                    if (withGraphics)
                        linkedObjects[i].position = bottomLeftCorner;
                }
            } else if (state.Contains("CORNER")) {
                //In any multi corner scenario, we want to move the drones to the corners.
                drones[0] = new Drone(width, height, 0, 0);
                drones[1] = new Drone(width, height, width - 1, 0);
                drones[2] = new Drone(width, height, 0, height - 1);
                drones[3] = new Drone(width, height, width - 1, height - 1);

                if (withGraphics) {
                    //Move the GameObjects too if graphics are enabled.
                    linkedObjects[0].position = bottomLeftCorner;
                    linkedObjects[1].position = bottomRightCorner;
                    linkedObjects[2].position = topLeftCorner;
                    linkedObjects[3].position = topRightCorner;
                }
            } else if (state.Contains("RANDOM")) {
                if (state.Contains("QUADRANT")) {
                    //In any multi random quadrant scenario, we want to move the drones to a random position in each quadrant.
                    drones[0] = new Drone(width, height, Random.Range(0, width / 2), Random.Range(0, height / 2));
                    drones[1] = new Drone(width, height, Random.Range(width / 2, width), Random.Range(0, height / 2));
                    drones[2] = new Drone(width, height, Random.Range(0, width / 2), Random.Range(height / 2, height));
                    drones[3] = new Drone(width, height, Random.Range(width / 2, width), Random.Range(height / 2, height));

                    if (withGraphics) {
                        //Move the GameObjects too if graphics are enabled.
                        linkedObjects[0].position = new Vector3(((Drone)drones[0]).getPosX() * unitWidth - 9.5f, ((Drone)drones[0]).getPosY() * unitHeight - 3.8f, 0f);
                        linkedObjects[1].position = new Vector3(((Drone)drones[1]).getPosX() * unitWidth - 9.5f, ((Drone)drones[1]).getPosY() * unitHeight - 3.8f, 0f);
                        linkedObjects[2].position = new Vector3(((Drone)drones[2]).getPosX() * unitWidth - 9.5f, ((Drone)drones[2]).getPosY() * unitHeight - 3.8f, 0f);
                        linkedObjects[3].position = new Vector3(((Drone)drones[3]).getPosX() * unitWidth - 9.5f, ((Drone)drones[3]).getPosY() * unitHeight - 3.8f, 0f);
                    }
                } else {
                    for (int i = 0; i < drones.Count; i++) {
                        //In any multi random scenario besides random quadrant, we want to move the drones to a random position.
                        drones[i] = new Drone(width, height, Random.Range(0, width), Random.Range(0, height));

                        //Move the GameObjects too if graphics are enabled.
                        if (withGraphics)
                            linkedObjects[i].position = new Vector3(((Drone)drones[i]).getPosX() * unitWidth - 9.5f, ((Drone)drones[i]).getPosY() * unitHeight - 3.8f, 0f);
                    }
                }
            }

            if (state.Contains("QUADRANT_LIMITED")) {
                int [,] map = gameState.getMap();
                int[,] topLeftQuadrant = new int[height / 2, width / 2];
                int[,] topRightQuadrant = new int[height / 2, width / 2];
                int[,] bottomLeftQuadrant = new int[height / 2, width / 2];
                int[,] bottomRightQuadrant = new int[height / 2, width / 2];

                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++) {
                        if (i < height / 2 && j < width / 2) {
                            topLeftQuadrant[i, j] = map[i, j];
                        } else if (i < height / 2 && j >= width / 2) {
                            topRightQuadrant[i, j - width / 2] = map[i, j];
                        } else if (i >= height / 2 && j < width / 2) {
                            bottomLeftQuadrant[i - height / 2, j] = map[i, j];
                        } else {
                            bottomRightQuadrant[i - height / 2, j - width / 2] = map[i, j];
                        }
                    }
                }

                ((Drone) drones[0]).setPlayzone(topLeftQuadrant, 0, 0);
                ((Drone) drones[1]).setPlayzone(topRightQuadrant, width/2, 0);
                ((Drone) drones[2]).setPlayzone(bottomLeftQuadrant, 0, height/2);
                ((Drone) drones[3]).setPlayzone(bottomRightQuadrant, width/2, height/2);
            }

            //If we are in the moving goal scenario, we are using the drone as a psuedo-goal, so update its position accordingly.
            if (state.Contains("MOVING_GOAL"))
                drone = new Drone(width, height, gameState.getGoal().x, gameState.getGoal().y);
        }

        //Solo game helper function that decides the moves of one drone.
        void PlaySoloGame() {
            //Increment the move count at the beginning of each turn.
            moveCount++;

            //Access game state information.
            int[,] map = gameState.getMap();
            bool[,] explored = gameState.getGlobalExplored();
            float[,] heuristics = gameState.getHeuristics();

            //Find the list of adjacent spaces.
            ArrayList adjacent = drone.adjacent(map, false);

            //Find the move from the list of adjacent spaces.
            (int x, int y) decision = (0, 0);
            decision = drone.findMove(map, explored, heuristics, adjacent, false, gameState.getState(), moveCount);

            //Move to the decision location and mark it as explored.
            drone.move(decision);
            explored[decision.y, decision.x] = true;

            //If we are at the goal and graphics are off, reset the current iteration.
            if (map[decision.y, decision.x] == 1 && !withGraphics)
                ResetIteration("GAME_SOLO_UNINFORMED", false);
            
            //If graphics are on, start rendering after each move has been decided.
            if (withGraphics)
                renderingSolo = true;
        }

        //Multi game helper function that decides the moves of four drones.
        void PlayMultiGame(string state) {
            //Increment the move count at the beginning of each turn.
            moveCount++;

            //Access game state information.
            int[,] map = gameState.getMap();
            bool[,] explored = gameState.getGlobalExplored();
            float[,] heuristics = gameState.getHeuristics();

            //Iterate through each drone.
            foreach (Drone d in drones) {
                //If we are uninformed, the drones should not have a shared explored array.
                if (state.Contains("UNINFORMED"))
                    explored = d.getExplored();

                bool quadrantLimited = false;

                if (state.Contains("QUADRANT_LIMITED"))
                    quadrantLimited = true;

                //Find the list of adjacent spaces.
                ArrayList adjacent = d.adjacent(map, quadrantLimited);

                //Find the move from the list of adjacent spaces.
                (int x, int y) decision = (0, 0);
                decision = d.findMove(map, explored, heuristics, adjacent, quadrantLimited, gameState.getState(), moveCount);

                //Move to the decision location and mark it as explored.
                d.move(decision);
                explored[decision.y, decision.x] = true;

                //If we are at the goal and graphics are off, reset the current iteration.
                if (map[decision.y, decision.x] == 1 && !withGraphics) {
                    ResetIteration(state, false);
                    return;
                }
            }

            //If we are in the Moving Goal scenario, we want to move the goal.
            if (state.Contains("MOVING_GOAL")) {

                //Find the list of adjacent spaces using the drone as a psuedo-goal.
                ArrayList adjacent = drone.adjacent(map, false);

                //Create an all-zero heuristic array for the goal.
                float[,] goalHeuristics = new float[width, height];

                //Create an all-zero map array for the goal so it does not try to chase itself.
                int[,] goalMap = new int[width, height];

                //Use the drone's explored array only.
                explored = drone.getExplored();

                //Find the move from the list of adjacent spaces.
                (int x, int y) decision = (0, 0);
                decision = drone.findMove(goalMap, explored, goalHeuristics, adjacent, false, gameState.getState(), moveCount);

                //Move to the decision location and mark it as explored.
                drone.move(decision);
                explored[decision.y, decision.x] = true;

                //Move the goal on the map to the new position.
                gameState.setGoalPos(decision.x, decision.y);
            }
            
            //If graphics are on, start rendering after each move has been decided.
            if (withGraphics)
                renderingMulti = true;
        }
    }
}