using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Services
{
    public class CommandsService : ICommandsService
    {
        private const char typeBreak = (char)28;
        private const char elementBreak = (char)29;
        private const char subElementBreak = (char)30;

        private const int indexOfCommandType = 0;

        private const int indexOfSeperatorReplaceBoolean = 1;
        private const int indexOfSeperatorName = 0;
        private const int indexOfSeperatorSearchTerm = 1;

        private const int indexOfWallName = 0;
        private const int indexOfWallTopBottomTrim = 1;
        private const int indexOfWallLeftTrim = 2;
        private const int indexOfWallRightTrim = 3;

        private const string SeperatorCommandIdentifier = "SeperatorCommand";
        private const string WallCommandIdentifier = "Wall";

        public string GenerateCommandsString(bool replaceNameNotAdd, List<CommandSeparatorElement> seperatorNamesAndQualifiers, List<CommandWall> walls)
        {
            bool seperator = !(seperatorNamesAndQualifiers == null || seperatorNamesAndQualifiers.Count == 0);//if the seperator list is null or empty no seperator command will be made
            bool wall = !(walls == null || walls.Count == 0);//if the wall list is null or empty no wall command will be made
            if(seperator == false && wall == false)
            {
                return string.Empty;
            }
            else if(seperator == true && wall == true)
            {
                return GenerateCommandsStringFromSeperator(replaceNameNotAdd, seperatorNamesAndQualifiers) + typeBreak + GenerateCommandsStringFromWalls(walls);
            }
            else if(seperator == true && wall == false)
            {
                return GenerateCommandsStringFromSeperator(replaceNameNotAdd, seperatorNamesAndQualifiers);
            }
            else if(seperator == false && wall == true)
            {
                return GenerateCommandsStringFromWalls(walls);
            }
            else
            {
                throw new Exception("this should not be possible as every combination of the booleans seperator and wall were supposed to be covered");
            }
        }

        public string GenerateCommandsStringFromSeperator(bool replaceNameNotAdd, List<CommandSeparatorElement> seperatorNamesAndQualifiers)
        {
            if(indexOfCommandType != 0 || indexOfSeperatorReplaceBoolean != 1 || indexOfSeperatorName != 0 || indexOfSeperatorSearchTerm != 1)
            {
                throw new Exception("this function is made with certain assumptions, which includes the order of indexes. the index order does not match and so this function doesn't work appropriately");
            }

            if(seperatorNamesAndQualifiers == null || seperatorNamesAndQualifiers.Count == 0)
            {
                return string.Empty;
            }

            string commandString = SeperatorCommandIdentifier + elementBreak + replaceNameNotAdd.ToString();

            foreach(CommandSeparatorElement element in seperatorNamesAndQualifiers)
            {
                commandString += elementBreak + element.SeperatorName + subElementBreak + element.SearchTerm;
            }

            return commandString;
        }

        public string GenerateCommandsStringFromWalls(List<CommandWall> walls)
        {
            if (indexOfCommandType != 0 || indexOfWallName != 0 || indexOfWallTopBottomTrim != 1 || indexOfWallLeftTrim != 2 || indexOfWallRightTrim != 3)
            {
                throw new Exception("this function is made with certain assumptions, which includes the order of indexes. the index order does not match and so this function doesn't work appropriately");
            }

            if (walls == null || walls.Count == 0)
            {
                return string.Empty;
            }

            string commandString = WallCommandIdentifier;

            foreach(CommandWall wall in walls)
            {
                commandString += elementBreak + wall.ImageName + subElementBreak + wall.TopBottomTrim + subElementBreak + wall.LeftTrim + subElementBreak + wall.RightTrim;
            }

            throw new NotImplementedException();
        }

        public bool HasSeperatorCommand(string commandString)
        {
            if(commandString == null || string.IsNullOrWhiteSpace(commandString))
            {
                return false;
            }

            string[] commands = commandString.Split(typeBreak);

            foreach (string command in commands)
            {
                if (command.Split(elementBreak)[indexOfCommandType].Equals(SeperatorCommandIdentifier))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasWallCommand(string commandString)
        {
            if (commandString == null || string.IsNullOrWhiteSpace(commandString))
            {
                return false;
            }

            string[] commands = commandString.Split(typeBreak);

            foreach (string command in commands)
            {
                if (command.Split(elementBreak)[indexOfCommandType].Equals(WallCommandIdentifier))
                {
                    return true;
                }
            }

            return false;
        }

        public List<CommandSeparatorElement> SeperateCommandGetElements(string commandString)
        {
            List<CommandSeparatorElement> result = new();

            if (commandString == null || string.IsNullOrWhiteSpace(commandString))
            {
                return result;
            }

            string[] commands = commandString.Split(typeBreak);

            foreach (string command in commands)
            {
                string[] commandElements = command.Split(elementBreak);

                if (commandElements[indexOfCommandType].Equals(SeperatorCommandIdentifier))
                {
                    for (int i = 0; i < commandElements.Length; i++)
                    {
                        if(i != indexOfCommandType || i != indexOfSeperatorReplaceBoolean)
                        {
                            string[]commandSubElements = commandElements[i].Split(subElementBreak);
                            result.Add(new() {
                                SeperatorName = commandSubElements[indexOfSeperatorName],
                                SearchTerm = commandSubElements[indexOfSeperatorSearchTerm]
                            });
                        }
                    }
                    break;
                }
            }

            return result;
        }

        public bool SeperateCommandGetReplaceInsteadOfAddName(string commandString)
        {
            if (commandString == null || string.IsNullOrWhiteSpace(commandString))
            {
                return false;
            }

            string[] commands = commandString.Split(typeBreak);

            foreach (string command in commands)
            {
                string[] commandElements = command.Split(elementBreak);

                if (commandElements[indexOfCommandType].Equals(SeperatorCommandIdentifier))
                {
                    return bool.Parse(commandElements[indexOfSeperatorReplaceBoolean]);
                }
            }

            return false;
        }

        public List<CommandWall> WallCommandGetElements(string commandString)
        {
            List<CommandWall> result = new();

            if (commandString == null || string.IsNullOrWhiteSpace(commandString))
            {
                return result;
            }

            string[] commands = commandString.Split(typeBreak);

            foreach (string command in commands)
            {
                string[] commandElements = command.Split(elementBreak);

                if (commandElements[indexOfCommandType].Equals(WallCommandIdentifier))
                {
                    for (int i = 0; i < commandElements.Length; i++)
                    {
                        if (i != indexOfCommandType)
                        {
                            string[] commandSubElements = commandElements[i].Split(subElementBreak);
                            result.Add(new() {
                                ImageName = commandSubElements[indexOfWallName],
                                TopBottomTrim = int.Parse(commandSubElements[indexOfWallTopBottomTrim]),
                                LeftTrim = int.Parse(commandSubElements[indexOfWallLeftTrim]),
                                RightTrim = int.Parse(commandSubElements[indexOfWallRightTrim])
                            });
                        }
                    }
                    break;
                }
            }

            return result;
        }
    }
}
