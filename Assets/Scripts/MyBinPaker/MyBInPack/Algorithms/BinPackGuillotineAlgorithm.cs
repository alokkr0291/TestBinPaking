using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobackPacker.Internal;
using UnityEngine;

namespace MobackPacker.Algorithms
{
    public class BinPackGuillotineAlgorithm : IBinPackAlgorithm
    {
        private readonly BinPackParameter _parameter;
        private readonly FreeCuboidChoiceHeuristic _cuboidChoice;
        private readonly GuillotineSplitHeuristic _splitMethod;
        private readonly List<Cuboid> _usedCuboids;
        private readonly List<Cuboid> _TruckCuboids;
        private static Cuboid[,,] binMatrix;
        
        public BinPackGuillotineAlgorithm(BinPackParameter parameter, FreeCuboidChoiceHeuristic cuboidChoice, GuillotineSplitHeuristic splitMethod)
        {
            _parameter = parameter;
            _cuboidChoice = cuboidChoice;
            _splitMethod = splitMethod;
            _usedCuboids = new List<Cuboid>();
            _TruckCuboids = new List<Cuboid>();
            AddFreeCuboid(new Cuboid(parameter.BinWidth, parameter.BinHeight, parameter.BinDepth));
            Debug.Log($" MyTruck--> {_TruckCuboids[0].Width} -{_TruckCuboids[0].Height} -{_TruckCuboids[0].Depth}");
            //  Debug.Log($" parameter.BinWidth--> {parameter.BinWidth} -{parameter.BinHeight} -{parameter.BinDepth}");
            binMatrix = new Cuboid[decimal.ToInt32(parameter.BinWidth), decimal.ToInt32(parameter.BinHeight), decimal.ToInt32(parameter.BinDepth)];
            
        }

        public void Insert(IEnumerable<Cuboid> cuboids)
        {
            foreach (var cuboid in cuboids)
            {
                Insert(cuboid, _cuboidChoice, _splitMethod);
            }
        }

        private void Insert(Cuboid cuboid, FreeCuboidChoiceHeuristic cuboidChoice, GuillotineSplitHeuristic splitMethod)
        {
           
            // Check is overweight
            if (cuboid.Weight + _usedCuboids.Sum(x => x.Weight) > _parameter.BinWeight)
                return;

            // Find where to put the new cuboid
            var freeNodeIndex = 0;
            FindPositionForNewNode(cuboid, cuboidChoice, out freeNodeIndex);

            // Abort if we didn't have enough space in the bin
            if (!cuboid.IsPlaced)
                return;
            
            //  Debug.Log($" --> {_TruckCuboids.Count}");
          //  Debug.Log($"free**** -{freeNodeIndex}--> {_TruckCuboids[freeNodeIndex]}");
          // Remove the space that was just consumed by the new cuboid
          
            if (freeNodeIndex < 0)
                throw new ArithmeticException("freeNodeIndex < 0");
            SplitFreeCuboidByHeuristic(_TruckCuboids[freeNodeIndex], cuboid, splitMethod);
            _TruckCuboids.RemoveAt(freeNodeIndex);

            // Remember the new used cuboid
            _usedCuboids.Add(cuboid);
        }

