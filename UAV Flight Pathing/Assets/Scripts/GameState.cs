using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class GameState {
        private int[,] map;
        private bool[,] occupied;
        private float[,] weights;
        private string state;

        public GameState() {
            this.map = new int[10, 10];
            this.occupied = new bool[10, 10];
            this.weights = new float[10, 10];
            this.state = "GAME_ACTIVE";
        }

        public GameState(int height, int width) {
            this.map = new int[width, height];
            this.occupied = new bool[width, height];
            this.weights = new float[width, height];
            this.state = "GAME_ACTIVE";
        }

        public void setGoalPos(int posX, int posY) {
            map[posY, posX] = 1;
        }

        public string getState() {
            return this.state;
        }

        public void setState(string newState) {
            this.state = newState;
        }
    }
}


