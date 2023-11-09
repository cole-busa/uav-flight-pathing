using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class GameState {
        private int[,] map;
        private bool[,] occupied;
        private float[,] weights;
        private enum state {
            GAME_ACTIVE,
            GAME_LOADING,
            GAME_WIN
        };

        public GameState() {
            this.map = new int[10, 10];
            this.occupied = new bool[10, 10];
            this.weights = new float[10, 10];
        }

        public GameState(int height, int width) {
            this.map = new int[width, height];
            this.occupied = new bool[width, height];
            this.weights = new float[width, height];
        }

        public string test() {
            return "test";
        }
    }
}


