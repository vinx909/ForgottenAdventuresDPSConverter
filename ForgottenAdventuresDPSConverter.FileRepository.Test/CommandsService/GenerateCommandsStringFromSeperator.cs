using ForgottenAdventuresDPSConverter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.FileRepository.Test.CommandsService
{
    public class GenerateCommandsStringFromSeperator : CommandsServiceTestBase
    {
        private const char elementBreak = (char)29;

        [Fact]
        public void ReturnsEmptyStringIfListIsNull()
        {
            //arrange


            //act
            var result = sut.GenerateCommandsStringFromSeperator(false, null);

            //assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ReturnsEmptyStringIfListIsEmpty()
        {
            //arrange
            List<CommandSeparatorElement> commandSeparatorElements = new();

            //act
            var result = sut.GenerateCommandsStringFromSeperator(false, commandSeparatorElements);

            //assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ReturnsNotEmptyStringIfListNotEmpty()
        {
            //arrange
            List<CommandSeparatorElement> commandSeparatorElements = new() { new() { SeperatorName = "abc", SearchTerm = "def" } };

            //act
            var result = sut.GenerateCommandsStringFromSeperator(false, commandSeparatorElements);

            //assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(100)]
        public void ReturnsStringSeperatableIntoNumberEqualToListLengthPlusTwo(int numberInList)
        {
            //arrange
            List<CommandSeparatorElement> commandSeparatorElements = new();
            for(int i = 0; i < numberInList; i++)
            {
                commandSeparatorElements.Add(new() { SeperatorName = "a" + i, SearchTerm = "b" + i });
            }

            //act
            string result = sut.GenerateCommandsStringFromSeperator(false, commandSeparatorElements);
            int number = result.Split(elementBreak).Length;

            //assert
            Assert.Equal(numberInList+2, number);
        }

        [Fact]
        public void SeperatedStringFirstPartEqualsCorrectIdentifierIfListNotEmpty()
        {
            //arrange
            List<CommandSeparatorElement> commandSeparatorElements = new() { new() { SeperatorName = "abc", SearchTerm = "def" } };

            //act
            string[] results = sut.GenerateCommandsStringFromSeperator(false, commandSeparatorElements).Split(elementBreak);

            //assert
            Assert.Equal("SeperatorCommand", results[0]);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SeperatedStringSecondPartEqualsGivenBooleanIfListNotEmpty(bool boolean)
        {
            //arrange
            List<CommandSeparatorElement> commandSeparatorElements = new() { new() { SeperatorName = "abc", SearchTerm = "def" } };

            //act
            string[] results = sut.GenerateCommandsStringFromSeperator(boolean, commandSeparatorElements).Split(elementBreak);

            //assert
            Assert.Equal(boolean.ToString(), results[1]);
        }
    }
}
