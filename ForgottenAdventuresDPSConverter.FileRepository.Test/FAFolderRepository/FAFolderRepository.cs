using Moq;
using ForgottenAdventuresDPSConverter.FileRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.FileRepository.Test.FAFolderRepository
{
    public class FAFolderRepository : IDisposable
    {
        #region values to get a unique filepath
        private static int fileNumber = 20;
        private static List<int> usedFileNumbers = new();
        private static Random random = new();
        private const string filePathBase = @"C:\Users\Octavia\AppData\Local\Temp\ForgottenAdventuresDPSConverterTest{0}.tmp";

        #endregion
        private const string incorrectFileContentWithNoId = "this file has no identifier and should thus fail the test";
        private static readonly string incorrectFileCententWithId = "-1" + (char)31 + "this file has identifier but should still fail the test";
        #region values reused in tests

        #endregion

        private readonly string filePath;
        private readonly Mock<IFileRepositorySettings> mockSettings;

        private FileRepository.FAFolderFileRepository sut;

        public FAFolderRepository()
        {
            while (usedFileNumbers.Contains(fileNumber))
            {
                fileNumber += (int)random.NextInt64(1, 20);
            }
            usedFileNumbers.Add(fileNumber);

            filePath = string.Format(filePathBase, fileNumber);

            mockSettings = new();
            mockSettings.Setup(m => m.FAFolderRepositoryFilePath).Returns(filePath);
        }

        [Fact]
        public void Creates_file_if_it_does_not_exist()
        {
            //arrange
            if (File.Exists(filePath))
            {
                throw new Exception("file already existed");
            }

            //act
            sut = new FileRepository.FAFolderFileRepository(mockSettings.Object);

            //assert
            Assert.True(File.Exists(filePath));
        }

        [Fact]
        public void Thorws_nothing_when_file_matches_what_is_expected()
        {
            //arrange
            if (File.Exists(filePath))
            {
                throw new Exception("file already existed");
            }
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine(""+ -1 + (char)31 + 4 + "the first line of the file will serve the purpose of keeping track of some file specific things such as the next id. the rest of the file will contain one line per folder item. this file should practically never be touched by hand as that will near quarentied create problems. this line also functions as a check to make sure the file is a file to be edited by the program and should thus not be touched.");
                writer.WriteLine(1 + (char)31 + "Burial_and_Graves" + (char)31 + @"\Burial_and_Graves" + (char)31 + (char)31 + "False" + (char)31 + (char)31 + (char)31 + (char)31 + "False" + (char)31 + "False" + (char)31 + (char)31);
                writer.WriteLine(2 + (char)31 + "Coffins" + (char)31 + @"\Burial_and_Graves\Coffins" + (char)31 + "1" + (char)31 + "True" + (char)31 + (char)31 + (char)31 + (char)31 + "False" + (char)31 + "False" + (char)31 + (char)31);
                writer.WriteLine(3 + (char)31 + "Graves" + (char)31 + @"\Burial_and_Graves\Graves" + (char)31 + "1" + (char)31 + "True" + (char)31 + (char)31 + (char)31 + (char)31 + "False" + (char)31 + "False" + (char)31 + (char)31);
            }

            //act
            new FileRepository.FAFolderFileRepository(mockSettings.Object);

            //assert
        }

        [Fact]
        public void Throws_exception_when_file_does_not_have_identifiers_of_being_the_right_file_with_no_id()
        {
            //arrange
            if (File.Exists(filePath))
            {
                throw new Exception("file already existed");
            }
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine(incorrectFileContentWithNoId);
            }

            //act
            Action action = () => { sut = new FileRepository.FAFolderFileRepository(mockSettings.Object); };

            //assert
            Assert.ThrowsAny<Exception>(action);
        }

        [Fact]
        public void Throws_correct_exception_when_file_does_not_have_identifiers_of_being_the_right_file_with_no_id()
        {
            //arrange
            if (File.Exists(filePath))
            {
                throw new Exception("file already existed");
            }
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine(incorrectFileContentWithNoId);
            }

            //act
            Action action = () => { sut = new FileRepository.FAFolderFileRepository(mockSettings.Object); };

            //assert
            InvalidDataException exception = Assert.Throws<InvalidDataException>(action);
        }

        [Fact]
        public void Throws_exception_when_file_does_not_have_identifiers_of_being_the_right_file_with_id()
        {
            //arrange
            if (File.Exists(filePath))
            {
                throw new Exception("file already existed");
            }
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine(incorrectFileCententWithId);
            }

            //act
            Action action = () => { sut = new FileRepository.FAFolderFileRepository(mockSettings.Object); };

            //assert
            Assert.ThrowsAny<Exception>(action);
        }

        [Fact]
        public void Throws_correct_exception_when_file_does_not_have_identifiers_of_being_the_right_file_with_id()
        {
            //arrange
            if (File.Exists(filePath))
            {
                throw new Exception("file already existed");
            }
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine(incorrectFileCententWithId);
            }

            //act
            Action action = () => { sut = new FileRepository.FAFolderFileRepository(mockSettings.Object); };

            //assert
            InvalidDataException exception = Assert.Throws<InvalidDataException>(action);
        }

        public void Dispose()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