        private void FindPositionForNewNode(Cuboid cuboid, FreeCuboidChoiceHeuristic cuboidChoice, out int freeCuboidIndex)
        {
            var width = cuboid.Width;
            var height = cuboid.Height;
            var depth = cuboid.Depth;
            var bestScore = decimal.MaxValue;
            freeCuboidIndex = -1;
 
            // Try each free cuboid to find the best one for placement a given cuboid.
            // Rotate a cuboid in every possible way and find which choice is the best.
            for (int index = 0; index < _TruckCuboids.Count; ++index)
            {
                var freeCuboidList = _TruckCuboids[index];
                //  Debug.Log($"ooo --> {_TruckCuboids.Count}");
                //  Debug.Log($"{index}-cubid freecubid1--> {cuboid.Width},{cuboid.Height},{cuboid.Depth} -- {freeCuboidList.Width} ,{freeCuboidList.Height},{freeCuboidList.Depth}");
                //  Debug.Log($"{index}-cubid freecubid2--> {cuboid.X},{cuboid.Y},{cuboid.Z} -- {freeCuboidList.X} ,{freeCuboidList.Y},{freeCuboidList.Z}");
              
                // 1 Width x Height x Depth (no rotate)
                if (width <= freeCuboidList.Width && height <= freeCuboidList.Height && depth <= freeCuboidList.Depth)
                {
                    
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                    if (score < bestScore)
                    {
                        bestScore = GetBestScore(cuboid, out freeCuboidIndex, freeCuboidList,
                            width, height, depth, score, index,RotationDirection.None);
                    }
                    
                }
                
                //2 Width x Depth x Height (rotate vertically)
                if (width <= freeCuboidList.Width && depth <= freeCuboidList.Height && height <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                    if (score < bestScore)
                    {
                        bestScore = GetBestScore(cuboid, out freeCuboidIndex, freeCuboidList,
                            width, depth, height, score, index,RotationDirection.Vertical);
                        // cuboid.IsPlaced = true;
                        // cuboid.X = freeCuboidList.X;
                        // cuboid.Y = freeCuboidList.Y;
                        // cuboid.Z = freeCuboidList.Z;
                        // cuboid.Width = width;
                        // cuboid.Height = depth;
                        // cuboid.Depth = height;
                        // bestScore = score;
                        // freeCuboidIndex = index;
                        // cuboid.RotationDir = RotationDirection.Vertical;
                        // cuboid.Layer = cuboid.Y == 0 ? 0 : Convert.ToInt32 (cuboid.Height / cuboid.Y);

                    }
                }

                
                //3 Depth x Height x Width (rotate horizontally)
                if (depth <= freeCuboidList.Width && height <= freeCuboidList.Height && width <= freeCuboidList.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                    if (score < bestScore)
                    {
                        bestScore = GetBestScore(cuboid, out freeCuboidIndex, freeCuboidList,
                            depth, height, width, score, index,RotationDirection.Horizontal);
                        // cuboid.IsPlaced = true;
                        // cuboid.X = freeCuboidList.X;
                        // cuboid.Y = freeCuboidList.Y;
                        // cuboid.Z = freeCuboidList.Z;
                        // cuboid.Width = depth;
                        // cuboid.Height = height;
                        // cuboid.Depth = width;
                        // bestScore = score;
                        // freeCuboidIndex = index;
                        // cuboid.RotationDir = RotationDirection.Horizontal;
                        // cuboid.Layer = cuboid.Y == 0 ? 0 : Convert.ToInt32 (cuboid.Height / cuboid.Y);
                    }
                    // Debug.Log($"3 FindPositionForNewNode --> {bestScore}");
                }
                
                //4 Depth x Width x Height (rotate horizontally and vertically)
                if (depth <= freeCuboidList.Width && width <= freeCuboidList.Height && height <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                    if (score < bestScore)
                    {
                        bestScore = GetBestScore(cuboid, out freeCuboidIndex, freeCuboidList,
                            depth, width, height, score, index,RotationDirection.Both);
                        // cuboid.IsPlaced = true;
                        // cuboid.X = freeCuboidList.X;
                        // cuboid.Y = freeCuboidList.Y;
                        // cuboid.Z = freeCuboidList.Z;
                        // cuboid.Width = depth;
                        // cuboid.Height = width;
                        // cuboid.Depth = height;
                        // bestScore = score;
                        // freeCuboidIndex = index;
                        // cuboid.RotationDir = RotationDirection.Both;
                        // cuboid.Layer = cuboid.Y == 0 ? 0 : Convert.ToInt32 (cuboid.Height / cuboid.Y);
                    }
                    // Debug.Log($"4 FindPositionForNewNode --> {bestScore}");
                }

                //5 Height x Width x Depth (rotate vertically)
                if (height <= freeCuboidList.Width && width <= freeCuboidList.Height && depth <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
                  
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                    if (score < bestScore)
                    {
                        bestScore = GetBestScore(cuboid, out freeCuboidIndex, freeCuboidList,
                            height, width, depth, score, index,RotationDirection.Vertical);
                        // cuboid.IsPlaced = true;
                        // cuboid.X = freeCuboidList.X;
                        // cuboid.Y = freeCuboidList.Y;
                        // cuboid.Z = freeCuboidList.Z;
                        // cuboid.Width = height;
                        // cuboid.Height = width;
                        // cuboid.Depth = depth;
                        // bestScore = score;
                        // freeCuboidIndex = index;
                        // cuboid.RotationDir = RotationDirection.Vertical;
                        // cuboid.Layer = cuboid.Y == 0 ? 0 : Convert.ToInt32 (cuboid.Height / cuboid.Y);
                    }
                    // Debug.Log($"5 FindPositionForNewNode --> {bestScore}");
                }

                //6 Height x Depth x Width (rotate horizontally and vertically)
                if (height <= freeCuboidList.Width && depth <= freeCuboidList.Height && width <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
              
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                    if (score < bestScore)
                    {
                        bestScore = GetBestScore(cuboid, out freeCuboidIndex, freeCuboidList,
                            height, depth, width, score, index,RotationDirection.Both);
                        // cuboid.IsPlaced = true;
                        // cuboid.X = freeCuboidList.X;
                        // cuboid.Y = freeCuboidList.Y;
                        // cuboid.Z = freeCuboidList.Z;
                        // cuboid.Width = height;
                        // cuboid.Height = depth;
                        // cuboid.Depth = width;
                        // bestScore = score;
                        // freeCuboidIndex = index;
                        // cuboid.RotationDir = RotationDirection.Both;
                        // cuboid.Layer = cuboid.Y == 0 ? 0 : Convert.ToInt32 (cuboid.Height / cuboid.Y);
                        
                    }
                    //  Debug.Log($"6 FindPositionForNewNode --> {bestScore}");
                }
            }
        }

