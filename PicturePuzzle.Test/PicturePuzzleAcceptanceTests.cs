using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PicturePuzzle.Test
{
    [TestFixture]
    public class PicturePuzzleAcceptanceTests
    {
        private const char BLANK = ' ';
        private const string BLACK = "1";

        [TestCase("5 3", "??1??")]
        [TestCase("5 4", "?111?")]
        public void Level_1_spec_examples(string input, string expected)
        {
            var cols = int.Parse(input.Split(BLANK)[0]);
            var blockLength = int.Parse(input.Split(BLANK)[1]);
            var actual = FillCells(cols, blockLength);
            Assert.That(actual, Is.EqualTo(expected), "cells");
        }

        [TestCase("18 18", "111111111111111111")]
        public void Level_1(string input, string expected)
        {
            var cols = int.Parse(input.Split(BLANK)[0]);
            var blockLength = int.Parse(input.Split(BLANK)[1]);
            var actual = FillCells(cols, blockLength);
            Assert.That(actual, Is.EqualTo(expected), "cells");
        }



        private string FillCells(int cols, int blockLength)
        {
            var allCells = new List<List<int>>();
            for (var i = 0; i < cols; i++)
            {
                if (blockLength > cols)
                {
                    break;
                }
                var cells = new List<int>();
                for (var j = i; j < blockLength; j++)
                {
                    cells.Add(j);
                }
                blockLength++;
                allCells.Add(cells);
            }
            var intersection = allCells.Aggregate((prevCells, nextCells) => prevCells.Intersect(nextCells).ToList());

            var picture = "";
            for (int i = 0; i < cols; i++)
            {
                picture += "?";
            }

            foreach (var i in intersection)
            {
                picture = picture.Remove(i, 1);
                picture = picture.Insert(i, BLACK);
            }

            return picture;
        }
    }
}
