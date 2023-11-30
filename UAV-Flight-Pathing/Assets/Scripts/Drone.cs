using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class Drone {
        //Location of drone.
        private int posX;
        private int posY;

        //Size of global map.
        private int width;
        private int height;

        //Name of current state. 
        private string state;

        //Arrays storing local information on each tile of the playzone.
        private int[,] localTimesExplored;
        private float[,] localHeuristics;

        //Quadrant limited status.
        private bool quadrantLimited;
        
        //Quadrant map if drone is quadrant limited.
        private int[,] playzone;

        //Position offsets if drone is quadrant limited.
        private int offsetX;
        private int offsetY;

        //Current move count of drone.
        private int moveCount;

        //Information status of drone.
        private bool informed;


        //Constructor for only passing in size of the map and game state name.
        public Drone(int width, int height, string state) {
            //Default location to origin.
            posX = 0;
            posY = 0;

            //Initialize passed in variables.
            this.width = width;
            this.height = height;
            this.state = state;

            //Default to uninformed and not quadrant limited.
            informed = false;
            quadrantLimited = false;

            //Set arrays to the specified sizes and mark the origin as explored.
            localTimesExplored = new int[width, height];
            localTimesExplored[0, 0] = 1;
            localHeuristics = new float[width, height];

            //Initialize the values of the explored heuristic array.
            UpdateExploredHeuristics(localTimesExplored);
        }

        //Constructor for passing in size of the map, game state name, an initial position, the information status, and a global times explored array.
        public Drone(int width, int height, string state, (int x, int y) pos, bool informed, int[,] globalTimesExplored) {
            //Default location to origin.
            posX = pos.x;
            posY = pos.y;

            //Initialize passed in variables.
            this.width = width;
            this.height = height;
            this.state = state;
            this.informed = informed;

            //Default to not quadrant limited.
            quadrantLimited = false;
            
            //If we are informed, we want to set the initial local times explored to the global times explored
            //to take into account the other drones' initial positions. Otherwise, just create an empty array at the passed in size.
            if (!informed) {
                localTimesExplored = new int[width, height];
            } else {
                this.localTimesExplored = globalTimesExplored;
            }
            
            //Mark the initial position as explored.
            localTimesExplored[pos.y, pos.x] = 1;

            //Set the heuristics array to the passed in size.
            localHeuristics = new float[width, height];

            //Initialize the values of the explored heuristic array.
            UpdateExploredHeuristics(localTimesExplored);
        }

        //Getters and Setters for local variables.
        public int GetPosX() {
            return posX;
        }

        public int GetPosY() {
            return posY;
        }

        public int[,] GetLocalTimesExplored() {
            return localTimesExplored;
        }

        public float[,] GetLocalHeuristics() {
            return localHeuristics;
        }

        public void SetLocalHeuristics(float[,] localHeuristics) {
            this.localHeuristics = localHeuristics;
        }

        //Function to set the playzone of a quadrant limited drone.
        public void SetPlayzone(int[,] playzone, int offsetX, int offsetY) {
            //If we are calling this function then the drone is now quadrant limited.
            quadrantLimited = true;

            //Initialize passed in values.
            this.playzone = playzone;
            this.offsetX = offsetX;
            this.offsetY = offsetY;

            //Since we are confined to a quadrant, we want to set the local heuristic array
            //to a high value when it is outside the quadrant so as to not distract the drone.
            int startX = 0;
            int startY = 0;
            int endX = height/2;
            int endY = width/2;

            if (offsetX == height/2) {
                startX = height/2;
                endX = height;
            }
            if (offsetY == width/2) {
                startY = width/2;
                startY = width;
            }
            for (int x = 0; x < localHeuristics.GetLength(1); x++) {
                for (int y = 0; y < localHeuristics.GetLength(0); y++) {
                    if (x < startX || x > endX || y < startY || y > endY) {
                        localHeuristics[y, x] = 999;
                    }
                }
            }

        }

        //Helper function to update the local explored heuristic based on the sum of the times explored for each tile and its surrounding tiles.
        public void UpdateExploredHeuristics(int[,] globalTimesExplored) {
            //If we are not informed, we do not care about the global times explored.
            if (!informed)
                globalTimesExplored = localTimesExplored;

            //Iterate through each tile in the local heuristics array.
            for (int x = 0; x < localHeuristics.GetLength(1); x++) {
                for (int y = 0; y < localHeuristics.GetLength(0); y++) {
                    //Find the adjacent tiles of each tile.
                    ArrayList adjacentTiles = GetAdjacentTiles(width, height, (x, y));

                    //Keep track of the number of times the tiles have been explored.
                    int numExplored = globalTimesExplored[y, x];

                    //Iterate through the adjacent tiles and add their times explored to the working count.
                    for (int i = 0; i < adjacentTiles.Count; i++) {
                        (int x, int y) pos = ((int x, int y)) adjacentTiles[i];
                        if (globalTimesExplored[pos.y, pos.x] > 0)
                            numExplored += globalTimesExplored[pos.y, pos.x];
                    }

                    //Set the heuristic to the the sum of tile's and its surrounding tiles' explored count.
                    localHeuristics[y, x] = numExplored;
                }
            }
        }

        //Helper function to move the drone to a given position.
        public void Move((int x, int y) pos) {
            //Calculate the Manhattan distance we are trying to move.
            int distance = Mathf.Abs(posX - pos.x) + Mathf.Abs(posY - pos.y);

            //If the distance is within legal limits, update the x and y positions,
            //increment the times explored of the new tile, and increment the move count.
            if (distance >= 1 && distance <= 2) {
                posX = pos.x;
                posY = pos.y;
                localTimesExplored[pos.y, pos.x]++;
                moveCount++;
            }
        }

        //Helper function to find and return the adjacent tiles of a position given the size of its playzone.
        public ArrayList GetAdjacentTiles(int width, int height, (int x, int y) pos) {
            //Store the positions in an ArrayList.
            var posList = new ArrayList();

            //Check if we are obstructed, otherwise add all 8 surrounding tiles.
            if (pos.x == 0 && pos.y == 0) {
                //If we are in the top left corner, add the right tile, 
                //the down tile, and the down-right tile.
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            } else if (pos.x == width - 1 && pos.y == 0) {
                //If we are in the top right corner, add the left tile,
                //the down-left tile, and the down tile.
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
            } else if (pos.x == 0 && pos.y == height - 1) {
                //If we are in the bottom left corner, add the up tile,
                //the up-right tile, and the right tile.
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x + 1, pos.y));
            } else if (pos.x == width - 1 && pos.y == height - 1) {
                //If we are in the bottom right corner, add the up-left tile,
                //the up tile, and the left tile.
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
            } else if (pos.x == 0) {
                //If we are on the left edge, add the up tile,
                //the up-right tile, the right tile, the down tile,
                //and the down-right tile.
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            } else if (pos.y == 0) {
                //If we are on the top edge, add the left tile,
                //the right tile, the down-left tile, the down tile,
                //and the down-right tile.
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            } else if (pos.x == width - 1) {
                //If we are on the right edge, add the up-left tile,
                //the up tile, the left tile, the down-left tile,
                //and the down tile.
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
            } else if (pos.y == height - 1) {
                //If we are on the bottom edge, add the up-left tile,
                //the up tile, the up-right tile, the left tile,
                //and the right tile.
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x + 1, pos.y));
            } else {
                //If we have no obstruction, add all surrounding 8 tiles.
                posList.Add((pos.x - 1, pos.y - 1));
                posList.Add((pos.x, pos.y - 1));
                posList.Add((pos.x + 1, pos.y - 1));
                posList.Add((pos.x - 1, pos.y));
                posList.Add((pos.x + 1, pos.y));
                posList.Add((pos.x - 1, pos.y + 1));
                posList.Add((pos.x, pos.y + 1));
                posList.Add((pos.x + 1, pos.y + 1));
            }

            //Return the list of adjacent positions.
            return posList;
        }

        //Helper function to find and return the best move depending on the heuristics.
        public (int x, int y) FindMove(int[,] map, int[,] globalTimesExplored, float[,] globalHeuristics) {
            //If we are in a scenario that requires information decay, we want to consider the explored heuristic
            //rather than the Euclidean distance heuristic once the ratio of the fastest route to the move count is
            //less than a random float between 0.5 and 1. The fastest route is simply the hypotenuse of a triangle
            //whose edges are half the height and half the width. For simplicity we assume the height and width are the same.
            float fastestRoute = (height / 2) * Mathf.Sqrt(2);
            bool informationDecay = state.Contains("INFORMATION_DECAY") && fastestRoute / moveCount < Random.Range(0.5f, 1f);
        
            //If we are not informed, we do not care about the global times explored.
            if (!informed) {
                globalTimesExplored = localTimesExplored;
            }
        
            //Initialize three ArrayLists, the best global heuristic moves, the best local heuristic moves, and the best overall moves.
            ArrayList bestGlobalHeuristicMoves = new ArrayList();
            ArrayList bestLocalHeuristicMoves = new ArrayList();
            ArrayList bestMoves = new ArrayList();

            //Find the adjacent positions that we could move to.
            ArrayList adjacent = new ArrayList();
            if (quadrantLimited) {
                //If we are quadrant limited, we want to find the adjacent positions as if we are stuck inside the playzone.
                //So, we should set the height and width to those of the playzone and the position as the current position
                //of the drone minus its offsets. Then we simply add the offsets back to the adjacent positions after they are found.
                adjacent = GetAdjacentTiles(playzone.GetLength(1), playzone.GetLength(0), (posX - offsetX, posY - offsetY));
                for (int i = 0; i < adjacent.Count; i++) {
                    (int x, int y) pos = ((int x, int y)) adjacent[i];
                    adjacent[i] = (pos.x + offsetX, pos.y + offsetY);
                }
            } else {
                //Otherwise, find the adjacent tiles of the normal drone position.
                adjacent = GetAdjacentTiles(map.GetLength(1), map.GetLength(0), (posX, posY));
            }

            //Initialize the minimum heuristics for both the global and local heuristics as the
            //value of the first adjacent position in their respective arrays.
            (int x, int y) firstPos = ((int x, int y)) adjacent[0];
            float minGlobalHeuristic = globalHeuristics[firstPos.y, firstPos.x];
            float minLocalHeuristic = localHeuristics[firstPos.y, firstPos.x];

            //Iterate through the adjacent positions.
            foreach ((int x, int y) pos in adjacent) {
                if (map[pos.y, pos.x] == 1) {
                    //If the adjacent position is the goal, return that position.
                    return pos;
                }

                //If the scenario is perfectly informed or the position is unexplored:
                if (state.Contains("PERFECT") || globalTimesExplored[pos.y, pos.x] == 0) {
                    //Update the list of best moves according to the global heuristic.
                    //Find the current global heuristic and compare it to the minimum.
                    //If it is less, clear the best global heuristic moves ArrayList
                    //and add it. Otherwise, if it is equal, add it to the list as an
                    //equally good move.
                    float currentGlobalHeuristic = globalHeuristics[pos.y, pos.x];
                    if (currentGlobalHeuristic < minGlobalHeuristic) {
                        minGlobalHeuristic = currentGlobalHeuristic;
                        bestGlobalHeuristicMoves.Clear();
                        bestGlobalHeuristicMoves.Add(pos);
                    } else if (currentGlobalHeuristic == minGlobalHeuristic) {
                        bestGlobalHeuristicMoves.Add(pos);
                    }
                }
                
                //Update the list of best moves according to the local heuristic.
                //Find the current global heuristic and compare it to the minimum.
                //If it is less, clear the best local heuristic moves ArrayList
                //and add it. Otherwise, if it is equal, add it to the list as an
                //equally good move.
                float currentLocalHeuristic = localHeuristics[pos.y, pos.x];
                if (currentLocalHeuristic < minLocalHeuristic) {
                    minLocalHeuristic = currentLocalHeuristic;
                    bestLocalHeuristicMoves.Clear();
                    bestLocalHeuristicMoves.Add(pos);
                } else if (currentLocalHeuristic == minLocalHeuristic) {
                    bestLocalHeuristicMoves.Add(pos);
                }
            }
            
            //Decide the best moves.
            if (informationDecay || !informed) {
                //If we have information decay or are uninformed, the best moves will be
                //the best local heuristic moves.
                bestMoves = bestLocalHeuristicMoves;
            } else if (bestGlobalHeuristicMoves.Count == 0) {
                //If the best global heuristic moves ArrayList is empty, the best moves
                //are equally all of the adjacent positions.
                bestMoves = adjacent;
            } else {
                //Otherwise, the best moves will be the best global heuristic moves.
                bestMoves = bestGlobalHeuristicMoves;
            }
            
            //Choose randomly between equivalent moves and return that position.
            return ((int x, int y))bestMoves[Random.Range(0, bestMoves.Count)];
        }
    }
}