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

        [TestCase("5 3", "??1??")]
        public void Level_1_spec_examples(string input, string expected)
        {
            var cols = input.Split(BLANK)[0];
            var blockLength = input.Split(BLANK)[1];
            var actual = FillCells(cols, blockLength);
            Assert.That(actual, Is.EqualTo(expected), "cells");
        }

        private string FillCells(string cols, string blockLength)
        {
            return string.Empty;
        }
    }
}
