using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;

namespace Refit.Generator.App
{
    class TestNested
    {
        public interface INestedInterface
        {
            [Get("")]
            Task getA();
        }
    }
    
    public static class IntegrationTestHelper
    {
        public static string GetPath(params string[] paths)
        {
            var ret = GetIntegrationTestRootDirectory();
            return (new FileInfo(paths.Aggregate(ret, Path.Combine))).FullName;
        }

        public static string GetIntegrationTestRootDirectory()
        {
            // XXX: This is an evil hack, but it's okay for a unit test
            // We can't use Assembly.Location because unit test runners love
            // to move stuff to temp directories
            var st = new StackFrame(true);
            var di = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(st.GetFileName())));

            return di.FullName;
        }
    }
    
    class Program
    {
        static int Main(string[] args)
        {
            
          

            /*using(StreamReader sr = new StreamReader("GitHubApi.cs"))
            {
                var txt = sr.ReadToEnd();
                var file = CSharpSyntaxTree.ParseText(txt);
                var fixture = new InterfaceStubGenerator();
                
                var input = file.GetRoot().DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .First(x => x.Identifier.ValueText == "INestedGitHubApi6");

                var result = fixture.GenerateClassInfoForInterface(input);
                ;
            }
          

            var y = 1;*/
           // var y = RestService.For<TestNested.INestedInterface>("http://www.uni.lodz.pl/");
            
            // NB: @Compile passes us a list of files relative to the project
            // directory - pass in the project and use its dir 
            var generator = new InterfaceStubGenerator(msg => Console.Out.WriteLine(msg));
            var target = new FileInfo(args[0]);
            var targetDir = new DirectoryInfo(args[1]);

            var files = default(FileInfo[]);

            if (args.Length ==3)
            {
                // We get a file with each line being a file
                files = File.ReadLines(args[2])
                    .Distinct()
                    .Select(x => File.Exists(x) ? new FileInfo(x) : new FileInfo(Path.Combine(targetDir.FullName, x)))
                    .Where(x => x.Name.Contains("RefitStubs") == false && x.Exists && x.Length > 0)
                    .ToArray();
            }
            else
            {
                return -1;
            }
           
            var template = generator.GenerateInterfaceStubs(files.Select(x => x.FullName).ToArray()).Trim();

            string contents = null;

            if (target.Exists)
            {
                // Only try writing if the contents are different. Don't cause a rebuild
                contents = File.ReadAllText(target.FullName, Encoding.UTF8).Trim();
                if (string.Equals(contents, template, StringComparison.Ordinal))
                {
                    return 0;
                }
            }


            // If the file is read-only, we might be on a build server. Check the file to see if 
            // the contents match what we expect
            if (target.Exists && target.IsReadOnly)
            {
                if (!string.Equals(contents, template, StringComparison.Ordinal))
                {
                    Console.Error.WriteLine(new ReadOnlyFileError(target));
                    return -1; // error....
                }
            }
            else
            {
                var retryCount = 3;

            retry:
                var file = default(FileStream);

                // NB: Parallel build weirdness means that we might get >1 person 
                // trying to party on this file at the same time.
                try
                {
                    file = File.Open(target.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
                }
                catch (Exception)
                {
                    if (retryCount < 0)
                    {
                        throw;
                    }

                    retryCount--;
                    Thread.Sleep(500);
                    goto retry;
                }

                using (var sw = new StreamWriter(file, Encoding.UTF8))
                {
                    sw.WriteLine(template);
                }
            }
            
            return 0;
        }
    }

    static class ConcatExtension
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> This, params IEnumerable<T>[] others)
        {
            foreach (var t in This)
            {
                yield return t;
            }

            foreach (var list in others)
            {
                foreach (var t in list)
                {
                    yield return t;
                }
            }
        }
    }
}
