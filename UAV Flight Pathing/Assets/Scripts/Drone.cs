using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Drone {
        private int posX;
        private int posY;

        public Drone() {
            this.posX = 0;
            this.posY = 0;
        }

        public Drone(int posX, int posY) {
            this.posX = posX;
            this.posY = posY;
        }

        public void move(string direction) {
            switch (direction) {
                case "UP LEFT":
                    this.posX -= 1;
                    this.posY -= 1;
                    break;
                case "UP":
                    this.posY -= 1;
                    break;
                case "UP RIGHT":
                    this.posX -= 1;
                    this.posY += 1;
                    break;
                case "LEFT":
                    this.posX -= 1;
                    break;
                case "RIGHT":
                    this.posX += 1;
                    break;
                case "DOWN LEFT":
                    this.posX -= 1;
                    this.posY += 1;
                    break;
                case "DOWN":
                    this.posY += 1;
                    break;
                case "DOWN RIGHT":
                    this.posX += 1;
                    this.posY += 1;
                    break;
            }
        }
    }
}
