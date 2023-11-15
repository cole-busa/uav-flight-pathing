using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class GameState {
        private int[,] map;
        private bool[,] globalExplored;
        private float[,] heuristics;
        private string state;
        private int width;
        private int height;
        private (int x, int y) goal;

        public GameState() {
            this.width = 10;
            this.height = 10;
            this.map = new int[width, height];
            this.globalExplored = new bool[width, height];
            globalExplored[0, 0] = true;
            this.heuristics = new float[10, 10];
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
            this.globalExplored = new bool[width, height];
            globalExplored[0, 0] = true;
            this.heuristics = new float[width, height];
            this.state = state;
            int randY = Random.Range(0, map.GetLength(0));
            int randX = Random.Range(0, map.GetLength(1));
            map[randY, randX] = 1;
            goal = (randX, randY);
            if (state == "GAME_MULTI_ORIGIN_MANHATTAN_INFORMED") {
                setManhattanDistanceHeuristic("perfect");
            } else if (state == "GAME_MULTI_ORIGIN_EUCLIDEAN_INFORMED" || state == "GAME_MULTI_CORNER_PERFECTLY_INFORMED") {
                setEuclideanDistanceHeuristic("perfect", (0,0));
            } else if (state.Contains("DECENTLY")) {
                setEuclideanDistanceHeuristic("decent", (0, 0));
            } else if (state.Contains("BADLY")) {
                randY = Random.Range(0, map.GetLength(0));
                randX = Random.Range(0, map.GetLength(1));
                setEuclideanDistanceHeuristic("bad", (randX, randY));
            }
        }

        public void setGoalPos(int posX, int posY) {
            map[posY, posX] = 1;
        }

        public string getState() {
            return this.state;
        }

        public int[,] getMap() {
            return this.map;
        }

        public bool[,] getGlobalExplored() {
            return this.globalExplored;
        }

        public float[,] getHeuristics() {
            return this.heuristics;
        }

        public void setState(string newState) {
            this.state = newState;
        }

        public void setNoHeuristic() {
            heuristics = new float[width, height];
        }

        public void setManhattanDistanceHeuristic(string accuracy) {
            for (int x = 0; x < heuristics.GetLength(1); x++) {
                for (int y = 0; y < heuristics.GetLength(0); y++) {
                    float random = 0;
                    if (accuracy == "decent") {
                        random = Random.Range(-5.0f, 5.0f);
                    }
                    heuristics[y, x] = Mathf.Abs(x - goal.x) + Mathf.Abs(y - goal.y) + random;
                }
            }
        }

        public void setEuclideanDistanceHeuristic(string accuracy, (int x, int y) target) {
            if (accuracy == "bad") {
                goal = target;
            }
            for (int x = 0; x < heuristics.GetLength(1); x++) {
                for (int y = 0; y < heuristics.GetLength(0); y++) {
                    float random = 0;
                    if (accuracy == "decent") {
                        random = Random.Range(-5.0f, 5.0f);
                    }
                    heuristics[y, x] = Mathf.Sqrt(Mathf.Pow(x - goal.x, 2) + Mathf.Pow(y - goal.y,2)) + random;
                }
            }
        }
    }
}


