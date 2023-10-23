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
        private  List<Cuboid> _AvailableSpace;
        private static Cuboid[,,] binMatrix;
        
        public BinPackGuillotineAlgorithm(BinPackParameter parameter, FreeCuboidChoiceHeuristic cuboidChoice, GuillotineSplitHeuristic splitMethod)
        {
            _parameter = parameter;
            _cuboidChoice = cuboidChoice;
            _splitMethod = splitMethod;
            _usedCuboids = new List<Cuboid>();
            _AvailableSpace = new List<Cuboid>();
            AddFreeCuboid(new Cuboid(parameter.BinWidth, parameter.BinHeight, parameter.BinDepth));
           // Debug.Log($" MyTruck--> {_TruckCuboids[0].Width} -{_TruckCuboids[0].Height} -{_TruckCuboids[0].Depth}");
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
            
            if (freeNodeIndex < 0)
                throw new ArithmeticException("freeNodeIndex < 0");
            SplitFreeCuboidByHeuristic(_AvailableSpace[freeNodeIndex], cuboid, splitMethod);
            _AvailableSpace.RemoveAt(freeNodeIndex);

            // Remember the new used cuboid
            _usedCuboids.Add(cuboid);
        }

        private int placingCount = 0;
        private void FindPositionForNewNode(Cuboid cuboid, FreeCuboidChoiceHeuristic cuboidChoice, out int freeCuboidIndex)
        {
            placingCount++;
            var cuboidWidth = cuboid.Width;
            var cuboidHeight = cuboid.Height;
            var cuboidDepth = cuboid.Depth;
            cuboid.UniqueId = cuboid.Tag+"-"+GenerateUniqueId();
            var bestScore = decimal.MaxValue;
            freeCuboidIndex = -1;
            Debug.Log($"________________" + placingCount);
            Debug.Log($"ooo --> {_AvailableSpace.Count}");
            
            _AvailableSpace = _AvailableSpace.OrderByDescending(cuboid => cuboid.Z).ToList();
            
            // Try each free cuboid to find the best one for placement a given cuboid.
            // Rotate a cuboid in every possible way and find which choice is the best.
            //int index = 0;// _TruckCuboids.Count-1;
            for (int index = 0; index < _AvailableSpace.Count; ++index)
            {
                var freeCuboidList = _AvailableSpace[index];
                 
                  Debug.Log($"{index}- freecubid pos dim-->  {freeCuboidList.X} ,{freeCuboidList.Y},{freeCuboidList.Z}-- {freeCuboidList.Width} ,{freeCuboidList.Height},{freeCuboidList.Depth}");
                 // Debug.Log($"{index}-cubid  pos--> {cuboid.X},{cuboid.Y},{cuboid.Z} -- {cuboid.Width},{cuboid.Height},{cuboid.Depth} ");
              
                // 1 Width x Height x Depth (no rotate)
                if (cuboidWidth <= freeCuboidList.Width && cuboidHeight <= freeCuboidList.Height && cuboidDepth <= freeCuboidList.Depth)
                {
                    
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                  //  Debug.Log($"WHD --> {score}-{bestScore}");
                    if (score < bestScore)
                    {
                        bestScore = UpdateCuboid(cuboid, out freeCuboidIndex, freeCuboidList,
                            cuboidWidth, cuboidHeight, cuboidDepth, score, index,RotationDirection.None);
                    }
                    
                }
                
                //2 Width x Depth x Height (rotate vertically)
                if (cuboidWidth <= freeCuboidList.Width && cuboidDepth <= freeCuboidList.Height && cuboidHeight <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                  //  Debug.Log($"WDH --> {score}-{bestScore}");
                    if (score < bestScore)
                    {
                        bestScore = UpdateCuboid(cuboid, out freeCuboidIndex, freeCuboidList,
                            cuboidWidth, cuboidDepth, cuboidHeight, score, index,RotationDirection.Vertical);
                    }
                }
                
                //3 Depth x Height x Width (rotate horizontally)
                if (cuboidDepth <= freeCuboidList.Width && cuboidHeight <= freeCuboidList.Height && cuboidWidth <= freeCuboidList.Depth)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                   // Debug.Log($"DHW --> {score}-{bestScore}");
                    if (score < bestScore)
                    {
                        bestScore = UpdateCuboid(cuboid, out freeCuboidIndex, freeCuboidList,
                            cuboidDepth, cuboidHeight, cuboidWidth, score, index,RotationDirection.Horizontal);
                    }
                    // Debug.Log($"3 FindPositionForNewNode --> {bestScore}");
                }
                
                //4 Depth x Width x Height (rotate horizontally and vertically)
                if (cuboidDepth <= freeCuboidList.Width && cuboidWidth <= freeCuboidList.Height && cuboidHeight <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                  //  Debug.Log($"DWH --> {score}-{bestScore}");
                    if (score < bestScore)
                    {
                        bestScore = UpdateCuboid(cuboid, out freeCuboidIndex, freeCuboidList,
                            cuboidDepth, cuboidWidth, cuboidHeight, score, index,RotationDirection.Both);
                    }
                    // Debug.Log($"4 FindPositionForNewNode --> {bestScore}");
                }

                //5 Height x Width x Depth (rotate vertically)
                if (cuboidHeight <= freeCuboidList.Width && cuboidWidth <= freeCuboidList.Height && cuboidDepth <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
                  
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                  //  Debug.Log($"HWD --> {score}-{bestScore}");
                    if (score < bestScore)
                    {
                        bestScore = UpdateCuboid(cuboid, out freeCuboidIndex, freeCuboidList,
                            cuboidHeight, cuboidWidth, cuboidDepth, score, index,RotationDirection.Vertical);
                     
                    }
                    // Debug.Log($"5 FindPositionForNewNode --> {bestScore}");
                }

                //6 Height x Depth x Width (rotate horizontally and vertically)
                if (cuboidHeight <= freeCuboidList.Width && cuboidDepth <= freeCuboidList.Height && cuboidWidth <= freeCuboidList.Depth && _parameter.AllowRotateVertically)
                {
              
                    var score = ScoreByHeuristic(cuboid, freeCuboidList, cuboidChoice);
                   // Debug.Log($"HDW --> {score}-{bestScore}");
                    if (score < bestScore)
                    {
                        bestScore = UpdateCuboid(cuboid, out freeCuboidIndex, freeCuboidList,
                            cuboidHeight, cuboidDepth, cuboidWidth, score, index,RotationDirection.Both);
                    }
                    //  Debug.Log($"6 FindPositionForNewNode --> {bestScore}");
                }
            }
        }

        private  decimal UpdateCuboid(Cuboid cuboid, out int freeCuboidIndex, Cuboid freeCuboidList, decimal width,
            decimal height, decimal depth, decimal score, int index, RotationDirection dir)
        {
            decimal bestScore;
            cuboid.UniqueId = cuboid.UniqueId;//GenerateUniqueId();
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
            
            float averageHeight = 1.76f; //TODO remove hardcode
           int tempHeight = Convert.ToInt32(cuboid.Y);
           cuboid.Layer = cuboid.Y == 0 ? 1 : (int) Math.Ceiling(tempHeight / averageHeight);
           Debug.Log($"{score} --> {cuboid.UniqueId}");
           UpdateCoordinates(cuboid);
            return bestScore;
        }

        public void UpdateCoordinates(Cuboid cuboid)
        {
            
            int x = 999, y , z = 999;
            y = cuboid.Layer - 1;
            if (cuboid.X == 0)
                x = 0;
            else
            {
               // this way of calculating cordonate is not correct 
               //TODO remove the hard code  3.5 is average width approx
               x = (int)Math.Round((float)cuboid.X / 3.5f)+1;
                 
            }

            if (cuboid.Z == 0)
                z = 0;
            else
            {
                // this way of calculating cordonate is not correct 
                //TODO remove the hard code  3.5 is average width approx
                z = (int)Math.Round((float)cuboid.Z / 3.5f);//(float)cuboid.Depth
            }

            int newX = 0, newZ = 0;
         //   Debug.Log($"****" +cuboid.Tag +"-->"+ cuboid.UniqueId);
           // Debug.Log($"false X Z --> {x}-{z}");
            for (int j = 0; j < x; j++)
            {
              //  Debug.Log($" XXX {j}{y}{z}--> {binMatrix[j,y,z]}");
                if(binMatrix[j,y,z] != null)
                {
                   // Debug.Log($"{cuboid.Tag } -{binMatrix[j,y,z].UniqueId} X--> {binMatrix[j,y,z]}");
                    newX++;
                }
             
            }

          
            for (int i = 0; i <= z; i++)
            {
               // Debug.Log($" ZZZ {x}{y}{i}--> {binMatrix[x,y,i]}");
                if(binMatrix[x,y,i] != null)
                {
                   // Debug.Log($"{cuboid.Tag } -{binMatrix[x,y,i].UniqueId} Z--> {binMatrix[x,y,i]}");
                    newZ++;
                }
            }

          //  Debug.Log($"---- --> {newX}-{y}-{newZ}");
            
            cuboid.Coordinate = new Vector3Int( newX,y,newZ);
           binMatrix[newX, y, newZ] = cuboid;
        }
        
        
        private static decimal ScoreByHeuristic(Cuboid cuboid, Cuboid freeCuboid, FreeCuboidChoiceHeuristic cuboidChoice)
        {
            
            switch (cuboidChoice)
            {
                case FreeCuboidChoiceHeuristic.CuboidMinHeight:
                    return freeCuboid.Y + cuboid.Height;
                case FreeCuboidChoiceHeuristic.CuboidMinWidth:
                {
                   // var score = freeCuboid.X + cuboid.Depth;
                    var score =  freeCuboid.X+cuboid.Depth;
                  //  Debug.Log($" myScore--> {score}");
                    return score;
                }
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
            _AvailableSpace.Add(freeCuboid);
        }

        public override string ToString()
        {
            return $"Guillotine({_cuboidChoice}, {_splitMethod})";
        }
        
        public  string GenerateUniqueId()
        {
             System.Random random = new System.Random ();
            const string chars = "0123456789";
            char[] id = new char[5];

            for (int i = 0; i < 5; i++)
            {
                id[i] = chars[random.Next(0, chars.Length)];
            }

            

            return new string( id);
        }
    }
}