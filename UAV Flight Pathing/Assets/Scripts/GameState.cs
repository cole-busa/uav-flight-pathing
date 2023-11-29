using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class GameState {
        //Arrays storing global information on each tile of the playzone.
        private int[,] map;
        private int[,] globalTimesExplored;
        private float[,] globalHeuristics;

        //Name of current state.
        private string state;

        //Size of playzone.
        private int width;
        private int height;

        //Location of the goal.
        private (int x, int y) goal;


        //Empty constructor for default values.
        public GameState() {
            //Default to 10 x 10 playzone.
            width = 10;
            height = 10;

            //Set the arrays to the specified sizes and mark the origin as explored.
            map = new int[width, height];
            globalTimesExplored = new int[width, height];
            globalTimesExplored[0, 0] = 1;
            globalHeuristics = new float[width, height];

            //Default to solo scenario.
            state = "GAME_SOLO_UNINFORMED";

            //Initialize the goal to random tile on the map.
            int randY = Random.Range(0, map.GetLength(0));
            int randX = Random.Range(0, map.GetLength(1));
            map[randY, randX] = 1;
            goal = (randX, randY);
        }

        //Constructor for passing in size of playzone and name of state.
        public GameState(int height, int width, string state) {
            //Initialize passed in variables.
            this.width = width;
            this.height = height;

            map = new int[width, height];
            globalTimesExplored = new int[width, height];
            globalHeuristics = new float[width, height];

            this.state = state;

            //Initialize goal to random tile on the map.
            int randY = Random.Range(0, map.GetLength(0));
            int randX = Random.Range(0, map.GetLength(1));
            map[randY, randX] = 1;
            goal = (randX, randY);

            //Set global heuristic if state requires.
            if (state == "GAME_MULTI_ORIGIN_PERFECT_MANHATTAN_INFORMED") {
                SetManhattanDistanceGlobalHeuristic("perfect");
            } else if (state.Contains("PERFECT")) {
                SetEuclideanDistanceGlobalHeuristic("perfect", (0, 0));
            } else if (state.Contains("DECENTLY")) {
                SetEuclideanDistanceGlobalHeuristic("decent", (0, 0));
            } else if (state.Contains("BADLY")) {
                //If the heuristic is bad, pass in a random tile to use as a false goal.
                randY = Random.Range(0, map.GetLength(0));
                randX = Random.Range(0, map.GetLength(1));
                SetEuclideanDistanceGlobalHeuristic("bad", (randX, randY));
            }
        }

        public void SetGoalPos(int posX, int posY) {
            map[goal.y, goal.x] = 0;
            map[posY, posX] = 1;
            goal = (posX, posY);
        }

        public string GetState() {
            return state;
        }

        public int[,] GetMap() {
            return map;
        }

        public int[,] GetGlobalTimesExplored() {
            return globalTimesExplored;
        }

        public float[,] GetGlobalHeuristics() {
            return globalHeuristics;
        }

        public (int x, int y) GetGoal() {
            return goal;
        }

        public void SetState(string newState) {
            state = newState;
        }

        public void SetNoHeuristic() {
            globalHeuristics = new float[width, height];
        }

        //Helper function to set each tile of the heuristic array to the Manhattan distance
        //of it and the goal.
        public void SetManhattanDistanceGlobalHeuristic(string accuracy) {
            for (int x = 0; x < globalHeuristics.GetLength(1); x++) {
                for (int y = 0; y < globalHeuristics.GetLength(0); y++) {
                    globalHeuristics[y, x] = Mathf.Abs(x - goal.x) + Mathf.Abs(y - goal.y);
                }
            }
        }

        //Helper function to set each tile of the heuristic array to the Euclidean distance
        //of it and the goal. If the accuracy is decent, a random float between -5 and 5 will
        //be added to each to simulate uncertainty. If the accuracy is bad, the goal will
        //actually be a false goal, simulating bad information.
        public void SetEuclideanDistanceGlobalHeuristic(string accuracy, (int x, int y) target) {
            if (accuracy == "bad") {
                goal = target;
            }
            for (int x = 0; x < globalHeuristics.GetLength(1); x++) {
                for (int y = 0; y < globalHeuristics.GetLength(0); y++) {
                    float random = 0;
                    if (accuracy == "decent") {
                        random = Random.Range(-5.0f, 5.0f);
                    }
                    globalHeuristics[y, x] = Mathf.Sqrt(Mathf.Pow(x - goal.x, 2) + Mathf.Pow(y - goal.y,2)) + random;
                }
            }
        }
    }
}