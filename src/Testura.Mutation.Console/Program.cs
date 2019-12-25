using System;
using McMaster.Extensions.CommandLineUtils;
using Testura.Mutation.Console.CommandConfigurations;
using Testura.Mutation.Console.Commands;

namespace Testura.Mutation.Console
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
                Name = "Testura.Mutation.Console",
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