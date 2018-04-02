using MathGame.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
