using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.FileRepository.Test.CommandsService
{
    public abstract class CommandsServiceTestBase
    {
        protected readonly ICommandsService sut;

        public CommandsServiceTestBase()
        {
            sut = new Core.Services.CommandsService();
        }
    }
}
