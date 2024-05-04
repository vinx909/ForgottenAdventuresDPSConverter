using ForgottenAdventuresDPSConverter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface ICommandsService
    {
        /// <summary>
        /// generate the string containing the commands for a seperator command.
        /// </summary>
        /// <param name="replaceNameNotAdd">a boolean that's true if the CommandSeparatorElement.Name is to replace the folder name instead of being added to the end</param>
        /// <param name="seperatorNamesAndQualifiers">a list of CommandSeparatorElements that contain the name and the search term for each seperator. this list is ordered as something that will be found with the search term of the first element will not be added to a later element if it also matches that search element</param>
        /// <returns>a string containing all the given elements of the Seperator command</returns>
        public string GenerateCommandsStringFromSeperator(bool replaceNameNotAdd, List<CommandSeparatorElement> seperatorNamesAndQualifiers);
        /// <summary>
        /// generate the string containing the commands for a wall command.
        /// </summary>
        /// <param name="walls">a list of CommandWall elements each with a name and the margins that need to be cut to create a wall item</param>
        /// <returns>a string containing all the given elements of the Wall command</returns>
        public string GenerateCommandsStringFromWalls(List<CommandWall> walls);
        /// <summary>
        /// generate the string containing the commands for a seperator and a wall command.
        /// </summary>
        /// <param name="replaceNameNotAdd">a boolean that's true if the CommandSeparatorElement.Name is to replace the folder name instead of being added to the end</param>
        /// <param name="seperatorNamesAndQualifiers">a list of CommandSeparatorElements that contain the name and the search term for each seperator. this list is ordered as something that will be found with the search term of the first element will not be added to a later element if it also matches that search element</param>
        /// <param name="walls">a list of CommandWall elements each with a name and the margins that need to be cut to create a wall item</param>
        /// <returns>a string containing all given commands of both the seperator command and the wall command</returns>
        public string GenerateCommandsString(bool replaceNameNotAdd, List<CommandSeparatorElement> seperatorNamesAndQualifiers, List<CommandWall> walls);
        /// <summary>
        /// get a boolean that tells if there are seperator commands
        /// </summary>
        /// <param name="commandString">the string to be checked for seperator commands</param>
        /// <returns>a boolean that's true if there are seperator commands</returns>
        public bool HasSeperatorCommand(string commandString);
        /// <summary>
        /// get a boolean that tells if there are seperator commands
        /// </summary>
        /// <param name="folder">the FAFolder to be checked for seperator commands</param>
        /// <returns>>a boolean that's true if there are seperator commands</returns>
        public virtual bool HasSeperatorCommand(FAFolder folder)
        {
            return HasSeperatorCommand(folder.Commands);
        }
        /// <summary>
        /// get a boolean that tells if there are wall commands
        /// </summary>
        /// <param name="commandString">the string to be checked for wall commands</param>
        /// <returns>a boolean that's true if there are wall commands</returns>
        public bool HasWallCommand(string commandString);
        /// <summary>
        /// get a boolean that tells if there are wall commands
        /// </summary>
        /// <param name="folder">the FAFolder to be checked for wall commands</param>
        /// <returns>a boolean that's true if there are wall commands</returns>
        public virtual bool HasWallCommand(FAFolder folder)
        {
            return HasWallCommand(folder.Commands);
        }
        /// <summary>
        /// get a boolean that determains if the name is to be replaced instead of added to
        /// </summary>
        /// <param name="commandString">the string to get the boolean out of</param>
        /// <returns>a boolean that tells if the name is to be replaced or not</returns>
        public bool SeperateCommandGetReplaceInsteadOfAddName(string commandString);
        /// <summary>
        /// get a boolean that determains if the name is to be replaced instead of added to
        /// </summary>
        /// <param name="folder">the FAFolder to get the boolean out of</param>
        /// <returns>a boolean that tells if the name is to be replaced or not</returns>
        public virtual bool SeperateCommandGetReplaceInsteadOfAddName(FAFolder folder)
        {
            return SeperateCommandGetReplaceInsteadOfAddName(folder.Commands);
        }
        /// <summary>
        /// get a list of all seperate command elements
        /// </summary>
        /// <param name="commandString">the string to get the seperate command elements out of</param>
        /// <returns>a list of seperate command elements</returns>
        public List<CommandSeparatorElement> SeperateCommandGetElements(string commandString);
        /// <summary>
        /// get a list of all seperate command elements
        /// </summary>
        /// <param name="folder">the FAFolder to get the seperate command elements out of</param>
        /// <returns>a list of seperate command elements</returns>
        public virtual List<CommandSeparatorElement> SeperateCommandGetElements(FAFolder folder)
        {
            return SeperateCommandGetElements(folder.Commands);
        }
        /// <summary>
        /// get a list of all wall commands stored
        /// </summary>
        /// <param name="commandString">the string to get the wall commands out of</param>
        /// <returns>a list of wall commands</returns>
        public List<CommandWall> WallCommandGetElements(string commandString);
        /// <summary>
        /// get a list of all wall commands stored
        /// </summary>
        /// <param name="folder">the FAFolder to get the wall commands out of</param>
        /// <returns>a list of wall commands</returns>
        public virtual List<CommandWall> WallCommandGetElements(FAFolder folder)
        {
            return WallCommandGetElements(folder.Commands);
        }
    }
}
