/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using UnityEngine.Tilemaps;

public class Pathfinding : ComponentSystem {

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> dynamicBuffer, ref PathfindingParamsComponent pathfindingParamsComponent) => 
        { 
            FindPathJob findPathJob = new FindPathJob {
                startPosition = pathfindingParamsComponent.startPosition,
                endPosition = pathfindingParamsComponent.endPosition,
                pathPositionBuffer = dynamicBuffer
            };

            findPathJob.Execute();

            PostUpdateCommands.RemoveComponent<PathfindingParamsComponent>(entity);
        });
    }

    [BurstCompile]
    private class FindPathJob 
    {
        public int2 startPosition;
        public int2 endPosition;
        public DynamicBuffer<PathPosition> pathPositionBuffer;
        private Tilemap tilemap;
    
        public void Execute() {
            tilemap = GameHandler.instance.environmentTilemap;

            int2 translatedStartPosition = tilemapGridToPathfindingGrid(startPosition);
            int2 translatedEndPosition = tilemapGridToPathfindingGrid(endPosition);

            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(tilemap.size.x * tilemap.size.y, Allocator.Temp);
/*
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                    pathNode.CalculateFCost();

                    pathNode.isWalkable = true;
                    pathNode.cameFromNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }*/
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                // -14 ... 15
                int2 gridPosition = tilemapGridToPathfindingGrid(new int2(pos.x, pos.y));
                
                PathNode pathNode = new PathNode();
                pathNode.x = gridPosition.x;
                pathNode.y = gridPosition.y;
                pathNode.index = CalculateIndex(gridPosition.x, gridPosition.y, tilemap.size.x);
                pathNode.gCost = int.MaxValue;
                pathNode.hCost = CalculateDistanceCost(new int2(gridPosition.x, gridPosition.y), translatedEndPosition);
                pathNode.CalculateFCost();

                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (tilemap.HasTile(localPlace)) {
                    pathNode.isWalkable = false;
                } else {
                    pathNode.isWalkable = true;
                }
                pathNode.cameFromNodeIndex = -1;
                pathNodeArray[pathNode.index] = pathNode;
            }


             /*
            // Place Testing Walls
            {
                PathNode walkablePathNode = pathNodeArray[CalculateIndex(1, 0, gridSize.x)];
                walkablePathNode.SetIsWalkable(false);
                pathNodeArray[CalculateIndex(1, 0, gridSize.x)] = walkablePathNode;

                walkablePathNode = pathNodeArray[CalculateIndex(1, 1, gridSize.x)];
                walkablePathNode.SetIsWalkable(false);
                pathNodeArray[CalculateIndex(1, 1, gridSize.x)] = walkablePathNode;

                walkablePathNode = pathNodeArray[CalculateIndex(1, 2, gridSize.x)];
                walkablePathNode.SetIsWalkable(false);
                pathNodeArray[CalculateIndex(1, 2, gridSize.x)] = walkablePathNode;
            }
            */


            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int2(-1, 0); // Left
            neighbourOffsetArray[1] = new int2(+1, 0); // Right
            neighbourOffsetArray[2] = new int2(0, +1); // Up
            neighbourOffsetArray[3] = new int2(0, -1); // Down
            neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
            neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
            neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
            neighbourOffsetArray[7] = new int2(+1, +1); // Right Up

            int endNodeIndex = CalculateIndex(translatedEndPosition.x, translatedEndPosition.y, tilemap.size.x);

            PathNode startNode = pathNodeArray[CalculateIndex(translatedStartPosition.x, translatedStartPosition.y, tilemap.size.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while (openList.Length > 0) {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex) {
                    // Reached our destination!
                    break;
                }

                // Remove current node from Open List
                for (int i = 0; i < openList.Length; i++) {
                    if (openList[i] == currentNodeIndex) {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++) {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPosition, new int2(tilemap.size.x, tilemap.size.y))) {
                        // Neighbour not valid position
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, tilemap.size.x);

                    if (closedList.Contains(neighbourNodeIndex)) {
                        // Already searched this node
                        continue;
                    }

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if (!neighbourNode.isWalkable) {
                        // Not walkable
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

	                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
	                if (tentativeGCost < neighbourNode.gCost) {
		                neighbourNode.cameFromNodeIndex = currentNodeIndex;
		                neighbourNode.gCost = tentativeGCost;
		                neighbourNode.CalculateFCost();
		                pathNodeArray[neighbourNodeIndex] = neighbourNode;

		                if (!openList.Contains(neighbourNode.index)) {
			                openList.Add(neighbourNode.index);
		                }
	                }

                }
            }

            pathPositionBuffer.Clear();

            PathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1) {
                // Didn't find a path!
            } else {
                // Found a path
                CalculatePath(pathNodeArray, endNode, pathPositionBuffer);
            }

            pathNodeArray.Dispose();
            neighbourOffsetArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }
        
        private void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode, DynamicBuffer<PathPosition> pathPositionBuffer) 
        {
            if (endNode.cameFromNodeIndex == -1) {
                // Couldn't find a path!
            } else {
                // Found a path
                //Debug.Log(endNode.x + ", " + endNode.y + ": " + endNode.isWalkable);
                pathPositionBuffer.Add(new PathPosition { position = pathfindingGridToTilemapGrid(new int2(endNode.x, endNode.y)) });

                PathNode currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1) {
                    PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    //Debug.Log(cameFromNode.x + ", " + cameFromNode.y + ": " + cameFromNode.isWalkable);
                    pathPositionBuffer.Add(new PathPosition { position = pathfindingGridToTilemapGrid(new int2(cameFromNode.x, cameFromNode.y)) } );
                    currentNode = cameFromNode;
                }
            }
        }

        private int2 tilemapGridToPathfindingGrid(int2 coord) {
            return new int2(coord.x + math.abs(tilemap.cellBounds.xMin), coord.y + math.abs(tilemap.cellBounds.yMin));
        }

        private int2 pathfindingGridToTilemapGrid(int2 coord) {
            return new int2(coord.x - math.abs(tilemap.cellBounds.xMin), coord.y - math.abs(tilemap.cellBounds.yMin));
        }

        private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize) {
            return
                gridPosition.x >= 0 && 
                gridPosition.y >= 0 &&
                gridPosition.x < gridSize.x &&
                gridPosition.y < gridSize.y;
        }

        private int CalculateIndex(int x, int y, int gridWidth) {
            return x + y * gridWidth;
        }

        private int CalculateDistanceCost(int2 aPosition, int2 bPosition) {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

    
        private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray) {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];
            for (int i = 1; i < openList.Length; i++) {
                PathNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost) {
                    lowestCostPathNode = testPathNode;
                }
            }
            return lowestCostPathNode.index;
        }

        private struct PathNode {
            public int x;
            public int y;

            public int index;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;

            public int cameFromNodeIndex;

            public void CalculateFCost() {
                fCost = gCost + hCost;
            }

            public void SetIsWalkable(bool isWalkable) {
                this.isWalkable = isWalkable;
            }
        }

    }

}
