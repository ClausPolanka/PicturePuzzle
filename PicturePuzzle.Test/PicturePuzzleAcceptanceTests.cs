using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        [TestCase("30 0", "000000000000000000000000000000")]
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

        [TestCase("10 2 1 5 2 4", "?1111?222?")]
        [TestCase("10 2 1 5 2 3", "??111??2??")]
        [TestCase("12 3 1 4 2 3 3 1", "????????????")]
        [TestCase("8 2 2 4 2 3", "22220222")]
        public void Level_3_spec_examples(string input, string expected)
        {
            var args = input.Split(BLANK).ToList();
            var cols = int.Parse(args[0]);
            var nrBlocks = int.Parse(args[1]);

            var blocks = new List<Block>();

            for (var i = 0; i <= nrBlocks; i += 2)
                blocks.Add(new Block { Color = int.Parse(args[i + 2]), Length = int.Parse(args[i + 3]) });

            var actual = FillCells(cols, blocks);
            Assert.That(actual, Is.EqualTo(expected), "cells");
        }

        private string FillCells(int cols, List<Block> blocks)
        {
            if (blocks.Count == 0)
                return CreateWhitePicture(cols);

            var allCells = new List<List<Cell>>();

            for (var i = 0; i < cols; i++)
            {
                var cells = new List<Cell>();
                var tmpIndex = i;

                if (tmpIndex + NecessaryLength(blocks) > cols)
                    break;

                var lastColor = -1;
                foreach (var bl in blocks)
                {
                    if (lastColor == bl.Color)
                        tmpIndex += 1;

                    for (var j = tmpIndex; j < (bl.Length + tmpIndex); j++)
                        cells.Add(new Cell(index: j, color: bl.Color));

                    tmpIndex += bl.Length;
                    lastColor = bl.Color;
                }

                allCells.Add(cells);
            }

            var intersection = allCells.Aggregate((prevCells, nextCells) => prevCells.Intersect(nextCells).ToList());
            var picture = CreatePicture(cols, intersection);

            return allCells.Count > 1 ? picture : picture.Replace(AMBIGUOUS, WHITE);
        }

        private string FillCells(int cols, List<int> blockLengths)
        {
            if (blockLengths.Count == 0)
                return CreateWhitePicture(cols);

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

            var intersection = allCells.Aggregate((prevCells, nextCells) => prevCells.Intersect(nextCells).ToList());
            var picture = CreatePicture(cols, intersection);

            return allCells.Count > 1 ? picture : picture.Replace(AMBIGUOUS, WHITE);
        }

        private static string CreateWhitePicture(int cols)
        {
            var cells = "";

            for (var i = 0; i < cols; i++)
                cells += WHITE;

            return cells;
        }

        private int NecessaryLength(IEnumerable<Block> blocks)
        {
            var space = 0;
            var lastColor = -1;
            foreach (var bl in blocks)
            {
                if (lastColor == bl.Color)
                    space++;

                space += bl.Length;
                lastColor = bl.Color;
            }
            return space; // Last white cell not necessary
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

        private static string CreatePicture(int cols, List<Cell> cells)
        {
            var picture = "";

            for (var i = 0; i < cols; i++)
            {
                picture += AMBIGUOUS;
            }

            foreach (var c in cells)
            {
                picture = picture.Remove(c.Index, 1);
                picture = picture.Insert(c.Index, c.Color.ToString());
            }

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

    public class Cell
    {
        private readonly int index;
        private readonly int color;

        public Cell(int index, int color)
        {
            this.index = index;
            this.color = color;
        }

        public int Index
        {
            get { return index; }
        }

        public int Color
        {
            get { return color; }
        }

        protected bool Equals(Cell other)
        {
            return index == other.index && color == other.color;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Cell) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (index*397) ^ color;
            }
        }
    }

    public struct Block
    {
        public int Length { get; set; }
        public int Color { get; set; }
    }
}