        private  decimal GetBestScore(Cuboid cuboid, out int freeCuboidIndex, Cuboid freeCuboidList, decimal width,
            decimal height, decimal depth, decimal score, int index, RotationDirection dir)
        {
            decimal bestScore;
            cuboid.IsPlaced = true;
            cuboid.X = freeCuboidList.X;
            cuboid.Y = freeCuboidList.Y;
            cuboid.Z = freeCuboidList.Z;
            cuboid.Width = width;
            cuboid.Height = height;
            cuboid.Depth = depth;
            bestScore = score;
            freeCuboidIndex = index;
            cuboid.RotationDir = dir;
            float averageHeight = 1.76f;
           int tempHeight = Convert.ToInt32(cuboid.Y);
           cuboid.Layer = cuboid.Y == 0 ? 1 : (int) Math.Ceiling(tempHeight / averageHeight);
           
            return bestScore;
        }
        
        


        private static decimal ScoreByHeuristic(Cuboid cuboid, Cuboid freeCuboid, FreeCuboidChoiceHeuristic cuboidChoice)
        {
            
            switch (cuboidChoice)
            {
                case FreeCuboidChoiceHeuristic.CuboidMinHeight:
                    return freeCuboid.Y + cuboid.Height;
                case FreeCuboidChoiceHeuristic.CuboidMinWidth:
                    return  freeCuboid.Y + cuboid.Height;// freeCuboid.Z + cuboid.Depth + freeCuboid.Y + cuboid.Height+ freeCuboid.X + cuboid.Width;
                default:
                    throw new NotSupportedException($"cuboid choice is unsupported: {cuboidChoice}");
            }
        }

