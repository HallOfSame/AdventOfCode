using System;
using System.Collections.Generic;
using System.Text;

using DayOne.Enums;
using DayOne.Models;

namespace DayOne.Impl.Processors
{
    public class IntMultProcessor : BaseOperationProcessor
    {
        public override MathematicOperationType SupportedOperationType { get; }

        protected override T ProcessInternal<T>(MathematicOperation<T> mathematicOperation)
        {
            throw new NotImplementedException();
        }
    }
}
