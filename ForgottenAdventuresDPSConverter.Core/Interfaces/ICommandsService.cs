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
        /// todo: summery
        /// </summary>
        /// <param name="replaceNameNotAdd">a boolean that's true if the CommandSeparatorElement.Name is to replace the folder name instead of being added to the end</param>
        /// <param name="seperatorNamesAndQualifiers">a list of CommandSeparatorElements that contain the name and the search term for each seperator. this list is ordered as something that will be found with the search term of the first element will not be added to a later element if it also matches that search element</param>
        /// <returns>a string containing all the given elements of the Seperator command</returns>
        public string GenerateCommandsStringFromSeperator(bool replaceNameNotAdd, List<CommandSeparatorElement> seperatorNamesAndQualifiers);
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="walls">a list of CommandWall elements each with a name and the margins that need to be cut to create a wall item</param>
        /// <returns>a string containing all the given elements of the Wall command</returns>
        public string GenerateCommandsStringFromWalls(List<CommandWall> walls);
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="replaceNameNotAdd">a boolean that's true if the CommandSeparatorElement.Name is to replace the folder name instead of being added to the end</param>
        /// <param name="seperatorNamesAndQualifiers">a list of CommandSeparatorElements that contain the name and the search term for each seperator. this list is ordered as something that will be found with the search term of the first element will not be added to a later element if it also matches that search element</param>
        /// <param name="walls">a list of CommandWall elements each with a name and the margins that need to be cut to create a wall item</param>
        /// <returns>a string containing all given commands of both the seperator command and the wall command</returns>
        public string GenerateCommandsString(bool replaceNameNotAdd, List<CommandSeparatorElement> seperatorNamesAndQualifiers, List<CommandWall> walls);
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="commandString"></param>
        /// <returns></returns>
        public bool HasSeperatorCommand(string commandString);
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual bool HasSeperatorCommand(FAFolder folder)
        {
            return HasSeperatorCommand(folder.Commands);
        }
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="commandString"></param>
        /// <returns></returns>
        public bool HasWallCommand(string commandString);
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual bool HasWallCommand(FAFolder folder)
        {
            return HasWallCommand(folder.Commands);
        }
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="commandString"></param>
        /// <returns></returns>
        public bool SeperateCommandGetReplaceInsteadOfAddName(string commandString);
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual bool SeperateCommandGetReplaceInsteadOfAddName(FAFolder folder)
        {
            return WallCommandGetReplaceInsteadOfAddName(folder.Commands);
        }
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="commandString"></param>
        /// <returns></returns>
        public List<CommandSeparatorElement> SeperateCommandGetElements(string commandString);
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual List<CommandSeparatorElement> SeperateCommandGetElements(FAFolder folder)
        {
            return SeperateCommandGetElements(folder.Commands);
        }
        /// <summary>
        /// todo: summery
        /// </summary>
        /// <param name="commandString"></param>
        /// <returns></returns>
        public List<CommandWall> WallCommandGetElements(string commandString);
        /// <summary>
        /// get a list of all wall commands store in a folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual List<CommandWall> WallCommandGetElements(FAFolder folder)
        {
            return WallCommandGetElements(folder.Commands);
        }
    }
}
