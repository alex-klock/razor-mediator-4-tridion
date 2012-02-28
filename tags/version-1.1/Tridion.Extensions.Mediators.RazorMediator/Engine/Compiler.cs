﻿using System; 
using System.CodeDom.Compiler; 
using System.Collections.Generic; 
using System.IO;
using System.Linq;
using System.Reflection; 
using System.Text; 
using System.Web.Razor; 
using Microsoft.CSharp;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    /// <summary>
    /// Responsible for compiling the C# Razor Templates.
    /// </summary>
    public class Compiler
    {
        /// <summary>
        /// Compiles the RazorTemplateEntries.
        /// </summary>
        /// <param name="entries">The RazorTemplateEntry objects to compile.</param>
        /// <returns>The compiled assembly.</returns>
        public static Assembly Compile(IEnumerable<RazorTemplateEntry> entries, IEnumerable<string> assemblyReferences)
        {
            if (!entries.Any(entry => entry.Assembly == null))
                return null;

            StringBuilder builder = new StringBuilder();
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            using (var writer = new StringWriter(builder))
            {
                foreach (var razorTemplateEntry in entries)
                {
                    if (razorTemplateEntry.Assembly != null)
                        continue;

                    var generatorResults = GenerateCode(razorTemplateEntry);
                     
                    codeProvider.GenerateCodeFromCompileUnit(generatorResults.GeneratedCode, writer, new CodeGeneratorOptions());
                }
            }

            var result = codeProvider.CompileAssemblyFromSource(BuildCompilerParameters(assemblyReferences), new[] { builder.ToString() });
            if (result.Errors != null && result.Errors.Count > 0)
                throw new TemplateCompileException(result.Errors, builder.ToString());

            foreach (RazorTemplateEntry entry in entries)
            {
                if (entry.Assembly == null)
                {
                    entry.Assembly = result.CompiledAssembly;
                    entry.CompiledTime = DateTime.Now;
                }
            }

            return result.CompiledAssembly;
        }

        /// <summary>
        /// Creates the CompilerParameters object.
        /// </summary>
        private static CompilerParameters BuildCompilerParameters(IEnumerable<string> assemblyReferences)
        {
            var @params = new CompilerParameters();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.ManifestModule.Name != "<In Memory Module>")
                {
                    @params.ReferencedAssemblies.Add(assembly.Location);
                }
            }
            
            if (assemblyReferences != null)
            {
                foreach (string reference in assemblyReferences)
                {
                    bool containsReference = false;
                    foreach (string alreadyAdded in @params.ReferencedAssemblies)
                    {
                        if (alreadyAdded.EndsWith("\\" + reference))
                        {
                            containsReference = true;
                            break;
                        }
                    }
                    if (!containsReference)
                    {
                        if (reference.Contains("\\"))
                        {
                            @params.ReferencedAssemblies.Add(reference);
                        }
                        else
                        {
                            Assembly assembly = Assembly.Load(reference);
                            @params.ReferencedAssemblies.Add(assembly.Location);
                        }
                    }
                }
            }

            @params.GenerateInMemory = true;
            @params.IncludeDebugInformation = false;
            @params.GenerateExecutable = false;
            @params.CompilerOptions = "/target:library /optimize";

            return @params;
        } 

        /// <summary>
        /// Generates the code from a C# razor template.
        /// </summary>
        /// <param name="entry">The RazorTemplateEntry instance.</param>
        /// <returns>The GeneratorResults object.</returns>
        private static GeneratorResults GenerateCode(RazorTemplateEntry entry) 
        {
            RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = entry.TemplateType.FullName;
            host.DefaultNamespace = "Tridion.Extensions.Mediators.Razor.Templating"; 
            host.DefaultClassName = entry.TemplateName;
            host.GeneratedClassContext = new System.Web.Razor.Generator.GeneratedClassContext("Execute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo", "Tridion.Extensions.Mediators.Razor.Templating.TemplateWriter", "DefineSection");
            host.NamespaceImports.Add("System");
            foreach (string ns in entry.Namespaces)
            {
                host.NamespaceImports.Add(ns);
            }

            GeneratorResults razorResult = null; 
            using (TextReader reader = new StringReader(entry.TemplateString)) 
            { 
                razorResult = new RazorTemplateEngine(host).GenerateCode(reader);
            } 
 
            return razorResult; 
        } 
    } 
}