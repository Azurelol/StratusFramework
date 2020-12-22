using System;
using NUnit.Framework;
using UnityEngine;
using Stratus;
using System.Collections.Generic;

namespace Stratus.Tests
{
    public class StratusSearchTests
    {
        [Test]
        public void TestSearchRange()
        {
            Vector3Int[] squareNeighbors(Vector3Int element)
            {
                List<Vector3Int> result = new List<Vector3Int>();
                result.Add(new Vector3Int(element.x + 1, element.y, element.z));
                result.Add(new Vector3Int(element.x - 1, element.y, element.z));
                result.Add(new Vector3Int(element.x, element.y + 1, element.z));
                result.Add(new Vector3Int(element.x, element.y - 1, element.z));
                return result.ToArray();
            }

            Vector3Int startElement = new Vector3Int(0, 0, 0);

            StratusSearch<Vector3Int>.RangeSearch search
                = new StratusSearch<Vector3Int>.RangeSearch()
                {
                    debug = true,
                    distanceFunction = Vector3Int.Distance,
                    neighborFunction = squareNeighbors,
                    range = 1,
                    startElement = startElement
                };

            Vector3Int[] range = search.Search();
            Debug.Log($"Range : {range.Length}");
            foreach(var element in range)
            {
                Debug.Log(element);
            }
        }
    }

}