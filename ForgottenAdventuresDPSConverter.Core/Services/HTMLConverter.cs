using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Services
{
    public class HtmlConverter : IHtmlConverter
    {
        public void FAHtmlConvert(List<FAFolder> folders, string targetPath)
        {
            if (File.Exists(targetPath))
                return;

            const string tableStart = "<head><style> body {background-color: black} table {border-collapse: collapse;}th, td {padding: 8px;text - align: left;border: 1px solid #ddd; background-color:darkgrey}</style></head><body><table>";
            const string tableEnd = "</table></body>";
            const string tableRowStart = "<tr>";
            const string tableRowEnd = "</tr>";
            const string tableCollumStart = "<td rowspan=\"{0}\" title=\"{1}\">"; //0: rowspan value
            const string tableCollumEnd = "</td>";
            const string boldStart = "<b>";
            const string boldEnd = "</b>";

            const string TableCollumStartContainsItems = tableCollumStart;
            const string TableCollumEndContainsItems = tableCollumEnd;
            const string TableCollumStartNoItems = tableCollumStart + boldStart;
            const string TableCollumEndNoItems = boldEnd + tableCollumEnd;

            bool didRowEndLast = false;

            Dictionary<FAFolder, int> folderHeights = new();

            fillFolderHeights();

            string tablestring = tableStart;

            foreach (FAFolder folder in folders.Where(f => f.ParentId == null))
            {
                if (folder.HasItems)
                {
                    tablestring += tableRowStart + string.Format(TableCollumStartContainsItems, folderHeights[folder], GetParentString(folders, folder.Id)) + folder.Name + TableCollumEndContainsItems;
                }
                else
                {
                    tablestring += tableRowStart + string.Format(TableCollumStartNoItems, folderHeights[folder], GetParentString(folders, folder.Id)) + folder.Name + TableCollumEndNoItems;
                }
                tryAddSubfolders(folder.Id);
                tablestring += tableRowEnd;//maybe instead remove the length of tableRowStart if didRowEndLast == true if an unnessisary end seems to happen
            }

            tablestring += tableEnd;


            void fillFolderHeights()
            {
                folderHeights.Clear();//clear folderHeights to avoid potential for duplicate items.
                List<FAFolder> foldersToDo = new(folders); //reference to all folders that will be removed from when they have their height figured out to keep track of which ones have and have not been done
                while (foldersToDo.Count() > 0)
                {
                    FigureOutFolderHeight(foldersToDo[0]);
                }

                void FigureOutFolderHeight(FAFolder folderToFigureOut)
                {
                    int height;
                    List<FAFolder> subfolders = new(folders.Where(f => f.ParentId == folderToFigureOut.Id)); //reference to all direct subfolders of the folder to figure out

                    if (subfolders.Count == 0)
                    {
                        height = 1; //if it has no subfolders the height is 1, the hight of only it's own box
                    }
                    else
                    {
                        height = 0;
                        foreach (FAFolder subfolder in subfolders) //if it has subfolders check for each one if it's either known in folderHeights or figure it out with recursion, then add them all together to get the folder height of this folder
                        {
                            if (!folderHeights.ContainsKey(subfolder))
                            {
                                FigureOutFolderHeight(subfolder);
                            }
                            height += folderHeights[subfolder];
                        }
                    }

                    folderHeights.Add(folderToFigureOut, height); //when the height is figure out add it to folderHeights and remove it from folders to do
                    foldersToDo.Remove(folderToFigureOut);
                }
            }

            void tryAddSubfolders(int parentFolderId)
            {
                foreach (FAFolder folder in folders.Where(f => f.ParentId == parentFolderId))
                {
                    if (folder.HasItems)
                    {
                        tablestring += string.Format(TableCollumStartContainsItems, folderHeights[folder], GetParentString(folders, folder.Id)) + folder.Name + TableCollumEndContainsItems;
                    }
                    else
                    {
                        tablestring += string.Format(TableCollumStartNoItems, folderHeights[folder], GetParentString(folders, folder.Id)) + folder.Name + TableCollumEndNoItems;
                    }
                    didRowEndLast = false;
                    tryAddSubfolders(folder.Id);
                    if (didRowEndLast == false)
                    {
                        tablestring += tableRowEnd + tableRowStart;
                        didRowEndLast = true;
                    }
                }
            }

            using (StreamWriter writer = new(targetPath))
            {
                writer.Write(tablestring);
            }
        }
        
        private string GetParentString(List<FAFolder> folders, int? ID)
        {
            const string indicator = " -> ";

            string returnString = "";
            while(ID != null)
            {
                FAFolder folder = folders.First(folder => folder.Id == ID);
                if (!string.IsNullOrWhiteSpace(returnString))
                {
                    returnString = indicator + returnString;
                }
                returnString = folder.Name + returnString;
                ID = folder.ParentId;
            }
            return returnString;
        }
    }
}
