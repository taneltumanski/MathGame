using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Core.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
