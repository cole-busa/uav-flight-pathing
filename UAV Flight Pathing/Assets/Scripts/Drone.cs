using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Drone {
        private int posX;
        private int posY;
        private int width;
        private int height;
        private int[,] timesExplored;
        private float[,] heuristics;
        private int[,] playzone;
        private int offsetX;
        private int offsetY;

        public Drone(int width, int height) {
            this.posX = 0;
            this.posY = 0;

            this.width = width;
            this.height = height;

            this.timesExplored = new int[width, height];
            timesExplored[0, 0] = 1;

            this.heuristics = new float[width, height];
            UpdateExploredHeuristics(false, timesExplored);
        }

        public Drone(int width, int height, (int x, int y) pos, bool informed, int[,] timesExplored) {
            this.posX = pos.x;
            this.posY = pos.y;

            this.width = width;
            this.height = height;

            if (!informed) {
                this.timesExplored = new int[width, height];
            } else {
                this.timesExplored = timesExplored;
            }
            
            this.timesExplored[pos.y, pos.x] = 1;

            this.heuristics = new float[width, height];
            UpdateExploredHeuristics(false, timesExplored);
        }

        public int GetPosX() {
            return posX;
        }

        public int GetPosY() {
            return posY;
        }

        public int[,] GetTimesExplored() {
            return timesExplored;
        }

        public float[,] GetHeuristics() {
            return heuristics;
        }

        public void SetPlayzone(int[,] playzone, int offsetX, int offsetY) {
            this.playzone = playzone;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public void SetHeuristics(float[,] heuristics) {
            this.heuristics = heuristics;
        }

        //Helper function to update a heuristic based on the number of surrounding explored tiles of each tile.
        public void UpdateExploredHeuristics(bool informed, int[,] globalTimesExplored) {
            if (!informed)
                globalTimesExplored = timesExplored;

            for (int x = 0; x < heuristics.GetLength(1); x++) {
                for (int y = 0; y < heuristics.GetLength(0); y++) {
                    ArrayList adjacentTiles = GetAdjacentTiles(width, height, (x, y));

                    int numExplored = 0;
                    for (int i = 0; i < adjacentTiles.Count; i++) {
                        (int x, int y) pos = ((int x, int y)) adjacentTiles[i];
                        if (globalTimesExplored[pos.y, pos.x] > 0)
                            numExplored += globalTimesExplored[pos.y, pos.x];
                    }

                    heuristics[y, x] = numExplored;
                }
            }
        }

        public void Move((int x, int y) pos) {
            int distance = Mathf.Abs(posX - pos.x) + Mathf.Abs(posY - pos.y);
            if (distance >= 1 && distance <= 2) {
                posX = pos.x;
                posY = pos.y;
            }
        }

        public ArrayList GetAdjacentTiles(int width, int height, (int x, int y) pos) {

            var posList = new ArrayList();
            if (pos.x == 0 && pos.y == 0) {
                //Top left corner
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            } else if (pos.x == width - 1 && pos.y == 0) {
                //Top right corner
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
            } else if (pos.x == 0 && pos.y == height - 1) {
                //Bottom left corner
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x + 1, pos.y));
            } else if (pos.x == width - 1 && pos.y == height - 1) {
                //Bottom right corner
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
            } else if (pos.x == 0) {
                //Left edge
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            } else if (pos.y == 0) {
                //Top edge
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            } else if (pos.x == width - 1) {
                //Right edge
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
            } else if (pos.y == height - 1) {
                //Bottom edge
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x + 1, pos.y));
            } else {
                //No obstruction
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            }

            return posList;
        }

        public (int x, int y) FindMove(int[,] map, bool informed, int[,] globalTimesExplored, float[,] globalHeuristics, bool quadrantLimited, string state, int moveCount) {
            if (!informed) {
                globalTimesExplored = timesExplored;
                globalHeuristics = heuristics;
            }
        
            ArrayList validMoves = new ArrayList();

            ArrayList unexploredMoves = new ArrayList();

            ArrayList adjacent = new ArrayList();
            if (quadrantLimited) {
                adjacent = GetAdjacentTiles(map.GetLength(1) / 2, map.GetLength(0) / 2, (posX - offsetX, posY - offsetY));
                for (int i = 0; i < adjacent.Count; i++) {
                    (int x, int y) pos = ((int x, int y)) adjacent[i];
                    adjacent[i] = (pos.x + offsetX, pos.y + offsetY);
                }
            } else {
                adjacent = GetAdjacentTiles(map.GetLength(1), map.GetLength(0), (posX, posY));
            }

            (int x, int y) firstPos = ((int x, int y)) adjacent[0];
            float minHeuristic = globalHeuristics[firstPos.y, firstPos.x];

            bool informationDecay = state.Contains("INFORMATION_DECAY") && 1f / Mathf.Sqrt(moveCount) < Random.Range(0f, 1f);

            foreach ((int x, int y) pos in adjacent) {
                if (map[pos.y, pos.x] == 1) {
                    //If adjacent to the goal
                    validMoves.Clear();
                    validMoves.Add(pos);
                    break;
                } else if (globalTimesExplored[pos.y, pos.x] == 0) {
                    unexploredMoves.Add(pos);

                    float currentHeuristic = globalHeuristics[pos.y, pos.x];

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