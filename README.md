# UAV Flight Pathing

Hello! This is my final project for my Systems of Networks class. :grinning: 

This project aims to find the optimal pathing algorithm for a system of drones to find a given target. To determine if an algorithm is more optimal than another we will examine how many moves it takes on average to reach the goal. First, we must determine whether it is better to have more drones than one. As it turns out, it is better. From this point on we will use four drones as the base case. Next, we want to see if corner spawning is better than origin spawning for the multiple drones. As it turns out, it is better. Then, we want to see how a heuristic affects the drone's search quality. The first heuristic we want to consider is the Manhattan distance heuristic, which is simply the absolute value in the difference in the X coordinates plus the absolute value of the difference in the Y coordinates. A perfectly informed drone will have access to the perfect Manhattan distance. A decently informed drone will have access to a random value added or subtracted from the Manhattan distance to simulate uncertainty. A bad heuristic will have the Manhattan distance between the drone and a totally incorrect spot on the map to simulate bad info.


## Uninformed Search
- Same starting point
- Four corner starting points
- Random starting points
- Quadrant-based random starting points
## Informed Search
- Shared information
- Bad heuristic
- Good heuristic
- Perfect heuristic
- Moving target
