using System;
using System.Text;
using System.IO;

namespace Tridion.Extensions.Mediators.Razor.Templating
{
    public delegate void SectionWriter();

    /// <summary>
    /// The base template for all Razor Templates to be inherited from.
    /// </summary>
    public abstract class RazorTemplateBase
    {
        /// <summary>
        /// Gets or sets the underlying buffer of the template.
        /// </summary>
        public StringBuilder Buffer { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RazorTemplateBase()
        {
            Buffer = new StringBuilder();
        }

        /// <summary>
        /// Executes the template.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        protected virtual void Clear()
        {
            Buffer.Clear();
        }

        /// <summary>
        /// Defines a template section.
        /// </summary>
        /// <param name="name">The section name.</param>
        /// <param name="writer">The writer delegate.</param>
        protected virtual void DefineSection(string name, SectionWriter writer)
        {
            writer.Invoke();
        }

        /// <summary>
        /// Writes to the buffer.
        /// </summary>
        /// <param name="value">The value to write to the buffer.</param>
        protected virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        /// <summary>
        /// Writes to the buffer.
        /// </summary>
        /// <param name="value">The value to write to the buffer.</param>
        protected virtual void WriteLiteral(object value)
        {
            if (value == null)
            {
                return;
            }

            Buffer.Append(value);
        }

        /// <summary>
        /// Writes a string literal to the specified TextWriter.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="literal">The literal to be written.</param>
        protected static void WriteLiteralTo(TextWriter writer, string literal)
        {
            if (literal == null)
                return;

            writer.Write(literal);
        }

        /// <summary>
        /// Writes the specified object to the specified TextWriter.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">The object to be written.</param>
        protected static void WriteTo(TextWriter writer, object obj)
        {
            if (obj == null)
                return;

            writer.Write(obj);
        }

        /// <summary>
        /// Writes out the value of the underlying buffer.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return Buffer.ToString();
            }
            finally
            {
                Clear();
            }
        }
    }
}