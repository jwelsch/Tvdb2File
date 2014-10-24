using System;

namespace Tvdb2File
{
   public interface IExecutionStep
   {
      void Execute( ExecutionContext context );
   }
}
