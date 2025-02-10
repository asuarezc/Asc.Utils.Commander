using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asc.Utils.Commander.Implementation;

internal class CommandBuilder : ICommandBuilder
{
    public ICommand Build()
    {
        throw new NotImplementedException();
    }

    public ICommandBuilder Job(Action job)
    {
        return this;
    }

    public ICommandBuilder Job(Func<Task> job)
    {
        return this;
    }

    public ICommandBuilder OnFailure(Action<Exception> onFailure)
    {
        return this;
    }

    public ICommandBuilder OnFailure(Func<Exception, Task> onFailure)
    {
        return this;
    }

    public ICommandBuilder OnSpecificFailure<TException>(Action<TException> onFailure) where TException : Exception
    {
        return this;
    }

    public ICommandBuilder OnSpecificFailure<TException>(Func<TException, Task> onSuccess) where TException : Exception
    {
        return this;
    }

    public ICommandBuilder OnSuccess(Action onSuccess)
    {
        return this;
    }

    public ICommandBuilder OnSuccess(Func<Task> onSuccess)
    {
        return this;
    }
}
