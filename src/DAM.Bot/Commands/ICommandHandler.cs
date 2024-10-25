using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands
{
    public interface ICommandHandler
    {
        Task InitializeAsync();
    }
}
