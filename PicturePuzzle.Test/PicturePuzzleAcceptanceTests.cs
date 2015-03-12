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
        private const string WHITE = "0";
        private const string BLACK = "1";
        private const string AMBIGUOUS = "?";
        private const int WHITE_CELL = -12345;

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
        [TestCase("9 6", "???111???")]
        [TestCase("123 69",
            "??????????????????????????????????????????????????????111111111111111??????????????????????????????????????????????????????"
            )]
        public void Level_1(string input, string expected)
        {
            var cols = int.Parse(input.Split(BLANK)[0]);
            var blockLength = int.Parse(input.Split(BLANK)[1]);
            var actual = FillCells(cols, blockLength);
            Assert.That(actual, Is.EqualTo(expected), "cells");
        }

        [TestCase("5 2 1 2", "???1?")]
        [TestCase("5 2 2 2", "11011")]
        public void Level_2_spec_examples(string input, string expected)
        {
            var args = input.Split(BLANK).ToList();
            var cols = int.Parse(args[0]);
            var blocks = int.Parse(args[1]);

            var blockLengths = new List<int>();

            for (var i = 0; i < blocks; i++)
                blockLengths.Add(int.Parse(args[i + 2]));

            var actual = FillCells(cols, blockLengths);
            Assert.That(actual, Is.EqualTo(expected), "cells");
        }

        [TestCase("10 2 5 4", "1111101111")]
        [TestCase("10 2 5 3", "?1111??11?")]
        //[TestCase("30 0", "000000000000000000000000000000")]
        [TestCase("40 4 11 10 5 7", "????1111111?????111111?????1?????111????")]
        public void Level_2(string input, string expected)
        {
            var args = input.Split(BLANK).ToList();
            var cols = int.Parse(args[0]);
            var blocks = int.Parse(args[1]);

            var blockLengths = new List<int>();

            for (var i = 0; i < blocks; i++)
                blockLengths.Add(int.Parse(args[i + 2]));

            var actual = FillCells(cols, blockLengths);
            Assert.That(actual, Is.EqualTo(expected), "cells");
        }

        private string FillCells(int cols, List<int> blockLengths)
        {
            var allCells = new List<List<int>>();

            for (var i = 0; i < cols; i++)
            {
                var cells = new List<int>();
                var tmpIndex = i;

                if (tmpIndex + NecessaryLength(blockLengths) > cols)
                    break;

                foreach (var bl in blockLengths)
                {
                    for (var j = tmpIndex; j < (bl + tmpIndex); j++)
                        cells.Add(j);

                    tmpIndex += bl + 1; // White Cell
                }

                allCells.Add(cells);
            }

            if (allCells.Count > 1)
            {
                var intersection = allCells.Aggregate((prevCells, nextCells) => prevCells.Intersect(nextCells).ToList());
                return CreatePicture(cols, intersection);
            }
            else
            {
                // Handle white cells
                var intersection = allCells.Aggregate((prevCells, nextCells) => prevCells.Intersect(nextCells).ToList());
                var picture = CreatePicture(cols, intersection);
                return picture.Replace(AMBIGUOUS, WHITE);
            }
        }

        private int NecessaryLength(IEnumerable<int> blockLengths)
        {
            var space = 0;
            foreach (var bl in blockLengths)
            {
                space += bl;
                space++; // White cell;
            }
            return space - 1; // Last white cell not necessary
        }

        private string FillCells(int cols, int blockLength)
        {
            var allCells = new List<List<int>>();

            for (var i = 0; i < cols; i++)
            {
                if (blockLength > cols)
                    break;

                var cells = new List<int>();

                for (var j = i; j < blockLength; j++)
                    cells.Add(j);

                blockLength++;
                allCells.Add(cells);
            }

            var intersection = allCells.Aggregate((prevCells, nextCells) => prevCells.Intersect(nextCells).ToList());

            var picture = CreatePicture(cols, intersection);

            return picture;
        }

        private static string CreatePicture(int cols, IEnumerable<int> intersection)
        {
            var picture = "";

            for (var i = 0; i < cols; i++)
                picture += AMBIGUOUS;

            foreach (var i in intersection)
            {
                picture = picture.Remove(i, 1);
                picture = picture.Insert(i, BLACK);
            }

            return picture;
        }
    }
}