        private void SplitFreeCuboidByHeuristic(Cuboid freeCuboid, Cuboid placedCuboid, GuillotineSplitHeuristic method)
        {
            // Compute the lengths of the leftover area.
            var w = freeCuboid.Width - placedCuboid.Width;
            var d = freeCuboid.Depth - placedCuboid.Depth;

            // Debug.Log($" --> {w} -{d}");

            // Use the given heuristic to decide which choice to make.

            bool splitHorizontal;
            switch (method)
            {
                case GuillotineSplitHeuristic.SplitShorterLeftoverAxis:
                    // Split along the shorter leftover axis.
                    splitHorizontal = (w <= d);
                    break;
                case GuillotineSplitHeuristic.SplitLongerLeftoverAxis:
                    // Split along the longer leftover axis.
                    splitHorizontal = (w > d);
                    break;
                case GuillotineSplitHeuristic.SplitShorterAxis:
                    // Split along the shorter total axis.
                    splitHorizontal = (freeCuboid.Width <= freeCuboid.Depth);
                    break;
                case GuillotineSplitHeuristic.SplitLongerAxis:
                    // Split along the longer total axis.
                    splitHorizontal = (freeCuboid.Width > freeCuboid.Depth);
                    break;
                default:
                    throw new NotSupportedException($"split method is unsupported: {method}");
            }

            // Perform the actual split.
            SplitFreeCuboidAlongAxis(freeCuboid, placedCuboid, splitHorizontal);
        }

  


        private void SplitFreeCuboidAlongAxis(Cuboid freeCuboid, Cuboid placedCuboid, bool splitHorizontal)
        {
            var bottom = new Cuboid();
            bottom.X = freeCuboid.X;
            bottom.Y = freeCuboid.Y;
            bottom.Z = freeCuboid.Z + placedCuboid.Depth;
            bottom.Depth = freeCuboid.Depth - placedCuboid.Depth;
            bottom.Height = placedCuboid.Height;

            var right = new Cuboid();
            right.X = freeCuboid.X + placedCuboid.Width;
            right.Y = freeCuboid.Y;
            right.Z = freeCuboid.Z;
            right.Width = freeCuboid.Width - placedCuboid.Width;
            right.Height = placedCuboid.Height;

            var top = new Cuboid();
            top.X = freeCuboid.X;
            top.Y = freeCuboid.Y + placedCuboid.Height;
            top.Z = freeCuboid.Z;
            top.Height = freeCuboid.Height - placedCuboid.Height;
            top.Width = freeCuboid.Width;
            top.Depth = freeCuboid.Depth;

            if (splitHorizontal)
            {
                bottom.Width = freeCuboid.Width;
                right.Depth = placedCuboid.Depth;
            }
            else // Split vertically
            {
                bottom.Width = placedCuboid.Width;
                right.Depth = freeCuboid.Depth;
            }

            // Add new free cuboids.
            if (bottom.Width > 0 && bottom.Height > 0 && bottom.Depth > 0)
                AddFreeCuboid(bottom);
            if (right.Width > 0 && right.Height > 0 && right.Depth > 0)
                AddFreeCuboid(right);
            if (top.Width > 0 && top.Height > 0 && top.Depth > 0)
                AddFreeCuboid(top);
        }

        private void AddFreeCuboid(Cuboid freeCuboid)
        {
            if (freeCuboid.X < 0 || freeCuboid.Y < 0 || freeCuboid.Z < 0)
            {
                throw new ArithmeticException(
                    $"add free cuboid failed: negative position, algorithm: {this}, cuboid: {freeCuboid}");
            }
            if (freeCuboid.X + freeCuboid.Width > _parameter.BinWidth || freeCuboid.Y + freeCuboid.Height > _parameter.BinHeight
                                                                      || freeCuboid.Z + freeCuboid.Depth > _parameter.BinDepth)
            {
                throw new ArithmeticException(
                    $"add free cuboid failed: out of bin, algorithm: {this}, cuboid: {freeCuboid}");
            }
            _TruckCuboids.Add(freeCuboid);
        }

        public override string ToString()
        {
            return $"Guillotine({_cuboidChoice}, {_splitMethod})";
        }
    }
}