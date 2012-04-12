using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    /// <summary>
    /// Represents a template compiler exception.
    /// </summary>
    public class TemplateCompileException : Exception 
    {
        /// <summary>
        /// The lines of source code.
        /// </summary>
        private List<string> _lines;

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

        /// <summary>
        /// Gets the lines of source code.
        /// </summary>
        public List<string> Lines
        {
            get
            {
                if (_lines == null)
                {
                    _lines = new List<string>();
                    Regex pattern = new Regex("^(.*)$", RegexOptions.Multiline);
                    foreach (Match match in pattern.Matches(SourceCode))
                    {
                        _lines.Add(match.Value);
                    }
                }
                return _lines;
            }
        }

        /// <summary>
        /// Gets a specific line of source code.
        /// </summary>
        /// <param name="lineNumber">The line number to retrieve.</param>
        /// <returns>A line of source code.</returns>
        public string GetSourceLine(int lineNumber)
        {
            return Lines[lineNumber - 1];
        }
    }
}
