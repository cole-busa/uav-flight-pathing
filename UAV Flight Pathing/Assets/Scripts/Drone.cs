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

        public int getPosX() {
            return this.posX;
        }

        public int getPosY() {
            return this.posY;
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

        public ArrayList adjacent(int[,] map) {
            var posList = new ArrayList();
            if (posX == 0 && posY == 0) {
                //Top left corner
                posList.Add((posX + 1, posY));
                posList.Add((posX, posY + 1));
                posList.Add((posX + 1, posY + 1));
            } else if (posX == map.GetLength(1) - 1 && posY == 0) {
                //Top right corner
                posList.Add((posX - 1, posY));
                posList.Add((posX - 1, posY + 1));
                posList.Add((posX, posY + 1));
            } else if (posX == 0 && posY == map.GetLength(0) - 1) {
                //Bottom left corner
                posList.Add((posX, posY - 1));
                posList.Add((posX + 1, posY - 1));
                posList.Add((posX + 1, posY));
            } else if (posX == map.GetLength(1) - 1 && posY == map.GetLength(0) - 1) {
                //Bottom right corner
                posList.Add((posX - 1, posY - 1));
                posList.Add((posX, posY - 1));
                posList.Add((posX - 1, posY));
            } else if (posX == 0) {
                //Left edge
                posList.Add((posX, posY - 1));
                posList.Add((posX + 1, posY - 1));
                posList.Add((posX + 1, posY));
                posList.Add((posX, posY + 1));
                posList.Add((posX + 1, posY + 1));
            } else if (posY == 0) {
                //Top edge
                posList.Add((posX - 1, posY));
                posList.Add((posX + 1, posY));
                posList.Add((posX - 1, posY + 1));
                posList.Add((posX, posY + 1));
                posList.Add((posX + 1, posY + 1));
            } else if (posX == map.GetLength(1) - 1) {
                //Right edge
                posList.Add((posX - 1, posY - 1));
                posList.Add((posX, posY - 1));
                posList.Add((posX - 1, posY));
                posList.Add((posX - 1, posY + 1));
                posList.Add((posX, posY + 1));
            } else if (posY == map.GetLength(0) - 1) {
                //Bottom edge
                posList.Add((posX - 1, posY - 1));
                posList.Add((posX, posY - 1));
                posList.Add((posX + 1, posY - 1));
                posList.Add((posX - 1, posY));
                posList.Add((posX + 1, posY));
            } else {
                //No obstruction
                posList.Add((posX - 1, posY - 1));
                posList.Add((posX, posY - 1));
                posList.Add((posX + 1, posY - 1));
                posList.Add((posX - 1, posY));
                posList.Add((posX + 1, posY));
                posList.Add((posX - 1, posY + 1));
                posList.Add((posX, posY + 1));
                posList.Add((posX + 1, posY + 1));
            }
            return posList;
        }
    }
}
