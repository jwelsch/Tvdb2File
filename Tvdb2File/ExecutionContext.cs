using System;

namespace Tvdb2File
{
   public class ExecutionContext
   {
      public LocalStoragePath LocalStoragePath
      {
         get;
         private set;
      }

      public CommandLine CommandLine
      {
         get;
         private set;
      }

      public InputPathInfo InputPathInfo
      {
         get;
         private set;
      }

      public ExecutionContext( LocalStoragePath localStoragePath, CommandLine commandLine, InputPathInfo inputPathInfo )
      {
         this.LocalStoragePath = localStoragePath;
         this.CommandLine = commandLine;
         this.InputPathInfo = inputPathInfo;
      }
   }
}
