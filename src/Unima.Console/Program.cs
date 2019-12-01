using System;
using McMaster.Extensions.CommandLineUtils;
using Unima.Console.CommandConfigurations;
using Unima.Console.Commands;

namespace Unima.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var o = @"                                    ,(((((                                                                             
                                   #,  /( (/                                                                           
                                 #,  /(     (/                                                                         
                              ,#,  /(   #/#,  (/                                                                       
                            ,#   /(  .#(/  ,#,  ,#                                                                     
                          ,#   /(  .#,   //  .#,  ,#                                                                   
                        ,#   #*  .#,       //  .(/  ,#                                                                 
                       #,  /(  .#,           //   (/  ,(                                                               
                       #,#.  (#*               (#   (/.(                                                               
                       #  ,#.  *#            ,#. ,#   ((                                                               
                       .#.  /(   ((         #*  .#,  /(                                                                
                         .#,  /#.  ((     #*  *#,  /(                                                                  
                           .#,  ,#.  (( #*  *#   /(                                                                    
                             .#/  ,#..#*  *#   #(                                                                      
                                (/  ,*  /#   #,                                                                        
                                  (/  /#   #,                                                                          
                                    /#####,                                                                            
                                                                                                                       
                                                                                                                       
         *****   .*** ***       ***. ****  ***         **.     /                                                       
          #@.      #   ,&@*      #    &@    %@%       %@.     (@(                                                      
          #@.      #   ,. @@,    #    &@    #,@#     (/@.    ,./@,                                                     
          #@.      #   ,.  ,@@   #    &@    # ,@@   ( /@.    *  %@/                                                    
          (@.      #   ,.    /@@.#    &@    #  *@@ (  /@.   #    @@.                                                   
          .@/     ,.   *.      *@&    &@    #    @@   /@.  (     .@@                                                   
            ./#((,    *//*       /   ////. ///.  .   ////,///   .////.          ";

            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(o);
            System.Console.WriteLine(string.Empty);

            var app = new CommandLineApplication
            {
                Name = "Unima.Console",
                FullName = "C# mutation testing"
            };

            app.HelpOption("-?|-h|--help");

            app.Command("local", a => MutateLocalConfiguration.Configure(a));

            app.OnExecute(() => new RootCommand(app).RunAsync().Wait());

            var result = app.Execute(args);
            Environment.Exit(result);
        }
    }
}