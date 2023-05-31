using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.ConsoleApp
{
    static internal class WallsConverter
    {
        const string filesFolderPath = @"F:\Games\Steam\steamapps\common\Dungeon Painter Studio\data\collections\.1ST.Wall\objects";
        const string wallsFolderPath = @"F:\Games\Steam\steamapps\common\Dungeon Painter Studio\data\collections\.1ST.Wall\walls";
        const string fileMustContain = "Straight";
        const char splitOnChar = '\\';

        static public void ConvertAllWalls(IWallConverter wallConverter)
        {
            Console.WriteLine("this will try to convert all straight walls from \"" + filesFolderPath + "\". it's probably done best if \""+wallsFolderPath+"\"is empty. are you sure you want to continue? if yes type \"yup\"");
            if (Console.ReadLine() == "yup")
            {
                List<string> files = new(Directory.GetFiles(filesFolderPath, "", SearchOption.AllDirectories)); //get all files

                for (int i = files.Count-1; i >= 0; i--)
                {
                    if (files[i].Split(splitOnChar).Last().Contains(fileMustContain) == false)
                    {
                        files.RemoveAt(i); //only keep the files that have "Straight" in their name
                    }
                }

                List<Task> tasks = new();
                foreach (string file in files)
                {
                    string newFile = file.Replace(filesFolderPath, wallsFolderPath);

                    string[] newFileSubstrings = newFile.Split(splitOnChar);
                    string newDirectory = newFileSubstrings[0];
                    for(int i = 1; i < newFileSubstrings.Length-1; i++)
                    {
                        newDirectory += splitOnChar + newFileSubstrings[i];
                    }

                    if(Directory.Exists(newDirectory) == false)
                    {
                        Directory.CreateDirectory(newDirectory);
                    }

                    tasks.Add(wallConverter.ConvertWall(file, newFile));
                }

                foreach (Task task in tasks)
                {
                    task.Wait();
                }
                Console.WriteLine("done");
            }
        }
    }
}
