using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class GameState {
        private int[,] map;
        private bool[,] explored;
        private float[,] weights;
        private string state;

        public GameState() {
            this.map = new int[10, 10];
            this.explored = new bool[10, 10];
            explored[0, 0] = true;
            this.weights = new float[10, 10];
            this.state = "GAME_ACTIVE";
            int randY = Random.Range(0, map.GetLength(0));
            int randX = Random.Range(0, map.GetLength(1));
            map[randY, randX] = 1;
            Debug.Log("GOAL AT " + (randX, randY));
        }

        public GameState(int height, int width) {
            this.map = new int[width, height];
            this.explored = new bool[width, height];
            explored[0, 0] = true;
            this.weights = new float[width, height];
            this.state = "GAME_ACTIVE";
            int randY = Random.Range(0, map.GetLength(0));
            int randX = Random.Range(0, map.GetLength(1));
            map[randY, randX] = 1;
            Debug.Log("GOAL AT " + (randX, randY));
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

        public bool[,] getExplored() {
            return this.explored;
        }

        public float[,] getWeights() {
            return this.weights;
        }

        public void setState(string newState) {
            this.state = newState;
        }
    }
}


