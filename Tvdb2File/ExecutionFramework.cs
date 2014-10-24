using System;
using System.Linq;

namespace Tvdb2File
{
   public class ExecutionFramework
   {
      public ExecutionStepCollection Steps
      {
         get;
         private set;
      }

      public ExecutionFramework()
      {
         this.Steps = new ExecutionStepCollection();
      }

      public void Run( ExecutionContext context )
      {
         foreach ( var step in this.Steps )
         {
            step.Execute( context );
         }
      }
   }
}
