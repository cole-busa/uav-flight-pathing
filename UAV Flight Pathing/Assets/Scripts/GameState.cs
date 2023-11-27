using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class GameState {
        private int[,] map;
        private int[,] globalTimesExplored;
        private float[,] globalHeuristics;
        private string state;
        private int width;
        private int height;
        private (int x, int y) goal;

        public GameState() {
            this.width = 10;
            this.height = 10;
            this.map = new int[width, height];
            this.globalTimesExplored = new int[width, height];
            globalTimesExplored[0, 0] = 1;
            this.globalHeuristics = new float[width, height];
            this.state = "GAME_SOLO_UNINFORMED";
            int randY = Random.Range(0, map.GetLength(0));
            int randX = Random.Range(0, map.GetLength(1));
            map[randY, randX] = 1;
            goal = (randX, randY);
        }

        public GameState(int height, int width, string state) {
            this.width = width;
            this.height = height;
            this.map = new int[width, height];
            this.globalTimesExplored = new int[width, height];
            this.globalHeuristics = new float[width, height];
            this.state = state;
            int randY = Random.Range(0, map.GetLength(0));
            int randX = Random.Range(0, map.GetLength(1));
            map[randY, randX] = 1;
            goal = (randX, randY);
            if (state == "GAME_MULTI_ORIGIN_MANHATTAN_INFORMED") {
                SetManhattanDistanceGlobalHeuristic("perfect");
            } else if (state == "GAME_MULTI_ORIGIN_EUCLIDEAN_INFORMED" || state == "GAME_MULTI_CORNER_PERFECTLY_INFORMED") {
                SetEuclideanDistanceGlobalHeuristic("perfect", (0,0));
            } else if (state.Contains("DECENTLY")) {
                SetEuclideanDistanceGlobalHeuristic("decent", (0, 0));
            } else if (state.Contains("BADLY")) {
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
            return this.state;
        }

        public int[,] GetMap() {
            return this.map;
        }

        public int[,] GetGlobalTimesExplored() {
            return this.globalTimesExplored;
        }

        public float[,] GetGlobalHeuristics() {
            return this.globalHeuristics;
        }

        public (int x, int y) GetGoal() {
            return goal;
        }

        public void SetState(string newState) {
            this.state = newState;
        }

        public void SetNoHeuristic() {
            globalHeuristics = new float[width, height];
        }

        public void SetManhattanDistanceGlobalHeuristic(string accuracy) {
            for (int x = 0; x < globalHeuristics.GetLength(1); x++) {
                for (int y = 0; y < globalHeuristics.GetLength(0); y++) {
                    float random = 0;
                    if (accuracy == "decent") {
                        random = Random.Range(-5.0f, 5.0f);
                    }
                    globalHeuristics[y, x] = Mathf.Abs(x - goal.x) + Mathf.Abs(y - goal.y) + random;
                }
            }
        }

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