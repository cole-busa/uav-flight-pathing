using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Drone {
        private int posX;
        private int posY;
        private int width;
        private int height;
        private bool[,] explored;
        private int[,] playzone;
        private int offsetX;
        private int offsetY;

        public Drone(int width, int height) {
            this.posX = 0;
            this.posY = 0;
            this.width = width;
            this.height = height;
            this.explored = new bool[width, height];
            explored[0, 0] = true;
        }

        public Drone(int width, int height, int posX, int posY) {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.explored = new bool[width, height];
            explored[posY, posX] = true;
        }

        public int getPosX() {
            return posX;
        }

        public int getPosY() {
            return posY;
        }

        public bool[,] getExplored() {
            return explored;
        }

        public void setPlayzone(int [,] playzone, int offsetX, int offsetY) {
            this.playzone = playzone;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public void move((int x, int y) pos) {
            int distance = Mathf.Abs(posX - pos.x) + Mathf.Abs(posY - pos.y);
            if (distance >= 1 && distance <= 2) {
                posX = pos.x;
                posY = pos.y;
            }
        }

        public ArrayList adjacent(int[,] map, bool quadrantLimited) {
            if (quadrantLimited) {
                map = playzone;
                posX -= offsetX;
                posY -= offsetY;
            }

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

            if (quadrantLimited) {
                posX += offsetX;
                posY += offsetY;
            }

            return posList;
        }

        public (int x, int y) findMove(int[,] map, bool[,] droneExplored, float[,] heuristics, ArrayList adjacent, bool quadrantLimited, string state, int moveCount) {
            ArrayList validMoves = new ArrayList();

            ArrayList unexploredMoves = new ArrayList();

            if (quadrantLimited) {
                for (int i = 0; i < adjacent.Count; i++) {
                    (int x, int y) pos = ((int x, int y)) adjacent[i];
                    adjacent[i] = (pos.x + offsetX, pos.y + offsetY);
                }
            }

            (int x, int y) firstPos = ((int x, int y)) adjacent[0];
            float minHeuristic = heuristics[firstPos.y, firstPos.x];

            bool informationDecay = state.Contains("INFORMATION_DECAY") && 1f / Mathf.Sqrt(moveCount) < Random.Range(0f, 1f);

            foreach ((int x, int y) pos in adjacent) {
                if (map[pos.y, pos.x] == 1) {
                    //If adjacent to the goal
                    validMoves.Clear();
                    validMoves.Add(pos);
                    break;
                } else if (!droneExplored[pos.y, pos.x]) {
                    unexploredMoves.Add(pos);

                    float currentHeuristic = heuristics[pos.y, pos.x];

                    if (currentHeuristic < minHeuristic) {
                        minHeuristic = currentHeuristic;
                        validMoves.Clear();
                        validMoves.Add(pos);
                    } else if (currentHeuristic == minHeuristic) {
                        validMoves.Add(pos);
                    }
                }
            }
            
            if (informationDecay) {
                validMoves = unexploredMoves;
            }

            if (validMoves.Count == 0) {
                //If backed into a corner with all adjacent explored
                validMoves = adjacent;
            }
            
            //Choose randomly between equivalent moves
            return ((int x, int y))validMoves[Random.Range(0, validMoves.Count)];
        }
    }
}