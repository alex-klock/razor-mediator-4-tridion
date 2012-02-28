using System;
using System.CodeDom.Compiler;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    /// <summary>
    /// Represents a template compiler exception.
    /// </summary>
    public class TemplateCompileException : Exception 
    {
        /// <summary>
        /// Gets the errors that were thrown by the compiled code.
        /// </summary>
        public CompilerErrorCollection Errors { get; private set; }

        /// <summary>
        /// Gets the source code.
        /// </summary>
        public string SourceCode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errors">The collection of errors that were thrown by the compiled code.</param>
        /// <param name="sourceCode">The source code.</param>
        public TemplateCompileException(CompilerErrorCollection errors, string sourceCode) 
        { 
            Errors = errors; 
            SourceCode = sourceCode; 
        } 
    }
}
